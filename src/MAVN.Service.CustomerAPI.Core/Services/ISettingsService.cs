namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface ISettingsService
    {
        string GetTokenName();

        string GetUsdAssetName();

        string GetBaseCurrencyCode();

        string GetTransitAccountAddress();
    }
}
