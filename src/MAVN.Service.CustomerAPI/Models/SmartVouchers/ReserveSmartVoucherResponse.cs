using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Response model for smart voucher reservation operation
    /// </summary>
    [PublicAPI]
    public class ReserveSmartVoucherResponse
    {
        /// <summary>
        /// The payment url for the voucher
        /// </summary>
        public string PaymentUrl { get; set; }

        /// <summary>
        /// Error Code
        /// </summary>
        public ReserveSmartVoucherErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// Short code of the already reserved voucher
        /// </summary>
        public string AlreadyReservedVoucherShortCode { get; set; }
    }
}
