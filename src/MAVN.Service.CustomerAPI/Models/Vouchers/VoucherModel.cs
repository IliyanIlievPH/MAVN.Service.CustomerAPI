using System;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Vouchers
{
    /// <summary>
    /// Represents a voucher.
    /// </summary>
    [PublicAPI]
    public class VoucherModel
    {
        /// <summary>
        /// Voucher Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Spend Rule Name associated with the voucher
        /// </summary>
        public string SpendRuleName { get; set; }

        /// <summary>
        /// Partner Name associated with the voucher
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// Price in Tokens
        /// </summary>
        public string PriceToken { get; set; }

        /// <summary>
        /// Price in Base Currency
        /// </summary>
        public decimal PriceBaseCurrency { get; set; }

        /// <summary>
        /// Purchase Date
        /// </summary>
        public DateTime PurchaseDate { get; set; }
    }
}
