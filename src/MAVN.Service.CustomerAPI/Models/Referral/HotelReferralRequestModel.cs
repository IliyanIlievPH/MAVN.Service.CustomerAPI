using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    [PublicAPI]
    public class HotelReferralRequestModel
    {
        /// <summary>
        /// The email of the referred person.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The country phone code identifier.
        /// </summary>
        public int CountryPhoneCodeId { get; set; }

        /// <summary>
        /// The phone number of the referred lead.
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Full name of the referred person.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Represents a campaign's id
        /// </summary>
        public Guid CampaignId { get; set; }
    }
}
