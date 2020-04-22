using System;
using Falcon.Numerics;

namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    public class EarnRuleBaseModel
    {
        /// <summary>
        /// The earn rule unique identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The earn rule localized title. 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Represents status of the Earn Rule
        /// </summary>
        public CampaignStatus Status { get; set; }

        /// <summary>
        /// The earn rule localized description. 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The earn rule localized imageUrl. 
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Represents the Reward that is going to be granted once all conditions are met
        /// </summary>
        public string Reward { get; set; }

        /// <summary>
        /// Type of the reward for the Earn Rule
        /// </summary>
        public RewardType RewardType { get; set; }

        /// <summary>
        /// Represents Start Date of the Earn Rule
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Represents End Date of the Earn Rule
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Represents identification of User who created Earn Rule
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Represents the creation date of the Earn Rule
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Represents how many times, Earn Rule can be completed
        /// </summary>
        public int CompletionCount { get; set; }

        /// <summary>
        /// Represents a display value when percentage reward type is selected
        /// </summary>
        public Money18? ApproximateAward { get; set; }

        /// <summary>
        /// Indicates if the reward type is percentage or conversion rate
        /// </summary>
        public bool IsApproximate { get; set; }

        /// <summary>
        /// Indicates the order of the earn rule.
        /// </summary>
        public int Order { get; set; }
    }
}
