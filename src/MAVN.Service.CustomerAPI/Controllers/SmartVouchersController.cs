using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Falcon.Common.Middleware.Version;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.PartnerManagement.Client;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Models.Enums;
using MAVN.Service.CustomerAPI.Models.SmartVouchers;
using MAVN.Service.SmartVouchers.Client;
using MAVN.Service.SmartVouchers.Client.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/smartVouchers")]
    [LowerVersion(Devices = "android", LowerVersion = 659)]
    [LowerVersion(Devices = "IPhone,IPad", LowerVersion = 181)]
    public class SmartVouchersController : ControllerBase
    {
        private readonly ISmartVouchersClient _smartVouchersClient;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IMapper _mapper;

        public SmartVouchersController(
            ISmartVouchersClient smartVouchersClient,
            IPartnerManagementClient partnerManagementClient,
            IMapper mapper)
        {
            _smartVouchersClient = smartVouchersClient;
            _partnerManagementClient = partnerManagementClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of smart voucher campaigns.
        /// </summary>
        /// <remarks>
        /// Used to get collection of smart voucher campaigns.
        /// </remarks>
        /// <returns>
        /// 200 - a collection of smart voucher campaigns.
        /// </returns>
        [HttpGet("campaigns")]
        [ProducesResponseType(typeof(SmartVoucherCampaignsListResponse), (int) HttpStatusCode.OK)]
        public async Task<SmartVoucherCampaignsListResponse> GetSmartVouchersCampaignsAsync([FromQuery] GetSmartVoucherCampaignsRequest request)
        {
            var paginatedCampaigns = await _smartVouchersClient.CampaignsApi.GetAsync(new VoucherCampaignsPaginationRequestModel
            {
                CampaignName = request.CampaignName,
                CurrentPage = request.CurrentPage,
                PageSize = request.PageSize,
                OnlyActive = request.OnlyActive
            });

            var result = _mapper.Map<SmartVoucherCampaignsListResponse>(paginatedCampaigns);

            var partnersIds = result.SmartVoucherCampaigns.Select(x => Guid.Parse(x.PartnerId)).ToArray();

            var partnersVerticalsDictionary =
                (await _partnerManagementClient.Partners.GetByIdsAsync(partnersIds)).ToDictionary(k => k.Id,
                    v => v.BusinessVertical);

            foreach (var campaign in result.SmartVoucherCampaigns)
            {
                campaign.Vertical = (BusinessVertical?)partnersVerticalsDictionary[Guid.Parse(campaign.PartnerId)];
            }

            return result;
        }

        /// <summary>
        /// Returns smart voucher campaign details.
        /// </summary>
        /// <returns>
        /// 200 - smart voucher campaign.
        /// </returns>
        [HttpGet("campaigns/{id}")]
        [ProducesResponseType(typeof(SmartVoucherCampaignDetailsModel), (int)HttpStatusCode.OK)]
        public async Task<SmartVoucherCampaignDetailsModel> GetSmartVouchersCampaignByIdAsync([FromRoute] Guid id)
        {
            var campaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(id);

            if (campaign == null)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);

            var result = _mapper.Map<SmartVoucherCampaignDetailsModel>(campaign);

            var partner = await _partnerManagementClient.Partners.GetByIdAsync(Guid.Parse(campaign.PartnerId));

            result.Vertical = (BusinessVertical?) partner.BusinessVertical;

            return result;
        }
    }
}
