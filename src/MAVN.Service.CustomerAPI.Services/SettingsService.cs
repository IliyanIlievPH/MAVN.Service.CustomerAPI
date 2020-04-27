using MAVN.Service.CustomerAPI.Core.Services;

namespace MAVN.Service.CustomerAPI.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _tokenName;
        private readonly string _usdAssetName;
        private readonly string _baseCurrencyCode;
        private readonly string _transitAccountAddress;
        private readonly bool _isPublicBlockchainFeatureDisabled;

        public SettingsService(
            string tokenName,
            string usdAssetName,
            string baseCurrencyCode,
            string transitAccountAddress,
            bool isPublicBlockchainFeatureDisabled)
        {
            _tokenName = tokenName;
            _usdAssetName = usdAssetName;
            _baseCurrencyCode = baseCurrencyCode;
            _transitAccountAddress = transitAccountAddress;
            _isPublicBlockchainFeatureDisabled = isPublicBlockchainFeatureDisabled;
        }

        public string GetTokenName()
            => _tokenName;

        public string GetUsdAssetName()
            => _usdAssetName;

        public string GetBaseCurrencyCode()
            => _baseCurrencyCode;

        public string GetTransitAccountAddress()
            => _transitAccountAddress;

        public bool GetIsPublicBlockchainFeatureDisabled() => _isPublicBlockchainFeatureDisabled;
    }
}
