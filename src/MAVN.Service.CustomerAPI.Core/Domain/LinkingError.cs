namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public enum LinkingError
    {
        None,
        InvalidCustomerId,
        LinkingRequestAlreadyExists,
        CustomerWalletMissing,
        LinkingRequestDoesNotExist,
        InvalidPublicAddress,
        InvalidSignature,
        LinkingRequestAlreadyApproved,
        InvalidPrivateAddress,
        CannotDeleteLinkingRequestWhileConfirming,
        NotEnoughFunds,
        CustomerDoesNotExist,
        CustomerWalletBlocked,
    }
}
