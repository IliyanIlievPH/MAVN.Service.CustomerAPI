using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Extensions;
using MAVN.Service.CustomerAPI.Models.Extensions;
using MAVN.Service.CustomerAPI.Models.PartnerPayments;
using MAVN.Service.EligibilityEngine.Client;
using Lykke.Service.PartnerManagement.Client;
using MAVN.Service.PartnersIntegration.Client;
using MAVN.Service.PartnersPayments.Client.Models;
using MAVN.Service.PrivateBlockchainFacade.Client;

namespace MAVN.Service.CustomerAPI.Services
{
    public class PartnersPaymentsResponseFormatter
    {
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IPrivateBlockchainFacadeClient _pbfClient;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IPartnersIntegrationClient _partnersIntegrationClient;
        private readonly ILog _log;

        public PartnersPaymentsResponseFormatter(
            IPartnerManagementClient partnerManagementClient, 
            IPrivateBlockchainFacadeClient pbfClient,
            ICurrencyConverter currencyConverter, 
            IPartnersIntegrationClient partnersIntegrationClient, 
            ILogFactory logFactory)
        {
            _partnerManagementClient = partnerManagementClient;
            _pbfClient = pbfClient;
            _currencyConverter = currencyConverter;
            _partnersIntegrationClient = partnersIntegrationClient;
            _log = logFactory.CreateLog(this);
        }

        public async Task<PartnerPaymentRequestDetailsResponse> FormatAsync(PaymentDetailsResponseModel ppResponseModel)
        {
            if (ppResponseModel == null)
                throw new ArgumentNullException(nameof(ppResponseModel));
            
            #region Tasks

            var partnerTask = _partnerManagementClient.Partners.GetByIdAsync(Guid.Parse(ppResponseModel.PartnerId));

            var customersBalanceTask = _pbfClient.CustomersApi.GetBalanceAsync(Guid.Parse(ppResponseModel.CustomerId));
            
            var totalBillInBaseCurrencyTask = _currencyConverter.GetCurrencyAmountInBaseCurrencyAsync(
                ppResponseModel.TotalBillAmount, ppResponseModel.Currency,
                ppResponseModel.CustomerId, ppResponseModel.PartnerId);
            
            var totalBillInTokenTask = _currencyConverter.GetCurrencyAmountInTokensAsync(
                ppResponseModel.TotalBillAmount, ppResponseModel.Currency,
                ppResponseModel.CustomerId, ppResponseModel.PartnerId);

            await Task.WhenAll(partnerTask, customersBalanceTask, totalBillInBaseCurrencyTask, totalBillInTokenTask);
            
            #endregion
            
            #region Task results

            var partner = partnerTask.Result;

            var customersBalance = customersBalanceTask.Result;

            var totalBillInBaseCurrency = totalBillInBaseCurrencyTask.Result;

            var totalBillInToken = totalBillInTokenTask.Result;
            
            #endregion
            
            var paymentInfo = string.Empty;
            
            if (!string.IsNullOrEmpty(ppResponseModel.PartnerMessageId))
            {
                var partnerMessageResponse =
                    await _partnersIntegrationClient.MessagesApi.GetMessageAsync(ppResponseModel.PartnerMessageId);

                if (partnerMessageResponse == null)
                {
                    _log.Error(null, "Could not find partner message",
                        new { ppResponseModel.CustomerId, ppResponseModel.PaymentRequestId, ppResponseModel.PartnerMessageId });
                }
                else
                {
                    paymentInfo = partnerMessageResponse.Message;
                }
            }

            return new PartnerPaymentRequestDetailsResponse
            {
                PaymentRequestId = ppResponseModel.PaymentRequestId,
                Date = ppResponseModel.Timestamp,
                Status = ppResponseModel.Status.ConvertToPublicStatus().ToString(),
                CurrencyCode = _currencyConverter.GetBaseCurrencyCode(),
                LocationId = ppResponseModel.LocationId,
                LocationName = partner.GetLocationName(ppResponseModel.LocationId),
                PartnerId = ppResponseModel.PartnerId,
                PartnerName = partner?.Name,
                WalletBalance = customersBalance.Total.ToDisplayString(),
                PaymentInfo = paymentInfo,
                LastUpdatedDate = ppResponseModel.LastUpdatedTimestamp,
                TokensToFiatConversionRate = ppResponseModel.TokensToFiatConversionRate,
                TotalInCurrency = decimal.Parse(totalBillInBaseCurrency.ToString()),
                TotalInToken = totalBillInToken.ToDisplayString(),
                SendingAmountInToken = ppResponseModel.TokensSendingAmount.ToDisplayString(),
                CustomerActionExpirationTimestamp = ppResponseModel.CustomerActionExpirationTimestamp,
                CustomerActionExpirationTimeLeftInSeconds = ppResponseModel.CustomerActionExpirationTimeLeftInSeconds,
                RequestedAmountInTokens = ppResponseModel.TokensAmount.ToDisplayString()
            };
        }

