using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Core.Domain.CommonReferral
{
    public class RewardRatioAttribute
    {
        /// <summary>
        /// Represents a list of condition's ratios 
        /// </summary>
        public IReadOnlyList<RatioAttributeModel> Ratios { get; set; }

        /// <summary>
        /// Represents a list of condition's ratios completions
        /// </summary>
        public IReadOnlyList<RatioCompletion> RatioCompletion { get; set; }
    }
}
