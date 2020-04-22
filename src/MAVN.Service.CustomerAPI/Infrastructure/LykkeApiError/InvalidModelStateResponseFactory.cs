using System.Linq;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAVN.Service.CustomerAPI.Infrastructure.LykkeApiError
{
    public static class InvalidModelStateResponseFactory
    {
        private const string _emailKey = "Email";
        private const string _passwordKey = "Password";

        /// <summary>
        ///     General validation error processing delegate.
        ///     Wraps any failed model validation into <see cref="LykkeApiErrorResponse" />.
        ///     To return custom error code, throw <see cref="LykkeApiErrorException" /> from validator
        ///     with appropriate code from <see cref="ApiErrorCodes.ModelValidation" />.
        ///     If code does not exist feel free to create a new one.
        /// </summary>
        public static IActionResult CreateInvalidModelResponse(ActionContext context)
        {
            var errorCode = ApiErrorCodes.ModelValidation.ModelValidationFailed.Name;

            if (context.ModelState.ContainsKey(_emailKey) && context.ModelState[_emailKey].Errors.Any())
                errorCode = nameof(ApiErrorCodes.Service.InvalidEmailFormat);
            else if (context.ModelState.ContainsKey(_passwordKey) && context.ModelState[_passwordKey].Errors.Any())
                errorCode = nameof(ApiErrorCodes.Service.InvalidPasswordFormat);

            var apiErrorResponse = new LykkeApiErrorResponse
            {
                Error = errorCode,
                Message = GetErrorMessage(context.ModelState)
            };
            return new BadRequestObjectResult(apiErrorResponse)
            {
                ContentTypes = {"application/json"}
            };
        }

        private static string GetErrorMessage(ModelStateDictionary modelStateDictionary)
        {
            var modelError = modelStateDictionary?.Values
                .SelectMany(x => x.Errors)
                .Select(err => !string.IsNullOrEmpty(err.ErrorMessage)
                    ? err.ErrorMessage
                    : err.Exception?.Message);

            return string.Join(' ', modelError ?? Enumerable.Empty<string>());
        }
    }
}
