using System;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Models.Enums;

namespace MAVN.Service.CustomerAPI.Models.SpendRules
{
    /// <summary>
    /// Represents a spend rule.
    /// </summary>
    [PublicAPI]
    public class SpendRuleBaseModel
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The spend rule title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The name of the target currency 
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// The spend rule localized description. 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The spend rule localized imageUrl. 
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The spend rule business vertical. 
        /// </summary>
        public BusinessVertical BusinessVertical { get; set; }

        /// <summary>
        /// Represents spend rule's creation date
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Indicates the order of the burn rule.
        /// </summary>
        public int Order { get; set; }

        ///
        ///  Represents the fiat price for the spend rule
        ///
        public decimal? Price { get; set; }

        /// <summary>
        /// Represents the available stock of the item offered with the spend rule
        /// </summary>
        public long? StockCount { get; set; }

        /// <summary>
        /// Represents the sold count of the item offered with the spend rule
        /// </summary>
        public long? SoldCount { get; set; }

        /// <summary>
        /// Represents the token price for the spend rule
        /// </summary>
        public string PriceInToken { get; set; }
    }
}
