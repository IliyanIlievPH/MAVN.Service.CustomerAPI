using MAVN.Service.CustomerAPI.Core.Constants;

namespace MAVN.Service.CustomerAPI.Models.ConversionRate
{
    public class ConversionRateResponseModel
    {
        public string Amount { get; set; }

        public string Rate { get; set; }

        public string CurrencyCode { get; set; }

        public ConversionRateErrorCodes Error { get; set; }
    }
}
