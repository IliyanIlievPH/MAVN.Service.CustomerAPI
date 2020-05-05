using System;
using System.Linq;
using MAVN.Service.PartnerManagement.Client.Models.Partner;

namespace MAVN.Service.CustomerAPI.Extensions
{
    public static class PartnerModelExtensions
    {
        public static string GetLocationName(this PartnerDetailsModel src, string locationId)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            
            return src.Locations.FirstOrDefault(x => x.Id.ToString() == locationId)?.Name;  
        }
    }
}
