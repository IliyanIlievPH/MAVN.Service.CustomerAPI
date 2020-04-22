using System;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    /// <summary>
    /// Represents referral lead.
    /// </summary>
    [PublicAPI]
    public class LeadReferral
    {
        /// <summary>
        /// Name of the referred lead
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Status of the referral
        /// </summary>
        public ReferralLeadStatus Status { get; set; }

        /// <summary>
        /// TimeStamp of the referral
        /// </summary>
        public DateTime TimeStamp { get; set; }
        
        /// <summary>
        /// The number of SPA (sales purchase agreement)
        /// </summary>
        public int PurchaseCount { get; set; }

        /// <summary>
        /// The number of OTP (offers to purchase).
        /// </summary>
        public int OffersCount { get; set; }
    }
}
