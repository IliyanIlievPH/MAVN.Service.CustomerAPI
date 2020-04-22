namespace MAVN.Service.CustomerAPI.Models.EarnRules
{
    public class RatioCompletion
    {
        public string PaymentId { get; set; }

        public string Name { get; set; }

        public decimal GivenThreshold { get; set; }

        public int Checkpoint { get; set; }

        public string GivenRatioRewardBonus { get; set; }

        public string TotalRatioRewardBonus { get; set; }
    }
}
