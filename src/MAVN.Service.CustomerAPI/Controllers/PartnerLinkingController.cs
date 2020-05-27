using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Common.Middleware.Authentication;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Enums;
using MAVN.Service.PartnerManagement.Client.Models.PartnerLinking;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/partnersLinking")]
    [ApiController]
    public class PartnerLinkingController : ControllerBase
    {
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IRequestContext _requestContext;

        public PartnerLinkingController(IPartnerManagementClient partnerManagementClient, IRequestContext requestContext)
        {
            _partnerManagementClient = partnerManagementClient;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Links the logged customer to a partner
        /// </summary>
        [HttpPost]
        [LykkeAuthorize]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task LinkToPartnerAsync([FromBody] Models.PartnersLinking.LinkPartnerRequest request)
        {
            var result = await _partnerManagementClient.Linking.LinkPartnerAsync(new LinkPartnerRequest
            {
                PartnerCode = request.PartnerCode,
                PartnerLinkingCode = request.PartnerLinkingCode,
                CustomerId = Guid.Parse(_requestContext.UserId),
            });

            switch (result.Error)
            {
                case PartnerLinkingErrorCode.None:
                    break;
                case PartnerLinkingErrorCode.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case PartnerLinkingErrorCode.CustomerAlreadyLinked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerAlreadyLinkedToAPartner);
                case PartnerLinkingErrorCode.PartnerLinkingInfoDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PartnerLinkingInfoDoesNotExist);
                case PartnerLinkingErrorCode.PartnerLinkingInfoDoesNotMatch:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PartnerLinkingInfoDoesNotMatch);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
