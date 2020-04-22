using System.ComponentModel.DataAnnotations;
using Falcon.Numerics;

namespace MAVN.Service.CustomerAPI.Models.RealEstate
{
    public class InitiateRealEstatePaymentRequest
    {
        [Required]
        public string Id { get; set; }

        public Money18? AmountInTokens { get; set; }

        public decimal? AmountInFiat { get; set; }

        [Required]
        public string FiatCurrencyCode { get; set; }

        [Required]
        public string SpendRuleId { get; set; }

        [Required]
        public string InstalmentName { get; set; }
    }
}
