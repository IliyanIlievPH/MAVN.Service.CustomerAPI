using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Response model for smart vouchers
    /// </summary>
    public class SmartVouchersListResponse
    {
        /// <summary>
        /// Collection of smart vouchers
        /// </summary>
        public IReadOnlyList<SmartVoucherResponse> SmartVouchers { get; set; }

        /// <summary>
        /// Total count of smart vouchers
        /// </summary>
        public int TotalCount { get; set; }
    }
}
