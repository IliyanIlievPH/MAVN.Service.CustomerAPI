using System;
using MAVN.Service.CustomerAPI.Models.PartnerPayments;
using Lykke.Service.PartnersPayments.Client.Enums;

namespace MAVN.Service.CustomerAPI.Models.Extensions
{
    public static class PartnersPaymentsStatusExtensions
    {
        public static PartnerPaymentPublicStatus ConvertToPublicStatus(this PaymentRequestStatus partnerPaymentStatus)
        {
            switch (partnerPaymentStatus)
            {
                case PaymentRequestStatus.Created:
                case PaymentRequestStatus.TokensTransferStarted:
                    return PartnerPaymentPublicStatus.Pending;
                case PaymentRequestStatus.RejectedByCustomer:
                case PaymentRequestStatus.TokensRefundSucceeded:
                case PaymentRequestStatus.CancelledByPartner:
                    return PartnerPaymentPublicStatus.Cancelled;
                case PaymentRequestStatus.TokensTransferSucceeded:
                case PaymentRequestStatus.TokensBurnStarted:
                case PaymentRequestStatus.TokensRefundStarted:
                    return PartnerPaymentPublicStatus.Confirmed;
                case PaymentRequestStatus.TokensBurnSucceeded:
                    return PartnerPaymentPublicStatus.Completed;
                case PaymentRequestStatus.TokensBurnFailed:
                case PaymentRequestStatus.TokensRefundFailed:
                case PaymentRequestStatus.TokensTransferFailed:
                case PaymentRequestStatus.ExpirationTokensRefundFailed:
                    return PartnerPaymentPublicStatus.Failed;
                case PaymentRequestStatus.RequestExpired:
                    return PartnerPaymentPublicStatus.RequestExpired;
                case PaymentRequestStatus.ExpirationTokensRefundStarted:
                case PaymentRequestStatus.ExpirationTokensRefundSucceeded:
                    return PartnerPaymentPublicStatus.PaymentExpired;
                default:
                    throw new ArgumentOutOfRangeException(nameof(partnerPaymentStatus), partnerPaymentStatus, null);
            }
        }
    }
}
