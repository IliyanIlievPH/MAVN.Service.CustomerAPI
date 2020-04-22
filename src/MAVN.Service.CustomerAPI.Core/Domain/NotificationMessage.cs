using System;
using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    /// <summary>
    /// Notification message
    /// </summary>
    public class NotificationMessage
    {
        /// <summary>
        /// Message group id
        /// </summary>
        public string MessageGroupId { get; set; }

        /// <summary>
        /// Sent timestamp
        /// </summary>
        public DateTime CreationTimestamp { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Payload
        /// </summary>
        public Dictionary<string, string> Payload { get; set; }

        /// <summary>
        /// States if message is read
        /// </summary>
        public bool IsRead { get; set; }
    }
}
