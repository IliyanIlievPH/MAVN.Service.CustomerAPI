﻿namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Holds the possible statues of a smart voucher
    /// </summary>
    public enum SmartVoucherStatus
    {
        /// <summary>Unspecified status.</summary>
        None,
        /// <summary>Indicates that the voucher is in stock.</summary>
        InStock,
        /// <summary>Indicated that the voucher is reserved by customer and waiting for payment.</summary>
        Reserved,
        /// <summary>Indicates that the voucher is bought by a customer.</summary>
        Sold,
        /// <summary>Indicates that the voucher is used by a customer.</summary>
        Used,
        /// <summary>Indicates that the voucher has expired.</summary>
        Expired,
    }
}
