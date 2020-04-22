using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    public class EarnRuleModel : EarnRuleBaseModel
    {
        /// <summary>
        /// Represents a list of Conditions' names
        /// </summary>
        public IReadOnlyList<ConditionModel> Conditions { get; set; }

        /// <summary>
        /// Represents a list of optional Conditions' names
        /// </summary>
        public IReadOnlyList<ConditionModel> OptionalConditions { get; set; }
    }
}
