namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public enum HistoryOperationType
    {
        SendTransfer,
        ReceiveTransfer,
        BonusReward,
        PartnerPayment,
        PartnerPaymentRefund,
        LinkedWalletSendTransfer,
        LinkedWalletReceiveTransfer,
        ReferralStake,
        ReleasedReferralStake,
        WalletLinkingFee,
        TransferToPublicFee,
        VoucherPurchasePayment,
        SmartVoucherPayment,
        SmartVoucherUse,
        SmartVoucherTransferSend,
        SmartVoucherTransferReceive,
    }
}
