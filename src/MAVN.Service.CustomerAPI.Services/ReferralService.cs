using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.Log;
using Lykke.Service.BonusEngine.Client;
using Lykke.Service.BonusEngine.Client.Models.Customers;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Campaign.Client.Models.Campaign.Responses;
using Lykke.Service.Campaign.Client.Models.Condition;
using Lykke.Service.Campaign.Client.Models.Enums;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Domain.CommonReferral;
using MAVN.Service.CustomerAPI.Core.Exceptions;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.Service.EligibilityEngine.Client;
using Lykke.Service.EligibilityEngine.Client.Enums;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using Lykke.Service.OperationsHistory.Client;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.Referral.Client;
using Lykke.Service.Referral.Client.Enums;
using Lykke.Service.Referral.Client.Models.Requests;
using Lykke.Service.Referral.Client.Models.Responses;
using Lykke.Service.Staking.Client;
using Lykke.Service.Staking.Client.Models;
using Newtonsoft.Json;
using CommonReferralModel = MAVN.Service.CustomerAPI.Core.Domain.CommonReferral.CommonReferralModel;
using CommonReferralStatus = MAVN.Service.CustomerAPI.Core.Domain.CommonReferral.CommonReferralStatus;
using RewardRatioAttribute = MAVN.Service.CustomerAPI.Core.Domain.CommonReferral.RewardRatioAttribute;

namespace MAVN.Service.CustomerAPI.Services
{
    public class ReferralService : IReferralService
    {
        private readonly IReferralClient _referralClient;
        private readonly ICampaignClient _campaignClient;
        private readonly IOperationsHistoryClient _operationsHistoryClient;
        private readonly IBonusEngineClient _bonusEngineClient;
        private readonly IStakingClient _stakingClient;
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IEligibilityEngineClient _eligibilityEngine;
        private readonly ISettingsService _settingsService;

        private readonly IMapper _mapper;
        private readonly ILog _log;

        public ReferralService(
            IReferralClient referralClient,
            ICampaignClient campaignClient,
            IBonusEngineClient bonusEngineClient,
            IStakingClient stakingClient,
            IPartnerManagementClient partnerManagementClient,
            IEligibilityEngineClient eligibilityEngine,
            ISettingsService settingsService,
            IOperationsHistoryClient operationsHistoryClient,
            ILogFactory logFactory,
            IMapper mapper)
        {
            _referralClient = referralClient;
            _eligibilityEngine = eligibilityEngine;
            _settingsService = settingsService;
            _campaignClient = campaignClient;
            _bonusEngineClient = bonusEngineClient;
            _stakingClient = stakingClient;
            _partnerManagementClient = partnerManagementClient;
            _mapper = mapper;
            _operationsHistoryClient = operationsHistoryClient;
            _log = logFactory.CreateLog(this);
        }

