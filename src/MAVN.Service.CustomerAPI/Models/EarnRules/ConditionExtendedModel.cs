using JetBrains.Annotations;
using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    /// <summary>
    /// Represents Condition with Customer's completion count
    /// </summary>
    [PublicAPI]
    public class ConditionExtendedModel : ConditionBaseModel
    {
        /// <summary>
        /// The count of condition completion by a current customer.
        /// </summary>
        public int CustomerCompletionCount { get; set; }

        /// <summary>
        /// A collection of associated partners.
        /// </summary>
        public IReadOnlyList<PartnerModel> Partners { get; set; }
    }
}
