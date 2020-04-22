using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class PaginatedOperationsHistoryModel
    {
        public IEnumerable<OperationHistoryModel> Operations { get; set; }

        public int TotalCount { get; set; }
    }
}
