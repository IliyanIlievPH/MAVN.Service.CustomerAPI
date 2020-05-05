using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using MAVN.Service.CrossChainTransfers.Client;
using MAVN.Service.CrossChainTransfers.Client.Models.Requests;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using TransferToExternalErrorCodes = MAVN.Service.CrossChainTransfers.Client.Models.Enums.TransferToExternalErrorCodes;

namespace MAVN.Service.CustomerAPI.Services
{
    public class PublicWalletTransferService : IPublicWalletTransferService
    {
        private readonly ICrossChainTransfersClient _ccTransfersClient;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public PublicWalletTransferService(ICrossChainTransfersClient ccTransfersClient, ILogFactory logFactory, IMapper mapper)
        {
            _ccTransfersClient = ccTransfersClient;
            _mapper = mapper;
            _log = logFactory.CreateLog(this);
        }

        public async Task<TransferToExternalResultModel> TransferAsync(string customerId, string amount)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));
            
            if (string.IsNullOrEmpty(amount))
                throw new ArgumentNullException(nameof(amount));
            
            if (!Money18.TryParse(amount, out var amount18))
                throw new FormatException("Amount is in wrong format");

            var response = await _ccTransfersClient.Api.TransferToExternalAsync(
                new TransferToExternalRequest {CustomerId = customerId, Amount = amount18});

            if (response.Error != TransferToExternalErrorCodes.None)
                _log.Warning("Couldn't transfer to external wallet", context: new {customerId, amount, error = response.Error.ToString()});

            return _mapper.Map<TransferToExternalResultModel>(response);
        }
    }
}
