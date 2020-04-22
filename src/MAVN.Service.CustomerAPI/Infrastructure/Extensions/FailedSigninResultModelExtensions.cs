using System.Net;
using MAVN.Service.CustomerAPI.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Infrastructure.Extensions
{
    public static class FailedSignInResultModelExtensions
    {
        private const string WarningErrorCode = "LoginAttemptsWarning";
        public static ObjectResult GetWarningResponse(this FailedSigninResultModel model)
        {
            var response = new
            {
                error = WarningErrorCode, message = model.Message, attemptsLeft = model.AttemptsLeftBeforeLock
            };
            
            return new ObjectResult(response)
            {
                StatusCode = (int) HttpStatusCode.Unauthorized
            };
        }

        public static ObjectResult GetSigninLockedResponse(this FailedSigninResultModel model)
        {
            var response = new {message = model.Message, retryPeriodInMinutes = model.RetryPeriodInMinutesWhenLocked};
            
            return new ObjectResult(response)
            {
                StatusCode = (int) HttpStatusCode.TooManyRequests
            };
        }
    }
}
