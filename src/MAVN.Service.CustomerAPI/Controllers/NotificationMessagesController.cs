using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models;
using MAVN.Service.CustomerAPI.Models.NotificationMessages;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/notificationMessages")]
    public class NotificationMessagesController : ControllerBase
    {
        private readonly INotificationMessagesService _notificationMessagesService;
        private readonly IMapper _mapper;
        private readonly IRequestContext _requestContext;

        public NotificationMessagesController(INotificationMessagesService notificationMessagesService, IMapper mapper,
            IRequestContext requestContext)
        {
            _notificationMessagesService = notificationMessagesService;
            _mapper = mapper;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Retrieve notifications messages for a customer
        /// </summary>
        /// <param name="request">Current page and page size</param>
        /// <returns>Paginated list of notification messages for specified customer</returns>
        [HttpGet]
        [SwaggerOperation("Retrieve notifications messages for a customer")]
        [ProducesResponseType(typeof(PaginatedNotificationMessagesResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<PaginatedNotificationMessagesResponse> GetCustomerNotificationMessagesAsync(
            [FromQuery] PaginationRequestModel request)
        {
            var customerId = _requestContext.UserId;

            var result = await _notificationMessagesService.GetAsync(customerId, request.CurrentPage, request.PageSize);

            return _mapper.Map<PaginatedNotificationMessagesResponse>(result);
        }

        /// <summary>
        /// Mark a message as read
        /// </summary>
        /// <param name="model">Mark message as read request model</param>
        /// <returns></returns>
        [HttpPost("read")]
        [SwaggerOperation("Mark a message as read")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task MarkMessageAsReadAsync(MarkMessageAsReadRequestModel model)
        {
            await _notificationMessagesService.MarkMessageAsReadAsync(model.MessageGroupId);
        }

        /// <summary>
        /// Get number of unread messages for customer
        /// </summary>
        /// <returns></returns>
        [HttpGet("unread/count")]
        [SwaggerOperation("Get number of unread messages for customer")]
        [ProducesResponseType(typeof(UnreadMessagesCountResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<UnreadMessagesCountResponse> GetNumberOfUnreadMessagesForCustomerAsync()
        {
            var customerId = _requestContext.UserId;

            var result = await _notificationMessagesService.GetNumberOfUnreadMessagesAsync(customerId);

            return new UnreadMessagesCountResponse { UnreadMessagesCount = result };
        }

        /// <summary>
        /// Mark all messages as read for a customer
        /// </summary>
        /// <returns></returns>
        [HttpPost("read/all")]
        [SwaggerOperation("Mark all messages as read for a customer")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task MarkAllCustomerMessagesAsReadAsync()
        {
            var customerId = _requestContext.UserId;

            await _notificationMessagesService.MarkAllCustomerMessagesAsReadAsync(customerId);
        }
    }
}
