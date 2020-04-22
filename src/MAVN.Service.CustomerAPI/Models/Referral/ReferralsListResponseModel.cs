using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    public class ReferralsListResponseModel
    {
        public IReadOnlyList<CustomerCommonReferralResponseModel> Referrals { get; set; }

        /// <summary>
        /// Current page
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total count of records
        /// </summary>
        public int TotalCount { get; set; }
    }
}
