using System;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class HotelReferralModel
    {
        /// <summary>
        /// Id of the referral
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the referred lead if customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Name of the referred lead if customer
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Full name of the referred lead
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Phone country code Id
        /// </summary>
        public int CountryPhoneCodeId { set; get; }

        /// <summary>
        /// Phone number of the referred lead
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Status of the referral
        /// </summary>
        public ReferralHotelStatus Status { get; set; }

        /// <summary>
        /// TimeStamp of the referral
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Email of the referred person
        /// </summary>
        public string Email { get; set; }
    }
}
