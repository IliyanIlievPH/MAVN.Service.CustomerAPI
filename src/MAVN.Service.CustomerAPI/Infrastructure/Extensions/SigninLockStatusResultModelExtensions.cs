using System.Net;
using MAVN.Service.CustomerAPI.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Infrastructure.Extensions
{
    public static class SigninLockStatusResultModelExtensions
    {
        public static ObjectResult GetSigninLockedResponse(this SigninLockStatusResultModel model)
        {
            var response = new {message = model.Message, retryPeriodInMinutes = model.RetryPeriodInMinutesWhenLocked};

            return new ObjectResult(response) {StatusCode = (int) HttpStatusCode.TooManyRequests};
        }
    }
}
