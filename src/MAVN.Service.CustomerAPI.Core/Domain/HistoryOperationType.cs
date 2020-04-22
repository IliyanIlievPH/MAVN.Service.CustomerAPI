namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public enum HistoryOperationType
    {
        SendTransfer,
        ReceiveTransfer,
        BonusReward,
        PaymentTransfer,
        PaymentTransferRefund,
        PartnerPayment,
        PartnerPaymentRefund,
        LinkedWalletSendTransfer,
        LinkedWalletReceiveTransfer,
        ReferralStake,
        ReleasedReferralStake,
        WalletLinkingFee,
        TransferToPublicFee,
        VoucherPurchasePayment
    }
}
