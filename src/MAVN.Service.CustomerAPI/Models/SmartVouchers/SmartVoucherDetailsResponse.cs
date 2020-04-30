using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Holds information about smart voucher
    /// </summary>
    [PublicAPI]
    public class SmartVoucherDetailsResponse : SmartVoucherResponse
    {
        /// <summary>Voucher validation code</summary>
        public string ValidationCode { get; set; }
    }
}
