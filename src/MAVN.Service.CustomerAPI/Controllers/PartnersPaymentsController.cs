using System;
using System.Net;
using System.Threading.Tasks;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Models.PartnerPayments;
using MAVN.Service.CustomerAPI.Services;
using Lykke.Service.PartnersPayments.Client;
using Lykke.Service.PartnersPayments.Client.Enums;
using Lykke.Service.PartnersPayments.Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/partners/payments")]
    public class PartnersPaymentsController : ControllerBase
    {
        private readonly IPartnersPaymentsClient _partnersPaymentsClient;
        private readonly IRequestContext _requestContext;
        private readonly PartnersPaymentsResponseFormatter _ppResponseFormatter;

        public PartnersPaymentsController(
            IPartnersPaymentsClient partnersPaymentsClient,
            IRequestContext requestContext,
            PartnersPaymentsResponseFormatter ppResponseFormatter)
        {
            _partnersPaymentsClient = partnersPaymentsClient;
            _requestContext = requestContext;
            _ppResponseFormatter = ppResponseFormatter;
        }

        /// <summary>
        /// Get the details about payment request
        /// </summary>
        /// - **PartnersPaymentNotFound**
        /// - **PaymentRequestsIsForAnotherCustomer**
        [HttpGet]
        [ProducesResponseType(typeof(PartnerPaymentRequestDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<PartnerPaymentRequestDetailsResponse> GetPaymentRequestDetailsAsync([FromQuery] GetPartnerPaymentRequestDetailsRequest request)
        {
            var response = await _partnersPaymentsClient.Api.GetPaymentDetailsAsync(request.PaymentRequestId);

            if (response == null)
                throw LykkeApiErrorException.NotFound(ApiErrorCodes.Service.PartnersPaymentNotFound);

            if (response.CustomerId != _requestContext.UserId)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PaymentRequestsIsForAnotherCustomer);

            return await _ppResponseFormatter.FormatAsync(response);
        }

        /// <summary>
        /// Get pending payment requests for customer
        /// </summary>
        /// <returns></returns>
        [HttpGet("pending")]
        [ProducesResponseType(typeof(PaginatedPartnerPaymentRequestsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<PaginatedPartnerPaymentRequestsResponse> GetPendingPaymentRequestsAsync([FromQuery] PaginatedRequestModel request)
        {
            var response = await _partnersPaymentsClient.Api.GetPendingPaymentsAsync(
                new PaginatedRequestForCustomer
                {
                    CustomerId = _requestContext.UserId,
                    PageSize = request.PageSize,
                    CurrentPage = request.CurrentPage
                });

            return await _ppResponseFormatter.FormatAsync(response);
        }

        /// <summary>
        /// Get payment requests which where successfully completed
        /// </summary>
        /// <returns></returns>
        [HttpGet("succeeded")]
        [ProducesResponseType(typeof(PaginatedPartnerPaymentRequestsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<PaginatedPartnerPaymentRequestsResponse> GetSucceededPaymentRequestsAsync([FromQuery] PaginatedRequestModel request)
        {
            var response = await _partnersPaymentsClient.Api.GetSucceededPaymentsAsync(
                new PaginatedRequestForCustomer
                {
                    CustomerId = _requestContext.UserId,
                    PageSize = request.PageSize,
                    CurrentPage = request.CurrentPage
                });

            return await _ppResponseFormatter.FormatAsync(response);
        }

        /// <summary>
        /// Get payment requests which failed or were cancelled
        /// </summary>
        /// <returns></returns>
        [HttpGet("failed")]
        [ProducesResponseType(typeof(PaginatedPartnerPaymentRequestsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<PaginatedPartnerPaymentRequestsResponse> GetFailedPaymentRequestsAsync([FromQuery] PaginatedRequestModel request)
        {
            var response = await _partnersPaymentsClient.Api.GetFailedPaymentsAsync(
                new PaginatedRequestForCustomer
                {
                    CustomerId = _requestContext.UserId,
                    PageSize = request.PageSize,
                    CurrentPage = request.CurrentPage
                });
            
            return await _ppResponseFormatter.FormatAsync(response);
        }

        /// <summary>
        /// Approve a payment request as a customer
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Error codes:
        /// - **InvalidCustomerId**
        /// - **CustomerWalletMissing**
        /// - **InvalidAmount**
        /// - **NotEnoughTokens**
        /// - **PaymentDoesNotExist**
        /// - **PaymentRequestsIsForAnotherCustomer**
        /// - **PaymentIsNotInACorrectStatusToBeUpdated**
        /// - **CustomerWalletBlocked**     
        /// </remarks>
        [HttpPost("approval")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task ApprovePaymentRequestAsync(ApprovePartnerPaymentRequest request)
        {
            var result = await _partnersPaymentsClient.Api.CustomerApprovePartnerPaymentAsync(
                new CustomerApprovePaymentRequest
                {
                    SendingAmount = request.SendingAmount,
                    CustomerId = _requestContext.UserId,
                    PaymentRequestId = request.PaymentRequestId
                });

            switch (result.Error)
            {
                case PaymentStatusUpdateErrorCodes.None:
                    return;
                case PaymentStatusUpdateErrorCodes.InvalidSenderId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                case PaymentStatusUpdateErrorCodes.SenderWalletMissing:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                case PaymentStatusUpdateErrorCodes.InvalidAmount:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidAmount);
                case PaymentStatusUpdateErrorCodes.NotEnoughFunds:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NotEnoughTokens);
                case PaymentStatusUpdateErrorCodes.PaymentDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PaymentDoesNotExist);
                case PaymentStatusUpdateErrorCodes.CustomerIdDoesNotMatch:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PaymentRequestsIsForAnotherCustomer);
                case PaymentStatusUpdateErrorCodes.PaymentIsInInvalidStatus:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PaymentIsNotInACorrectStatusToBeUpdated);
                case PaymentStatusUpdateErrorCodes.CustomerWalletIsBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletBlocked);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Reject a payment request as a customer
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Error codes:
        /// - **InvalidCustomerId**
        /// - **CustomerWalletMissing**
        /// - **InvalidAmount**
        /// - **NotEnoughTokens**
        /// - **PaymentDoesNotExist**
        /// - **PaymentRequestsIsForAnotherCustomer**
        /// - **PaymentIsNotInACorrectStatusToBeUpdated**
        /// - **CustomerWalletBlocked**     
        /// </remarks>
        [HttpPost("rejection")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task RejectPaymentRequestAsync(RejectPartnerPaymentRequest request)
        {
            var result = await _partnersPaymentsClient.Api.CustomerRejectPartnerPaymentAsync(
                new CustomerRejectPaymentRequest
                {
                    CustomerId = _requestContext.UserId,
                    PaymentRequestId = request.PaymentRequestId
                });

            switch (result.Error)
            {
                case PaymentStatusUpdateErrorCodes.None:
                    return;
                case PaymentStatusUpdateErrorCodes.InvalidSenderId:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                case PaymentStatusUpdateErrorCodes.SenderWalletMissing:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletMissing);
                case PaymentStatusUpdateErrorCodes.InvalidAmount:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidAmount);
                case PaymentStatusUpdateErrorCodes.NotEnoughFunds:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NotEnoughTokens);
                case PaymentStatusUpdateErrorCodes.PaymentDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PaymentDoesNotExist);
                case PaymentStatusUpdateErrorCodes.CustomerIdDoesNotMatch:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PaymentRequestsIsForAnotherCustomer);
                case PaymentStatusUpdateErrorCodes.PaymentIsInInvalidStatus:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.PaymentIsNotInACorrectStatusToBeUpdated);
                case PaymentStatusUpdateErrorCodes.CustomerWalletIsBlocked:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletBlocked);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
