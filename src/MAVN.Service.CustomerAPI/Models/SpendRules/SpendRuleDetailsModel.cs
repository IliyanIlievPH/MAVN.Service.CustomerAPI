using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.SpendRules
{
    /// <summary>
    /// Represents a spend rule.
    /// </summary>
    [PublicAPI]
    public class SpendRuleDetailsModel : SpendRuleBaseModel
    {
        /// <summary>
        /// The amount in tokens.
        /// </summary>
        public string AmountInTokens { get; set; }

        /// <summary>
        /// The amount in the target currency.
        /// </summary>
        public decimal AmountInCurrency { get; set; }

        /// <summary>
        /// A collection of associated partners.
        /// </summary>
        public IReadOnlyList<PartnerModel> Partners { get; set; }
    }
}
