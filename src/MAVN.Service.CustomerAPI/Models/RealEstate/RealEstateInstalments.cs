using System;
using Falcon.Numerics;

namespace MAVN.Service.CustomerAPI.Models.RealEstate
{
    public class RealEstateInstalments
    {
        public string Id { get; set; }

        public Money18 AmountInTokens { get; set; }

        public decimal AmountInFiat { get; set; }

        public DateTime DueDate { get; set; }

        public string Name { get; set; }

        public string FiatCurrencyCode { get; set; }
    }
}
