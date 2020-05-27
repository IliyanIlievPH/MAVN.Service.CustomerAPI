namespace MAVN.Service.CustomerAPI.Models.Vouchers
{
    /// <summary>
    /// Request model for smart voucher redemption.
    /// </summary>
    public class VoucherRedemptionRequest
    {
        /// <summary>
        /// Voucher short code
        /// </summary>
        public string VoucherShortCode { get; set; }

        /// <summary>
        /// Voucher validation code
        /// </summary>
        public string VoucherValidationCode { get; set; }
    }
}
