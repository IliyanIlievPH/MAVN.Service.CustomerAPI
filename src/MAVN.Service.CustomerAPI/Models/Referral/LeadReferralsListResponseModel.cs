using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    public class LeadReferralsListResponseModel
    {
        /// <summary>
        /// ReferralLeads
        /// </summary>
        public IReadOnlyCollection<LeadReferral> LeadReferrals { get; set; }
    }
}
