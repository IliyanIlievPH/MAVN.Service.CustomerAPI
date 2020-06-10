﻿using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.History
{
    /// <summary>
    /// The type of the operation in the history log
    /// </summary>
    [PublicAPI]
    public enum HistoryOperationType
    {
        /// <summary>Incoming transfer operation</summary>
        SendTransfer,

        /// <summary>Outgoing transfer operation</summary>
        ReceiveTransfer,

        /// <summary>The bonus reward operation</summary>
        BonusReward,

        /// <summary>Partners payment operation</summary>
        PartnerPayment,

        /// <summary>Partner payment refund operation</summary>
        PartnerPaymentRefund,

        /// <summary>
        /// Transfer to public network
        /// </summary>
        LinkedWalletSendTransfer,

        /// <summary>
        /// Transfer from public network
        /// </summary>
        LinkedWalletReceiveTransfer,

        /// <summary>Referral stake operation</summary>
        ReferralStake,

        /// <summary>Released referral stake operation</summary>
        ReleasedReferralStake,

        /// <summary>Wallet Linking fee collection operation</summary>
        WalletLinkingFee,

        /// <summary>
        /// transfer to public fee collection operation
        /// </summary>
        TransferToPublicFee,

        /// <summary>
        /// Voucher purchase payment operation
        /// </summary>
        VoucherPurchasePayment,

        /// <summary>
        /// Smart voucher payment operation
        /// </summary>
        SmartVoucherPayment,

        /// <summary>
        /// Smart voucher use operation
        /// </summary>
        SmartVoucherUse,

        /// <summary>
        /// Smart voucher transfer operation for sender
        /// </summary>
        SmartVoucherTransferSend,

        /// <summary>
        /// Smart voucher transfer operation for receiver
        /// </summary>
        SmartVoucherTransferReceive,
    }
}
