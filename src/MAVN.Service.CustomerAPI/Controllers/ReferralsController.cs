using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Falcon.Common.Middleware.Version;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models.Referral;
using Lykke.Service.Referral.Client;
using Lykke.Service.Referral.Client.Enums;
using Lykke.Service.Referral.Client.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CommonReferralStatus = MAVN.Service.CustomerAPI.Models.Enums.CommonReferralStatus;
using HotelReferralModel = MAVN.Service.CustomerAPI.Models.Referral.HotelReferralModel;
using LykkeApiErrorResponse = Lykke.Common.ApiLibrary.Contract.LykkeApiErrorResponse;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [Route("api/referrals")]
    [LowerVersion(Devices = "android", LowerVersion = 659)]
    [LowerVersion(Devices = "IPhone,IPad", LowerVersion = 181)]
    public class ReferralsController : Controller
    {
        private readonly IRequestContext _requestContext;
        private readonly IReferralClient _referralClient;
        private readonly ICustomerService _customerService;
        private readonly IReferralService _referralService;
        private readonly IMapper _mapper;

        public ReferralsController(
            IRequestContext requestContext,
            IReferralClient referralClient,
            ICustomerService customerService,
            IReferralService referralService,
            IMapper mapper)
        {
            _requestContext = requestContext;
            _referralClient = referralClient;
            _customerService = customerService;
            _referralService = referralService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get referral code for logged customer.
        /// </summary>
        /// <remarks>
        /// Function. Return the referral code for the logged customer.
        /// </remarks>
        /// <returns>
        /// 200 - function done
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(ReferralResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [LykkeAuthorize]
        public async Task<ReferralResponseModel> GetReferralCodeAsync()
        {
            var result = await _referralService.GetOrCreateReferralCodeAsync(_requestContext.UserId);

            if (result != null)
            {
                return new ReferralResponseModel { ReferralCode = result };
            }

            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReferralNotFound);
        }

        /// <summary>
        /// Get list of lead referrals for a customer
        /// </summary>
        /// <returns>
        /// 200 - function done
        /// </returns>
        [HttpGet("leads")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(LeadReferralsListResponseModel), (int)HttpStatusCode.OK)]
        public async Task<LeadReferralsListResponseModel> GetLeadReferralsAsync()
        {
            var result = await _referralService.GetLeadReferralsAsync(_requestContext.UserId);

            return new LeadReferralsListResponseModel
            {
                LeadReferrals = _mapper.Map<IReadOnlyCollection<LeadReferral>>(result.ReferralLeads)
            };
        }

        /// <summary>
        /// Add lead referral for a customer.
        /// </summary>
        /// <remarks>
        /// Function. Return whenever the referral was added successfully or not.
        /// Error codes:
        /// - **ReferralLeadCustomerIdInvalid**
        /// - **ReferralLeadAlreadyExist**
        /// - **CustomerNotApprovedAgent**
        /// - **ReferralLeadNotProcessed**
        /// - **CanNotReferYourself**
        /// - **ReferralLeadAlreadyConfirmed**
        /// </remarks>
        /// <returns>
        /// 204 - function done
        /// </returns>
        [HttpPost("lead")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task AddLeadReferralAsync(ReferralLeadRequestModel leadRequestModel)
        {
            var result = await _referralService.AddReferralLeadAsync(_requestContext.UserId,
                _mapper.Map<ReferralLeadCreateModel>(leadRequestModel));

            if (result != null)
            {
                throw LykkeApiErrorException.BadRequest(result);
            }
        }

        /// <summary>
        /// Confirm pending lead referral.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **ReferralNotFound**
        /// </remarks>
        [HttpPost("lead/confirm")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task ConfirmReferralLeadAsync(ConfirmReferralLeadRequest model)
        {
            var result = await _referralService.ConfirmReferralAsync(model.ConfirmationCode);

            if (result != null)
            {
                throw LykkeApiErrorException.BadRequest(result);
            }
        }

        /// <summary>
        /// Add hotel referral for a customer.
        /// </summary>
        /// <remarks>
        /// Function. Return whenever the referral was added successfully or not.
        /// Error codes:
        /// - **ReferralAlreadyConfirmed**
        /// - **ReferralsLimitExceeded**
        /// - **CampaignDoesNotExists**
        /// </remarks>
        /// <returns>
        /// 204 - function done
        /// </returns>
        [HttpPost("hotels")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task AddHotelReferralAsync(HotelReferralRequestModel hotelReferralRequestModel)
        {
            var result = await _referralService.AddHotelReferralAsync(
                _requestContext.UserId,
                hotelReferralRequestModel.Email,
                hotelReferralRequestModel.CampaignId,
                hotelReferralRequestModel.FullName,
                hotelReferralRequestModel.CountryPhoneCodeId,
                hotelReferralRequestModel.PhoneNumber);

            if (result != null)
            {
                throw LykkeApiErrorException.BadRequest(result);
            }
        }

        /// <summary>
        /// Get list of hotel referrals for a customer
        /// </summary>
        /// <returns>
        /// 200 - function done
        /// </returns>
        [HttpGet("hotels")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(HotelReferralsListResponseModel), (int)HttpStatusCode.OK)]
        public async Task<HotelReferralsListResponseModel> GetHotelReferralAsync()
        {
            var result = await _referralService.GetHotelReferralAsync(_requestContext.UserId);

            return new HotelReferralsListResponseModel
            {
                HotelReferrals = _mapper.Map<IReadOnlyCollection<HotelReferralModel>>(result)
            };
        }

        /// <summary>
        /// Confirm pending hotel referral.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **ReferralNotFound**
        /// - **ReferralExpired**
        /// </remarks>
        [HttpPost("hotel/confirm")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ConfirmReferralHotelResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<ConfirmReferralHotelResponse> ConfirmReferralHotelAsync(ConfirmReferralHotelRequest model)
        {
            var result = await _referralClient.ReferralHotelsApi.ConfirmAsync(new ReferralHotelConfirmRequest
            {
                ConfirmationToken = model.ConfirmationCode
            });

            switch (result.ErrorCode)
            {
                case ReferralHotelConfirmErrorCode.None:
                    var customerInfo = await _customerService.GetCustomerInfoAsync(result.HotelReferral.ReferrerId);
                    if (customerInfo == null)
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReferralNotFound);

                    return new ConfirmReferralHotelResponse
                    {
                        Email = customerInfo.Email
                    };
                case ReferralHotelConfirmErrorCode.ReferralDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReferralNotFound);
                case ReferralHotelConfirmErrorCode.ReferralExpired:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReferralExpired);
                default:
                    throw new InvalidOperationException($"Unexpected error occured: {model.ConfirmationCode} - {result}");
            }

        }

        /// <summary>
        /// Get list of all referrals (hotel and lead) for a customer
        /// </summary>
        /// <returns>
        /// 200 - function done
        /// </returns>
        [HttpGet("all")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(ReferralsListResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ReferralsListResponseModel> GetAllReferralsAsync([FromQuery]ReferralPaginationRequestModel model)
        {
            var statuses = new List<Lykke.Service.Referral.Client.Enums.CommonReferralStatus>();

            switch (model.Status)
            {
                case CommonReferralStatus.Ongoing:
                    statuses.Add(Lykke.Service.Referral.Client.Enums.CommonReferralStatus.Pending);
                    statuses.Add(Lykke.Service.Referral.Client.Enums.CommonReferralStatus.Confirmed);
                    break;
                case CommonReferralStatus.Accepted:
                    statuses.Add(Lykke.Service.Referral.Client.Enums.CommonReferralStatus.Accepted);
                    break;
                case CommonReferralStatus.Expired:
                    statuses.Add(Lykke.Service.Referral.Client.Enums.CommonReferralStatus.Expired);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var result = await _referralClient.CommonReferralApi.GetReferralsByCustomerIdAsync(
                new CommonReferralByCustomerIdRequest()
                    {
                        CustomerId = Guid.Parse(_requestContext.UserId),
                        Statuses = statuses,
                        CampaignId = model.EarnRuleId
                    });

            switch (result.ErrorCode)
            {
                case ReferralErrorCodes.None:
                    var paged = result.Referrals
                        .Skip((model.CurrentPage - 1) * model.PageSize)
                        .Take(model.PageSize);

                    var mapped = _mapper.Map<IEnumerable<Core.Domain.CommonReferral.CommonReferralModel>>(paged);

                    var pagedResult = await _referralService
                        .PrepareReferralCommonData(mapped, _requestContext.UserId, model.EarnRuleId);

                    return new ReferralsListResponseModel()
                    {
                        TotalCount = result.Referrals.Count,
                        CurrentPage = model.CurrentPage,
                        PageSize = model.PageSize,
                        Referrals = _mapper.Map<List<CustomerCommonReferralResponseModel>>(pagedResult)
                    };

                case ReferralErrorCodes.ReferralNotFound:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ReferralNotFound);
                case ReferralErrorCodes.GuidCanNotBeParsed:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                default:
                    throw new InvalidOperationException("An unexpected error has occured");
            }
        }

        /// <summary>
        /// Add friend referral for a customer.
        /// </summary>
        /// <remarks>
        /// Function. Return whenever the referral was added successfully or not.
        /// Error codes:
        /// - **ReferralAlreadyConfirmed**
        /// - **CanNotReferYourself**
        /// - **ReferralAlreadyExist**
        /// - **CampaignDoesNotExists**
        /// - **CustomerDoesNotExist**
        /// </remarks>
        /// <returns>
        /// 204 - function done
        /// </returns>
        [HttpPost("friend")]
        [LykkeAuthorize]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task AddFriendReferralAsync(ReferralFriendRequestModel leadRequestModel)
        {
            var result = await _referralService.AddReferralFriendAsync(_requestContext.UserId,
                _mapper.Map<ReferralFriendCreateModel>(leadRequestModel));

            if (result != null)
            {
                throw LykkeApiErrorException.BadRequest(result);
            }
        }
    }
}
