using System.Threading.Tasks;
using Falcon.Common.Middleware.Authentication;
using Falcon.Common.Middleware.Version;
using Falcon.Numerics;
using Lykke.Service.CurrencyConvertor.Client;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Models.CurrencyConverter;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/currencyConverter")]
    [LowerVersion(Devices = "android", LowerVersion = 659)]
    [LowerVersion(Devices = "IPhone,IPad", LowerVersion = 181)]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConvertorClient _currencyConverterClient;

        public CurrencyConverterController(ICurrencyConvertorClient currencyConverterClient)
        {
            _currencyConverterClient = currencyConverterClient;
        }

        /// <summary>
        /// Converts tokens to base currency using global currency rate.
        /// </summary>
        /// <param name="amount">The amount to be converted.</param>
        /// <returns>
        /// 200 - The result of convert.
        /// </returns>
        [HttpGet("tokens/baseCurrency")]
        public async Task<CurrencyConverterResponse> ConvertTokensToBaseCurrencyAsync(Money18 amount)
        {
            var result = await _currencyConverterClient.Converter.ConvertTokensToBaseCurrencyAsync(amount);

            return new CurrencyConverterResponse { Amount = Money18.Create(result.Amount).ToDisplayString() };
        }
    }
}
