using System;

namespace MAVN.Service.CustomerAPI.Core.Exceptions
{
    public class UnhandledErrorCodeException: Exception
    {
        public UnhandledErrorCodeException(string message)
            : base(message)
        {
            
        }


        public UnhandledErrorCodeException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public string ErrorCode { get; }
    }
}
