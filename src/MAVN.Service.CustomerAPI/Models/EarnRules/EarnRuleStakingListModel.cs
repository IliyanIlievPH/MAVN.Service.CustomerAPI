using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    /// <summary>
    /// Represents Earn Rules Stakings
    /// </summary>
    public class EarnRuleStakingListModel
    {
        /// <summary>
        /// Earn Rule stakings
        /// </summary>
        public IEnumerable<EarnRuleStakingModel> EarnRuleStakings { get; set; }

        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; set; }
    }
}
