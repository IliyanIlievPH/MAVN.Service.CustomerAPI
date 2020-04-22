using System;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    public class ReferralStakingModel
    {
        /// <summary>
        /// Represents stake amount 
        /// </summary>
        public string StakeAmount { get; set; }

        /// <summary>
        /// Staking expiration date
        /// </summary>
        public DateTime StakingExpirationDate { get; set; }
    }
}
