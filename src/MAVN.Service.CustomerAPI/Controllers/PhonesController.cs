using System;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using MAVN.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Models.Phones;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerManagement.Client.Models;
using MAVN.Service.CustomerManagement.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/phones")]
    [ApiController]
    [LykkeAuthorize]
    [Produces("application/json")]
    public class PhonesController : Controller
    {
        private readonly ICustomerManagementServiceClient _customerManagementClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IRequestContext _requestContext;
        private readonly ILog _log;

        public PhonesController(
            ICustomerManagementServiceClient customerManagementClient,
            ICustomerProfileClient customerProfileClient,
            IRequestContext requestContext,
            ILogFactory logFactory)
        {
            _customerManagementClient = customerManagementClient;
            _customerProfileClient = customerProfileClient;
            _requestContext = requestContext;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Generates verification sms to verify customer's phone number.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **PhoneIsAlreadyVerified**
        /// - **SenderCustomerNotFound**
        /// - **ReachedMaximumRequestForPeriod**
        /// - **CustomerPhoneIsMissing**
        /// </remarks>
        [HttpPost("generate-verification")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task GeneratePhoneVerificationAsync()
        {
            var customerId = _requestContext.UserId;

            var result = await _customerManagementClient.PhonesApi.RequestVerificationAsync(
                new VerificationCodeRequestModel
                {
                    CustomerId = customerId
                });

            switch (result.Error)
            {
                case VerificationCodeError.None:
                    return;
                case VerificationCodeError.AlreadyVerified:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PhoneIsAlreadyVerified);
                case VerificationCodeError.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case VerificationCodeError.ReachedMaximumRequestForPeriod:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReachedMaximumRequestForPeriod);
                case VerificationCodeError.CustomerPhoneIsMissing:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerPhoneIsMissing);
                default:
                    throw new InvalidOperationException($"Unexpected error during GeneratePhoneVerificationAsync for {customerId} - {result}");
            }
        }

        /// <summary>
        /// Verifies customer's phone number in the system.
        /// </summary>
        /// <param name="model">Phone verification request model</param>
        /// <remarks>
        /// Error codes:
        /// - **PhoneIsAlreadyVerified**
        /// - **VerificationCodeDoesNotExist**
        /// - **VerificationCodeExpired**
        /// - **CustomerPhoneIsMissing**
        /// </remarks>
        [HttpPost("verify")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task VerifyPhoneAsync([FromBody] VerifyPhoneRequest model)
        {
            var customerId = _requestContext.UserId;

            var result = await _customerManagementClient.PhonesApi.ConfirmPhoneAsync(new PhoneVerificationCodeConfirmationRequestModel
            {
                VerificationCode = model.VerificationCode,
                CustomerId = customerId
            });

            if (result.Error != VerificationCodeError.None)
            {
                switch (result.Error)
                {
                    case VerificationCodeError.AlreadyVerified:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PhoneIsAlreadyVerified);
                    case VerificationCodeError.CustomerPhoneIsMissing:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerPhoneIsMissing);
                    case VerificationCodeError.PhoneAlreadyExists:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PhoneAlreadyExists);
                    case VerificationCodeError.VerificationCodeDoesNotExist:
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode(result.Error.ToString(), "Verification code does not exist"));
                    case VerificationCodeError.VerificationCodeExpired:
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode(result.Error.ToString(), "Verification code has expired"));
                    default:
                        throw new InvalidOperationException($"Unexpected error during VerifyPhoneAsync for {customerId} - {result}");
                }
            }
        }

        /// <summary>
        /// Sets phone number for the customer
        /// </summary>
        /// <param name="model">Phone verification request model</param>
        /// <remarks>
        /// Error codes:
        /// - **CustomerProfileDoesNotExist**
        /// - **CountryPhoneCodeDoesNotExist**
        /// - **InvalidPhoneNumber**
        /// - **PhoneAlreadyExists**
        /// </remarks>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task SetCustomerPhoneAsync([FromBody] SetCustomerPhoneRequest model)
        {
            var customerId = _requestContext.UserId;

            var result =
                await _customerProfileClient.CustomerPhones.SetCustomerPhoneInfoAsync(
                    new SetCustomerPhoneInfoRequestModel
                    {
                        CustomerId = customerId,
                        CountryPhoneCodeId = model.CountryPhoneCodeId,
                        PhoneNumber = model.PhoneNumber
                    });

            if(result.ErrorCode == CustomerProfileErrorCodes.CustomerProfileDoesNotExist)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerProfileDoesNotExist);

            if (result.ErrorCode == CustomerProfileErrorCodes.InvalidCountryPhoneCodeId)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CountryPhoneCodeDoesNotExist);

            if(result.ErrorCode == CustomerProfileErrorCodes.PhoneAlreadyExists)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PhoneAlreadyExists);

            if (result.ErrorCode == CustomerProfileErrorCodes.InvalidPhoneNumber)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidPhoneNumber);

            if (result.ErrorCode != CustomerProfileErrorCodes.None)
                throw new InvalidOperationException($"Unexpected error during SetCustomerPhoneAsync for {customerId} - {result}");

            var requestVerificationResult = await _customerManagementClient.PhonesApi.RequestVerificationAsync(
                new VerificationCodeRequestModel {CustomerId = customerId});

            if (requestVerificationResult.Error != VerificationCodeError.None)
                _log.Warning("Could not send phone verification phone when changing customer's phone",
                    context: requestVerificationResult.Error);
        }
    }
}
