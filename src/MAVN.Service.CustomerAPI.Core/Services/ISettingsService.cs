namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface ISettingsService
    {
        string GetEmaarTokenName();

        string GetUsdAssetName();

        string GetBaseCurrencyCode();

        string GetTransitAccountAddress();
    }
}
