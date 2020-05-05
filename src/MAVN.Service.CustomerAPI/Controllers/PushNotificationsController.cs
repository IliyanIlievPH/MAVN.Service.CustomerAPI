using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.Middleware.Authentication;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.PushNotifications;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/pushNotifications")]
    [ApiController]
    [LykkeAuthorize]
    public class PushNotificationsController : ControllerBase
    {
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;

        public PushNotificationsController(IPushNotificationService pushNotificationService,
            IRequestContext requestContext, IMapper mapper)
        {
            _pushNotificationService = pushNotificationService;
            _requestContext = requestContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Register customer for push notifications
        /// </summary>
        /// <param name="model">Push notification registration model</param>
        /// <returns>
        /// 200
        /// Result Codes:
        /// - **Ok**
        /// - **PushRegistrationAlreadyExists**
        /// </returns>
        [HttpPost("registrations")]
        [SwaggerOperation("Register for push notifications")]
        [ProducesResponseType(typeof(PushNotificationRegisterResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public async Task<PushNotificationRegisterResponseModel> RegisterForPushNotificationsAsync(
            PushNotificationRegisterRequestModel model)
        {
            var result = await _pushNotificationService.RegisterForPushNotificationsAsync(_requestContext.UserId,
                _mapper.Map<PushNotificationRegistrationCreateModel>(model));

            return new PushNotificationRegisterResponseModel {ResultCode = result};
        }

        [HttpDelete("registrations")]
        [SwaggerOperation("Cancel registration for push notifications")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task CancelPushRegistrationNotificationsAsync([FromQuery] string pushRegistrationToken)
        {
            await _pushNotificationService.CancelPushRegistrationNotificationsAsync(pushRegistrationToken);
        }
    }
}
