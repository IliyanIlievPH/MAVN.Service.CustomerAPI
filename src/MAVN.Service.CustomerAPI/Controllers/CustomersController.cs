using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using MAVN.Service.Credentials.Client;
using MAVN.Service.Credentials.Client.Enums;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.Auth;
using MAVN.Service.CustomerAPI.Models.Customers;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerManagement.Client.Enums;
using MAVN.Service.CustomerManagement.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.Sessions.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ChangePasswordRequestModel = MAVN.Service.CustomerAPI.Models.Customers.ChangePasswordRequestModel;
using LoginProvider = MAVN.Service.CustomerManagement.Client.Enums.LoginProvider;
using PasswordResetError = MAVN.Service.CustomerManagement.Client.Enums.PasswordResetError;
using RegistrationRequestModel = MAVN.Service.CustomerAPI.Models.Customers.RegistrationRequestModel;
using ResetIdentifierValidationRequest = MAVN.Service.CustomerManagement.Client.Models.Requests.ResetIdentifierValidationRequest;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerManagementServiceClient _customerManagementServiceClient;
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;
        private readonly IPasswordValidator _passwordValidator;
        private readonly ILog _log;
        private readonly ISessionsServiceClient _sessionsServiceClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly ICredentialsClient _credentialsClient;

        public CustomersController(
            ICustomerService customerService,
            ICustomerManagementServiceClient customerManagementServiceClient,
            IRequestContext requestContext,
            IMapper mapper,
            IPasswordValidator passwordValidator,
            ILogFactory logFactory,
            ISessionsServiceClient sessionsServiceClient,
            ICustomerProfileClient customerProfileClient,
            ICredentialsClient credentialsClient)
        {
            _customerService = customerService;
            _customerManagementServiceClient = customerManagementServiceClient;
            _requestContext = requestContext;
            _mapper = mapper;
            _passwordValidator = passwordValidator;
            _sessionsServiceClient = sessionsServiceClient;
            _customerProfileClient = customerProfileClient;
            _credentialsClient = credentialsClient;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Gets the current Customer info.
        /// </summary>
        [HttpGet]
        [LykkeAuthorize]
        [SwaggerOperation("GetCustomerInfo")]
        [ProducesResponseType(typeof(CustomerInfoResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<CustomerInfoResponseModel> GetCustomerInfoAsync()
        {
            var customerId = _requestContext.UserId;
            var customerProfileTask = _customerService.GetCustomerInfoAsync(
                customerId,
                true,
                true);
            var blockStatusTask = _customerManagementServiceClient.CustomersApi.GetCustomerBlockStateAsync(customerId);
            var hasPinTask = _credentialsClient.Api.HasPinAsync(customerId);
            await Task.WhenAll(customerProfileTask, blockStatusTask, hasPinTask);

            var customerProfile = customerProfileTask.Result;
            var blockStatus = blockStatusTask.Result;
            var result = new CustomerInfoResponseModel
            {
                FirstName = customerProfile.FirstName,
                LastName = customerProfile.LastName,
                PhoneNumber = customerProfile.ShortPhoneNumber,
                Email = customerProfile.Email,
                IsEmailVerified = customerProfile.IsEmailVerified,
                IsAccountBlocked = blockStatus.Status.HasValue && blockStatus.Status.Value == CustomerActivityStatus.Blocked,
                IsPhoneNumberVerified = customerProfile.IsPhoneVerified,
                CountryPhoneCode = customerProfile.CountryPhoneCode,
                CountryPhoneCodeId = customerProfile.CountryPhoneCodeId,
                CountryOfNationalityId = customerProfile.CountryOfNationalityId,
                CountryOfNationalityName = customerProfile.CountryOfNationalityName,
                HasPin = hasPinTask.Result.HasPin
            };

            return result;
        }

        /// <summary>
        /// Register a new client.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidEmailFormat**
        /// - **LoginAlreadyInUse**
        /// - **InvalidPasswordFormat**
        /// - **AlreadyRegisteredWithGoogle**
        /// - **InvalidCountryOfNationalityId**
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation("RegisterCustomer")]
        [ProducesResponseType(typeof(RegistrationResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<RegistrationResponseModel> Post([FromBody] RegistrationRequestModel model)
        {
            if (!model.Email.IsValidEmailAndRowKey())
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailFormat);

            var result = await _customerService.RegisterAsync(_mapper.Map<RegistrationRequestDto>(model));

            switch (result.Error)
            {
                case CustomerError.None:
                    return _mapper.Map<RegistrationResponseModel>(result);
                case CustomerError.RegisteredWithAnotherPassword:
                case CustomerError.AlreadyRegistered:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LoginAlreadyInUse);
                case CustomerError.InvalidLoginFormat:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailFormat);
                case CustomerError.InvalidPasswordFormat:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidPasswordFormat);
                case CustomerError.AlreadyRegisteredWithGoogle:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AlreadyRegisteredWithGoogle);
                case CustomerError.InvalidCountryOfNationalityId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCountryOfNationalityId);
                case CustomerError.EmailIsNotAllowed:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.EmailIsNotAllowed);
                default:
                    throw new InvalidOperationException($"Unexpected error during Register for {model.Email.SanitizeEmail()} - {result.Error}");
            }
        }

        /// <summary>
        /// Register a new customer with google account.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **AlreadyRegisteredWithGoogle**
        /// - **LoginAlreadyInUse**
        /// - **InvalidOrExpiredGoogleAccessToken**
        /// - **InvalidCountryOfNationalityId**
        /// </remarks>
        [HttpPost("google-register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegistrationResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<RegistrationResponseModel> GoogleRegisterAsync([FromBody] GoogleRegistrationRequestModel model)
        {
            var result = await _customerService.GoogleRegisterAsync(_mapper.Map<GoogleRegistrationRequestDto>(model));

            switch (result.Error)
            {
                case CustomerError.None:
                    return _mapper.Map<RegistrationResponseModel>(result);
                case CustomerError.AlreadyRegistered:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LoginAlreadyInUse);
                case CustomerError.AlreadyRegisteredWithGoogle:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AlreadyRegisteredWithGoogle);
                case CustomerError.InvalidOrExpiredGoogleAccessToken:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidOrExpiredGoogleAccessToken);
                case CustomerError.InvalidCountryOfNationalityId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCountryOfNationalityId);
                default:
                    throw new InvalidOperationException($"Unexpected error during Register with access token {model.AccessToken} - {result.Error}");
            }
        }

        /// <summary>
        /// Generates link for resetting the user password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="LykkeApiErrorException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost("generateresetpasswordlink")]
        [SwaggerOperation("GenerateResetPasswordLink")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task GenerateResetPasswordLink([FromBody] GenerateResetPasswordLinkRequestModel model)
        {
            if (!model.Email.IsValidEmailAndRowKey())
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailFormat);

            var result = await _customerManagementServiceClient.CustomersApi.GenerateResetPasswordLink(new GenerateResetPasswordRequest { Email = model.Email });

            switch (result.Error)
            {
                case PasswordResetError.None:
                    return;
                case PasswordResetError.NoCustomerWithSuchEmail:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NoCustomerWithSuchEmail);
                case PasswordResetError.ThereIsNoIdentifierForThisCustomer:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ThereIsNoIdentifierForThisCustomer);
                case PasswordResetError.ReachedMaximumRequestForPeriod:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReachedMaximumRequestForPeriod);
                case PasswordResetError.CustomerIsNotVerified:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerIsNotVerified);
                default:
                    throw new InvalidOperationException($"Unexpected error during GenerateResetPasswordLink for {model.Email.SanitizeEmail()} - {result}");
            }
        }

        /// <summary>
        /// Changes the password of a customer and re-logins customer with new password.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidPasswordFormat**
        /// - **InvalidCredentials**
        /// - **InvalidEmailFormat**
        /// - **LoginExistsWithDifferentProvider**
        /// - **CustomerBlocked**
        /// </remarks>
        [HttpPost("change-password")]
        [LykkeAuthorize]
        [SwaggerOperation("ChangePassword")]
        [ProducesResponseType(typeof(LoginResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequestModel model)
        {
            var customerId = _requestContext.UserId;

            var changePasswordResult = await _customerService.ChangePasswordAsync(customerId, model.Password);

            switch (changePasswordResult.Error)
            {
                case CustomerError.None:
                    var customerInfo = await _customerService.GetCustomerInfoAsync(customerId, true);
                    if (customerInfo == null)
                        throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.InvalidCredentials);

                    var reLoginResult = await _customerManagementServiceClient.AuthApi.AuthenticateAsync(
                        new AuthenticateRequestModel
                        {
                            Email = customerInfo.Email,
                            Password = model.Password,
                            LoginProvider = LoginProvider.Standard
                        });

                    switch (reLoginResult.Error)
                    {
                        case CustomerManagementError.None:
                            return Ok(_mapper.Map<LoginResponseModel>(reLoginResult));
                        case CustomerManagementError.LoginNotFound:
                        case CustomerManagementError.PasswordMismatch:
                            throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.InvalidCredentials);
                        case CustomerManagementError.InvalidLoginFormat:
                            throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.InvalidEmailFormat);
                        case CustomerManagementError.InvalidPasswordFormat:
                            throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.InvalidPasswordFormat);
                        case CustomerManagementError.LoginExistsWithDifferentProvider:
                            throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.LoginExistsWithDifferentProvider);
                        case CustomerManagementError.CustomerBlocked:
                            throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.CustomerBlocked);
                        default:
                            throw new InvalidOperationException($"Unexpected error during Authenticate for {customerInfo.Email.SanitizeEmail()} - {reLoginResult.Error}");
                    }
                case CustomerError.InvalidPasswordFormat:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidPasswordFormat);
                case CustomerError.CustomerBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerBlocked);
                default:
                    throw new InvalidOperationException($"Unexpected error during password change for {customerId} - {changePasswordResult}");
            }
        }

        /// <summary>
        /// Validates the reset password identifier of a customer.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **IdentifierDoesNotExist**
        /// - **ProvidedIdentifierHasExpired**
        /// </remarks>
        [HttpPost("validate-reset-password-identifier")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task ValidateResetPasswordIdentifierAsync([FromBody] ValidateResetPasswordIdentifierRequest model)
        {
            var result = await _customerManagementServiceClient.CustomersApi.ValidateResetIdentifierAsync(
                new ResetIdentifierValidationRequest { ResetIdentifier = model.ResetPasswordIdentifier });

            switch (result.Error)
            {
                case ValidateResetIdentifierError.None:
                    return;
                case ValidateResetIdentifierError.IdentifierDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.IdentifierDoesNotExist);
                case ValidateResetIdentifierError.ProvidedIdentifierHasExpired:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ProvidedIdentifierHasExpired);
                default:
                    throw new InvalidOperationException(
                        $"Unexpected error during for {model.ResetPasswordIdentifier} - {result}");
            }
        }

        /// <summary>
        /// Returns password validation rules
        /// </summary>
        /// <returns></returns>
        [HttpGet("password-validation-rules")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PasswordValidationRulesDto), (int)HttpStatusCode.OK)]
        public IActionResult GetPasswordValidationRules()
        {
            return Ok(_passwordValidator.GetPasswordValidationRules());
        }

        /// <summary>
        /// Resets the password of a customer.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **ThereIsNoIdentifierForThisCustomer**
        /// - **ReachedMaximumRequestForPeriod**
        /// - **NoCustomerWithSuchEmail**
        /// - **IdentifierMismatch**
        /// - **ProvidedIdentifierHasExpired**
        /// - **CustomerDoesNotExist**
        /// - **InvalidPasswordFormat**
        /// </remarks>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task ResetPasswordAsync([FromBody] ResetPasswordRequestModel model)
        {
            _log.Info(
                $"Reset password opened with {nameof(model.ResetIdentifier)}: '{model.ResetIdentifier}' by {HttpContext?.Connection?.RemoteIpAddress?.ToString().SanitizeIp()}");

            var result =
                await _customerService.ResetPasswordAsync(model.CustomerEmail, model.ResetIdentifier, model.Password);

            if (result.Error != PasswordResetErrorCodes.None)
                _log.Warning(result.Error.ToString());

            switch (result.Error)
            {
                case PasswordResetErrorCodes.None:
                    _log.Info($"Reset password success with {nameof(model.ResetIdentifier)}: '{model.ResetIdentifier}'");
                    return;
                case PasswordResetErrorCodes.ThereIsNoIdentifierForThisCustomer:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ThereIsNoIdentifierForThisCustomer);
                case PasswordResetErrorCodes.ReachedMaximumRequestForPeriod:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReachedMaximumRequestForPeriod);
                case PasswordResetErrorCodes.NoCustomerWithSuchEmail:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NoCustomerWithSuchEmail);
                case PasswordResetErrorCodes.IdentifierMismatch:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.IdentifierMismatch);
                case PasswordResetErrorCodes.ProvidedIdentifierHasExpired:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ProvidedIdentifierHasExpired);
                case PasswordResetErrorCodes.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case PasswordResetErrorCodes.InvalidPasswordFormat:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidPasswordFormat);
                case PasswordResetErrorCodes.CustomerBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerBlocked);
                default:
                    throw new InvalidOperationException
                        ($"Unexpected error during password reset for {_requestContext.UserId} - {result}");
            }
        }

        /// <summary>
        /// Refreshes the customer authorization token
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [LykkeAuthorize]
        [SwaggerOperation("RefreshToken")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task RefreshTokenAsync()
        {
            _log.Info($"Request to refresh session token for customer {_requestContext.UserId}",
                new { customerId = _requestContext.UserId });

            await _sessionsServiceClient.SessionsApi.RefreshSessionAsync(_requestContext.SessionToken);
        }

        /// <summary>
        /// Deactivates the profile of the customer
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **CustomerProfileDoesNotExist**
        /// - **CustomerIsNotActive**
        /// </remarks>
        [HttpPut("deactivate")]
        [LykkeAuthorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task RequestDeactivationAsync()
        {
            var customerId = _requestContext.UserId;
            var resultError = await _customerProfileClient.CustomerProfiles.RequestDeactivationAsync(customerId);

            switch (resultError)
            {
                case CustomerProfileErrorCodes.None:
                    await _sessionsServiceClient.SessionsApi.DeleteClientSessionsAsync(customerId);
                    break;
                case CustomerProfileErrorCodes.CustomerProfileDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerProfileDoesNotExist);
                case CustomerProfileErrorCodes.CustomerIsNotActive:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerIsNotActive);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Checks if the provided pin code matches the customer's one
        /// </summary>
        /// Error codes:
        /// <remarks>
        /// Error codes:
        /// - **PinIsNotSet**
        /// - **PinCodeMismatch**
        /// </remarks>
        [HttpPost("pin/check")]
        [LykkeAuthorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task CheckPinCodeAsync([FromBody] PinRequestModel request)
        {
            var customerId = _requestContext.UserId;

            var result = await _credentialsClient.Api.ValidatePinAsync(new ValidatePinRequest
            {
                CustomerId = customerId, PinCode = request.Pin
            });

            switch (result.Error)
            {
                case PinCodeErrorCodes.None:
                    return;
                case PinCodeErrorCodes.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case PinCodeErrorCodes.PinIsNotSet:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PinIsNotSet);
                case PinCodeErrorCodes.PinCodeMismatch:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PinCodeMismatch);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Sets customer's pin code.
        /// </summary>
        /// Error codes:
        /// <remarks>
        /// Error codes:
        /// - **InvalidPin**
        /// - **PinAlreadySet**
        /// </remarks>
        [HttpPost("pin")]
        [LykkeAuthorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task SetPinCodeAsync([FromBody] PinRequestModel request)
        {
            var customerId = _requestContext.UserId;

            var result = await _credentialsClient.Api.SetPinAsync(new SetPinRequest
            {
                CustomerId = customerId,
                PinCode = request.Pin
            });

            switch (result.Error)
            {
                case PinCodeErrorCodes.None:
                    return;
                case PinCodeErrorCodes.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case PinCodeErrorCodes.InvalidPin:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidPin);
                case PinCodeErrorCodes.PinAlreadySet:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PinAlreadySet);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Sets customer's pin code.
        /// </summary>
        /// Error codes:
        /// <remarks>
        /// Error codes:
        /// - **InvalidPin**
        /// - **PinIsNotSet**
        /// </remarks>
        [HttpPut("pin")]
        [LykkeAuthorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task UpdatePinCodeAsync([FromBody] PinRequestModel request)
        {
            var customerId = _requestContext.UserId;

            var result = await _credentialsClient.Api.UpdatePinAsync(new SetPinRequest
            {
                CustomerId = customerId,
                PinCode = request.Pin
            });

            switch (result.Error)
            {
                case PinCodeErrorCodes.None:
                    return;
                case PinCodeErrorCodes.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case PinCodeErrorCodes.InvalidPin:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidPin);
                case PinCodeErrorCodes.PinIsNotSet:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PinIsNotSet);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
