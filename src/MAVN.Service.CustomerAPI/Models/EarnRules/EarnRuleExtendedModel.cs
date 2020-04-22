using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    /// <summary>
    /// Represents an earn rule with customer's completion count.
    /// </summary>
    [PublicAPI]
    public class EarnRuleExtendedModel : EarnRuleBaseModel
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
        /// The count of condition completion by a current customer.
        /// </summary>
        public int CustomerCompletionCount { get; set; }

        /// <summary>
        /// The amount in tokens which the customer has earned for the moment
        /// </summary>
        public string CurrentRewardedAmount { get; set; }

        /// <summary>
        /// Represents a mandatory condition
        /// </summary>
        public IReadOnlyList<ConditionExtendedModel> Conditions { get; set; }

        /// <summary>
        /// Represents a list of optional Conditions' names
        /// </summary>
        public IReadOnlyList<ConditionExtendedModel> OptionalConditions { get; set; }
    }
}
