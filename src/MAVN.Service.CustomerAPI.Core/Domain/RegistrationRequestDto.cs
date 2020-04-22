namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string ReferralCode { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? CountryOfNationalityId { get; set; }

    }
}
