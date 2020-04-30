using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Response model for smart voucher campaigns list
    /// </summary>
    [PublicAPI]
    public class SmartVoucherCampaignsListResponse
    {
        /// <summary>
        /// List of campaigns
        /// </summary>
        public IReadOnlyList<SmartVoucherCampaignDetailsModel> SmartVoucherCampaigns { get; set; }
        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; set; }
    }
}
