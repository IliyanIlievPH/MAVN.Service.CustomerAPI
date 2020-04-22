using System;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class ReferralLeadCreateModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int CountryPhoneCodeId { get; set; }

        public string PhoneNumber { get; set; }

        public string Note { get; set; }

        public Guid CampaignId { get; set; }
    }
}
