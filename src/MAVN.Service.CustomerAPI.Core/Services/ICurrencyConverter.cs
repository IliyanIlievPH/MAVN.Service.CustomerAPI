using System.Threading.Tasks;
using MAVN.Numerics;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface ICurrencyConverter
    {
        Task<Money18> GetCurrencyAmountInBaseCurrencyAsync(decimal amount, string fromAsset, string customerId, string partnerId);

        Task<Money18> GetCurrencyAmountInTokensAsync(decimal amount, string fromAsset, string customerId, string partnerId);

        string GetBaseCurrencyCode();
    }
}
