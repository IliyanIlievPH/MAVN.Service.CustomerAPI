using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.History
{
    /// <summary>
    /// Paginated response model with list of operations history
    /// </summary>
    [PublicAPI]
    public class PaginatedOperationsHistoryResponseModel
    {
        /// <summary>
        /// The list of operations history
        /// </summary>
        public IEnumerable<OperationHistoryResponseModel> Operations { get; set; }
        
        /// <summary>
        /// The total count of records
        /// </summary>
        public int TotalCount { get; set; }
    }
}
