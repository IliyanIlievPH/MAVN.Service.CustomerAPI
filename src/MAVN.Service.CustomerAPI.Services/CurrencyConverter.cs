using System;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CurrencyConvertor.Client.Models.Enums;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.Service.EligibilityEngine.Client;
using Lykke.Service.EligibilityEngine.Client.Enums;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;

namespace MAVN.Service.CustomerAPI.Services
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly IEligibilityEngineClient _eligibilityEngineClient;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public CurrencyConverter(
            ISettingsService settingsService,
            ILogFactory logFactory, 
            IEligibilityEngineClient eligibilityEngineClient)
        {
            _settingsService = settingsService;
            _eligibilityEngineClient = eligibilityEngineClient;
            _log = logFactory.CreateLog(this);
        }

        public async Task<Money18> GetCurrencyAmountInBaseCurrencyAsync(decimal amount, string fromAsset, string customerId, string partnerId)
        {
            #region Validation
            
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            if (string.IsNullOrEmpty(fromAsset))
                throw new ArgumentNullException(nameof(fromAsset));
            
            #endregion
            
            var toAsset = _settingsService.GetBaseCurrencyCode();

            if (amount == 0)
                return 0;
            
            if (fromAsset == toAsset)
                return amount;

            var conversion = await _eligibilityEngineClient.ConversionRate.ConvertOptimalByPartnerAsync(
                new ConvertOptimalByPartnerRequest
                {
                    Amount = amount,
                    CustomerId = Guid.Parse(customerId),
                    FromCurrency = fromAsset,
                    ToCurrency = toAsset,
                    PartnerId = Guid.Parse(partnerId)
                });
            
            if (conversion.ErrorCode != EligibilityEngineErrors.None)
            {
                _log.Error(message: "Currency conversion error",
                    context: new {fromAsset, toAsset, error = conversion.ErrorCode.ToString()});
            }

            return conversion.Amount;
        }

        public async Task<Money18> GetCurrencyAmountInTokensAsync(decimal amount, string fromAsset, string customerId, string partnerId)
        {
            #region Validation

            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            if (string.IsNullOrEmpty(fromAsset))
                throw new ArgumentNullException(nameof(fromAsset)); 

            #endregion

            if (amount == 0)
                return 0;
            
            var toAsset = _settingsService.GetTokenName();

            if (fromAsset == toAsset)
            {
                return Money18.Create(amount);
            }
            else
            {
                var conversion = await _eligibilityEngineClient.ConversionRate.ConvertOptimalByPartnerAsync(
                    new ConvertOptimalByPartnerRequest
                    {
                        Amount = amount,
                        CustomerId = Guid.Parse(customerId),
                        FromCurrency = fromAsset,
                        ToCurrency = toAsset,
                        PartnerId = Guid.Parse(partnerId)
                    });
            
                if (conversion.ErrorCode != EligibilityEngineErrors.None)
                {
                    _log.Error(message: "Currency conversion error",
                        context: new {fromAsset, toAsset, error = conversion.ErrorCode.ToString()});
                }

                return conversion.Amount;
            }
        }

        public string GetBaseCurrencyCode()
        {
            return _settingsService.GetBaseCurrencyCode();
        }
    }
}