        public async Task<string> GetOrCreateReferralCodeAsync(string customerId)
        {
            var referral = await _referralClient.ReferralApi.GetAsync(customerId);

            if (referral.ReferralCode != null)
            {
                return referral.ReferralCode;
            }

            if (referral.ReferralCode == null && referral.ErrorCode == ReferralErrorCodes.ReferralNotFound)
            {
                var referralCreate = await _referralClient.ReferralApi.PostAsync(new ReferralCreateRequest()
                {
                    CustomerId = customerId
                });

                return referralCreate.ReferralCode;
            }

            return null;
        }
        public async Task<ILykkeApiErrorCode> AddReferralLeadAsync(string customerId, ReferralLeadCreateModel referralLeadCreateModel)
        {
            var result = await _referralClient.ReferralLeadApi.PostAsync(new ReferralLeadCreateRequest
            {
                FirstName = referralLeadCreateModel.FirstName,
                LastName = referralLeadCreateModel.LastName,
                Email = referralLeadCreateModel.Email,
                PhoneCountryCodeId = referralLeadCreateModel.CountryPhoneCodeId,
                PhoneNumber = referralLeadCreateModel.PhoneNumber,
                Note = referralLeadCreateModel.Note,
                CustomerId = customerId,
                CampaignId = referralLeadCreateModel.CampaignId
            });

            switch (result.ErrorCode)
            {
                case ReferralErrorCodes.None:
                    return null;
                case ReferralErrorCodes.GuidCanNotBeParsed:
                    return ApiErrorCodes.Service.ReferralLeadCustomerIdInvalid;
                case ReferralErrorCodes.ReferralLeadAlreadyExist:
                    return ApiErrorCodes.Service.ReferralLeadAlreadyExist;
                case ReferralErrorCodes.CustomerNotApprovedAgent:
                    return ApiErrorCodes.Service.CustomerNotApprovedAgent;
                case ReferralErrorCodes.ReferralLeadProcessingFailed:
                    return ApiErrorCodes.Service.ReferralLeadNotProcessed;
                case ReferralErrorCodes.ReferYourself:
                    return ApiErrorCodes.Service.CanNotReferYourself;
                case ReferralErrorCodes.ReferralLeadAlreadyConfirmed:
                    return ApiErrorCodes.Service.ReferralLeadAlreadyConfirmed;
                case ReferralErrorCodes.CampaignNotFound:
                    return ApiErrorCodes.Service.ReferralCampaignNotFound;
                case ReferralErrorCodes.InvalidStake:
                    return ApiErrorCodes.Service.ReferralInvalidStake;
                case ReferralErrorCodes.CustomerDoesNotExist:
                    return ApiErrorCodes.Service.CustomerDoesNotExist;
                default:
                    throw new UnhandledErrorCodeException(result.ErrorCode.ToString(), result.ErrorMessage);
            }
        }

        public async Task<ReferralLeadListResultModel> GetLeadReferralsAsync(string agentId)
        {
            var result = await _referralClient.ReferralLeadApi.GetAsync(agentId);

            if (result.ErrorCode != ReferralErrorCodes.None)
            {
                var errorCode = ApiErrorCodes.Service.ReferralLeadNotProcessed;

                switch (result.ErrorCode)
                {
                    case ReferralErrorCodes.ReferralLeadAlreadyExist:
                        errorCode = ApiErrorCodes.Service.ReferralLeadCustomerIdInvalid;
                        break;
                    case ReferralErrorCodes.GuidCanNotBeParsed:
                        errorCode = ApiErrorCodes.Service.ReferralLeadAlreadyExist;
                        break;
                }

                return new ReferralLeadListResultModel
                {
                    ErrorCode = errorCode
                };
            }

            return new ReferralLeadListResultModel
            {
                ReferralLeads = result.ReferralLeads
                    .Select(o => new Core.Domain.ReferralLeadModel
                    {
                        Name = $"{o.FirstName} {o.LastName}",
                        Status = GetStatus(o.State),
                        TimeStamp = o.CreationDateTime,
                        OffersCount = o.OffersCount,
                        PurchaseCount = o.PurchaseCount
                    })
                    .OrderByDescending(o => o.TimeStamp)
                    .ToList()
            };
        }

        public async Task<ILykkeApiErrorCode> ConfirmReferralAsync(string confirmationCode)
        {
            var result = await _referralClient.ReferralLeadApi.ConfirmAsync(new ReferralLeadConfirmRequest
            {
                ConfirmationToken = confirmationCode
            });

            if (result.ErrorCode != ReferralErrorCodes.None)
            {
                if (result.ErrorCode == ReferralErrorCodes.ReferralDoesNotExist)
                    return ApiErrorCodes.Service.ReferralNotFound;

                if (result.ErrorCode == ReferralErrorCodes.LeadAlreadyConfirmed)
                    return ApiErrorCodes.Service.LeadAlreadyConfirmed;
            }

            return null;
        }

