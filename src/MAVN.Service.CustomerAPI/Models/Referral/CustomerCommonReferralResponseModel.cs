using System;
using MAVN.Service.CustomerAPI.Core.Domain.CommonReferral;
using MAVN.Service.CustomerAPI.Models.EarnRules;
using BusinessVertical = MAVN.Service.CustomerAPI.Models.Enums.BusinessVertical;
using CommonReferralStatus = MAVN.Service.CustomerAPI.Models.Enums.CommonReferralStatus;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    public class CustomerCommonReferralResponseModel
    {
        /// <summary>
        ///  Referral typed in common referral model
        /// </summary>
        public ReferralType ReferralType { get; set; }

        /// <summary>
        /// Common referral status
        /// </summary>
        public CommonReferralStatus Status { get; set; }

        /// <summary>
        /// If real estate
        /// </summary>
        public BusinessVertical? Vertical { get; set; }
        
        /// <summary>
        /// The first name of the referred customer.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the referred customer.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The email of the referred customer.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Partner's name.
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// TimeStamp of the referral
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Identifies if the referral is stakeable
        /// </summary>
        public bool HasStaking { get; set; }

        /// <summary>
        /// Represents total reward amount for earn rule
        /// </summary>
        public string TotalReward { get; set; }
    
        /// <summary>
        /// The amount in tokens which the customer has earned for the moment
        /// </summary>
        public string CurrentRewardedAmount { get; set; }

        /// <summary>
        /// Represents a condition reward ratio attribute
        /// </summary>
        public RewardRatioAttributeModel RewardRatio { get; set; }

        /// <summary>
        /// Identifies if referral has reward ratio
        /// </summary>
        public bool RewardHasRatio { get; set; }

        /// <summary>
        /// Represents a staking object of the earn rule has one
        /// </summary>
        public ReferralStakingModel Staking { get; set; }

        /// <summary>
        /// Indicates if the reward type is percentage or conversion rate
        /// </summary>
        public bool IsApproximate { get; set; }
    }
}
