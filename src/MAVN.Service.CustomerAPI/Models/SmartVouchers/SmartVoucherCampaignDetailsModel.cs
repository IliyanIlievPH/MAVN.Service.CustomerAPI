using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    public class SmartVoucherCampaignDetailsModel : SmartVoucherCampaignModel
    {
        public List<GeolocationModel> Geolocations { get; set; }
    }
}
