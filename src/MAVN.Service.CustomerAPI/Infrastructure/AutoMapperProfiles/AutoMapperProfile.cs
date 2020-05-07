using System;
using System.Linq;
using AutoMapper;
using JetBrains.Annotations;
using MAVN.Service.Campaign.Client.Models;
using MAVN.Service.Campaign.Client.Models.BurnRule.Responses;
using MAVN.Service.Campaign.Client.Models.Campaign.Responses;
using MAVN.Service.Campaign.Client.Models.Condition;
using MAVN.Service.CrossChainTransfers.Client.Models.Responses;
using MAVN.Service.CrossChainWalletLinker.Client.Models;
using MAVN.Service.CustomerManagement.Client.Enums;
using MAVN.Service.CustomerManagement.Client.Models;
using MAVN.Service.CustomerManagement.Client.Models.Requests;
using MAVN.Service.CustomerManagement.Client.Models.Responses;
using MAVN.Service.CustomerProfile.Client.Models.Responses;
using MAVN.Service.Dictionaries.Client.Models.Notifications;
using MAVN.Service.PartnerManagement.Client.Models.Location;
using MAVN.Service.PartnersIntegration.Client.Models;
using MAVN.Service.PushNotifications.Client.Enums;
using MAVN.Service.PushNotifications.Client.Models.Responses;
using MAVN.Service.Referral.Client.Enums;
using MAVN.Service.Referral.Client.Models.Responses;
using MAVN.Service.Referral.Client.Models.Responses.CommonReferral;
using MAVN.Service.Staking.Client.Models;
using MAVN.Service.WalletManagement.Client.Models.Responses;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Models;
using MAVN.Service.CustomerAPI.Models.Auth;
using MAVN.Service.CustomerAPI.Models.Customers;
using MAVN.Service.CustomerAPI.Models.EarnRules;
using MAVN.Service.CustomerAPI.Models.History;
using MAVN.Service.CustomerAPI.Models.Lists;
using MAVN.Service.CustomerAPI.Models.NotificationMessages;
using MAVN.Service.CustomerAPI.Models.Operations;
using MAVN.Service.CustomerAPI.Models.PartnersMessages;
using MAVN.Service.CustomerAPI.Models.PushNotifications;
using MAVN.Service.CustomerAPI.Models.Referral;
using MAVN.Service.CustomerAPI.Models.SmartVouchers;
using MAVN.Service.CustomerAPI.Models.SpendRules;
using MAVN.Service.CustomerAPI.Models.Wallets;
using MAVN.Service.SmartVouchers.Client.Models.Responses;
using ConditionModel = MAVN.Service.CustomerAPI.Models.EarnRules.ConditionModel;
using FileResponseModel = MAVN.Service.CustomerAPI.Models.SmartVouchers.FileResponseModel;
using RatioAttributeModel = MAVN.Service.CustomerAPI.Models.EarnRules.RatioAttributeModel;
using RatioCompletion = MAVN.Service.CustomerAPI.Models.EarnRules.RatioCompletion;
using ReferralLeadModel = MAVN.Service.Referral.Client.Models.Responses.ReferralLeadModel;
using ReferralStakingModel = MAVN.Service.CustomerAPI.Models.Referral.ReferralStakingModel;
using RegistrationRequestModel = MAVN.Service.CustomerManagement.Client.Models.Requests.RegistrationRequestModel;
using RegistrationResponseModel = MAVN.Service.CustomerManagement.Client.Models.Responses.RegistrationResponseModel;
using TransferResponse = MAVN.Service.OperationsHistory.Client.Models.Responses.TransferResponse;

