using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Request model to cancel a smart voucher reservation
    /// </summary>
    public class CancelSmartVoucherReservationRequest
    {
        /// <summary>
        /// The short code of the voucher
        /// </summary>
        [Required]
        public string ShortCode { get; set; }
    }
}
