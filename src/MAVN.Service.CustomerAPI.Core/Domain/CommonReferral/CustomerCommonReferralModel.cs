using System;

namespace MAVN.Service.CustomerAPI.Core.Domain.CommonReferral
{
    public class CustomerCommonReferralModel
    {
        /// <summary>
        /// Represents common referral type
        /// </summary>
        public ReferralType ReferralType { get; set; }

        /// <summary>
        ///  Represents a common referral status
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
        public RewardRatioAttribute RewardRatio { get; set; }

        /// <summary>
        /// Indicates iof the referral has ratio
        /// </summary>
        public bool RewardHasRatio { get; set; }

        /// <summary>
        /// Represents referral staking model
        /// </summary>
        public ReferralStakingModel Staking { get; set; }

        /// <summary>
        /// Indicates if the reward type is percentage or conversion rate
        /// </summary>
        public bool IsApproximate { get; set; }
    }
}
