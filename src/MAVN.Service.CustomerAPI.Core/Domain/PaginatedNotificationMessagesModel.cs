using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    /// <summary>
    /// Paginated notification messages model
    /// </summary>
    public class PaginatedNotificationMessagesModel
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
