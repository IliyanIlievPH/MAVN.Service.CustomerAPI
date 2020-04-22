using JetBrains.Annotations;
using System.Collections.Generic;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    [PublicAPI]
    public class HotelReferralsListResponseModel
    {
        /// <summary>
        /// Hotel referral leads
        /// </summary>
        public IReadOnlyCollection<HotelReferralModel> HotelReferrals { get; set; }
    }
}
