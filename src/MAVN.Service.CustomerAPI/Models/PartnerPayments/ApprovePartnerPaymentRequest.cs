using System.ComponentModel.DataAnnotations;
using MAVN.Numerics;

namespace MAVN.Service.CustomerAPI.Models.PartnerPayments
{
    /// <summary>
    /// Request model
    /// </summary>
    public class ApprovePartnerPaymentRequest
    {
        /// <summary>
        /// Id of the payment request
        /// </summary>
        [Required]
        public string PaymentRequestId { get; set; }

        /// <summary>
        /// Amount which customer wants to pay
        /// </summary>
        public Money18 SendingAmount { get; set; }
    }
}
