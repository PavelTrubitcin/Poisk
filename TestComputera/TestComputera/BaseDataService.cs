using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestComputera
{
    public class BaseDataService
    {
        public BaseDataService(string baseApiUrl, string login, string password)
        {
            BaseApiUrl = baseApiUrl;
            Login = login;
            Password = password;

            ApiClient = new ApiClient();
        }

        protected ApiClient ApiClient { get; }
        protected string BaseApiUrl { get; }
        protected string Login { get; }
        protected string Password { get; }
    }
}
