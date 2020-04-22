using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.CustomerAPI.Models.History
{
    /// <summary>
    /// Response model for operation history
    /// </summary>
    [PublicAPI]
    public class OperationHistoryResponseModel
    {
        /// <summary>
        /// The type of the operation
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public HistoryOperationType Type { get; set; }
        
        /// <summary>
        /// The timestamp of the operation
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// The amount of tokens
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// The name of the campaign or condition for bonus reward
        /// </summary>
        public string ActionRule { get; set; }

        /// <summary>
        /// Email of the other customer
        /// </summary>
        public string OtherSideCustomerEmail { get; set; }

        /// <summary>
        /// Name of the partner
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// Name of the instalment
        /// </summary>
        public string InstalmentName { get; set; }
    }
}
