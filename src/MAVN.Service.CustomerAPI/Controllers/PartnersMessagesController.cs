using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using JetBrains.Annotations;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.PartnersMessages;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/partners")]
    [ApiController]
    public class PartnersMessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPartnersMessagesService _partnersMessagesService;
        private readonly IRequestContext _requestContext;

        public PartnersMessagesController(IMapper mapper, IPartnersMessagesService partnersMessagesService,
            IRequestContext requestContext)
        {
            _mapper = mapper;
            _partnersMessagesService = partnersMessagesService;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Get partner message by id
        /// </summary>
        /// <param name="partnerMessageId">Partner message id</param>
        /// <returns><see cref="GetPartnerMessageResponseModel"/></returns>
        [LykkeAuthorize]
        [HttpGet("messages")]
        [SwaggerOperation("Get partner message by id")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public async Task<GetPartnerMessageResponseModel> GetPartnerMessageAsync(
            [FromQuery] [Required, NotNull] string partnerMessageId)
        {
            var result = await _partnersMessagesService.GetPartnerMessageAsync(partnerMessageId);

            if (result.CustomerId != _requestContext.UserId)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.MessageRequestsIsForAnotherCustomer);

            return _mapper.Map<GetPartnerMessageResponseModel>(result);
        }
    }
}
