using System.Collections.Generic;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Models.NotificationMessages
{
    /// <summary>
    /// Paginated notification messages response
    /// </summary>
    public class PaginatedNotificationMessagesResponse
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
        /// Collection of notification messages
        /// </summary>
        public IEnumerable<NotificationMessage> NotificationMessages { get; set; }
    }
}
