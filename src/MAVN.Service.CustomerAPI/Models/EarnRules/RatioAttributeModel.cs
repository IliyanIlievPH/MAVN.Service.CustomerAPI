namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    /// <summary>
    /// Represents a ration attribute model
    /// </summary>
    public class RatioAttributeModel
    {
        /// <summary>
        /// Represents ratio order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Represents reward ratio percent
        /// </summary>
        public decimal RewardRatio { get; set; }

        /// <summary>
        /// Represents payment ratio percent
        /// </summary>
        public decimal PaymentRatio { get; set; }

        /// <summary>
        /// Represents reward threshold
        /// </summary>
        public decimal Threshold { get; set; }
    }
}
