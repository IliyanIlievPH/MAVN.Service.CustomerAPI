using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IWalletOperationsService
    {
        Task<TransferResultModel> TransferBalanceAsync(string senderCustomerId, string receiverAddress, Money18 amount);

        Task<WalletModel> GetCustomerWalletAsync(string customerId);
    }
}
