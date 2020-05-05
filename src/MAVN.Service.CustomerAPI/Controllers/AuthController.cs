using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MAVN.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Infrastructure.Extensions;
using MAVN.Service.CustomerAPI.Models.Auth;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerManagement.Client.Models.Requests;
using MAVN.Service.Sessions.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ISessionsServiceClient _sessionsServiceClient;
        private readonly ICustomerManagementServiceClient _customerManagementServiceClient;
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly ISigninThrottlingService _signinThrottlingService;

        public AuthController(
            ISessionsServiceClient sessionsServiceClient,
            ICustomerManagementServiceClient customerManagementServiceClient,
            IRequestContext requestContext,
            IMapper mapper,
            IAuthService authService, 
            ISigninThrottlingService signinThrottlingService)
        {
            _sessionsServiceClient = sessionsServiceClient;
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
            _customerManagementServiceClient = customerManagementServiceClient;
            _mapper = mapper;
            _authService = authService;
            _signinThrottlingService = signinThrottlingService;
        }

        /// <summary>
        /// Login the client.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidEmailFormat**
        /// - **InvalidCredentials**
        /// - **InvalidPasswordFormat**
        /// - **LoginExistsWithDifferentProvider**
        /// - **CustomerBlocked**
        /// - **LoginAttemptsWarning**
        /// - **CustomerIsNotActive**
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation("Login")]
        [ProducesResponseType(typeof(LoginResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        public async Task<IActionResult> Login([FromBody]LoginRequestModel model)
        {
            if (!model.Email.IsValidEmailAndRowKey())
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailFormat);

            var signinLockStatus = await _signinThrottlingService.IsSigninLockedAsync(model.Email);

            if (signinLockStatus.IsLocked)
                return signinLockStatus.GetSigninLockedResponse();
            
            var result = await _customerManagementServiceClient.AuthApi.AuthenticateAsync(
                _mapper.Map<AuthenticateRequestModel>(model));
            
            if (result.Error.IsPasswordWrong())
            {
                var failedSigninResult = await _signinThrottlingService.RegisterFailedSigninAsync(model.Email);

                if (failedSigninResult.Effect != FailedSigninEffect.None)
                {
                    if (failedSigninResult.Effect == FailedSigninEffect.Warning)
                        return failedSigninResult.GetWarningResponse();
                    
                    if (failedSigninResult.Effect == FailedSigninEffect.SigninLocked)
                        return failedSigninResult.GetSigninLockedResponse();
                    
                    throw new InvalidOperationException($"Unexpected throttling effect during authentication for {model.Email.SanitizeEmail()} - {failedSigninResult.Effect.ToString()}");
                }
            }

            switch (result.Error)
            {
                case CustomerManagementError.None:
                    await _signinThrottlingService.FlushFailuresAsync(model.Email);
                    return Ok(_mapper.Map<LoginResponseModel>(result));
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
                case CustomerManagementError.CustomerProfileDeactivated:
                    throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.CustomerIsNotActive);
                default:
                    throw new InvalidOperationException($"Unexpected error during Authenticate for {model.Email.SanitizeEmail()} - {result.Error.ToString()}");
            }
        }

        /// <summary>
        /// Login the client.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidCredentials**
        /// - **LoginExistsWithDifferentProvider**
        /// - **InvalidOrExpiredGoogleAccessToken**
        /// - **CustomerIsNotActive**
        /// - **CustomerBlocked**
        /// </remarks>
        [HttpPost("google-login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GoogleLogin([FromBody]GoogleLoginRequestModel model)
        {

            var result = await _authService.GoogleAuthenticateAsync(model.AccessToken);

            switch (result.Error)
            {
                case CustomerError.None:
                    return Ok(_mapper.Map<LoginResponseModel>(result));
                case CustomerError.LoginNotFound:
                    throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.InvalidCredentials);
                case CustomerError.LoginExistsWithDifferentProvider:
                    throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.LoginExistsWithDifferentProvider);
                case CustomerError.InvalidOrExpiredGoogleAccessToken:
                    throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.InvalidOrExpiredGoogleAccessToken);
                case CustomerError.CustomerBlocked:
                    throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.CustomerBlocked);
                case CustomerError.CustomerProfileDeactivated:
                    throw LykkeApiErrorException.Unauthorized(ApiErrorCodes.Service.CustomerIsNotActive);
                default:
                    throw new InvalidOperationException(
                        $"Unexpected error during Authenticate with access token {model.AccessToken} - {result.Error}");
            }
        }

        /// <summary>
        /// Logout the client.
        /// </summary>
        [HttpPost("logout")]
        [LykkeAuthorize]
        [SwaggerOperation("logout")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await _sessionsServiceClient.SessionsApi.DeleteSessionIfExistsAsync(_requestContext.SessionToken);

            return Ok();
        }
    }
}
