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
using MAVN.Service.BonusEngine.Client;
using MAVN.Service.BonusEngine.Client.Models.Customers;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Campaign.Client.Models;
using MAVN.Service.Campaign.Client.Models.Campaign.Responses;
using MAVN.Service.Campaign.Client.Models.Enums;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models;
using MAVN.Service.CustomerAPI.Models.EarnRules;
using MAVN.Service.EligibilityEngine.Client;
using MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using MAVN.Service.OperationsHistory.Client;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.Referral.Client;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Staking.Client;
using MAVN.Service.Staking.Client.Models;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/earnRules")]
    [LowerVersion(Devices = "android", LowerVersion = 659)]
    [LowerVersion(Devices = "IPhone,IPad", LowerVersion = 181)]
    public class EarnRulesController : ControllerBase
    {
        private readonly ICampaignClient _campaignClient;
        private readonly IBonusEngineClient _bonusEngineClient;
        private readonly IRequestContext _requestContext;
        private readonly IEligibilityEngineClient _eligibilityEngineClient;
        private readonly ISettingsService _settingsService;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IStakingClient _stakingClient;
        private readonly IReferralClient _referralClient;
        private readonly IOperationsHistoryClient _operationsHistoryClient;
        private readonly IMapper _mapper;

        private const int BatchCustomersCount = 50;

        public EarnRulesController(
            ICampaignClient campaignClient,
            IBonusEngineClient bonusEngineClient,
            IRequestContext requestContext,
            IEligibilityEngineClient eligibilityEngineClient,
            ISettingsService settingsService,
            IPartnerManagementClient partnerManagementClient,
            IStakingClient stakingClient,
            IReferralClient referralClient,
            IOperationsHistoryClient operationsHistoryClient,
            IMapper mapper)
        {
            _campaignClient = campaignClient;
            _bonusEngineClient = bonusEngineClient;
            _requestContext = requestContext;
            _eligibilityEngineClient = eligibilityEngineClient;
            _settingsService = settingsService;
            _partnerManagementClient = partnerManagementClient;
            _referralClient = referralClient;
            _stakingClient = stakingClient;
            _operationsHistoryClient = operationsHistoryClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection earn rules.
        /// </summary>
        /// <remarks>
        /// Used to get available earn rules.
        /// </remarks>
        /// <returns>
        /// 200 - paginated collection of earn rules.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(EarnRulesListResponseModel), (int)HttpStatusCode.OK)]
        public async Task<EarnRulesListResponseModel> GetEarnRulesAsync(
            [FromQuery] Models.EarnRules.CampaignStatus[] statuses,
            [FromQuery] PaginationRequestModel pagination)
        {
            var earnRules = await _campaignClient.Mobile.GetEarnRulesAsync(Localization.En,
                _mapper.Map<MAVN.Service.Campaign.Client.Models.Enums.CampaignStatus[]>(statuses),
                _mapper.Map<BasePaginationRequestModel>(pagination));

            return _mapper.Map<EarnRulesListResponseModel>(earnRules);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(EarnRuleExtendedModel), (int)HttpStatusCode.OK)]
        public async Task<EarnRuleExtendedModel> GetEarnRuleAsync([FromQuery] Guid earnRuleId)
        {
            var earnRule = await _campaignClient.Mobile.GetEarnRuleAsync(earnRuleId, Localization.En);

            if (earnRule == null)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.EarnRuleNotFound);

            var completions = await _bonusEngineClient.Customers
                .GetCampaignCompletionsByCustomerIdAsync(_requestContext.UserId, earnRuleId.ToString());

            var partnersIdentifiers = earnRule.Conditions
                .SelectMany(o => o.PartnerIds)
                .Distinct();

            var partners = new List<PartnerModel>();

            foreach (var identifiers in partnersIdentifiers.Batch(10))
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

            var model = _mapper.Map<EarnRuleExtendedModel>(earnRule);

            model.CustomerCompletionCount = SetEarnRuleCompletionCount(completions, earnRule);

            model.CurrentRewardedAmount = await GetCurrentRewardedAmountAsync(earnRule.Id);

            foreach (var condition in model.Conditions)
            {
                AlignConditionCompletion(condition, completions, earnRule, partners);
            }

            foreach (var condition in model.OptionalConditions)
            {
                AlignConditionCompletion(condition, completions, earnRule, partners);
            }

            if (earnRule.UsePartnerCurrencyRate)
            {
                var rate = await _eligibilityEngineClient.ConversionRate.GetCurrencyRateByEarnRuleIdAsync(
                    new CurrencyRateByEarnRuleRequest
                    {
                        FromCurrency = _settingsService.GetTokenName(),
                        ToCurrency = _settingsService.GetBaseCurrencyCode(),
                        CustomerId = Guid.Parse(_requestContext.UserId),
                        EarnRuleId = earnRuleId
                    });

                model.AmountInCurrency = 1;
                model.AmountInTokens = rate.Rate.ToString();
            }

            return model;
        }

        private async Task<string> GetCurrentRewardedAmountAsync(string earnRuleId)
        {
            Money18 reward = 0m;

            var rewardsForCampaign =
                await _operationsHistoryClient.OperationsHistoryApi.GetBonusCashInsAsync(_requestContext.UserId,
                    earnRuleId);

            foreach (var payment in rewardsForCampaign)
            {
                reward += payment.Amount;
            }

            return reward.ToDisplayString();
        }

        [HttpGet("staking")]
        [ProducesResponseType(typeof(EarnRuleStakingListModel), (int)HttpStatusCode.OK)]
        public async Task<EarnRuleStakingListModel> GetEarnRuleStakingAsync([FromQuery] Guid earnRuleId)
        {
            var earnRule = await _campaignClient.Mobile.GetEarnRuleAsync(earnRuleId, Localization.En);

            if (earnRule == null)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.EarnRuleNotFound);

            var stakingCondition = earnRule.Conditions.FirstOrDefault(c => c.HasStaking);

            if (stakingCondition == null)
                return new EarnRuleStakingListModel();

            //the earn rule's reward type is always fixed when has a stakeable condition 
            var totalReward = earnRule.Reward;

            var stakes = await _stakingClient.ReferralStakesApi.GetReferralStakesAsync(
                new GetReferralStakesRequest
                {
                    CampaignId = earnRuleId.ToString(),
                    CustomerId = _requestContext.UserId
                });

            var model = new EarnRuleStakingListModel
            {
                EarnRuleStakings = _mapper.Map<IEnumerable<EarnRuleStakingModel>>(stakes),
                TotalCount = stakes.Count()
            };

            foreach (var batch in model.EarnRuleStakings.Batch(BatchCustomersCount))
            {
                await AlignStakingModel(batch.ToList(), totalReward);
            }

            return model;
        }

        private void AlignConditionCompletion(ConditionExtendedModel condition, CampaignCompletionModel completions,
            EarnRuleLocalizedResponse earnRule, List<PartnerModel> partners)
        {
            condition.CustomerCompletionCount = completions?.ConditionCompletions
                                                          .FirstOrDefault(x => x.ConditionId == condition.Id)
                                                          ?.CurrentCount ?? 0;

            //Partners
            var conditionPartners = earnRule.Conditions.Single(o => o.Id == condition.Id).PartnerIds;

            condition.Partners = partners
                .Where(o => conditionPartners.Contains(o.Id))
                .ToList();
        }

        private static int SetEarnRuleCompletionCount(CampaignCompletionModel completions, EarnRuleLocalizedResponse earnRule)
        {
            var customerCompletionCount = completions?.CampaignCompletionCount ?? 0;

            if (customerCompletionCount == 0 && earnRule.Conditions.Count > 1)
            {
                var mainCondition = earnRule.Conditions.FirstOrDefault(c => !c.IsHidden);

                if (mainCondition != null)
                {
                    return completions?.ConditionCompletions
                               .FirstOrDefault(c => c.ConditionId == mainCondition.Id)?.CurrentCount ?? 0;
                }
            }

            return customerCompletionCount;
        }

        private async Task AlignStakingModel(List<EarnRuleStakingModel> stakes, Money18 reward)
        {
            var referralIs = new List<Guid>();

            foreach (var st in stakes)
            {
                if (Guid.TryParse(st.ReferralId, out var guidId))
                {
                    referralIs.Add(guidId);
                }
            }

            var referralInformationResult = await _referralClient.CommonReferralApi.GetReferralsByReferralIdsAsync(
                new CommonReferralByReferralIdsRequest()
                {
                    ReferralIds = referralIs
                });

            foreach (var stake in stakes)
            {
                stake.TotalReward = reward;

                var referralInfo = referralInformationResult.CommonReferrals[stake.ReferralId];

                if (referralInfo != null)
                {
                    if (!string.IsNullOrWhiteSpace(referralInfo.FirstName)
                        && !string.IsNullOrWhiteSpace(referralInfo.LastName))
                    {
                        stake.ReferralName = referralInfo.FirstName + " " + referralInfo.LastName;
                    }
                    else
                    {
                        stake.ReferralName = referralInfo.Email;
                    }
                }
            }
        }
    }
}
