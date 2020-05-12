using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IPublicWalletLinkingService
    {
        Task<LinkingRequestResultModel> CreateLinkRequestAsync(string customerId);
        
        Task<LinkingApprovalResultModel> ApproveLinkRequestAsync(string privateAddress, string publicAddress, string signature);

        Task<UnlinkResultModel> UnlinkAsync(string customerId);

        Task<PublicAddressResultModel> GetLinkedWalletAddressAsync(string customerId);

        Task<Money18> GetNextFeeAsync(string customerId);
    }
}
