using System;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string VoucherShortCode { get; set; }

        /// <summary>
        /// Voucher validation code
        /// </summary>
        [Required]
        public string VoucherValidationCode { get; set; }

        /// <summary>
        /// Id of the seller
        /// </summary>
        public Guid? SellerCustomerId { get; set; }
    }
}
