using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Wallets
{
    /// <summary>
    /// Paginated response model with list of transfers
    /// </summary>
    [PublicAPI]
    public class PaginatedTransfersResponseModel
    {
        /// <summary>
        /// List of transfers
        /// </summary>
        public IEnumerable<TransferResponseModel> Transfers { get; set; }
        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; set; }
    }
}
