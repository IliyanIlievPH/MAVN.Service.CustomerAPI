using System;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    [PublicAPI]
    public class ReferralLeadRequestModel
    {
        /// <summary>
        /// The first name of the referred lead.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the referred lead.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The country phone code identifier.
        /// </summary>
        public int CountryPhoneCodeId { get; set; }

        /// <summary>
        /// The phone number of the referred lead.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The email of the referred lead.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The note to Emaar.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Campaign's id
        /// </summary>
        public Guid CampaignId { get; set; }
    }
}
