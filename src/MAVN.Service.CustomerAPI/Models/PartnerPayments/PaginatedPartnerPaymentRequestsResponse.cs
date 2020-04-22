using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.PartnerPayments
{
    /// <summary>
    /// Response model
    /// </summary>
    public class PaginatedPartnerPaymentRequestsResponse
    {
        /// <summary>
        /// The current page number
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// Size of a page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Total count of all items
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// Collection of payment requests
        /// </summary>
        public IEnumerable<PartnerPaymentRequestItemResponse> PaymentRequests { get; set; }
    }
}
