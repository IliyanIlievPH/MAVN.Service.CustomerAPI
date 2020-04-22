using System;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Wallets
{
    /// <summary>
    /// Response model representing a balance transfer between customers
    /// </summary>
    [PublicAPI]
    public class TransferResponseModel
    {
        /// <summary>
        /// Flag which shows if the customer is sender or receiver
        /// </summary>
        public bool IsSender { get; set; }

        /// <summary>
        /// AssetSymbol
        /// </summary>
        public string AssetSymbol { get; set; }

        /// <summary>
        /// Transferred amount
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Timestamp of the transfer
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
