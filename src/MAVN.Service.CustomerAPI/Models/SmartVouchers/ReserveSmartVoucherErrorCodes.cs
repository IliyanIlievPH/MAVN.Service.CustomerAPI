namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    public enum ReserveSmartVoucherErrorCodes
    {
        /// <summary>
        /// No errors
        /// </summary>
        None,
        /// <summary>
        /// There is already reserved voucher by this customer
        /// </summary>
        CustomerHaveAnotherReservedVoucher
    }
}
