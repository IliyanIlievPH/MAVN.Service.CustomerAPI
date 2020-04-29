using System;
using System.ComponentModel.DataAnnotations;
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
using MAVN.Service.SmartVouchers.Client.Models.Responses.Enums;
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
        private readonly IRequestContext _requestContext;
        private readonly ISmartVouchersClient _smartVouchersClient;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IMapper _mapper;

        public SmartVouchersController(
            IRequestContext requestContext,
            ISmartVouchersClient smartVouchersClient,
            IPartnerManagementClient partnerManagementClient,
            IMapper mapper)
        {
            _requestContext = requestContext;
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
        [ProducesResponseType(typeof(SmartVoucherCampaignsListResponse), (int)HttpStatusCode.OK)]
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
                    v => (v.BusinessVertical,v.Name));

            foreach (var campaign in result.SmartVoucherCampaigns)
            {
                var (businessVertical, name) = partnersVerticalsDictionary[Guid.Parse(campaign.PartnerId)];
                campaign.Vertical = (BusinessVertical?)businessVertical;
                campaign.PartnerName = name;
            }

            return result;
        }

        /// <summary>
        /// Returns smart voucher campaign details.
        /// </summary>
        /// <returns>
        /// 200 - smart voucher campaign.
        /// </returns>
        [HttpGet("campaigns/search")]
        [ProducesResponseType(typeof(SmartVoucherCampaignDetailsModel), (int)HttpStatusCode.OK)]
        public async Task<SmartVoucherCampaignDetailsModel> GetSmartVouchersCampaignByIdAsync([FromQuery] Guid id)
        {
            if(id == default)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);

            var campaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(id);

            if (campaign == null)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);

            var result = _mapper.Map<SmartVoucherCampaignDetailsModel>(campaign);

            var partner = await _partnerManagementClient.Partners.GetByIdAsync(campaign.PartnerId);

            result.Vertical = (BusinessVertical?)partner.BusinessVertical;
            result.PartnerName = partner.Name;

            return result;
        }

        /// <summary>
        /// Reserves a smart voucher
        /// </summary>
        /// <returns>
        /// 200 - payment url
        /// </returns>
        [HttpPost("reserve")]
        [ProducesResponseType(typeof(ReserveSmartVoucherResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ReserveSmartVoucherResponse> ReserveSmartVoucherAsync([FromBody] ReserveSmartVoucherRequest request)
        {
            var customerId = Guid.Parse(_requestContext.UserId);
            var result = await _smartVouchersClient.VouchersApi.ReserveVoucherAsync(new VoucherProcessingModel
            {
                CustomerId = customerId,
                VoucherCampaignId = request.SmartVoucherCampaignId
            });

            switch (result.ErrorCode)
            {
                case ProcessingVoucherErrorCodes.None:
                    return new ReserveSmartVoucherResponse { PaymentUrl = result.PaymentUrl };
                case ProcessingVoucherErrorCodes.VoucherCampaignNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);
                case ProcessingVoucherErrorCodes.VoucherCampaignNotActive:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotActive);
                case ProcessingVoucherErrorCodes.NoAvailableVouchers:
                case ProcessingVoucherErrorCodes.InvalidPartnerPaymentConfiguration:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NoAvailableVouchers);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Cancels smart voucher reservation
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost("cancelReservation")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task CancelSmartVoucherReservationAsync([FromBody] CancelSmartVoucherReservationRequest request)
        {
            var result = await _smartVouchersClient.VouchersApi.CancelVoucherReservationAsync(new VoucherCancelReservationModel
            {
                ShortCode = request.ShortCode
            });

            if(result != ProcessingVoucherErrorCodes.None)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherNotFound);
        }

        /// <summary>
        /// Returns smart vouchers for the logged in customer
        /// </summary>
        /// <returns>
        /// 200 - list of smart vouchers
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(SmartVouchersListResponse), (int)HttpStatusCode.OK)]
        public async Task<SmartVouchersListResponse> GetSmartVouchersForCustomerAsync([FromQuery] BasePaginationRequestModel request)
        {
            var customerId = Guid.Parse(_requestContext.UserId);

            var vouchersResponse =
                await _smartVouchersClient.VouchersApi.GetCustomerVouchersAsync(customerId,
                    new BasePaginationRequestModel {CurrentPage = request.CurrentPage, PageSize = request.PageSize});

            var result = _mapper.Map<SmartVouchersListResponse>(vouchersResponse);
            return result;
        }

        /// <summary>
        /// Returns details for a smart voucher
        /// </summary>
        /// <returns>
        /// 200 - smart voucher details
        /// </returns>
        [HttpGet("{voucherShortCode}")]
        [ProducesResponseType(typeof(SmartVoucherDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<SmartVoucherDetailsResponse> GetSmartVouchersForCustomerAsync([FromRoute] string voucherShortCode)
        {
            var voucherResponse = await _smartVouchersClient.VouchersApi.GetByShortCodeAsync(voucherShortCode);

            if(voucherResponse == null)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherNotFound);

            var result = _mapper.Map<SmartVoucherDetailsResponse>(voucherResponse);
            return result;
        }
    }
}
