using System;

namespace MAVN.Service.CustomerAPI.Models.PartnersMessages
{
    /// <summary>
    /// Get partner message response model
    /// </summary>
    public class GetPartnerMessageResponseModel
    {
        /// <summary>
        /// Partner message id
        /// </summary>
        public string PartnerMessageId { get; set; }

        /// <summary>
        /// Partner id
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Partner name
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// Location id
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Location name
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Customer id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        public string Message { get; set; }
    }
}
