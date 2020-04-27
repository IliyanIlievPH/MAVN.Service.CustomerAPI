using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.SpendRules
{
    /// <summary>
    /// Represents Spend Rules
    /// </summary>
    public class SpendRulesListResponseModel
    {
        /// <summary>Spend Rules</summary>
        public IEnumerable<SpendRuleListDetailsModel> SpendRules { get; set; }

        /// <summary>The current page number</summary>
        public int CurrentPage { get; set; }

        /// <summary>Size of a page</summary>
        public int PageSize { get; set; }

        /// <summary>Total count</summary>
        public int TotalCount { get; set; }
    }
}
