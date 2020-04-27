using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.RealEstate;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.EligibilityEngine.Client;
using Lykke.Service.EligibilityEngine.Client.Enums;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using Lykke.Service.EmaarPropertyIntegration.Client;
using Lykke.Service.EmaarPropertyIntegration.Client.Models.Requests;

namespace MAVN.Service.CustomerAPI.Services
{
    public class RealEstateResponseFormatter
    {
        private const string RealEstateIdDelimiter = "###";

        private readonly IEmaarPropertyIntegrationClient _emaarPropertyIntegrationClient;
        private readonly ICustomerProfileClient _cpClient;
        private readonly IEligibilityEngineClient _eligibilityEngineClient;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public RealEstateResponseFormatter(
            IEmaarPropertyIntegrationClient emaarPropertyIntegrationClient,
            ICustomerProfileClient cpClient,
            IEligibilityEngineClient eligibilityEngineClient,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _emaarPropertyIntegrationClient = emaarPropertyIntegrationClient;
            _cpClient = cpClient;
            _eligibilityEngineClient = eligibilityEngineClient;
            _settingsService = settingsService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<(RealEstatePropertiesResponse, RealEstateErrorCodes)> FormatAsync(string customerId, string spendRuleId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));

            if (string.IsNullOrEmpty(spendRuleId))
                throw new ArgumentNullException(nameof(spendRuleId));

            var customerProfile = await _cpClient.CustomerProfiles.GetByCustomerIdAsync(customerId);

            if (customerProfile.ErrorCode == CustomerProfileErrorCodes.CustomerProfileDoesNotExist)
                return (null, RealEstateErrorCodes.CustomerProfileDoesNotExist);

            var result = await _emaarPropertyIntegrationClient.Api.GetPendingInvoicePaymentsAsync(new GetPendingInvoicePaymentsRequestModel
            {
                Email = customerProfile.Profile.Email
            });

            if (!string.IsNullOrEmpty(result.ErrorCode))
            {
                _log.Error(message: "SalesForce returned error when tried to get pending installments for customer",
                    context: new { customerId, result.ErrorCode });

                return (null, RealEstateErrorCodes.SalesForceError);
            }

            var realEstateProperties = new List<RealEstateProperty>();

            foreach (var property in result.PendingInstallments)
            {
                var realEstateProperty = new RealEstateProperty
                {
                    Name = property.LocationCode,
                    Instalments = new List<RealEstateInstalments>()
                };

                foreach (var instalment in property.InvoiceDetail)
                {
                    var eligibilityEngineResponse = await _eligibilityEngineClient.ConversionRate.GetAmountBySpendRuleAsync(
                        new ConvertAmountBySpendRuleRequest
                        {
                            CustomerId = Guid.Parse(customerId),
                            Amount = instalment.InvoiceAmountRemain,
                            FromCurrency = instalment.InvoiceCurrencyCode,
                            SpendRuleId = Guid.Parse(spendRuleId),
                            ToCurrency = _settingsService.GetTokenName()
                        });

                    if (eligibilityEngineResponse.ErrorCode == EligibilityEngineErrors.ConversionRateNotFound)
                        return (null, RealEstateErrorCodes.ConversionRateNotFound);

                    if (eligibilityEngineResponse.ErrorCode == EligibilityEngineErrors.SpendRuleNotFound)
                        return (null, RealEstateErrorCodes.SpendRuleNotFound);

                    realEstateProperty.Instalments.Add(new RealEstateInstalments
                    {
                        Name = instalment.InvoiceDescription,
                        AmountInFiat = instalment.InvoiceAmountRemain,
                        AmountInTokens = eligibilityEngineResponse.Amount,
                        FiatCurrencyCode = instalment.InvoiceCurrencyCode,
                        DueDate = instalment.DueDate,
                        Id = ComposeRealEstateId(instalment.LocationCode, instalment.AccountNumber, instalment.CustomerTrxId, instalment.InstallmentType, instalment.OrgId)
                    });
                }

                realEstateProperties.Add(realEstateProperty);
            }

            return (new RealEstatePropertiesResponse { RealEstateProperties = realEstateProperties },
                RealEstateErrorCodes.None);
        }

        public string ComposeRealEstateId(string locationCode, string accountNumber, string customerTrxId, string type, string orgId)
        {
            var idAsString =
                $"{locationCode}{RealEstateIdDelimiter}{accountNumber}{RealEstateIdDelimiter}{customerTrxId}{RealEstateIdDelimiter}{type}{RealEstateIdDelimiter}{orgId}";

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(idAsString));
        }

        public RealEstateIdDto DecomposeRealEstateId(string id)
        {
            var idBytes = Convert.FromBase64String(id);
            var decomposedId = Encoding.UTF8.GetString(idBytes).Split(RealEstateIdDelimiter);

            return new RealEstateIdDto
            {
                LocationCode = decomposedId[0],
                AccountNumber = decomposedId[1],
                CustomerTrxId = decomposedId[2],
                Type = decomposedId[3],
                OrgId = decomposedId[4]
            };
        }
    }
}
