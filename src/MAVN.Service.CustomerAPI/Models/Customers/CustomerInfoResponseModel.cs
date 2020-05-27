using System;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Customers
{
    [PublicAPI]
    public class CustomerInfoResponseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsAccountBlocked { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public string CountryPhoneCode { get; set; }
        public int? CountryPhoneCodeId { get; set; }
        public int CountryOfNationalityId { get; set; }
        public string CountryOfNationalityName { get; set; }
        public bool HasPin { get; set; }
        public Guid? LinkedPartnerId { get; set; }
    }
}
