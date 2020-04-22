using System;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Models.Wallets;

namespace MAVN.Service.CustomerAPI.Models.Extensions
{
    public static class PublicAddressErrorExtensions
    {
        public static PublicAddressLinkingStatus ToLinkingStatus(this PublicAddressStatus src)
        {
            switch (src)
            {
                case PublicAddressStatus.Linked:
                    return PublicAddressLinkingStatus.Linked;
                case PublicAddressStatus.NotLinked:
                    return PublicAddressLinkingStatus.NotLinked;
                case PublicAddressStatus.PendingCustomerApproval:
                    return PublicAddressLinkingStatus.PendingCustomerApproval;
                case PublicAddressStatus.PendingConfirmation:
                    return PublicAddressLinkingStatus.PendingConfirmationInBlockchain;
                default:
                    throw new ArgumentOutOfRangeException(nameof(src), $"Public address status not expected: {src.ToString()}");
            }
        }
    }
}
