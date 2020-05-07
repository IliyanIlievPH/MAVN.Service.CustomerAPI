using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    public class SmartVoucherCampaignDetailsModel : SmartVoucherCampaignModel
    {
        public List<SmartVoucherCampaignContentResponseModel> LocalizedContents { get; set; }

        public List<GeolocationModel> Geolocations { get; set; }
    }
}
