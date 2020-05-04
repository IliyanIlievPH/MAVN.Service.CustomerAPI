using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Infrastructure.AutoMapperProfiles;
using MAVN.Service.CustomerAPI.Services;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerManagement.Client.Models.Requests;
using MAVN.Service.CustomerManagement.Client.Models.Responses;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Responses;
using MAVN.Service.Dictionaries.Client;
using MAVN.Service.Dictionaries.Client.Models.Salesforce;
using Moq;
using Refit;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Services
{
    public class CustomerServiceTests
    {
        private const string MockAccessToken = "token";

        private readonly Mock<IDictionariesClient> _dictionariesClientMock = new Mock<IDictionariesClient>();
        private readonly IMapper _mapper;

        public CustomerServiceTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();
        }

        [Fact]
        public async Task CustomerTriesToViewHisInfo_WithBeingSuccsesfullyLoggedIn_ShouldProvideCustomerEmail()
        {
            var expectedEmail = "test@email.com";

            var customerManagementClient = new Mock<ICustomerManagementServiceClient>();

            var googleApiClient = new Mock<IGoogleApi>();

            var customerProfileClient = new Mock<ICustomerProfileClient>();
            customerProfileClient
                .Setup(x => x.CustomerProfiles.GetByCustomerIdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(
                    new CustomerProfileResponse
                    {
                        Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile
                        {
                            Email = expectedEmail,
                        }
                    });

            _dictionariesClientMock.Setup(x => x.Salesforce.GetCountryOfResidenceByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new CountryOfResidenceModel {Name = "name"});

            var customerService = new CustomerService(
                customerManagementClient.Object,
                customerProfileClient.Object,
                googleApiClient.Object,
                _dictionariesClientMock.Object,
                _mapper);

            var actual = await customerService.GetCustomerInfoAsync("testCustomerId");

            Assert.Equal(expectedEmail, actual.Email);
        }

        [Fact]
        public async Task CustomerTriesRegisterWithGoogle_InvalidOrExpiredAuthToken_ErrorReturned()
        {
            var customerManagementClient = new Mock<ICustomerManagementServiceClient>();

            var googleApiClient = new Mock<IGoogleApi>();
            googleApiClient.Setup(x => x.GetGoogleUser(It.IsAny<string>()))
                .ThrowsAsync(
                    ApiException.Create(new HttpRequestMessage(HttpMethod.Get, "a"),
                        HttpMethod.Get, new HttpResponseMessage(HttpStatusCode.Unauthorized)).Result);

            var customerProfileClient = new Mock<ICustomerProfileClient>();

            var customerService = new CustomerService(
                customerManagementClient.Object,
                customerProfileClient.Object,
                googleApiClient.Object,
                _dictionariesClientMock.Object,
                _mapper);

            var actual = await customerService.GoogleRegisterAsync(
                    new GoogleRegistrationRequestDto {AccessToken = MockAccessToken});

            Assert.Equal(CustomerError.InvalidOrExpiredGoogleAccessToken, actual.Error);
        }

        [Fact]
        public async Task CustomerTriesRegisterWithGoogle_UnexpectedException_Rethrown()
        {
            var customerManagementClient = new Mock<ICustomerManagementServiceClient>();

            var googleApiClient = new Mock<IGoogleApi>();
            googleApiClient.Setup(x => x.GetGoogleUser(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var customerProfileClient = new Mock<ICustomerProfileClient>();

            var customerService = new CustomerService(
                customerManagementClient.Object,
                customerProfileClient.Object,
                googleApiClient.Object,
                _dictionariesClientMock.Object,
                _mapper);

            await Assert.ThrowsAsync<Exception>(() => customerService.GoogleRegisterAsync(
                new GoogleRegistrationRequestDto {AccessToken = MockAccessToken}));
        }

        [Theory]
        [InlineData(CustomerManagementError.AlreadyRegisteredWithGoogle, CustomerError.AlreadyRegisteredWithGoogle)]
        [InlineData(CustomerManagementError.AlreadyRegistered, CustomerError.AlreadyRegistered)]
        [InlineData(CustomerManagementError.LoginExistsWithDifferentProvider, CustomerError.LoginExistsWithDifferentProvider)]
        public async Task CustomerTriesRegisterWithGoogle_ErrorFromCustomerManagement_ErrorReturned
            (CustomerManagementError customerManagementError, CustomerError expectedError)
        {
            var customerManagementClient = new Mock<ICustomerManagementServiceClient>();
            customerManagementClient.Setup(x => x.CustomersApi.RegisterAsync(It.IsAny<RegistrationRequestModel>()))
                .ReturnsAsync(new RegistrationResponseModel
                {
                    Error = customerManagementError
                });

            var googleApiClient = new Mock<IGoogleApi>();
            googleApiClient.Setup(x => x.GetGoogleUser(It.IsAny<string>()))
                .ReturnsAsync(new GoogleUser
                {
                    Email = "mail"
                });

            var customerProfileClient = new Mock<ICustomerProfileClient>();

            var customerService = new CustomerService(
                customerManagementClient.Object,
                customerProfileClient.Object,
                googleApiClient.Object,
                _dictionariesClientMock.Object,
                _mapper);

            var actual = await customerService.GoogleRegisterAsync(
                new GoogleRegistrationRequestDto {AccessToken = MockAccessToken});

            Assert.Equal(expectedError, actual.Error);
        }
    }
}
