using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    /// <summary>
    /// Represents Earn Rules
    /// </summary>
    public class EarnRulesListResponseModel
    {
        /// <summary>
        /// Earn Rules
        /// </summary>
        public IEnumerable<EarnRuleModel> EarnRules { get; set; } 
        
        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; set; }
    }
}