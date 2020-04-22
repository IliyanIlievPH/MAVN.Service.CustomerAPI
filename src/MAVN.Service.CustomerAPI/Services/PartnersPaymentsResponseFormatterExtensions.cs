using System;
using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Models.PartnerPayments;
using Lykke.Service.PartnersPayments.Client.Models;

namespace MAVN.Service.CustomerAPI.Services
{
    public static class PartnersPaymentsResponseFormatterExtensions
    {
        public static async Task<PaginatedPartnerPaymentRequestsResponse> FormatAsync(
            this PartnersPaymentsResponseFormatter formatter, PaginatedPaymentRequestsResponse ppResponseModel)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            
            if (ppResponseModel == null)
                throw new ArgumentNullException(nameof(ppResponseModel));
            
            return new PaginatedPartnerPaymentRequestsResponse
            {
                CurrentPage = ppResponseModel.CurrentPage,
                PageSize = ppResponseModel.PageSize,
                TotalCount = ppResponseModel.TotalCount,
                PaymentRequests = await formatter.FormatAsync(ppResponseModel.PaymentRequests)
            };
        }
    }
}
