using System.ComponentModel.DataAnnotations;
using MAVN.Service.CustomerAPI.Validation;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Request model to transfer a smart voucher
    /// </summary>
    public class SmartVoucherTransferRequest
    {
        /// <summary>
        /// Email of the receiver
        /// </summary>
        [Required, DataType(DataType.EmailAddress)]
        [RegularExpression(Patterns.EmailValidationPattern)]
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// Short code of the voucher
        /// </summary>
        [Required]
        public string VoucherShortCode { get; set; }
    }
}
