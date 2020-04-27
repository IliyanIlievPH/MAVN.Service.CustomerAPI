using System;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Numerics;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Requests;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.WalletManagement.Client;
using Lykke.Service.WalletManagement.Client.Enums;
using Lykke.Service.WalletManagement.Client.Models.Requests;
using TransferErrorCodes = MAVN.Service.CustomerAPI.Core.Constants.TransferErrorCodes;

namespace MAVN.Service.CustomerAPI.Services
{
    public class WalletOperationsService : IWalletOperationsService
    {
        private readonly IWalletManagementClient _walletManagementClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IPrivateBlockchainFacadeClient _pbfClient;
        private readonly IMapper _mapper;
        private readonly string _tokenSymbol;
        private readonly ILog _log;

        public WalletOperationsService(
            IWalletManagementClient walletManagementClient,
            ICustomerProfileClient customerProfileClient,
            IPrivateBlockchainFacadeClient pbfClient,
            IMapper mapper,
            string tokenSymbol,
            ILogFactory logFactory)
        {
            _walletManagementClient = walletManagementClient;
            _customerProfileClient = customerProfileClient;
            _pbfClient = pbfClient;
            _log = logFactory.CreateLog(this);

            _mapper = mapper;
            _tokenSymbol = tokenSymbol;
        }
        public async Task<TransferResultModel> TransferBalanceAsync(string senderCustomerId, string receiverAddress, Money18 amount)
        {
            var receiverCustomer = await _customerProfileClient.CustomerProfiles.GetByEmailAsync(
                new GetByEmailRequestModel { Email = receiverAddress });

            if (receiverCustomer?.Profile == null)
                return new TransferResultModel { ErrorCode = TransferErrorCodes.TargetCustomerNotFound };

            var result = await _walletManagementClient.Api.TransferBalanceAsync(new TransferBalanceRequestModel
            {
                Amount = amount,
                ReceiverCustomerId = receiverCustomer.Profile.CustomerId,
                SenderCustomerId = senderCustomerId
            });

            return _mapper.Map<TransferResultModel>(result);
        }

        public async Task<WalletModel> GetCustomerWalletAsync(string customerId)
        {
            var isCustomerIdValidGuid = Guid.TryParse(customerId, out var customerIdAsGuid);

            if (!isCustomerIdValidGuid)
                return new WalletModel { Error = WalletsErrorCodes.InvalidCustomerId };

            var pbfWalletBalanceResponseTask = _pbfClient.CustomersApi.GetBalanceAsync(customerIdAsGuid);
            var pbfWalletAddressResponseTask = _pbfClient.CustomersApi.GetWalletAddress(customerIdAsGuid);
            var blockStateTask = _walletManagementClient.Api.GetCustomerWalletBlockStateAsync(customerId);
            await Task.WhenAll(pbfWalletBalanceResponseTask, blockStateTask, pbfWalletAddressResponseTask);

            var pbfWalletBalanceResponse = pbfWalletBalanceResponseTask.Result;
            if (pbfWalletBalanceResponse.Error != CustomerBalanceError.None)
                return new WalletModel { Error = (WalletsErrorCodes)pbfWalletBalanceResponse.Error };

            var blockState = blockStateTask.Result;

            var pbfWalletAddressResponse = pbfWalletAddressResponseTask.Result;
            if (pbfWalletAddressResponse.Error != CustomerWalletAddressError.None)
                _log.Warning("Error while getting private wallet address", context: new {customerId, error = pbfWalletAddressResponse.Error.ToString()});
            
            return new WalletModel
            {
                Balance = pbfWalletBalanceResponse.Total,
                AssetSymbol = _tokenSymbol,
                IsWalletBlocked = blockState.Status.HasValue && blockState.Status.Value == CustomerWalletActivityStatus.Blocked,
                StakedBalance = pbfWalletBalanceResponse.Staked,
                Address = pbfWalletAddressResponse.WalletAddress
            };
        }

    }
}
