namespace MAVN.Service.CustomerAPI.Core.Domain.CommonReferral
{
    public class RatioAttributeModel
    {
        public int Order { get; set; }

        public decimal RewardRatio { get; set; }

        public decimal PaymentRatio { get; set; }

        public decimal Threshold { get; set; }
    }
}
