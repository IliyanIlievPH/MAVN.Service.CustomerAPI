using System;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerManagement.Client.Models;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Models.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/emails")]
    [ApiController]
    [Produces("application/json")]
    public class EmailsController : Controller
    {
        private readonly IRequestContext _requestContext;
        private readonly ICustomerManagementServiceClient _customerManagementServiceClient;
        private readonly ILog _log;

        public EmailsController(
            IRequestContext requestContext, 
            ICustomerManagementServiceClient customerManagementServiceClient,
            ILogFactory logFactory)
        {
            _requestContext = requestContext;
            _customerManagementServiceClient = customerManagementServiceClient;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Generates verification email for the customer in the system.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **EmailIsAlreadyVerified**
        /// - **SenderCustomerNotFound**
        /// - **ReachedMaximumRequestForPeriod**
        /// </remarks>
        [HttpPost("verification")]
        [SwaggerOperation("GenerateVerificationEmailAsync")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [LykkeAuthorize]
        public async Task GenerateVerificationEmailAsync()
        {
            var customerId = _requestContext.UserId;

            var result = await _customerManagementServiceClient.EmailsApi.RequestVerificationAsync(
                new VerificationCodeRequestModel
                {
                    CustomerId = customerId
                });

            switch (result.Error)
            {
                case VerificationCodeError.None:
                    return;
                case VerificationCodeError.AlreadyVerified:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.EmailIsAlreadyVerified);
                case VerificationCodeError.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SenderCustomerNotFound);
                case VerificationCodeError.ReachedMaximumRequestForPeriod:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReachedMaximumRequestForPeriod);
                default:
                    throw new InvalidOperationException($"Unexpected error during GenerateVerificationEmail for {customerId} - {result}");
            }
        }

        /// <summary>
        /// Verifies email for the customer in the system.
        /// </summary>
        /// <param name="model">Email verification request model</param>
        /// <remarks>
        /// Error codes:
        /// - **EmailIsAlreadyVerified**
        /// - **VerificationCodeDoesNotExist**
        /// - **VerificationCodeMismatch**
        /// - **VerificationCodeExpired**
        /// </remarks>
        [HttpPost("verify-email")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task VerifyEmailAsync([FromBody] EmailVerificationRequest model)
        {
            var result = await _customerManagementServiceClient.EmailsApi.ConfirmEmailAsync(new VerificationCodeConfirmationRequestModel
            {
                VerificationCode = model.VerificationCode
            });

            if (result.Error != VerificationCodeError.None)
            {
                switch (result.Error)
                {
                    case VerificationCodeError.AlreadyVerified:
                        _log.Warning(result.Error.ToString());
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.EmailIsAlreadyVerified);
                    case VerificationCodeError.VerificationCodeDoesNotExist:
                        _log.Warning(result.Error.ToString());
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode(result.Error.ToString(), "Verification code does not exist"));
                    case VerificationCodeError.VerificationCodeMismatch:
                        _log.Warning(result.Error.ToString());
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode(result.Error.ToString(), "Verification code mismatch"));
                    case VerificationCodeError.VerificationCodeExpired:
                        _log.Warning(result.Error.ToString());
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode(result.Error.ToString(), "Verification code has expired"));
                }
            }

            _log.Info($"Email verification success with code '{model.VerificationCode}'");
        }
    }
}
