using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IPublicWalletTransferService
    {
        Task<TransferToExternalResultModel> TransferAsync(string customerId, string amount);
    }
}
