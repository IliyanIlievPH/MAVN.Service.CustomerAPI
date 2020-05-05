using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.Middleware.Authentication;
using MAVN.Common.Middleware.Version;
using Falcon.Numerics;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Campaign.Client.Models;
using MAVN.Service.Campaign.Client.Models.Enums;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models;
using MAVN.Service.CustomerAPI.Models.Enums;
using MAVN.Service.CustomerAPI.Models.SpendRules;
using MAVN.Service.EligibilityEngine.Client;
using MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.Vouchers.Client;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/spendRules")]
    [LowerVersion(Devices = "android", LowerVersion = 659)]
    [LowerVersion(Devices = "IPhone,IPad", LowerVersion = 181)]
    public class SpendRulesController : ControllerBase
    {
        private readonly ICampaignClient _campaignClient;
        private readonly IEligibilityEngineClient _eligibilityEngineClient;
        private readonly IRequestContext _requestContext;
        private readonly ISettingsService _settingsService;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IVouchersClient _vouchersClient;
        private readonly IMapper _mapper;

        public SpendRulesController(
            ICampaignClient campaignClient,
            IEligibilityEngineClient eligibilityEngineClient,
            IRequestContext requestContext,
            ISettingsService settingsService,
            IPartnerManagementClient partnerManagementClient,
            IVouchersClient vouchersClient,
            IMapper mapper)
        {
            _campaignClient = campaignClient;
            _eligibilityEngineClient = eligibilityEngineClient;
            _requestContext = requestContext;
            _settingsService = settingsService;
            _partnerManagementClient = partnerManagementClient;
            _vouchersClient = vouchersClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection spend rules.
        /// </summary>
        /// <remarks>
        /// Used to get available spend rules.
        /// </remarks>
        /// <returns>
        /// 200 - paginated collection of spend rules.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(SpendRulesListResponseModel), (int)HttpStatusCode.OK)]
        public async Task<SpendRulesListResponseModel> GetSpendRulesAsync([FromQuery] PaginationRequestModel pagination)
        {
            var spendRules = await _campaignClient.Mobile.GetSpendRulesAsync(Localization.En, _mapper.Map<BasePaginationRequestModel>(pagination));

            var result = _mapper.Map<SpendRulesListResponseModel>(spendRules);

            foreach (var spendRule in result.SpendRules)
            {
                if (spendRule.BusinessVertical != BusinessVertical.Retail)
                    continue;

                var report = await _vouchersClient.Reports.GetSpendRuleVouchersAsync(spendRule.Id);

                spendRule.StockCount = report.InStock;
                spendRule.SoldCount = report.Total - report.InStock;
                var rate = await _eligibilityEngineClient.ConversionRate.GetAmountBySpendRuleAsync(
                    new ConvertAmountBySpendRuleRequest()
                    {
                        Amount = Money18.Create(Math.Abs(spendRule.Price ?? 0)),
                        CustomerId = Guid.Parse(_requestContext.UserId),
                        SpendRuleId = spendRule.Id,
                        FromCurrency = _settingsService.GetBaseCurrencyCode(),
                        ToCurrency = _settingsService.GetTokenName(),
                    }
                );
                spendRule.PriceInToken = rate.Amount.ToDisplayString();
            }

            return result;
        }

        /// <summary>
        /// Returns a spend rule model
        /// </summary>
        /// <remarks>
        /// Used to get available spend rule.
        /// </remarks>
        /// <returns>
        /// 200 - a spend rule model.
        /// </returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(SpendRuleDetailsModel), (int) HttpStatusCode.OK)]
        public async Task<SpendRuleDetailsModel> GetSpendRuleAsync([FromQuery] Guid spendRuleId)
        {
            var burnRule = await _campaignClient.Mobile.GetSpendRuleAsync(spendRuleId, Localization.En);

            if (burnRule == null)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SpendRuleNotFound);

            var partners = new List<PartnerModel>();

            foreach (var identifiers in burnRule.PartnerIds.Batch(10))
            {
                var tasks = identifiers
                    .Select(o => _partnerManagementClient.Partners.GetByIdAsync(o))
                    .ToList();

                await Task.WhenAll(tasks);

                partners.AddRange(tasks
                    .Where(o => o.Result != null)
                    .Select(o => new PartnerModel
                    {
                        Id = o.Result.Id,
                        Name = o.Result.Name,
                        Locations = _mapper.Map<IReadOnlyCollection<LocationModel>>(o.Result.Locations)
                    }));
            }

            var model = _mapper.Map<SpendRuleDetailsModel>(burnRule);

            if (burnRule.UsePartnerCurrencyRate)
            {
                var rate = await _eligibilityEngineClient.ConversionRate.GetCurrencyRateBySpendRuleIdAsync(
                    new CurrencyRateBySpendRuleRequest
                    {
                        FromCurrency = _settingsService.GetTokenName(),
                        ToCurrency = _settingsService.GetBaseCurrencyCode(),
                        CustomerId = Guid.Parse(_requestContext.UserId),
                        SpendRuleId = spendRuleId
                    });

                model.AmountInCurrency = 1;
                model.AmountInTokens = rate.Rate.ToDisplayString();
            }

            model.Partners = partners;

            if (model.BusinessVertical == BusinessVertical.Retail)
            {
                var report = await _vouchersClient.Reports.GetSpendRuleVouchersAsync(model.Id);

                model.StockCount = report.InStock;
                model.SoldCount = report.Total - report.InStock;
                var rate = await _eligibilityEngineClient.ConversionRate.GetAmountBySpendRuleAsync(
                    new ConvertAmountBySpendRuleRequest()
                    {
                        Amount = Money18.Create(Math.Abs(model.Price ?? 0)),
                        CustomerId = Guid.Parse(_requestContext.UserId),
                        SpendRuleId = spendRuleId,
                        FromCurrency = _settingsService.GetBaseCurrencyCode(),
                        ToCurrency = _settingsService.GetTokenName(),
                    }
                );
                model.PriceInToken = rate.Amount.ToDisplayString();
            }

            return model;
        }
    }
}
