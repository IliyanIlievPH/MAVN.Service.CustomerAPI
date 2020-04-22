using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerManagement.Client.Models;
using Lykke.Service.CustomerManagement.Client.Models.Requests;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.Dictionaries.Client;
using Refit;
using LoginProvider = Lykke.Service.CustomerManagement.Client.Enums.LoginProvider;

namespace MAVN.Service.CustomerAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerManagementServiceClient _customerManagementServiceClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IGoogleApi _googleApiClient;
        private readonly IDictionariesClient _dictionariesClient;
        private readonly IMapper _mapper;

        public CustomerService(
            ICustomerManagementServiceClient customerManagementServiceClient,
            ICustomerProfileClient customerProfileClient,
            IGoogleApi googleApiClient,
            IDictionariesClient dictionariesClient,
            IMapper mapper)
        {
            _customerManagementServiceClient = customerManagementServiceClient;
            _customerProfileClient = customerProfileClient;
            _googleApiClient = googleApiClient;
            _dictionariesClient = dictionariesClient;
            _mapper = mapper;
        }

        public async Task<RegistrationResultModel> RegisterAsync(RegistrationRequestDto model)
        {
            var cmRequest = _mapper.Map<RegistrationRequestModel>(model);

            var result = await _customerManagementServiceClient.CustomersApi.RegisterAsync(cmRequest);

            return _mapper.Map<RegistrationResultModel>(result);
        }

        public async Task<RegistrationResultModel> GoogleRegisterAsync(GoogleRegistrationRequestDto model)
        {
            var authorization = $"Bearer {model.AccessToken}";

            GoogleUser user;
            try
            {
                user = await _googleApiClient.GetGoogleUser(authorization);

            }
            catch (ApiException e)
            {
                if (e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new RegistrationResultModel
                    {
                        Error = CustomerError.InvalidOrExpiredGoogleAccessToken
                    };
                }

                throw;
            }

            var result = await _customerManagementServiceClient.CustomersApi.RegisterAsync(new RegistrationRequestModel
            {
                LoginProvider = LoginProvider.Google,
                Email = user.Email,
                ReferralCode = model.ReferralCode,
                FirstName = model.FirstName,
                LastName = model.LastName
            });

            return _mapper.Map<RegistrationResultModel>(result);
        }

        public async Task<CustomerInfoModel> GetCustomerInfoAsync(
            string customerId,
            bool includeNotVerified = false,
            bool includeNotActive = false)
        {
            var result = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(
                customerId,
                includeNotVerified,
                includeNotActive);
            if (result == null)
                return null;

            var response = _mapper.Map<CustomerInfoModel>(result.Profile);

            var countryResponse =
                await _dictionariesClient.Salesforce.GetCountryOfResidenceByIdAsync(response.CountryOfNationalityId);

            response.CountryOfNationalityName = countryResponse?.Name;

            if (!response.CountryPhoneCodeId.HasValue)
                return response;

            var customerCountryPhoneCode =
                await _dictionariesClient.Salesforce.GetCountryPhoneCodeByIdAsync(response.CountryPhoneCodeId.Value);

            response.CountryPhoneCode = customerCountryPhoneCode.IsoCode;

            return response;
        }

        public async Task<ChangePasswordResultModel> ChangePasswordAsync(string customerId, string password)
        {
            var result = await _customerManagementServiceClient.CustomersApi.ChangePasswordAsync(
                new ChangePasswordRequestModel
                {
                    CustomerId = customerId,
                    Password = password
                });
            
            return _mapper.Map<ChangePasswordResultModel>(result);
        }

        public async Task<ResetPasswordResultModel> ResetPasswordAsync(string email, string resetIdentifier, string password)
        {
            var result = await _customerManagementServiceClient.CustomersApi.PasswordResetAsync(new PasswordResetRequest
            {
                CustomerEmail = email,
                Password = password,
                ResetIdentifier = resetIdentifier
            });
            
            return _mapper.Map<ResetPasswordResultModel>(result);
        }
    }
}
