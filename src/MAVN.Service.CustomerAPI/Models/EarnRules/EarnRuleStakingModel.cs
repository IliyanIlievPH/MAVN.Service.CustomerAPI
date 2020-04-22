using Falcon.Numerics;
using System;

namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    public class EarnRuleStakingModel
    {
        /// <summary>
        /// Unique identifier of the referral stake
        /// </summary>
        public string ReferralId { get; set; }

        /// <summary>
        /// Full name of the referral 
        /// </summary>
        public string ReferralName { get; set; }

        /// <summary>
        /// Represents stake amount 
        /// </summary>
        public Money18 StakeAmount { get; set; }

        /// <summary>
        /// Represents total reward amount 
        /// </summary>
        public Money18 TotalReward { get; set; }

        /// <summary>
        /// Represents a staking period
        /// </summary>
        public int StakingPeriod { get; set; }

        /// <summary>
        /// Represents stake warning period
        /// </summary>
        public int StakeWarningPeriod { get; set; }

        /// <summary>
        /// Represents staking percentage
        /// </summary>
        public decimal StakingRule { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
