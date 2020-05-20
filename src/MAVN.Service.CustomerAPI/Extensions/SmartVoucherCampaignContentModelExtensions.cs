using System.Linq;
using MAVN.Service.SmartVouchers.Client.Models.Enums;
using MAVN.Service.SmartVouchers.Client.Models.Responses;
using Localization = MAVN.Service.SmartVouchers.Client.Models.Enums.Localization;

namespace MAVN.Service.CustomerAPI.Extensions
{
    public static class SmartVoucherCampaignContentModelExtensions
    {
        
        public static string GetContentValue(this VoucherCampaignDetailsResponseModel src, Localization language, VoucherCampaignContentType contentType)
        {
            if (src == null)
                return null;

            var contentValue = src.LocalizedContents
                .FirstOrDefault(o => o.ContentType == contentType && o.Localization == language)?.Value;

            if (contentValue != null)
                return contentValue;

            return src.LocalizedContents
                .FirstOrDefault(o => o.ContentType == contentType && o.Localization == Localization.En)?.Value;
        }
    }
}
