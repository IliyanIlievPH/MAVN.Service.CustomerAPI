namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public enum TransferToExternalErrorCodes
    {
        None,
        InvalidAmount,
        CustomerDoesNotExist,
        CustomerWalletBlocked,
        CustomerIdIsNotAValidGuid,
        CustomerWalletMissing,
        NotEnoughBalance,
        WalletIsNotLinked,
    }
}
