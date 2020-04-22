using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class PaginatedTransfersModel
    {
        public IEnumerable<TransferInfoModel> Transfers { get; set; }

        public int TotalCount { get; set; }
    }
}
