using System;
using System.Threading.Tasks;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.PaymentTransfers.Client;
using Lykke.Service.PaymentTransfers.Client.Models.Requests;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Models.RealEstate;
using MAVN.Service.CustomerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using PaymentTransfersErrorCodes = Lykke.Service.PaymentTransfers.Client.Enums.PaymentTransfersErrorCodes;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/realEstate")]
    public class RealEstateController : ControllerBase
    {
        private readonly IPaymentTransfersClient _paymentTransfersClient;
        private readonly RealEstateResponseFormatter _realEstateResponseFormatter;
        private readonly IRequestContext _requestContext;

        public RealEstateController(
            IPaymentTransfersClient paymentTransfersClient,
            RealEstateResponseFormatter realEstateResponseFormatter,
            IRequestContext requestContext)
        {
            _paymentTransfersClient = paymentTransfersClient;
            _realEstateResponseFormatter = realEstateResponseFormatter;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Returns pending instalments for the customer
        /// </summary>
        /// <remarks>
        /// 
        /// 
        /// Error codes:
        /// - **CustomerProfileDoesNotExist**
        /// - **SalesForceError**
        /// - **ConversionRateNotFound**
        /// - **SpendRuleNotFound**
        /// </remarks>
        /// <returns>
        /// </returns>
        [HttpGet("properties")]
        public async Task<RealEstatePropertiesResponse> GetRealEstatePropertiesAsync([FromQuery] Guid spendRuleId)
        {
            var customerId = _requestContext.UserId;

            var (response,error) = await _realEstateResponseFormatter.FormatAsync(customerId, spendRuleId.ToString());

            switch (error)
            {
                case RealEstateErrorCodes.None:
                    return response;
                case RealEstateErrorCodes.CustomerProfileDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerProfileDoesNotExist);
                case RealEstateErrorCodes.SalesForceError:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SalesForceError);
                case RealEstateErrorCodes.ConversionRateNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ConversionRateNotFound);
                case RealEstateErrorCodes.SpendRuleNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SpendRuleNotFound);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Initiate payment for real estate
        /// </summary>
        /// <remarks>
        /// 
        /// 
        /// Error codes:
        /// - **CustomerProfileDoesNotExist**
        /// - **NotEnoughTokens**
        /// - **SpendRuleNotFound**
        /// - **InvalidVerticalInSpendRule**
        /// - **CustomerWalletBlocked**
        /// - **CustomerWalletMissing**
        /// - **CannotPassBothFiatAndTokensAmount**
        /// - **EitherFiatOrTokensAmountShouldBePassed**
        /// - **InvalidAmount**
        /// - **ConversionRateNotFound**
        /// </remarks>
        /// <returns>
        /// </returns>
        [HttpPost]
        public async Task InitiatePayment(InitiateRealEstatePaymentRequest request)
        {
            RealEstateIdDto decomposedId;
            try
            {
                decomposedId = _realEstateResponseFormatter.DecomposeRealEstateId(request.Id);
            }
            catch
            {
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidRealEstateId);
            }

            var result = await _paymentTransfersClient.Api.PaymentTransferAsync(new PaymentTransferRequest
            {
                CustomerId = _requestContext.UserId,
                AmountInFiat = request.AmountInFiat,
                AmountInTokens = request.AmountInTokens,
                Currency = request.FiatCurrencyCode,
                SpendRuleId = request.SpendRuleId,
                CustomerTrxId = decomposedId.CustomerTrxId,
                LocationCode = decomposedId.LocationCode,
                CustomerAccountNumber = decomposedId.AccountNumber,
                InstallmentType = decomposedId.Type,
                OrgId = decomposedId.OrgId,
                InstalmentName = request.InstalmentName,
            });

            switch (result.Error)
            {
                case PaymentTransfersErrorCodes.None:
                    return;
                case PaymentTransfersErrorCodes.NotEnoughFunds:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NotEnoughTokens);
                case PaymentTransfersErrorCodes.SpendRuleNotFound:
                case PaymentTransfersErrorCodes.InvalidSpendRuleId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SpendRuleNotFound);
                case PaymentTransfersErrorCodes.InvalidVerticalInSpendRule:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidVerticalInSpendRule);
                case PaymentTransfersErrorCodes.CustomerDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerProfileDoesNotExist);
                case PaymentTransfersErrorCodes.CustomerWalletBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletBlocked);
                case PaymentTransfersErrorCodes.CustomerWalletDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                case PaymentTransfersErrorCodes.CannotPassBothFiatAndTokensAmount:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CannotPassBothFiatAndTokensAmount);
                case PaymentTransfersErrorCodes.EitherFiatOrTokensAmountShouldBePassed:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.EitherFiatOrTokensAmountShouldBePassed);
                case PaymentTransfersErrorCodes.InvalidTokensAmount:
                case PaymentTransfersErrorCodes.InvalidFiatAmount:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidAmount);
                case PaymentTransfersErrorCodes.InvalidAmountConversion:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ConversionRateNotFound);
                default:
                    throw new ArgumentOutOfRangeException(); 
            }
        }
    }
}