        public async Task<ILykkeApiErrorCode> AddHotelReferralAsync(string customerId, string email, Guid campaignId,
            string fullName, int phoneCountryCode, string phoneNumber)
        {
            var result = await _referralClient.ReferralHotelsApi.CreateAsync(new ReferralHotelCreateRequest
            {
                ReferrerId = customerId,
                Email = email,
                CampaignId = campaignId,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                PhoneCountryCodeId = phoneCountryCode

            });

            if (result.ErrorCode == ReferralHotelCreateErrorCode.None)
                return null;

            switch (result.ErrorCode)
            {
                case ReferralHotelCreateErrorCode.ReferralAlreadyConfirmed:
                    return ApiErrorCodes.Service.ReferralAlreadyConfirmed;
                case ReferralHotelCreateErrorCode.ReferralsLimitExceeded:
                    return ApiErrorCodes.Service.ReferralsLimitExceeded;
                case ReferralHotelCreateErrorCode.AgentCantReferHimself:
                    return ApiErrorCodes.Service.CanNotReferYourself;
                case ReferralHotelCreateErrorCode.ReferralAlreadyExist:
                    return ApiErrorCodes.Service.ReferralAlreadyExist;
                case ReferralHotelCreateErrorCode.ReferralExpired:
                    return ApiErrorCodes.Service.ReferralExpired;
                case ReferralHotelCreateErrorCode.CampaignNotFound:
                    return ApiErrorCodes.Service.CampaignDoesNotExists;
                case ReferralHotelCreateErrorCode.CustomerDoesNotExist:
                    return ApiErrorCodes.Service.CustomerDoesNotExist;
                case ReferralHotelCreateErrorCode.InvalidStake:
                    return ApiErrorCodes.Service.ReferralInvalidStake;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<IEnumerable<HotelReferralModel>> GetHotelReferralAsync(string customerId)
        {
            var result = await _referralClient.ReferralHotelsApi.GetByReferrerIdAsync(customerId);

            return _mapper.Map<IEnumerable<HotelReferralModel>>(result.HotelReferrals);
        }

        private static ReferralLeadStatus GetStatus(ReferralLeadState state)
        {
            switch (state)
            {
                case ReferralLeadState.Pending:
                    return ReferralLeadStatus.Sent;
                case ReferralLeadState.Confirmed:
                    return ReferralLeadStatus.Accepted;
                case ReferralLeadState.Approved:
                    return ReferralLeadStatus.Approved;
                case ReferralLeadState.Rejected:
                    return ReferralLeadStatus.Rejected;
                default:
                    throw new InvalidEnumArgumentException(nameof(state), (int)state, typeof(ReferralLeadState));
            }
        }

        public async Task<IReadOnlyList<CustomerCommonReferralModel>> PrepareReferralCommonData(IEnumerable<CommonReferralModel> paged, string customerId, Guid? earnRuleId)
        {
            var result = new List<CustomerCommonReferralModel>();
            var earnRule = new EarnRuleLocalizedResponse();
            var campaignCompletions = new CampaignCompletionModel();

            var stakes = await _stakingClient.ReferralStakesApi.GetReferralStakesAsync(new GetReferralStakesRequest() { CustomerId = customerId });

            //when the referrals are filtered by earn rule
            if (earnRuleId.HasValue && earnRuleId != Guid.Empty)
            {
                earnRule = await _campaignClient.History.GetEarnRuleMobileAsync(earnRuleId.Value, Localization.En);

                if (earnRule != null)
                {
                    campaignCompletions = await _bonusEngineClient.Customers
                        .GetCampaignCompletionsByCustomerIdAsync(customerId, earnRule.Id);
                }
            }

            foreach (var referral in paged)
            {
                var customerReferral = new CustomerCommonReferralModel()
                {
                    FirstName = referral.FirstName,
                    LastName = referral.LastName,
                    Email = referral.Email,
                    ReferralType = referral.ReferralType,
                    Status = _mapper.Map<CommonReferralStatus>(referral.Status),
                    TimeStamp = referral.TimeStamp
                };

                if (earnRuleId.HasValue && earnRuleId != Guid.Empty)
                {
                    await SetReferralAdditionalInformation(campaignCompletions, customerReferral, earnRule, stakes, referral, customerId);
                }
                else if (referral.CampaignId.HasValue)
                {
                    earnRule = await _campaignClient.History.GetEarnRuleMobileAsync(referral.CampaignId.Value, Localization.En);

                    if (earnRule != null)
                    {
                        var completions = await _bonusEngineClient.Customers
                            .GetCampaignCompletionsByCustomerIdAsync(customerId, earnRule.Id);

                        await SetReferralAdditionalInformation(completions, customerReferral, earnRule, stakes, referral, customerId);
                    }
                }
                else
                {
                    Money18 defaultValue = 0m;

                    customerReferral.CurrentRewardedAmount = defaultValue.ToDisplayString();
                    customerReferral.TotalReward = defaultValue.ToDisplayString();
                }

                if (!string.IsNullOrWhiteSpace(referral.PartnerId))
                {
                    var request =
                        await _partnerManagementClient.Partners.GetByIdAsync(Guid.Parse(referral.PartnerId));

                    customerReferral.PartnerName = request.Name;
                }

                result.Add(customerReferral);
            }

            return result;
        }

        public async Task<ILykkeApiErrorCode> AddReferralFriendAsync(string customerId, ReferralFriendCreateModel referralFriendCreateModel)
        {
            var result = await _referralClient.ReferralFriendsApi.CreateAsync(new ReferralFriendCreateRequest
            {
                CustomerId = customerId,
                Email = referralFriendCreateModel.Email,
                CampaignId = referralFriendCreateModel.CampaignId,
                FullName = referralFriendCreateModel.FullName
            });

            if (result.ErrorCode == ReferralErrorCodes.None)
                return null;

            switch (result.ErrorCode)
            {
                case ReferralErrorCodes.LeadAlreadyConfirmed:
                    return ApiErrorCodes.Service.ReferralAlreadyConfirmed;
                case ReferralErrorCodes.ReferYourself:
                    return ApiErrorCodes.Service.CanNotReferYourself;
                case ReferralErrorCodes.ReferralFriendAlreadyExist:
                    return ApiErrorCodes.Service.ReferralAlreadyExist;
                case ReferralErrorCodes.CampaignNotFound:
                    return ApiErrorCodes.Service.CampaignDoesNotExists;
                case ReferralErrorCodes.CustomerDoesNotExist:
                    return ApiErrorCodes.Service.CustomerDoesNotExist;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task SetReferralAdditionalInformation(CampaignCompletionModel completions,
            CustomerCommonReferralModel customerReferral,
            EarnRuleLocalizedResponse earnRule,
            IEnumerable<ReferralStakeResponseModel> stakes,
            CommonReferralModel referral,
            string customerId)
        {
            var customerCompletionCount = completions?.CampaignCompletionCount ?? 0;

            customerReferral.CurrentRewardedAmount =
                (customerCompletionCount * earnRule.Reward).ToDisplayString();

            if (customerReferral.Status != CommonReferralStatus.Ongoing && customerReferral.Status != CommonReferralStatus.AcceptedByLead)
            {
                var payments =
                    await _operationsHistoryClient.OperationsHistoryApi.GetBonusCashInsByReferralAsync(customerId, referral.Id);

                if (payments != null && payments.Any())
                {
                    Money18 paymentsSum = 0m;

                    foreach (var payment in payments)
                    {
                        paymentsSum += payment.Amount;
                    }

                    customerReferral.TotalReward = paymentsSum.ToDisplayString();
                }
            }
            else
            {
                customerReferral.TotalReward = earnRule.Reward.ToDisplayString();
            }

            customerReferral.IsApproximate = earnRule.IsApproximate || earnRule.Conditions.Any(c => c.IsApproximate);

            if (earnRule.Conditions.Any(c => c.HasStaking))
            {
                customerReferral.HasStaking = true;

                var staking = stakes.FirstOrDefault(s => s.ReferralId == referral.Id);

                if (staking != null)
                    customerReferral.Staking = _mapper.Map<ReferralStakingModel>(staking);
            }

            var ratioConditions = earnRule.Conditions
                .Where(c => c.RewardRatio != null && c.RewardRatio.Ratios.Any()).ToList();

            if (ratioConditions.Any())
            {
                customerReferral.RewardHasRatio = true;

                foreach (var condition in ratioConditions)
                {
                    customerReferral.RewardRatio =
                       await GetRatioRewardByReferral(condition, completions, referral.Id, customerId);
                }
            }
        }

        private async Task<RewardRatioAttribute> GetRatioRewardByReferral(ConditionLocalizedResponse condition, CampaignCompletionModel completions, string referralId, string customerId)
        {
            if (!condition.RewardRatio.Ratios.Any())
                return null;

            var ratio = new RewardRatioAttribute
            {
                Ratios = _mapper.Map<List<RatioAttributeModel>>(condition.RewardRatio.Ratios)
            };

            var completion = completions?.ConditionCompletions.FirstOrDefault(x => x.ConditionId == condition.Id);

            if (completion == null)
                return ratio;

            var thresholds = condition.RewardRatio.Ratios.OrderBy(c => c.Order).Select(c => c.Threshold);

            //Data for given referral
            var dataDictionary = completion.Data.FirstOrDefault(c => c.ContainsKey(referralId));

            if (dataDictionary == null)
                return ratio;

            foreach (var data in dataDictionary)
            {
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.Value);

                if (dictionary != null)
                {
                    var givenBonus = dictionary["GivenRatioBonusPercent"];
                    var passedRatios = thresholds.Count(t => t <= Convert.ToDecimal(givenBonus));

                    var ratioCompletion = new RatioCompletion()
                    {
                        PaymentId = referralId,
                        GivenThreshold = Convert.ToDecimal(givenBonus),
                        Checkpoint = passedRatios
                    };

                    await SetRewardRatioGivenBonusAmountAsync(condition, customerId, dictionary, ratioCompletion);

                    ratio.RatioCompletion = new List<RatioCompletion>()
                    {
                        ratioCompletion
                    };
                }

                return ratio;
            }

            return null;
        }

        private async Task SetRewardRatioGivenBonusAmountAsync(ConditionLocalizedResponse condition, string customerId, Dictionary<string, string> dictionary, RatioCompletion ratioCompletion)
        {
            var amount = dictionary["Amount"];
            Money18 givenReward = 0m;
            Money18 totalReward = 0m;

            //calculate bonus
            switch (condition.RewardType)
            {
                case RewardType.Fixed:
                    {
                        givenReward = CalculateReward(condition.RewardRatio.Ratios, condition.ImmediateReward, ratioCompletion.GivenThreshold);
                        totalReward = condition.ImmediateReward;
                    }
                    break;
                case RewardType.Percentage:
                    if (amount != null)
                    {
                        var amountInCurrency = Money18.Parse(amount) * (condition.ImmediateReward / 100m);

                        var convertedAmount = await GetEligibilityEngineAmountByCondition(Guid.Parse(condition.Id),
                            Guid.Parse(customerId), amountInCurrency);

                        givenReward = CalculateReward(condition.RewardRatio.Ratios, convertedAmount, ratioCompletion.GivenThreshold);
                        totalReward = convertedAmount;
                    }

                    break;
                case RewardType.ConversionRate:
                    {
                        var convertedAmount = await GetEligibilityEngineAmountByCondition(Guid.Parse(condition.Id),
                            Guid.Parse(customerId), condition.ImmediateReward);

                        givenReward = CalculateReward(condition.RewardRatio.Ratios, convertedAmount, ratioCompletion.GivenThreshold);
                        totalReward = convertedAmount;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ratioCompletion.GivenRatioRewardBonus = givenReward.ToDisplayString();
            ratioCompletion.TotalRatioRewardBonus = totalReward.ToDisplayString();
        }

        private static Money18 CalculateReward(IReadOnlyCollection<RatioAttributeDetailsModel> ratios, Money18 conditionReward, decimal givenBonus)
        {
            Money18 reward = 0m;
            var giveBonusesFor = ratios.Where(r => r.Threshold <= givenBonus);

            foreach (var r in giveBonusesFor)
            {
                reward += ((Money18)r.RewardRatio / 100M) * conditionReward;
            }

            return reward;
        }

        private async Task<Money18> GetEligibilityEngineAmountByCondition(Guid conditionId, Guid customerId, Money18 rewardsAmount)
        {
            var response = await _eligibilityEngine.ConversionRate.GetAmountByConditionAsync(
                new ConvertAmountByConditionRequest()
                {
                    CustomerId = customerId,
                    ConditionId = conditionId,
                    Amount = rewardsAmount,
                    FromCurrency = _settingsService.GetBaseCurrencyCode(),
                    ToCurrency = _settingsService.GetEmaarTokenName()
                });

            if (response.ErrorCode != EligibilityEngineErrors.None)
            {
                _log.Error(message: "An error occured while converting currency amount",
                    context: $"from: {_settingsService.GetBaseCurrencyCode()}; to: {_settingsService.GetEmaarTokenName()}; error: {response.ErrorCode}");

                return 0;
            }

            return response.Amount;
        }
    }
}