        public async Task<IEnumerable<PartnerPaymentRequestItemResponse>> FormatAsync(IEnumerable<PaymentResponseModel> ppResponseModels)
        {
            var ppResponseModelsList = ppResponseModels?.ToList();
            
            if (ppResponseModelsList == null)
                throw new ArgumentNullException(nameof(ppResponseModels));

            var response = ppResponseModelsList.Select(x => new PartnerPaymentRequestItemResponse
            {
                PaymentRequestId = x.PaymentRequestId,
                Date = x.Date,
                Status = x.Status.ConvertToPublicStatus().ToString(),
                CurrencyCode = _currencyConverter.GetBaseCurrencyCode(),
                LocationId = x.LocationId,
                PartnerId = x.PartnerId,
                LastUpdatedDate = x.LastUpdatedDate,
                SendingAmountInToken = x.TokensSendingAmount?.ToDisplayString()
            }).ToList();

            await UpdatePaymentInfoAsync(ppResponseModelsList, response);
            await AddPartnersNamesAsync(response);
            await CalculateAmountsAsync(ppResponseModelsList, response);
            
            return response;
        }
        
        private async Task UpdatePaymentInfoAsync(IEnumerable<PaymentResponseModel> paymentResponses,
            IEnumerable<PartnerPaymentRequestItemResponse> paymentRequests)
        {
            var paymentRequestsDictionary =
                paymentResponses.ToDictionary(k => k.PaymentRequestId, v => v.PartnerMessageId);

            foreach (var paymentRequest in paymentRequests)
            {
                if (!paymentRequestsDictionary.TryGetValue(paymentRequest.PaymentRequestId, out var partnerMessageId))
                {
                    _log.Error(null, "Could not find partner message id",
                        new
                        {
                            paymentRequest.PaymentRequestId,
                            PartnerMessageId = (string)null
                        });
                    continue;
                }

                if (string.IsNullOrEmpty(partnerMessageId))
                    continue;

                var partnerMessage = await _partnersIntegrationClient.MessagesApi.GetMessageAsync(partnerMessageId);

                if (partnerMessage == null)
                {
                    _log.Error(null, "Could not find partner message",
                        new
                        {
                            paymentRequest.PaymentRequestId,
                            PartnerMessageId = partnerMessageId
                        });
                    continue;
                }

                paymentRequest.PaymentInfo = partnerMessage.Message;
            }
        }

        private async Task AddPartnersNamesAsync(IEnumerable<PartnerPaymentRequestItemResponse> paymentRequests)
        {
            var paymentRequestsList = paymentRequests.ToList();
            
            var partnerIds = paymentRequestsList.Select(p => Guid.Parse(p.PartnerId)).ToArray();

            var partnersNamesDictionary =
                (await _partnerManagementClient.Partners.GetByIdsAsync(partnerIds))
                .ToDictionary(k => k.Id.ToString(), v => v.Name);

            foreach (var paymentRequest in paymentRequestsList)
            {
                partnersNamesDictionary.TryGetValue(paymentRequest.PartnerId, out var partnerName);

                paymentRequest.PartnerName = partnerName ?? "";
            }
        }

        private async Task CalculateAmountsAsync(IEnumerable<PaymentResponseModel> ppFromInternalApi, IEnumerable<PartnerPaymentRequestItemResponse> ppToResponse)
        {
            var amounts = ppFromInternalApi.ToDictionary(e => e.PaymentRequestId, p => new {p.TotalBillAmount, p.Currency, p.CustomerId, p.PartnerId});
            
            foreach (var pp in ppToResponse)
            {
                if (!amounts.TryGetValue(pp.PaymentRequestId, out var ppAmount))
                {
                    _log.Error(null, "Could not find partners payment",
                        new {pp.PaymentRequestId});
                    continue;
                }
                
                var totalBillInBaseCurrency = await _currencyConverter.GetCurrencyAmountInBaseCurrencyAsync(
                    ppAmount.TotalBillAmount, ppAmount.Currency, ppAmount.CustomerId, ppAmount.PartnerId);
                
                var totalBillInToken = await _currencyConverter.GetCurrencyAmountInTokensAsync(
                    ppAmount.TotalBillAmount, ppAmount.Currency, ppAmount.CustomerId, ppAmount.PartnerId);

                pp.TotalInCurrency = decimal.Parse(totalBillInBaseCurrency.ToString());
                pp.TotalInToken = totalBillInToken.ToDisplayString();
            }
        }
    }
}
