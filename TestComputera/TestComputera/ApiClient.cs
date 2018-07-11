using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestComputera
{
    public class Response<T>
    {
        public byte[] RawBytes { get; set; }
        public T Data { get; set; }
        public bool FromCache { get; set; }
    }

    /// <summary>
    ///     OnlineService class.
    /// </summary>
    public class ApiClient
    {
        #region Implementation of IApiClient

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequest">The API request.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="customHeaders">Custom headers sending with request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(ApiRequest apiRequest, JsonConverter converter = null, Dictionary<string, string> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await ExecuteRequestAsync<T>(apiRequest, converter, customHeaders, cancellationToken).ConfigureAwait(false);
            return result != null ? result.Item1 : default(T);
        }

        /// <summary>
        /// Executes the reqest asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequest">The API request.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="customHeaders">The custom headers.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<Tuple<T, Dictionary<string, string>>> ExecuteRequestAsync<T>(ApiRequest apiRequest,
            JsonConverter converter, Dictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await ExecuteRequestWithResponseAsync<T>(apiRequest, converter, customHeaders, cancellationToken);

            return response.Data;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequest">The API request.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="customHeaders">Custom headers sending with request.</param>
        /// <returns></returns>
        /// <exception cref="DataServiceException"></exception>
        public async Task<Response<Tuple<T, Dictionary<string, string>>>> ExecuteRequestWithResponseAsync<T>(
            ApiRequest apiRequest, JsonConverter converter, Dictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new Response<Tuple<T, Dictionary<string, string>>>();
            }

            try
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

                var request = new HttpRequestMessage(apiRequest.Method,
                    $"{apiRequest.BaseUri}{apiRequest.BuildParametersString() ?? string.Empty}");

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(apiRequest.AuthHeaderValue))
                {
                    request.Headers.Add("Authorization", apiRequest.AuthHeaderValue);
                }

                if (customHeaders != null)
                {
                    foreach (var customHeader in customHeaders)
                    {
                        request.Headers.Add(customHeader.Key, customHeader.Value);
                    }
                }

                if (!string.IsNullOrEmpty(apiRequest.Body))
                {
                    request.Content = new StringContent(apiRequest.Body);
                }

                var client = new HttpClient(handler);

                var response = await client.SendAsync(request, cancellationToken);

                if (response != null)
                {
                    var responseBytes = await response.Content.ReadAsByteArrayAsync();
                    var responseContent = Encoding.UTF8.GetString(responseBytes, 0, responseBytes.Length);

                    var apiError = await GetErrorResult(response);
                    if (apiError != null)
                    {
                        throw new DataServiceException(apiError);
                    }

                    var result = DeserializeJson<T>(responseContent, apiRequest.Path, converter);

                    Dictionary<string, string> headers = null;

                    if (response.Headers != null)
                    {
                        var responseHeaders = response.Headers.ToArray();
                        headers = responseHeaders.ToDictionary(h => h.Key, v => string.Join(";", v.Value));
                    }

                    return new Response<Tuple<T, Dictionary<string, string>>>
                    {
                        Data = new Tuple<T, Dictionary<string, string>>(result, headers),
                        RawBytes = responseBytes
                    };
                }
                else
                {
                    throw new DataServiceException(new ApiError());
                }
            }

            catch (OperationCanceledException)
            {
                return new Response<Tuple<T, Dictionary<string, string>>>();
            }

            catch (HttpRequestException ex)
            {
                throw new DataServiceException(GetErrorResult(ex));
            }
            catch (Exception ex)
            {
                if (ex is DataServiceException)
                {
                    throw;
                }
                else
                {
                    throw new DataServiceException(GetErrorResult(ex));
                }
            }
        }

        public static T DeserializeJson<T>(string jsonString, string path, JsonConverter converter)
        {
            JToken jToken = null;

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                jToken = JToken.Parse(jsonString);
            }

            if (!string.IsNullOrEmpty(path))
            {
                jToken = jToken?.SelectToken(path);
            }

            T result;
            if (jToken != null)
            {
                var serializer = new JsonSerializer();
                if (converter != null)
                {
                    serializer.Converters.Add(converter);
                }

                result = jToken.ToObject<T>(serializer);
            }
            else
            {
                result = default(T);
            }
            return result;
        }

        #endregion

        /// <summary>
        ///     Gets the error result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseMessage">The response message.</param>
        /// <returns></returns>
        private static async Task<ApiError> GetErrorResult(HttpResponseMessage responseMessage)
        {
            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new ApiError
                {
                    ErrorMessage = string.Format(
                        "Response status code does not indicate success: {2} ({0}).{3}{1}{3}Request: {4}",
                        responseMessage.ReasonPhrase,
                        responseContent,
                        responseMessage.StatusCode,
                        Environment.NewLine,
                        responseMessage.RequestMessage.RequestUri),
                    ErrorCode = (int)responseMessage.StatusCode,
                };
            }

            ApiError apiError = null;

            try
            {
                var errorToken = JToken.Parse(responseContent);
                if (errorToken != null)
                {
                    apiError = errorToken.ToObject<ApiError>(new JsonSerializer());
                }
            }
            catch (Exception)
            {
                apiError = null;
            }

            return apiError != null && apiError.ErrorCode != 0 ? apiError : null;
        }


        /// <summary>
        ///     Gets the error result.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private static ApiError GetErrorResult(Exception ex)
        {
            return new ApiError
            {
                ErrorMessage = ex.InnerException?.Message ?? ex.Message,
                ErrorCode = -1
            };
        }
    }
}
