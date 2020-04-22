using MAVN.Service.CustomerAPI.Core.Services;

namespace MAVN.Service.CustomerAPI.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _emaarTokenName;
        private readonly string _usdAssetName;
        private readonly string _baseCurrencyCode;
        private readonly string _transitAccountAddress;

        public SettingsService(string emaarTokenName, string usdAssetName, string baseCurrencyCode, string transitAccountAddress)
        {
            _emaarTokenName = emaarTokenName;
            _usdAssetName = usdAssetName;
            _baseCurrencyCode = baseCurrencyCode;
            _transitAccountAddress = transitAccountAddress;
        }

        public string GetEmaarTokenName()
            => _emaarTokenName;

        public string GetUsdAssetName()
            => _usdAssetName;

        public string GetBaseCurrencyCode()
            => _baseCurrencyCode;

        public string GetTransitAccountAddress()
            => _transitAccountAddress;
    }
}
