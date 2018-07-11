using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestComputera
{
    public class СompeteraAPI : BaseDataService
    {
        public СompeteraAPI(string baseApiUrl, string login, string password) : base(baseApiUrl, login, password){ }


        public async Task<bool> test()
        {
            var request = new ApiRequest($"{BaseApiUrl}session/user") { Method = HttpMethod.Get };

            AppendAuthParams(ref request);

            //request.AddParameter("orderNumber", innerOrderId);
            //request.AddParameter("amount", amount);
            //request.AddParameter("description", orderDescription);
            //request.AddParameter("returnUrl", returnUrl);
            //request.AddParameter("currency", currency);
            //request.AddParameter("language", language);
            //request.AddParameter("pageView", pageView);


            //request.AddOptionalParameter("failUrl", failUrl);
            
             await ApiClient.ExecuteAsync<object>(request);
             return true;
        }

        private void AppendAuthParams(ref ApiRequest request)
        {
            request.AddParameter("username", Login);
            request.AddParameter("api_key", Password);
            request.AddParameter("format", "json");
        }

        //public async Task<AcquiringOrderInfo> RegisterOrder(string innerOrderId, string orderDescription, int amount, string returnUrl, string failUrl,
        //    DateTime? expirationDateUtc = null, int currency = 643, string language = "ru", string pageView = "DESKTOP")
        //{
        //    var request = new ApiRequest($"{BaseApiUrl}registerPreAuth.do") { Method = HttpMethod.Post };

        //    AppendAuthParams(ref request);

        //    request.AddParameter("orderNumber", innerOrderId);
        //    request.AddParameter("amount", amount);
        //    request.AddParameter("description", orderDescription);
        //    request.AddParameter("returnUrl", returnUrl);
        //    request.AddParameter("currency", currency);
        //    request.AddParameter("language", language);
        //    request.AddParameter("pageView", pageView);

        //    if (expirationDateUtc.HasValue)
        //    {
        //        request.AddParameter("expirationDate", expirationDateUtc.Value.AddHours(3).ToString("u").Replace(" ", "T"));
        //    }

        //    request.AddOptionalParameter("failUrl", failUrl);

        //    return await ApiClient.ExecuteAsync<AcquiringOrderInfo>(request);
        //}
    }
}
