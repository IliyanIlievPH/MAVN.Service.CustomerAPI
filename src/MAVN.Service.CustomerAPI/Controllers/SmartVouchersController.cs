using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using MAVN.Common.Middleware.Authentication;
using MAVN.Common.Middleware.Version;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Extensions;
using MAVN.Service.CustomerAPI.Models.Enums;
using MAVN.Service.CustomerAPI.Models.SmartVouchers;
using MAVN.Service.CustomerAPI.Models.Vouchers;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Models;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using MAVN.Service.PaymentManagement.Client;
using MAVN.Service.PaymentManagement.Client.Models.Requests;
using MAVN.Service.SmartVouchers.Client;
using MAVN.Service.SmartVouchers.Client.Models.Enums;
using MAVN.Service.SmartVouchers.Client.Models.Requests;
using MAVN.Service.SmartVouchers.Client.Models.Responses.Enums;
using Microsoft.AspNetCore.Mvc;
using Localization = MAVN.Service.SmartVouchers.Client.Models.Enums.Localization;

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
        private readonly IPaymentManagementClient _paymentManagementClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public SmartVouchersController(
            IRequestContext requestContext,
            ISmartVouchersClient smartVouchersClient,
            IPartnerManagementClient partnerManagementClient,
            IPaymentManagementClient paymentManagementClient,
            ICustomerProfileClient customerProfileClient,
            IMapper mapper,
            ILogFactory logFactory)
        {
            _requestContext = requestContext;
            _smartVouchersClient = smartVouchersClient;
            _partnerManagementClient = partnerManagementClient;
            _paymentManagementClient = paymentManagementClient;
            _customerProfileClient = customerProfileClient;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
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
            GetNearPartnersByCoordinatesResponse partnersNearCoordinates = null;
            if (request.Longitude.HasValue && request.Latitude.HasValue || !string.IsNullOrEmpty(request.CountryIso3Code))
            {
                var radiusInKm = request.RadiusInKm ?? 20;
                partnersNearCoordinates = await _partnerManagementClient.Partners.GetNearPartnerByCoordinatesAsync(
                    new GetNearPartnersByCoordinatesRequest
                    {
                        Longitude = request.Longitude,
                        Latitude = request.Latitude,
                        RadiusInKm = radiusInKm,
                        CountryIso3Code = request.CountryIso3Code
                    });
            }

            var paginatedCampaigns = await _smartVouchersClient.CampaignsApi.GetAsync(
                new VoucherCampaignsPaginationRequestModel
                {
                    CampaignName = request.CampaignName,
                    CurrentPage = request.CurrentPage,
                    PageSize = request.PageSize,
                    OnlyActive = true,
                    VoucherCampaignState = VoucherCampaignState.Published,
                    PartnerIds = partnersNearCoordinates != null && partnersNearCoordinates.PartnersIds.Any()
                        ? partnersNearCoordinates.PartnersIds
                        : null
                });

            var result = _mapper.Map<SmartVoucherCampaignsListResponse>(paginatedCampaigns);

            var partnersIds = result.SmartVoucherCampaigns.Select(x => Guid.Parse(x.PartnerId)).ToArray();

            var partners =
                await _partnerManagementClient.Partners.GetByIdsAsync(partnersIds);

            var partnersInfo = partners.ToDictionary(k => k.Id,
                v => (v.BusinessVertical, v.Name,
                    v.Locations.Where(l => l.Latitude.HasValue && l.Longitude.HasValue).Select(x =>
                        new GeolocationModel { Latitude = x.Latitude.Value, Longitude = x.Longitude.Value, Address = x.Address }).ToList()));

            foreach (var campaign in result.SmartVoucherCampaigns)
            {
                var partnerId = Guid.Parse(campaign.PartnerId);
                (Vertical? BusinessVertical, string Name, List<GeolocationModel> Geolocations) partnerInfo;

                if (!partnersInfo.TryGetValue(partnerId, out partnerInfo))
                {
                    _log.Warning("Smart voucher campaign partner does not exist", context: new { partnerId, campaignId = campaign.Id });
                    continue;
                }

                campaign.Vertical = (BusinessVertical?)partnerInfo.BusinessVertical;
                campaign.PartnerName = partnerInfo.Name;
                campaign.Geolocations = partnerInfo.Geolocations;
            }

            return result;
        }

        /// <summary>
        /// Returns smart voucher campaign details.
        /// </summary>
        /// <returns>
        /// 200 - smart voucher campaign.
        /// </returns>
        /// <remarks>
        /// Error codes:
        /// - **SmartVoucherCampaignNotFound**
        /// </remarks>
        [HttpGet("campaigns/search")]
        [ProducesResponseType(typeof(SmartVoucherCampaignDetailsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<SmartVoucherCampaignDetailsModel> GetSmartVouchersCampaignByIdAsync([FromQuery] Guid id)
        {
            if (id == default)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);

            var campaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(id);

            if (campaign == null)
                throw LykkeApiErrorException.NotFound(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);

            var result = _mapper.Map<SmartVoucherCampaignDetailsModel>(campaign);

            var partner = await _partnerManagementClient.Partners.GetByIdAsync(campaign.PartnerId);

            if (partner == null)
            {
                _log.Warning("Smart voucher campaign partner does not exist", context: new { campaign.PartnerId, campaignId = campaign.Id });
                throw LykkeApiErrorException.NotFound(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);
            }

            var geolocations = partner.Locations.Where(l => l.Longitude.HasValue && l.Latitude.HasValue)
                .Select(l => new GeolocationModel { Latitude = l.Latitude.Value, Longitude = l.Longitude.Value, Address = l.Address })
                .ToList();

            result.Vertical = (BusinessVertical?)partner.BusinessVertical;
            result.PartnerName = partner.Name;
            result.Geolocations = geolocations;

            return result;
        }

        /// <summary>
        /// Reserves a smart voucher
        /// </summary>
        /// <returns>
        /// 200 - payment url
        /// </returns>
        /// <remarks>
        /// Error codes:
        /// - **SmartVoucherCampaignNotFound**
        /// - **SmartVoucherCampaignNotActive**
        /// - **NoAvailableVouchers**
        /// - **PaymentProviderError**
        /// - **SmartVoucherNotFound**
        /// </remarks>
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
                    return new ReserveSmartVoucherResponse { PaymentUrl = result.PaymentUrl, ErrorCode = ReserveSmartVoucherErrorCodes.None };
                case ProcessingVoucherErrorCodes.VoucherCampaignNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);
                case ProcessingVoucherErrorCodes.VoucherCampaignNotActive:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotActive);
                case ProcessingVoucherErrorCodes.NoAvailableVouchers:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NoAvailableVouchers);
                case ProcessingVoucherErrorCodes.InvalidPartnerPaymentConfiguration:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PaymentProviderError);
                case ProcessingVoucherErrorCodes.VoucherNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherNotFound);
                case ProcessingVoucherErrorCodes.CustomerHaveAnotherReservedVoucher:
                    return new ReserveSmartVoucherResponse
                    {
                        ErrorCode = ReserveSmartVoucherErrorCodes.CustomerHaveAnotherReservedVoucher,
                        AlreadyReservedVoucherShortCode = result.AlreadyReservedVoucherShortCode
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Cancels smart voucher reservation
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **SmartVoucherNotFound**
        /// </remarks>
        [HttpPost("cancelReservation")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task CancelSmartVoucherReservationAsync([FromBody] CancelSmartVoucherReservationRequest request)
        {
            var result = await _smartVouchersClient.VouchersApi.CancelVoucherReservationAsync(new VoucherCancelReservationModel
            {
                ShortCode = request.ShortCode
            });

            if (result != ProcessingVoucherErrorCodes.None)
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
                    new BasePaginationRequestModel { CurrentPage = request.CurrentPage, PageSize = request.PageSize });

            var result = _mapper.Map<SmartVouchersListResponse>(vouchersResponse);

            if (!result.SmartVouchers.Any())
                return result;

            var campaignIds = vouchersResponse.Vouchers.Select(x => x.CampaignId).Distinct().ToArray();
            var campaigns = await _smartVouchersClient.CampaignsApi.GetCampaignsByIds(campaignIds);
            var campaignsDict = campaigns.Campaigns.ToDictionary(k => k.Id,
                v => new SmartVoucherCampaignDto
                {
                    Name = v.GetContentValue(Localization.En, VoucherCampaignContentType.Name),
                    ImageUrl = v.GetContentValue(Localization.En, VoucherCampaignContentType.ImageUrl),
                    Description = v.GetContentValue(Localization.En, VoucherCampaignContentType.Description),
                    VoucherPrice = v.VoucherPrice,
                    PartnerId = v.PartnerId,
                    ExpirationDate = v.ExpirationDate,
                    Currency = v.Currency
                });

            var partnerIds = campaigns.Campaigns.Select(c => c.PartnerId).Distinct().ToArray();
            var partners = await _partnerManagementClient.Partners.GetByIdsAsync(partnerIds);
            var partnersDict = partners.ToDictionary(k => k.Id, v => v.Name);

            foreach (var voucher in result.SmartVouchers)
            {
                if (!campaignsDict.TryGetValue(voucher.CampaignId, out var campaignInfo))
                {
                    _log.Warning("Smart voucher campaign is missing for existing voucher", context: new { VoucherShortCode = voucher.ShortCode, voucher.CampaignId });
                    continue;
                }

                voucher.CampaignName = campaignInfo.Name;
                voucher.ImageUrl = campaignInfo.ImageUrl;
                voucher.ExpirationDate = campaignInfo.ExpirationDate;
                voucher.PartnerId = campaignInfo.PartnerId;
                voucher.Description = campaignInfo.Description;
                voucher.Price = campaignInfo.VoucherPrice;
                voucher.Currency = campaignInfo.Currency;

                if (!partnersDict.TryGetValue(campaignInfo.PartnerId, out var partnerName))
                {
                    _log.Warning("Partner is missing for existing smart voucher campaign", context: new { campaignInfo.PartnerId, voucher.CampaignId });
                    continue;
                }

                voucher.PartnerName = partnerName;
            }

            return result;
        }

        /// <summary>
        /// Returns details for a smart voucher
        /// </summary>
        /// <returns>
        /// 200 - smart voucher details
        /// </returns>
        /// <remarks>
        /// Error codes:
        /// - **SmartVoucherNotFound**
        /// </remarks>
        [HttpGet("voucherShortCode")]
        [ProducesResponseType(typeof(SmartVoucherDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<SmartVoucherDetailsResponse> GetSmartVoucherByShortCodeAsync([FromQuery] string voucherShortCode)
        {
            var voucherResponse = await _smartVouchersClient.VouchersApi.GetByShortCodeAsync(voucherShortCode);

            if (voucherResponse == null)
                throw LykkeApiErrorException.NotFound(ApiErrorCodes.Service.SmartVoucherNotFound);

            var result = _mapper.Map<SmartVoucherDetailsResponse>(voucherResponse);

            var campaign = await _smartVouchersClient.CampaignsApi.GetByIdAsync(voucherResponse.CampaignId);

            if (campaign == null)
            {
                _log.Warning("Smart voucher campaign is missing for existing voucher", context: new { VoucherShortCode = voucherResponse.ShortCode, voucherResponse.CampaignId });
                return result;
            }

            var partner = await _partnerManagementClient.Partners.GetByIdAsync(campaign.PartnerId);

            result.CampaignName = campaign.GetContentValue(Localization.En, VoucherCampaignContentType.Name);
            result.PartnerId = campaign.PartnerId;
            result.ExpirationDate = campaign.ExpirationDate;
            result.PartnerName = partner?.Name;
            result.ImageUrl = campaign.GetContentValue(Localization.En, VoucherCampaignContentType.ImageUrl);
            result.Description = campaign.GetContentValue(Localization.En, VoucherCampaignContentType.Description);
            result.Price = campaign.VoucherPrice;
            result.Currency = campaign.Currency;

            return result;
        }

        /// <summary>
        /// Returns payment info for a smart voucher
        /// </summary>
        /// <returns>
        /// 200 - smart voucher payment info
        /// 404 - not found
        /// </returns>
        /// <remarks>
        /// Error codes:
        /// - **PaymentInfoNotFound**
        /// </remarks>
        [HttpGet("paymentUrl")]
        [ProducesResponseType(typeof(SmartVoucherPaymentInfoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<SmartVoucherPaymentInfoResponse> GetPaymentInfoAsync([FromQuery] GetSmartVoucherPaymentInfoRequest request)
        {
            var result = await _paymentManagementClient.Api.GetPaymentInfoAsync(new GetPaymentInfoRequest
            {
                ExternalPaymentEntityId = request.ShortCode
            });

            if (result.PaymentUrl == null)
                throw LykkeApiErrorException.NotFound(ApiErrorCodes.Service.PaymentInfoNotFound);

            return new SmartVoucherPaymentInfoResponse { PaymentUrl = result.PaymentUrl };
        }

        /// <summary>
        /// Redeem a voucher.
        /// </summary>
        /// <param name="request">The request that describes voucher redemption request.</param>
        /// <remarks>
        /// Error codes:
        /// - **SmartVoucherNotFound**
        /// - **SmartVoucherCampaignNotFound**
        /// - **SmartVoucherCampaignNotActive**
        /// - **WrongSmartVoucherValidationCode**
        /// - **SellerCustomerIsNotALinkedPartner**
        /// - **SellerCustomerIsNotTheVoucherIssuer**
        /// - **VoucherIsNotInCorrectStatusToBeRedeemed**
        /// </remarks>
        [HttpPost("usage")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task RedeemVoucherAsync([FromBody] VoucherRedemptionRequest request)
        {
            var requestModel = _mapper.Map<VoucherRedeptionModel>(request);
            requestModel.SellerCustomerId = Guid.Parse(_requestContext.UserId);
            var error = await _smartVouchersClient.VouchersApi.RedeemVoucherAsync(requestModel);

            switch (error)
            {
                case RedeemVoucherErrorCodes.None:
                    return;
                case RedeemVoucherErrorCodes.VoucherNotFound:
                    throw LykkeApiErrorException.NotFound(ApiErrorCodes.Service.SmartVoucherNotFound);
                case RedeemVoucherErrorCodes.WrongValidationCode:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.WrongSmartVoucherValidationCode);
                case RedeemVoucherErrorCodes.VoucherCampaignNotFound:
                    throw LykkeApiErrorException.NotFound(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);
                case RedeemVoucherErrorCodes.VoucherCampaignNotActive:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotActive);
                case RedeemVoucherErrorCodes.SellerCustomerIsNotALinkedPartner:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SellerCustomerIsNotALinkedPartner);
                case RedeemVoucherErrorCodes.SellerCustomerIsNotTheVoucherIssuer:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SellerCustomerIsNotTheVoucherIssuer);
                case RedeemVoucherErrorCodes.VoucherIsNotInCorrectStatusToBeRedeemed:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.VoucherIsNotInCorrectStatusToBeRedeemed);
            }
        }

        /// <summary>
        /// Redeem a voucher.
        /// </summary>
        /// <param name="request">The request that describes voucher redemption request.</param>
        /// <remarks>
        /// Error codes:
        /// - **CustomerDoesNotExist**
        /// - **SmartVoucherNotFound**
        /// - **LoggedCustomerIsNotOwnerOfTheSmartVoucher**
        /// - **VoucherIsNotInTheCorrectStateToTransfer**
        /// - **SmartVoucherCampaignNotFound**
        /// </remarks>
        [HttpPost("transfer")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task TransferVoucherAsync([FromBody] SmartVoucherTransferRequest request)
        {
            var receiverCustomer =
                await _customerProfileClient.CustomerProfiles.GetByEmailAsync(
                    new GetByEmailRequestModel { Email = request.ReceiverEmail });

            if (receiverCustomer.Profile == null)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);

            var error = await _smartVouchersClient.VouchersApi.TransferVoucherAsync(new VoucherTransferModel
            {
                NewOwnerId = Guid.Parse(receiverCustomer.Profile.CustomerId),
                OldOwnerId = Guid.Parse(_requestContext.UserId),
                VoucherShortCode = request.VoucherShortCode,
            });

            switch (error)
            {
                case TransferVoucherErrorCodes.None:
                    return;
                case TransferVoucherErrorCodes.VoucherNotFound:
                    throw LykkeApiErrorException.NotFound(ApiErrorCodes.Service.SmartVoucherNotFound);
                case TransferVoucherErrorCodes.NotAnOwner:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LoggedCustomerIsNotOwnerOfTheSmartVoucher);
                case TransferVoucherErrorCodes.VoucherIsNotInTheCorrectStateToTransfer:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.VoucherIsNotInTheCorrectStateToTransfer);
                case TransferVoucherErrorCodes.VoucherCampaignNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SmartVoucherCampaignNotFound);
            }
        }
    }
}
