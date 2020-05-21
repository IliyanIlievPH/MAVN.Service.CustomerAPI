using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Request to gey payment info for smart voucher
    /// </summary>
    public class GetSmartVoucherPaymentInfoRequest
    {
        /// <summary>
        /// Short code of the smart voucher
        /// </summary>
        [Required]
        public string ShortCode { get; set; }
    }
}