namespace MAVN.Service.CustomerAPI.Infrastructure.AutoMapperProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LoginRequestModel, AuthenticateRequestModel>()
                .ForMember(c => c.LoginProvider, opt => opt.MapFrom(x => LoginProvider.Standard));
            CreateMap<AuthenticateResponseModel, LoginResponseModel>();
            CreateMap<AuthenticateResponseModel, AuthenticationResultModel>();
            CreateMap<AuthenticationResultModel, LoginResponseModel>();

            CreateMap<Models.Customers.RegistrationRequestModel, RegistrationRequestDto>();
            CreateMap<RegistrationRequestDto, RegistrationRequestModel>()
                .ForMember(x => x.LoginProvider, opt => opt.MapFrom(src => LoginProvider.Standard));
            //CreateMap<Models.Customers.RegistrationRequestModel, RegistrationRequestModel>()
            //    .ForMember(c => c.LoginProvider, opt => opt.MapFrom(x => LoginProvider.Standard));
            CreateMap<RegistrationResponseModel, RegistrationResultModel>()
                .ForMember(x => x.Error, opt => opt.MapFrom(r => r.Error));
            CreateMap<RegistrationResultModel, Models.Customers.RegistrationResponseModel>(MemberList.Destination);

            CreateMap<ReferralCreateResponse, ReferralResponseModel>();
            CreateMap<ReferralResponseModel, ReferralCreateResponse>()
                .ForMember(c => c.ErrorCode, opt => opt.Ignore())
                .ForMember(c => c.ErrorMessage, opt => opt.Ignore());

            CreateMap<ReferralResultResponse, ReferralResponseModel>();
            CreateMap<ReferralResponseModel, ReferralResultResponse>()
                .ForMember(c => c.ErrorCode, opt => opt.Ignore())
                .ForMember(c => c.ErrorMessage, opt => opt.Ignore());

            CreateMap<ReferralLeadModel, Core.Domain.ReferralLeadModel>(MemberList.Destination)
                .ForMember(c => c.Name, opt => opt.MapFrom(c => $"{c.FirstName} {c.LastName}"))
                .ForMember(c => c.Status, opt => opt.MapFrom(c => c.State))
                .ForMember(c => c.TimeStamp, opt => opt.MapFrom(c => c.CreationDateTime));

            CreateMap<Core.Domain.ReferralLeadModel, ReferralLeadModel>()
                .ForMember(c => c.Id, opt => opt.Ignore())
                .ForMember(c => c.FirstName, opt => opt.Ignore())
                .ForMember(c => c.LastName, opt => opt.Ignore())
                .ForMember(c => c.PhoneNumber, opt => opt.Ignore())
                .ForMember(c => c.PhoneCountryCodeId, opt => opt.Ignore())
                .ForMember(c => c.Note, opt => opt.Ignore())
                .ForMember(c => c.AgentId, opt => opt.Ignore())
                .ForMember(c => c.AgentSalesforceId, opt => opt.Ignore())
                .ForMember(c => c.ConfirmationToken, opt => opt.Ignore())
                .ForMember(c => c.SalesforceId, opt => opt.Ignore())
                .ForMember(c => c.State, opt => opt.Ignore())
                .ForMember(c => c.CreationDateTime, opt => opt.Ignore())
                .ForMember(c => c.Email, opt => opt.Ignore())
                .ForMember(c => c.CampaignId, opt => opt.Ignore());

            CreateMap<LeadReferral, Core.Domain.ReferralLeadModel>();
            CreateMap<Core.Domain.ReferralLeadModel, LeadReferral>();

            CreateMap<ReferralHotelModel, Core.Domain.HotelReferralModel>()
                .ForMember(dest => dest.CountryPhoneCodeId, opt => opt.MapFrom(src => src.PhoneCountryCodeId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.CreationDateTime));

            CreateMap<Core.Domain.HotelReferralModel, Models.Referral.HotelReferralModel>();

            CreateMap<ReferralFriendRequestModel, ReferralFriendCreateModel>();

            CreateMap<TransferResponse, TransferInfoModel>()
                .ForMember(s => s.IsSender, opt => opt.Ignore());
            CreateMap<TransferInfoModel, TransferResponseModel>();
            CreateMap<PaginatedTransfersModel, PaginatedTransfersResponseModel>();

            CreateMap<ChangePasswordResponseModel, ChangePasswordResultModel>()
                .ForMember(x => x.Error, opt => opt.MapFrom(r => r.Error));

            CreateMap<PasswordResetErrorResponse, ResetPasswordResultModel>()
                .ForMember(x => x.Error, opt => opt.MapFrom(r => r.Error));

            CreateMap<PaginatedOperationsHistoryModel, PaginatedOperationsHistoryResponseModel>();
            CreateMap<OperationHistoryModel, OperationHistoryResponseModel>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDisplayString()));

            CreateMap<PushNotificationRegisterRequestModel, PushNotificationRegistrationCreateModel>();
            CreateMap<PushTokenInsertionResult, PushNotificationRegistrationResult>()
                .ConvertUsing(new PushNotificationRegistrationResultConverter());

            CreateMap<CommonReferralModel, Core.Domain.CommonReferral.CommonReferralModel>()
                .ForMember(src => src.IsApproximate, opt => opt.Ignore());

            CreateMap<Core.Domain.CommonReferral.CustomerCommonReferralModel, CustomerCommonReferralResponseModel>();

            CreateMap<RatioAttributeDetailsModel, Core.Domain.CommonReferral.RatioAttributeModel>();

            CreateMap<Core.Domain.CommonReferral.RewardRatioAttribute, RewardRatioAttributeModel>();
            CreateMap<Core.Domain.CommonReferral.RatioAttributeModel, RatioAttributeModel>();
            CreateMap<Core.Domain.CommonReferral.RatioCompletion, RatioCompletion>();
            CreateMap<Core.Domain.CommonReferral.ReferralStakingModel, ReferralStakingModel>();

            CreateMap<ReferralStakeResponseModel, Core.Domain.CommonReferral.ReferralStakingModel>()
                .ForMember(dest => dest.StakeAmount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.StakingExpirationDate, opt => opt.MapFrom(src => src.Timestamp.AddDays(src.StakingPeriodInDays)));

            CreateMap<CommonReferralStatus, Core.Domain.CommonReferral.CommonReferralStatus>()
               .ConvertUsing(c=> MapCommonReferralStatus(c));

            // Campaigns
            CreateMap<BurnRulePaginatedResponseModel, SpendRulesListResponseModel>(MemberList.Destination)
                .ForMember(x => x.SpendRules, opt => opt.MapFrom(src => src.BurnRules));
            CreateMap<BurnRuleLocalizedResponse, SpendRuleListDetailsModel>(MemberList.Destination)
                .ForMember(x => x.BusinessVertical, opt => opt.MapFrom(src => src.Vertical))
                .ForMember(x => x.StockCount, opt => opt.Ignore())
                .ForMember(x => x.SoldCount, opt => opt.Ignore())
                .ForMember(x => x.PriceInToken, opt => opt.Ignore())
                .ForSourceMember(x => x.AmountInCurrency, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.AmountInTokens, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.PartnerIds, opt => opt.DoNotValidate());

            CreateMap<BurnRuleLocalizedResponse, SpendRuleDetailsModel>(MemberList.Destination)
                .ForMember(dest => dest.AmountInTokens, opt => opt.MapFrom(src => src.AmountInTokens.ToDisplayString()))
                .ForMember(x => x.BusinessVertical, opt => opt.MapFrom(src => src.Vertical))
                .ForMember(x => x.StockCount, opt => opt.Ignore())
                .ForMember(x => x.SoldCount, opt => opt.Ignore())
                .ForMember(x => x.PriceInToken, opt => opt.Ignore())
                .ForMember(x => x.Partners, opt => opt.Ignore());

            CreateMap<PaginationRequestModel, BasePaginationRequestModel>(MemberList.Destination);
            CreateMap<EarnRulePaginatedResponseModel, EarnRulesListResponseModel>(MemberList.Destination);
            CreateMap<EarnRuleLocalizedResponse, EarnRuleModel>(MemberList.Destination)
                .ForMember(dest => dest.Reward, opt => opt.MapFrom(src => src.Reward.ToDisplayString()))
                .ForMember(dest => dest.Conditions, opt => opt.MapFrom(src => src.Conditions.Where(c => !c.IsHidden)))
                .ForMember(dest => dest.OptionalConditions,
                    opt => opt.MapFrom(src => src.Conditions.Where(c => c.IsHidden)));

            CreateMap<EarnRuleLocalizedResponse, EarnRuleExtendedModel>(MemberList.Destination)
                .ForMember(dest => dest.Reward, opt => opt.MapFrom(src => src.Reward.ToDisplayString()))
                .ForMember(dest => dest.AmountInTokens, opt => opt.MapFrom(src => src.AmountInTokens.ToDisplayString()))
                .ForMember(dest => dest.CustomerCompletionCount, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentRewardedAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Conditions, opt => opt.MapFrom(src => src.Conditions.Where(c => !c.IsHidden)))
                .ForMember(dest => dest.OptionalConditions, opt => opt.MapFrom(src => src.Conditions.Where(c => c.IsHidden)))
                .ForSourceMember(x => x.AmountInCurrency, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.AmountInTokens, opt => opt.DoNotValidate());

            CreateMap<LocationDetailsModel, LocationModel>(MemberList.Destination);

            CreateMap<ConditionLocalizedResponse, ConditionModel>(MemberList.Destination)
                .ForMember(dest => dest.ImmediateReward,
                    opt => opt.MapFrom(src => src.ImmediateReward.ToDisplayString()));
            CreateMap<ConditionLocalizedResponse, ConditionExtendedModel>(MemberList.Destination)
                .ForMember(dest => dest.ImmediateReward,
                    opt => opt.MapFrom(src => src.ImmediateReward.ToDisplayString()))
                .ForMember(dest => dest.CustomerCompletionCount, opt => opt.Ignore())
                .ForMember(dest => dest.Partners, opt => opt.Ignore());
            CreateMap<RewardRatioAttributeDetailsResponseModel, RewardRatioAttributeModel>(MemberList.Destination)
                .ForMember(r => r.RatioCompletion, opt => opt.Ignore());
            CreateMap<RatioAttributeDetailsModel, RatioAttributeModel>(MemberList.Destination);

            CreateMap<ReferralStakeResponseModel, EarnRuleStakingModel>()
                .ForMember(c => c.ReferralName, opt => opt.Ignore())
                .ForMember(c => c.StakeAmount, opt => opt.MapFrom(s => s.Amount))
                .ForMember(c => c.TotalReward, opt => opt.MapFrom(s => s.Amount))
                .ForMember(c => c.StakeWarningPeriod, opt => opt.MapFrom(s => s.WarningPeriodInDays))
                .ForMember(c => c.StakingPeriod, opt => opt.MapFrom(s => s.StakingPeriodInDays))
                .ForMember(c => c.StakingRule, opt => opt.MapFrom(s => s.ExpirationBurnRatio));

            CreateMap<MessageGetResponseModel, PartnerMessage>(MemberList.Destination)
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.CreationTimestamp))
                .ForMember(dest => dest.LocationId, opt => opt.Ignore())
                .ForMember(dest => dest.PartnerMessageId, opt => opt.Ignore())
                .ForMember(dest => dest.PartnerName, opt => opt.Ignore())
                .ForMember(dest => dest.LocationName, opt => opt.Ignore());

            CreateMap<PartnerMessage, GetPartnerMessageResponseModel>(MemberList.Destination);

            CreateMap<GoogleRegistrationRequestModel, GoogleRegistrationRequestDto>();
            CreateMap<CustomerProfile.Client.Models.Responses.CustomerProfile, CustomerInfoModel>()
                .ForMember(x => x.CountryPhoneCode, opt => opt.Ignore())
                .ForMember(x => x.CountryOfNationalityName, opt => opt.Ignore());

            CreateMap<CommonInformationPropertiesModel, CommonInformationResponse>()
                .ForSourceMember(src => src.UnsubscribeLink, opt => opt.DoNotValidate());

            CreateMap<MAVN.Service.Dictionaries.Client.Models.Salesforce.CountryPhoneCodeModel, CountryPhoneCodeModel>();
            CreateMap<MAVN.Service.Dictionaries.Client.Models.Salesforce.CountryOfResidenceModel, CountryOfResidenceModel>();

            CreateMap<LinkingRequestResponseModel, LinkingRequestResultModel>();
            CreateMap<LinkingApprovalResponseModel, LinkingApprovalResultModel>();
            CreateMap<UnlinkResponseModel, UnlinkResultModel>();
            CreateMap<PublicAddressResponseModel, PublicAddressResultModel>();
            CreateMap<TransferToExternalResponse, TransferToExternalResultModel>();

            CreateMap<TransferBalanceResponse, TransferResultModel>()
                .ForSourceMember(s => s.Timestamp, opt => opt.DoNotValidate())
                .ForSourceMember(s => s.ExternalOperationId, opt => opt.DoNotValidate());

            CreateMap<TransferResultModel, TransferOperationResponse>()
                .ForSourceMember(s => s.ErrorCode, opt => opt.DoNotValidate());

            CreateMap<PaginatedNotificationMessagesModel, PaginatedNotificationMessagesResponse>(MemberList.Destination);

            CreateMap<NotificationMessageResponseModel, NotificationMessage>(
                    MemberList.Destination)
                .ForMember(dest => dest.Payload, opt => opt.MapFrom(src => src.CustomPayload));

            CreateMap<PaginatedVoucherCampaignsListResponseModel, SmartVoucherCampaignsListResponse>()
                .ForMember(x => x.SmartVoucherCampaigns, opt => opt.MapFrom(x => x.Campaigns));

            CreateMap<VoucherCampaignResponseModel, SmartVoucherCampaignModel>()
                .ForMember(x => x.Vertical, opt => opt.Ignore())
                .ForMember(x => x.PartnerName, opt => opt.Ignore());
            CreateMap<VoucherCampaignDetailsResponseModel, SmartVoucherCampaignDetailsModel>()
                .ForMember(x => x.Vertical, opt => opt.Ignore())
                .ForMember(x => x.Geolocations, opt => opt.Ignore())
                .ForMember(x => x.PartnerName, opt => opt.Ignore());
            CreateMap<VoucherCampaignContentResponseModel, SmartVoucherCampaignContentResponseModel>();
            CreateMap<SmartVouchers.Client.Models.Responses.FileResponseModel, FileResponseModel>();
            CreateMap<VoucherResponseModel, SmartVoucherResponse>();
            CreateMap<VoucherDetailsResponseModel, SmartVoucherDetailsResponse>();
            CreateMap<PaginatedVouchersListResponseModel, SmartVouchersListResponse>()
                .ForMember(x => x.SmartVouchers, opt => opt.MapFrom(x => x.Vouchers));
        }


        private Core.Domain.CommonReferral.CommonReferralStatus MapCommonReferralStatus(CommonReferralStatus valueStatus)
        {
            switch (valueStatus)
            {
                case CommonReferralStatus.Confirmed:
                    return Core.Domain.CommonReferral.CommonReferralStatus.AcceptedByLead;
                case CommonReferralStatus.Pending:
                    return Core.Domain.CommonReferral.CommonReferralStatus.Ongoing;
                case CommonReferralStatus.Accepted:
                    return Core.Domain.CommonReferral.CommonReferralStatus.Accepted;
                case CommonReferralStatus.Expired:
                    return Core.Domain.CommonReferral.CommonReferralStatus.Expired;
                default:
                    return Core.Domain.CommonReferral.CommonReferralStatus.Expired;
            }
        }
    }

    public class PushNotificationRegistrationResultConverter
        : ITypeConverter<PushTokenInsertionResult, PushNotificationRegistrationResult>
    {
        public PushNotificationRegistrationResult Convert(
            PushTokenInsertionResult source,
            PushNotificationRegistrationResult destination,
            ResolutionContext context)
        {
            switch (source)
            {
                case PushTokenInsertionResult.PushRegistrationTokenAlreadyExists:
                    return PushNotificationRegistrationResult.PushRegistrationAlreadyExists;
                case PushTokenInsertionResult.Ok:
                    return PushNotificationRegistrationResult.Ok;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
