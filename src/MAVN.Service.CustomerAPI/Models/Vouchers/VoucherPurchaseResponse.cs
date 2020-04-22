using System;

namespace MAVN.Service.CustomerAPI.Models.Vouchers
{
    /// <summary>
    /// Represents a voucher purchase result.
    /// </summary>
    public class VoucherPurchaseResponse
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The voucher unique code that used to display.
        /// </summary>
        public string Code { get; set; }
    }
}
