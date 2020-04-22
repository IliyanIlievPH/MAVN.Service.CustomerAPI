using System;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class CustomerInfoModel
    {
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string PhoneNumber { get; set; }
        public string ShortPhoneNumber { get; set; }
        public DateTime Registered { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public string CountryPhoneCode { get; set; }
        public int? CountryPhoneCodeId { get; set; }
        public int CountryOfNationalityId { get; set; }
        public string CountryOfNationalityName { get; set; }
    }
}
