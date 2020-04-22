using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerManagement.Client.Models.Requests;
using Refit;
using LoginProvider = Lykke.Service.CustomerManagement.Client.Enums.LoginProvider;

namespace MAVN.Service.CustomerAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGoogleApi _googleApiClient;
        private readonly ICustomerManagementServiceClient _customerManagementServiceClient;
        private readonly IMapper _mapper;

        public AuthService(
            IGoogleApi googleApiClient,
            ICustomerManagementServiceClient customerManagementServiceClient,
            IMapper mapper)
        {
            _googleApiClient = googleApiClient;
            _customerManagementServiceClient = customerManagementServiceClient;
            _mapper = mapper;
        }

        public async Task<AuthenticationResultModel> GoogleAuthenticateAsync(string accessToken)
        {
            var authorization = $"Bearer {accessToken}";

            GoogleUser user;
            try
            {
                user = await _googleApiClient.GetGoogleUser(authorization);

            }
            catch (ApiException e)
            {
                if (e.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new AuthenticationResultModel
                    {
                        Error = CustomerError.InvalidOrExpiredGoogleAccessToken
                    };
                }

                throw;
            }

            var result = await _customerManagementServiceClient.AuthApi.AuthenticateAsync(new AuthenticateRequestModel
            {
                Email = user.Email,
                LoginProvider = LoginProvider.Google
            });

            return _mapper.Map<AuthenticationResultModel>(result);
        }
    }
}
