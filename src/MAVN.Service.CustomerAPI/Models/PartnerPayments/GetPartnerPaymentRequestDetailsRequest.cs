using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.CustomerAPI.Models.PartnerPayments
{
    /// <summary>
    /// Request model
    /// </summary>
    public class GetPartnerPaymentRequestDetailsRequest
    {
        /// <summary>
        /// Id of the payment request
        /// </summary>
        [Required]
        public string PaymentRequestId { get; set; }
    }
}
