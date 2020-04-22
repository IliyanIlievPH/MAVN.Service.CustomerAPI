using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Vouchers
{
    /// <summary>
    /// Represents a voucher list model.
    /// </summary>
    [PublicAPI]
    public class VoucherListModel
    {
        public IReadOnlyList<VoucherListDetailsModel> Vouchers { get; set; }

        public int TotalCount { get; set; }
    }
}
