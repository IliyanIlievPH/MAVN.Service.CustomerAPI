using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Falcon.Common.Middleware.Authentication;
using Falcon.Common.Middleware.Version;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Campaign.Client.Models.BurnRule.Responses;
using Lykke.Service.Campaign.Client.Models.Enums;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Models;
using MAVN.Service.CustomerAPI.Models.Vouchers;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.PartnerManagement.Client.Models.Partner;
using Lykke.Service.Vouchers.Client;
using Lykke.Service.Vouchers.Client.Models;
using Lykke.Service.Vouchers.Client.Models.Vouchers;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Nito.AsyncEx;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/vouchers")]
    [LowerVersion(Devices = "android", LowerVersion = 659)]
    [LowerVersion(Devices = "IPhone,IPad", LowerVersion = 181)]
    public class VouchersController : ControllerBase
    {
        private readonly IRequestContext _requestContext;
        private readonly IVouchersClient _vouchersClient;
        private readonly ICampaignClient _campaignClient;
        private readonly IPartnerManagementClient _partnerManagementClient;

        public VouchersController(
            IRequestContext requestContext,
            IVouchersClient vouchersClient,
            ICampaignClient campaignClient,
            IPartnerManagementClient partnerManagementClient)
        {
            _vouchersClient = vouchersClient;
            _campaignClient = campaignClient;
            _requestContext = requestContext;
            _partnerManagementClient = partnerManagementClient;
        }

        /// <summary>
        /// Returns a collection of vouchers.
        /// </summary>
        /// <remarks>
        /// Used to get collection of vouchers.
        /// </remarks>
        /// <returns>
        /// 200 - a collection of vouchers.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(VoucherListModel), (int) HttpStatusCode.OK)]
        public async Task<VoucherListModel> GetVouchersAsync([FromQuery] PaginationRequestModel request)
        {
            var customerId = Guid.Parse(_requestContext.UserId);

            var response = await _vouchersClient.Vouchers.GetByCustomerIdAsync(
                customerId,
                new PaginationModel { CurrentPage = request.CurrentPage, PageSize = request.PageSize });

            var spendRuleIdentifiers = response.Vouchers
                .Select(o => o.SpendRuleId)
                .Distinct()
                .ToList();

            var localizedSpendRules = new List<BurnRuleLocalizedResponse>();

            foreach (var group in spendRuleIdentifiers.Batch(5))
            {
                var tasks = group
                    .Select(o => _campaignClient.Mobile.GetSpendRuleAsync(o, Localization.En, true))
                    .ToList();

                await tasks.WhenAll();

                var responses = tasks
                    .Where(o => o.Result != null)
                    .Select(o => o.Result);

                localizedSpendRules.AddRange(responses);
            }

            var spendRulePartnerMap = localizedSpendRules
                .Where(o => o.PartnerIds?.Length > 0)
                .ToDictionary(o => o.Id, o => o.PartnerIds.First());

            var partnersIdentifiers = spendRulePartnerMap.Values
                .Distinct()
                .ToArray();

            var partners = await _partnerManagementClient.Partners.GetByIdsAsync(partnersIdentifiers);

            var model = new List<VoucherListDetailsModel>();

            foreach (var voucher in response.Vouchers)
            {
                var spendRule = localizedSpendRules.First(o => o.Id == voucher.SpendRuleId);

                PartnerListDetailsModel partner = null;

                if (spendRulePartnerMap.TryGetValue(spendRule.Id, out var partnerId))
                    partner = partners.First(o => o.Id == partnerId);

                model.Add(new VoucherListDetailsModel
                {
                    Code = voucher.Code,
                    SpendRuleName = spendRule.Title,
                    PartnerName = partner?.Name,
                    PriceToken = voucher.AmountInTokens.ToDisplayString(),
                    PriceBaseCurrency = voucher.AmountInBaseCurrency,
                    PurchaseDate = voucher.PurchaseDate
                });
            }

            return new VoucherListModel
            {
                Vouchers = model,
                TotalCount = response.TotalCount,
            };
        }

        /// <summary>
        /// Buy voucher
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **CustomerDoesNotExist**
        /// - **SpendRuleNotFound**
        /// - **InvalidPriceInSpendRule**
        /// - **InvalidVerticalInSpendRule**
        /// - **NotEnoughTokens**
        /// - **ConversionRateNotFound**
        /// - **CustomerWalletBlocked**
        /// - **CustomerWalletMissing**
        /// - **NoVouchersInStock**
        /// </remarks>
        [HttpPost("buy")]
        [ProducesResponseType(typeof(VoucherPurchaseResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public async Task<VoucherPurchaseResponse> BuyAsync([FromQuery] Guid spendRuleId)
        {
            var customerId = Guid.Parse(_requestContext.UserId);

            var response = await _vouchersClient.Vouchers.BuyAsync(new VoucherBuyModel
            {
                CustomerId = customerId, SpendRuleId = spendRuleId
            });

            switch (response.ErrorCode)
            {
                case VoucherErrorCode.None:
                    return new VoucherPurchaseResponse {Id = response.Id, Code = response.Code};
                case VoucherErrorCode.CustomerNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case VoucherErrorCode.SpendRuleNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SpendRuleNotFound);
                case VoucherErrorCode.InvalidSpendRulePrice:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidPriceInSpendRule);
                case VoucherErrorCode.InvalidSpendRuleVertical:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidVerticalInSpendRule);
                case VoucherErrorCode.NoEnoughTokens:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NotEnoughTokens);
                case VoucherErrorCode.InvalidConversion:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ConversionRateNotFound);
                case VoucherErrorCode.CustomerWalletBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletBlocked);
                case VoucherErrorCode.CustomerWalletDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                case VoucherErrorCode.NoVouchersInStock:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NoVouchersInStock);
                default:
                    throw new InvalidOperationException(
                        $"Unexpected error during buying voucher for {_requestContext.UserId} - {response.ErrorCode}");
            }
        }
    }
}
