using System;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.PartnerPayments
{
    /// <summary>
    /// The payment request as a part of array of requests
    /// </summary>
    [PublicAPI]
    public class PartnerPaymentRequestItemResponse
    {
        /// <summary>
        /// The payment request id
        /// </summary>
        public string PaymentRequestId { get; set; }
        
        /// <summary>
        /// The payment request status
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// The total bill in tokens
        /// </summary>
        public string TotalInToken { get; set; }
        
        /// <summary>
        /// The total bill in base currency
        /// </summary>
        public decimal TotalInCurrency { get; set; }
        
        /// <summary>
        /// The amount customer wished to pay in tokens
        /// </summary>
        public string SendingAmountInToken { get; set; }
        
        /// <summary>
        /// The fiat currency code, is base currency
        /// </summary>
        public string CurrencyCode { get; set; }
        
        /// <summary>
        /// The partner id
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// The name of the partner
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// The location id
        /// </summary>
        public string LocationId { get; set; }
        
        /// <summary>
        /// The payment info related to request and provided by partner
        /// </summary>
        public string PaymentInfo { get; set; }
        
        /// <summary>
        /// The datetime when payment request was created
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The datetime when payment request was last updated
        /// </summary>
        public DateTime LastUpdatedDate { get; set; }
    }
}
