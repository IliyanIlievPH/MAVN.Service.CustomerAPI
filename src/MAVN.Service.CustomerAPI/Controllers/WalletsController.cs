using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using Lykke.Service.CrossChainTransfers.Client;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Extensions;
using MAVN.Service.CustomerAPI.Models;
using MAVN.Service.CustomerAPI.Models.Extensions;
using MAVN.Service.CustomerAPI.Models.Operations;
using MAVN.Service.CustomerAPI.Models.Wallets;
using Lykke.Service.EthereumBridge.Client;
using Lykke.Service.OperationsHistory.Client;
using Lykke.Service.OperationsHistory.Client.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/wallets")]
    [ApiController]
    public class WalletsController : Controller
    {
        private readonly IWalletOperationsService _walletOperationsService;
        private readonly IRequestContext _requestContext;
        private readonly IOperationsHistoryClient _operationsHistoryClient;
        private readonly IPublicWalletLinkingService _publicWalletLinkingService;
        private readonly IPublicWalletTransferService _publicWalletTransferService;
        private readonly IEthereumBridgeClient _ethereumBridgeClient;
        private readonly ISettingsService _settingsService;
        private readonly ICrossChainTransfersClient _crossChainTransfersClient;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public WalletsController(
            IWalletOperationsService walletOperationsService,
            IRequestContext requestContext,
            IOperationsHistoryClient operationsHistoryClient,
            IMapper mapper, 
            IPublicWalletLinkingService publicWalletLinkingService, 
            IPublicWalletTransferService publicWalletTransferService,
            IEthereumBridgeClient ethereumBridgeClient,
            ISettingsService settingsService,
            ICrossChainTransfersClient crossChainTransfersClient,
            ILogFactory logFactory)
        {
            _walletOperationsService = walletOperationsService;
            _requestContext = requestContext;
            _operationsHistoryClient = operationsHistoryClient;
            _mapper = mapper;
            _publicWalletLinkingService = publicWalletLinkingService;
            _publicWalletTransferService = publicWalletTransferService;
            _ethereumBridgeClient = ethereumBridgeClient;
            _settingsService = settingsService;
            _crossChainTransfersClient = crossChainTransfersClient;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Get all wallets for specific customer.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidCustomerId**
        /// - **CustomerWalletMissing**
        /// </remarks>
        [HttpGet("customer")]
        [LykkeAuthorize]
        [SwaggerOperation("GetByCustomerAsync")]
        [ProducesResponseType(typeof(IList<WalletResponseModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetByCustomerAsync()
        {
            var customerId = _requestContext.UserId;
            
            var walletResult = await _walletOperationsService.GetCustomerWalletAsync(customerId);
            
            switch (walletResult.Error)
            {
                case WalletsErrorCodes.None:
            
                    var linkedWalletTask = _publicWalletLinkingService.GetLinkedWalletAddressAsync(customerId);
                    
                    var tokensStatisticsTask = 
                        _operationsHistoryClient.StatisticsApi.GetTokensStatisticsForCustomerAsync(
                            new TokensStatisticsForCustomerRequest {CustomerId = customerId});

                    await Task.WhenAll(linkedWalletTask, tokensStatisticsTask);

                    Money18? linkedWalletBalance = null;

                    if (linkedWalletTask.Result.Status == PublicAddressStatus.Linked)
                    {
                        var balance =
                            await _ethereumBridgeClient.Wallets.GetBalanceAsync(linkedWalletTask.Result.PublicAddress);
                        
                        linkedWalletBalance = balance.Amount;
                    }
                    
                    var response = new WalletResponseModel
                    {
                        Balance = walletResult.Balance.ToDisplayString(),
                        ExternalBalance = linkedWalletBalance?.ToDisplayString(),
                        AssetSymbol = walletResult.AssetSymbol,
                        TotalEarned = tokensStatisticsTask.Result.EarnedAmount.ToDisplayString(),
                        TotalSpent = tokensStatisticsTask.Result.BurnedAmount.ToDisplayString(),
                        IsWalletBlocked = walletResult.IsWalletBlocked,
                        PrivateWalletAddress = walletResult.Address,
                        PublicWalletAddress = linkedWalletTask.Result.PublicAddress,
                        PublicAddressLinkingStatus = linkedWalletTask.Result.Status.ToLinkingStatus(),
                        StakedBalance = walletResult.StakedBalance.ToDisplayString(),
                        TransitAccountAddress = _settingsService.GetTransitAccountAddress(),
                    };
                    return Ok(new List<WalletResponseModel> {response});
                
                case WalletsErrorCodes.InvalidCustomerId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                
                case WalletsErrorCodes.CustomerWalletMissing:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                
                default:
                    throw new InvalidOperationException(
                        $"Unexpected error during Transfer for {_requestContext.UserId} - {walletResult.Error}");
            }
        }

        /// <summary>
        /// Transfers balance from current Logged Customer to selected Receiver Customer
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidReceiver**
        /// - **InvalidAmount**
        /// - **SenderCustomerNotEnoughBalance**
        /// - **SenderCustomerNotFound**
        /// - **TargetCustomerNotFound**
        /// - **TransferSourceAndTargetMustBeDifferent**
        /// - **TransferSourceCustomerWalletBlocked**
        /// - **TransferTargetCustomerWalletBlocked**
        /// </remarks>
        [HttpPost("transfer")]
        [LykkeAuthorize]
        [SwaggerOperation("TransferAsync")]
        [ProducesResponseType(typeof(TransferOperationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<TransferOperationResponse> TransferAsync(TransferOperationRequest requestModel)
        {
            var result = await _walletOperationsService.TransferBalanceAsync(
                _requestContext.UserId,
                requestModel.ReceiverEmail, 
                requestModel.Amount);

            switch (result.ErrorCode)
            {
                case TransferErrorCodes.None:
                    return _mapper.Map<TransferOperationResponse>(result);
                case TransferErrorCodes.RecipientWalletMissing:
                case TransferErrorCodes.InvalidRecipientId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidReceiver);
                case TransferErrorCodes.NotEnoughFunds:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SenderCustomerNotEnoughBalance);
                case TransferErrorCodes.InvalidAmount:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidAmount);
                case TransferErrorCodes.SourceCustomerNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SenderCustomerNotFound);
                case TransferErrorCodes.TargetCustomerNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.TargetCustomerNotFound);
                case TransferErrorCodes.TransferSourceAndTargetMustBeDifferent:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.TransferSourceAndTargetMustBeDifferent);
                case TransferErrorCodes.SourceCustomerWalletBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.TransferSourceCustomerWalletBlocked);
                case TransferErrorCodes.TargetCustomerWalletBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.TransferTargetCustomerWalletBlocked);
                default:
                    throw new InvalidOperationException($"Unexpected error during Transfer for {_requestContext.UserId} - {result.ErrorCode}");
            }
        }

        /// <summary>
        /// Create public wallet link request
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidSignature**
        /// - **CustomerWalletMissing**
        /// - **InvalidCustomerId**
        /// - **InvalidPrivateAddress**
        /// - **InvalidPublicAddress**
        /// - **LinkingRequestAlreadyApproved**
        /// - **LinkingRequestAlreadyExists**
        /// - **LinkingRequestDoesNotExist**
        /// - **CustomerDoesNotExist**
        /// - **CustomerWalletBlocked**
        /// </remarks>
        [HttpPost("linkRequest")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(LinkWalletResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public async Task<LinkWalletResponse> SubmitExternalWalletLinkRequestAsync()
        {
            var result = await _publicWalletLinkingService.CreateLinkRequestAsync(_requestContext.UserId);

            switch (result.Error)
            {
                case LinkingError.None:
                    return new LinkWalletResponse {LinkCode = result.LinkCode};
                case LinkingError.InvalidSignature:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkSignature);
                case LinkingError.CustomerWalletMissing:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                case LinkingError.InvalidCustomerId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                case LinkingError.InvalidPrivateAddress:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkPrivateAddress);
                case LinkingError.InvalidPublicAddress:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkPublicAddress);
                case LinkingError.LinkingRequestAlreadyApproved:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestAlreadyApproved);
                case LinkingError.LinkingRequestAlreadyExists:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestAlreadyExist);
                case LinkingError.LinkingRequestDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestDoesNotExist);
                case LinkingError.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case LinkingError.CustomerWalletBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletBlocked);
                default:
                    throw new InvalidOperationException(
                        $"Unexpected error during wallet linking {_requestContext.UserId} - {result.Error}");
            }
        }

        /// <summary>
        /// Approve public wallet linking request
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidSignature**
        /// - **CustomerWalletMissing**
        /// - **InvalidCustomerId**
        /// - **InvalidPrivateAddress**
        /// - **InvalidPublicAddress**
        /// - **LinkingRequestAlreadyApproved**
        /// - **LinkingRequestAlreadyExists**
        /// - **LinkingRequestDoesNotExist**
        /// - **CustomerDoesNotExist**
        /// - **CustomerWalletBlocked**
        /// </remarks>
        [HttpPut("linkRequest")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task ApproveExternalWalletLinkRequestAsync([FromBody] ApproveExternalWalletLinkRequest request)
        {
            var result =
                await _publicWalletLinkingService.ApproveLinkRequestAsync(request.PrivateAddress, request.PublicAddress, request.Signature);

            switch (result.Error)
            {
                case LinkingError.None:
                    return;
                case LinkingError.InvalidSignature:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkSignature);
                case LinkingError.CustomerWalletMissing:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                case LinkingError.InvalidCustomerId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                case LinkingError.InvalidPrivateAddress:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkPrivateAddress);
                case LinkingError.InvalidPublicAddress:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkPublicAddress);
                case LinkingError.LinkingRequestAlreadyApproved:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestAlreadyApproved);
                case LinkingError.LinkingRequestAlreadyExists:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestAlreadyExist);
                case LinkingError.LinkingRequestDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestDoesNotExist);
                case LinkingError.NotEnoughFunds:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NotEnoughTokens);
                case LinkingError.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case LinkingError.CustomerWalletBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletBlocked);
                default:
                    throw new InvalidOperationException(
                        $"Unexpected error during wallet linking approval {_requestContext.UserId} - {result.Error}");
            }
        }

        /// <summary>
        /// Delete public wallet link request
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidSignature**
        /// - **CustomerWalletMissing**
        /// - **InvalidCustomerId**
        /// - **InvalidPrivateAddress**
        /// - **InvalidPublicAddress**
        /// - **LinkingRequestAlreadyApproved**
        /// - **LinkingRequestAlreadyExists**
        /// - **LinkingRequestDoesNotExist**
        /// - **CustomerDoesNotExist**
        /// - **CustomerWalletBlocked**
        /// </remarks>
        [HttpDelete("linkRequest")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public async Task DeleteExternalWalletLinkRequestAsync()
        {
            var result = await _publicWalletLinkingService.UnlinkAsync(_requestContext.UserId);
            
            switch (result.Error)
            {
                case LinkingError.None:
                    return;
                case LinkingError.InvalidSignature:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkSignature);
                case LinkingError.CustomerWalletMissing:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                case LinkingError.InvalidCustomerId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                case LinkingError.InvalidPrivateAddress:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkPrivateAddress);
                case LinkingError.InvalidPublicAddress:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidWalletLinkPublicAddress);
                case LinkingError.LinkingRequestAlreadyApproved:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestAlreadyApproved);
                case LinkingError.LinkingRequestAlreadyExists:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestAlreadyExist);
                case LinkingError.LinkingRequestDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestDoesNotExist);
                case LinkingError.CannotDeleteLinkingRequestWhileConfirming:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.LinkingRequestIsBeingConfirmed);
                case LinkingError.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case LinkingError.CustomerWalletBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletBlocked);
                default:
                    throw new InvalidOperationException(
                        $"Unexpected error during wallet linking approval {_requestContext.UserId} - {result.Error}");
            }
        }

        /// <summary>
        /// Transfer amount of tokens to linked public wallet
        /// </summary>
        /// <param name="request">The amount of tokens</param>
        /// <remarks>
        /// Error codes:
        /// - **InvalidAmount**
        /// - **CustomerWalletBlocked**
        /// - **CustomerWalletMissing**
        /// - **NotEnoughTokens**
        /// - **CustomerDoesNotExist**
        /// - **PublicWalletIsNotLinked**
        /// - **InvalidCustomerId**
        /// </remarks>
        [HttpPost("external-transfer")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public async Task ExternalWalletTransferAsync([FromBody] TransferToExternalWalletRequest request)
        {
            TransferToExternalResultModel result;
            
            try
            {
                result = await _publicWalletTransferService.TransferAsync(_requestContext.UserId, request.Amount);
            }
            catch (FormatException e)
            {
                _log.Warning(e.Message, e, new {customerId = _requestContext.UserId, request.Amount});
                
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidAmount);
            }

            switch (result.Error)
            {
                case TransferToExternalErrorCodes.None:
                    return;
                case TransferToExternalErrorCodes.InvalidAmount:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidAmount);
                case TransferToExternalErrorCodes.CustomerWalletBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletBlocked);
                case TransferToExternalErrorCodes.CustomerWalletMissing:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                case TransferToExternalErrorCodes.NotEnoughBalance:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NotEnoughTokens);
                case TransferToExternalErrorCodes.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerDoesNotExist);
                case TransferToExternalErrorCodes.WalletIsNotLinked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PublicWalletIsNotLinked);
                case TransferToExternalErrorCodes.CustomerIdIsNotAValidGuid:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                default:
                    throw new InvalidOperationException(
                        $"Unexpected error during external transfer {_requestContext.UserId} - {result.Error}");
            }
        }

        /// <summary>
        /// Get fee value for the next wallet linking approval
        /// </summary>
        [HttpGet("linkRequest/nextFee")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(NextWalletLinkingFeeResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        public async Task<NextWalletLinkingFeeResponseModel> GetNextWalletLinkingFeeAsync()
        {
            var fee = await _publicWalletLinkingService.GetNextFeeAsync(_requestContext.UserId);

            return new NextWalletLinkingFeeResponseModel {Fee = fee};
        }

        /// <summary>
        /// Get fee value for transfer for the public network
        /// </summary>
        [HttpGet("transferToPublic/fee")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(TransferToPublicFeeResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<TransferToPublicFeeResponseModel> GetTransferToPublicFeeAsync()
        {
            var feeResponse = await _crossChainTransfersClient.FeesApi.GetTransferToPublicFeeAsync();

            return new TransferToPublicFeeResponseModel { Fee = feeResponse.Fee };
        }
    }
}
