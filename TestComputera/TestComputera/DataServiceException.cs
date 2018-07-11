using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestComputera
{
    public class DataServiceException : Exception
    {
        private readonly ApiError _error;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
        /// </summary>
        /// <param name="error">The error.</param>
        public DataServiceException(ApiError error)
            : base(error.ErrorMessage)
        {
            _error = error;
        }

        public string ApiErrorMessage => _error.ErrorMessage;

        public int ApiErrorCode => _error.ErrorCode;
    }
}
