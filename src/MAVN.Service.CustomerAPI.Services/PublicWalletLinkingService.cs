using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.CrossChainWalletLinker.Client.Models;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using LinkingError = Lykke.Service.CrossChainWalletLinker.Client.Models.LinkingError;
using PublicAddressError = Lykke.Service.CrossChainWalletLinker.Client.Models.PublicAddressError;

namespace MAVN.Service.CustomerAPI.Services
{
    public class PublicWalletLinkingService : IPublicWalletLinkingService
    {
        private readonly ICrossChainWalletLinkerClient _walletLinkerClient;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public PublicWalletLinkingService(ICrossChainWalletLinkerClient walletLinkerClient, IMapper mapper, ILogFactory logFactory)
        {
            _walletLinkerClient = walletLinkerClient;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }

        public async Task<LinkingRequestResultModel> CreateLinkRequestAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));

            var response = await _walletLinkerClient.WalletLinkingApi.CreateLinkRequestAsync(Guid.Parse(customerId));
            
            if (response.Error != LinkingError.None) 
                _log.Warning(message: "Couldn't create new link request", context: new {customerId, error = response.Error.ToString()});

            return _mapper.Map<LinkingRequestResultModel>(response);
        }

        public async Task<LinkingApprovalResultModel> ApproveLinkRequestAsync(string privateAddress, string publicAddress, string signature)
        {
            if (string.IsNullOrEmpty(privateAddress))
                throw new ArgumentNullException(nameof(privateAddress));
            
            if (string.IsNullOrEmpty(publicAddress))
                throw new ArgumentNullException(nameof(publicAddress));
            
            if (string.IsNullOrEmpty(signature))
                throw new ArgumentNullException(nameof(signature));

            var response = await _walletLinkerClient.WalletLinkingApi.ApproveLinkRequestAsync(
                new LinkApprovalRequestModel
                {
                    PrivateAddress = privateAddress, PublicAddress = publicAddress, Signature = signature
                });
            
            if (response.Error != LinkingError.None)
                _log.Warning("Couldn't approve link request", context: new {privateAddress, publicAddress, signature});

            return _mapper.Map<LinkingApprovalResultModel>(response);
        }

        public async Task<UnlinkResultModel> UnlinkAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));
            
            var response = await _walletLinkerClient.WalletLinkingApi.UnlinkAsync(Guid.Parse(customerId));
            
            if (response.Error != LinkingError.None) 
                _log.Warning(message: "Couldn't unlink public wallet address", context: new {customerId, error = response.Error.ToString()});
            
            return _mapper.Map<UnlinkResultModel>(response);
        }

        public async Task<PublicAddressResultModel> GetLinkedWalletAddressAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));

            var response = await _walletLinkerClient.CustomersApi.GetLinkedPublicAddressAsync(Guid.Parse(customerId));
            
            if (response.Error != PublicAddressError.None)
                _log.Warning("Public wallet address is not linked yet", context: new {customerId, error = response.Error.ToString()});

            return _mapper.Map<PublicAddressResultModel>(response);
        }

        public async Task<Money18> GetNextFeeAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));

            var response = await _walletLinkerClient.CustomersApi.GetNextFeeAsync(Guid.Parse(customerId));

            return response.Fee;
        }
    }
}
