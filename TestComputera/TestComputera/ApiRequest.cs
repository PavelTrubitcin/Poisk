using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestComputera
{
    public class ApiRequest
    {
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiRequest" /> class.
        /// </summary>
        public ApiRequest(string baseUri)
        {
            BaseUri = baseUri;
            Method = HttpMethod.Get;
        }

        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        /// <value>
        ///     The parameters.
        /// </value>
        public Dictionary<string, string> Parameters => _parameters;

        /// <summary>
        ///     Gets the base URI.
        /// </summary>
        /// <value>
        ///     The base URI.
        /// </value>
        public string BaseUri { get; set; }

        /// <summary>
        ///     Gets the method.
        /// </summary>
        /// <value>
        ///     The method.
        /// </value>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets the request body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }

        public string AuthHeaderValue { get; set; }

        #region AddOptionalParameter helper overloads

        /// <summary>
        ///     Adds the optional parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddOptionalParameter(string key, DateTime? value)
        {
            if (!value.HasValue)
            {
                return;
            }
            AddOptionalParameter(key, value.ToString());
        }

        /// <summary>
        ///     Adds the optional parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddOptionalParameter(string key, int? value)
        {
            if (!value.HasValue)
            {
                return;
            }
            AddOptionalParameter(key, value.ToString());
        }

        /// <summary>
        ///     Adds the optional parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddOptionalParameter(string key, long? value)
        {
            if (!value.HasValue)
            {
                return;
            }
            AddOptionalParameter(key, value.ToString());
        }

        /// <summary>
        ///     Adds the optional parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void AddOptionalParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (_parameters.ContainsKey(key))
            {
                throw new InvalidOperationException($"The key {key} already exists.");
            }

            _parameters.Add(key, value);
        }

        #endregion

        #region AddParameter overloads

        /// <summary>
        ///     Adds the parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddParameter(string key, object value)
        {
            AddParameter(key, value.ToString());
        }


        /// <summary>
        ///     Adds the parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void AddParameter(string key, string value)
        {
            if (_parameters.ContainsKey(key))
            {
                throw new InvalidOperationException($"The key {key} already exists.");
            }

            _parameters.Add(key, value);
        }

        #endregion

        /// <summary>
        /// Builds the parameters string.
        /// </summary>
        /// <returns></returns>
        public string BuildParametersString()
        {
            if (!Parameters.Any())
            {
                return string.Empty;
            }

            var paramsBuilder = new StringBuilder();
            paramsBuilder.Append("?");

            foreach (var parameter in Parameters)
            {
                if (paramsBuilder.Length > 1)
                {
                    paramsBuilder.Append("&");
                }

                paramsBuilder.AppendFormat("{0}={1}", parameter.Key, parameter.Value);
            }

            return paramsBuilder.ToString();
        }
    }
}
