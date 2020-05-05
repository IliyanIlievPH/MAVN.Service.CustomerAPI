using Falcon.Numerics;
using JetBrains.Annotations;
using MAVN.Service.PartnerManagement.Client.Models;

namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    [PublicAPI]
    public class ConditionBaseModel
    {
        /// <summary>The unique identifier.</summary>
        public string Id { get; set; }

        /// <summary>The bonus type name.</summary>
        public string Type { get; set; }

        /// <summary>The bonus type vertical.</summary>
        public Vertical? Vertical { get; set; }

        /// <summary>The bonus type display name.</summary>
        public string DisplayName { get; set; }

        /// <summary>The amount of reward that will be given once the condition is completed.</summary>
        public string ImmediateReward { get; set; }

        /// <summary>The number of condition completion.</summary>
        public int CompletionCount { get; set; }

        /// <summary>Identify if the condition has staking</summary>
        public bool HasStaking { get; set; }

        /// <summary>Represents stake amount</summary>
        public Money18? StakeAmount { get; set; }

        /// <summary>Represents a staking period</summary>
        public int? StakingPeriod { get; set; }

        /// <summary>Represents stake warning period</summary>
        public int? StakeWarningPeriod { get; set; }

        /// <summary>Represents staking percentage</summary>
        public decimal? StakingRule { get; set; }

        /// <summary>Represents staking burning percent</summary>
        public decimal? BurningRule { get; set; }

        /// <summary>Indicates the reward type.</summary>
        public RewardType RewardType { get; set; }

        /// <summary>The amount in tokens to calculate rate.</summary>
        public Money18? AmountInTokens { get; set; }

        /// <summary>The amount in currency to calculate rate.</summary>
        public decimal? AmountInCurrency { get; set; }

        /// <summary>Indicates that the partner currency rate should be used to convert an amount.</summary>
        public bool UsePartnerCurrencyRate { get; set; }

        /// <summary>Represents a condition reward ratio attribute</summary>
        public RewardRatioAttributeModel RewardRatio { get; set; }

        /// <summary>Represents a display value when percentage reward type is selected</summary>
        public Money18? ApproximateAward { get; set; }

        /// <summary>Indicates if the reward type is percentage or conversion rate</summary>
        public bool IsApproximate { get; set; }
    }
}
