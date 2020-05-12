using System;
using System.Net;
using System.Threading.Tasks;
using MAVN.Common.Middleware.Authentication;
using MAVN.Numerics;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.ConversionRate;
using MAVN.Service.EligibilityEngine.Client;
using MAVN.Service.EligibilityEngine.Client.Enums;
using MAVN.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/conversionRate")]
    public class ConversionRateController : ControllerBase
    {
        private readonly IEligibilityEngineClient _eligibilityEngineClient;
        private readonly IRequestContext _requestContext;
        private readonly ISettingsService _settingsService;
        
        public ConversionRateController(
            IEligibilityEngineClient eligibilityEngineClient,
            IRequestContext requestContext,
            ISettingsService settingsService)
        {
            _eligibilityEngineClient = eligibilityEngineClient;
            _requestContext = requestContext;
            _settingsService = settingsService;
        }

        /// <summary>
        /// Converts given amount to base currency based on partner
        /// </summary>
        /// <param name="request">The request details</param>
        /// <returns></returns>
        [HttpGet("partner")]
        [SwaggerOperation("GetPartnerRate")]
        [ProducesResponseType(typeof(ConversionRateResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ConversionRateResponseModel> GetPartnerRateAsync(
            [FromQuery] PartnerConversionRateRequestModel request)
        {
            if (!Guid.TryParse(request.PartnerId, out var partnerIdIdAsGuid))
                return new ConversionRateResponseModel {Error = ConversionRateErrorCodes.InvalidPartnerId};

            if (!Guid.TryParse(_requestContext.UserId, out var customerIdAsGuid))
                return new ConversionRateResponseModel {Error = ConversionRateErrorCodes.InvalidCustomerId};

            var result = await _eligibilityEngineClient.ConversionRate.ConvertOptimalByPartnerAsync(
                new ConvertOptimalByPartnerRequest
                {
                    CustomerId = customerIdAsGuid,
                    FromCurrency = _settingsService.GetTokenName(),
                    ToCurrency = _settingsService.GetBaseCurrencyCode(),
                    Amount = Money18.Parse(request.Amount),
                    PartnerId = partnerIdIdAsGuid
                });

            switch (result.ErrorCode)
            {
                case EligibilityEngineErrors.PartnerNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.PartnerNotFound
                    };
                case EligibilityEngineErrors.CustomerNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.CustomerNotFound
                    };
                case EligibilityEngineErrors.EarnRuleNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.EarnRuleNotFound
                    };
                case EligibilityEngineErrors.ConversionRateNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.ConversionRateNotFound
                    };
                default:
                    return new ConversionRateResponseModel
                    {
                        Rate = result.UsedRate.ToDisplayString(),
                        Amount = result.Amount.ToDisplayString(),
                        CurrencyCode = result.CurrencyCode
                    };
            }

        }

        /// <summary>
        /// Converts given amount to base currency based on earn rule
        /// </summary>
        /// <param name="request">The request details</param>
        /// <returns></returns>
        [HttpGet("earnRule")]
        [SwaggerOperation("GetEarnRuleRate")]
        [ProducesResponseType(typeof(ConversionRateResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ConversionRateResponseModel> GetEarnRuleRateAsync(
            [FromQuery] EarnRuleConversionRateRequestModel request)
        {
            if (!Guid.TryParse(request.EarnRuleId, out var earnRuleIdIdAsGuid))
                return new ConversionRateResponseModel {Error = ConversionRateErrorCodes.InvalidEarnRuleId};

            if (!Guid.TryParse(_requestContext.UserId, out var customerIdAsGuid))
                return new ConversionRateResponseModel {Error = ConversionRateErrorCodes.InvalidCustomerId};

            var result = await _eligibilityEngineClient.ConversionRate.GetAmountByEarnRuleAsync(
                new ConvertAmountByEarnRuleRequest
                {
                    CustomerId = customerIdAsGuid,
                    FromCurrency = _settingsService.GetTokenName(),
                    ToCurrency = _settingsService.GetBaseCurrencyCode(),
                    Amount = Money18.Parse(request.Amount),
                    EarnRuleId = earnRuleIdIdAsGuid
                });


            switch (result.ErrorCode)
            {
                case EligibilityEngineErrors.PartnerNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.PartnerNotFound
                    };
                case EligibilityEngineErrors.CustomerNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.CustomerNotFound
                    };
                case EligibilityEngineErrors.EarnRuleNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.EarnRuleNotFound
                    };
                case EligibilityEngineErrors.ConversionRateNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.ConversionRateNotFound
                    };
                default:
                    return new ConversionRateResponseModel
                    {
                        Rate = result.UsedRate.ToDisplayString(),
                        Amount = result.Amount.ToDisplayString(),
                        CurrencyCode = result.CurrencyCode
                    };
            }
        }

        /// <summary>
        /// Converts given amount to base currency based on burn rule
        /// </summary>
        /// <param name="request">The request details</param>
        /// <returns></returns>
        [HttpGet("burnRule")]
        [SwaggerOperation("GetBurnRuleRate")]
        [ProducesResponseType(typeof(ConversionRateResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ConversionRateResponseModel> GetBurnRuleRateAsync(
            [FromQuery] BurnRuleConversionRateRequestModel request)
        {
            if (!Guid.TryParse(request.BurnRuleId, out var burnRuleIdIdAsGuid))
                return new ConversionRateResponseModel {Error = ConversionRateErrorCodes.InvalidBurnRuleId};

            if (!Guid.TryParse(_requestContext.UserId, out var customerIdAsGuid))
                return new ConversionRateResponseModel {Error = ConversionRateErrorCodes.InvalidCustomerId};

            var result = await _eligibilityEngineClient.ConversionRate.GetAmountBySpendRuleAsync(
                new ConvertAmountBySpendRuleRequest
                {
                    CustomerId = customerIdAsGuid,
                    FromCurrency = _settingsService.GetTokenName(),
                    ToCurrency = _settingsService.GetBaseCurrencyCode(),
                    Amount = Money18.Parse(request.Amount),
                    SpendRuleId = burnRuleIdIdAsGuid
                });


            switch (result.ErrorCode)
            {
                case EligibilityEngineErrors.PartnerNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.PartnerNotFound
                    };
                case EligibilityEngineErrors.CustomerNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.CustomerNotFound
                    };
                case EligibilityEngineErrors.SpendRuleNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.SpendRuleNotFound
                    };
                case EligibilityEngineErrors.ConversionRateNotFound:
                    return new ConversionRateResponseModel
                    {
                        Error = ConversionRateErrorCodes.ConversionRateNotFound
                    };
                default:
                    return new ConversionRateResponseModel
                    {
                        Rate = result.UsedRate.ToDisplayString(),
                        Amount = result.Amount.ToDisplayString(),
                        CurrencyCode = result.CurrencyCode
                    };
            }
        }
    }
}
