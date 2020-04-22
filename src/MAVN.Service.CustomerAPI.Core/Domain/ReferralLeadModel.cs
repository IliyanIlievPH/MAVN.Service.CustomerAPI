using System;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class ReferralLeadModel
    {
        public string Name { get; set; }

        public ReferralLeadStatus Status { get; set; }

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
