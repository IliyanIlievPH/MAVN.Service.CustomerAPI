namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class GoogleRegistrationRequestDto
    {
        public string AccessToken { get; set; }
        public string ReferralCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? CountryOfNationalityId { get; set; }
    }
}
