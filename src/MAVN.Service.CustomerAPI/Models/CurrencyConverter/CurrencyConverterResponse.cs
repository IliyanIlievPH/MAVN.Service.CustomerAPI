using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.CurrencyConverter
{
    /// <summary>
    /// Represents a currency converter response.
    /// </summary>
    [PublicAPI]
    public class CurrencyConverterResponse
    {
        /// <summary>
        /// The formatted target currency amount. 
        /// </summary>
        public string Amount { get; set; }
    }
}
