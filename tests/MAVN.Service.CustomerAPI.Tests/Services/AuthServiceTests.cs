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
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerManagement.Client.Enums;
using Lykke.Service.CustomerManagement.Client.Models.Requests;
using Lykke.Service.CustomerManagement.Client.Models.Responses;
using Moq;
using Refit;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Services
{
    public class AuthServiceTests
    {
        private const string FakeAccessToken = "token";
        private const string FakeEmail = "email@mail.com";

        private readonly Mock<IGoogleApi> _googleApiMock = new Mock<IGoogleApi>();
        private readonly Mock<ICustomerManagementServiceClient> _customerManagementClientMock = new Mock<ICustomerManagementServiceClient>();
        private readonly IMapper _mapper;

        public AuthServiceTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();
        }

        [Fact]
        public async Task GoogleAuthAsync_InvalidOrExpiredAccessToken_ErrorReturned()
        {
            _googleApiMock.Setup(x => x.GetGoogleUser(It.IsAny<string>()))
                .ThrowsAsync(
                    ApiException.Create(new HttpRequestMessage(HttpMethod.Get, "a"),
                        HttpMethod.Get, new HttpResponseMessage(HttpStatusCode.Unauthorized)).Result);

            var sut = CreateSutInstance();

            var result = await sut.GoogleAuthenticateAsync(FakeAccessToken);

            Assert.Equal(CustomerError.InvalidOrExpiredGoogleAccessToken, result.Error);
        }

        [Fact]
        public async Task GoogleAuthAsync_UnexpectedException_Rethrown()
        {
            _googleApiMock.Setup(x => x.GetGoogleUser(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<Exception>(() => sut.GoogleAuthenticateAsync(FakeAccessToken));
        }

        [Theory]
        [InlineData(CustomerManagementError.LoginExistsWithDifferentProvider, CustomerError.LoginExistsWithDifferentProvider)]
        [InlineData(CustomerManagementError.CustomerBlocked, CustomerError.CustomerBlocked)]
        [InlineData(CustomerManagementError.LoginNotFound, CustomerError.LoginNotFound)]

        public async Task CustomerTriesLoginWithGoogle_ErrorFromCustomerManagement_ErrorReturned
            (CustomerManagementError customerManagementError, CustomerError expectedError)
        {
            _googleApiMock.Setup(x => x.GetGoogleUser(It.IsAny<string>()))
                .ReturnsAsync(new GoogleUser
                {
                    Email = FakeEmail
                });

            _customerManagementClientMock.Setup(x => x.AuthApi.AuthenticateAsync(It.IsAny<AuthenticateRequestModel>()))
                .ReturnsAsync(new AuthenticateResponseModel { Error = customerManagementError });

            var sut = CreateSutInstance();

            var result = await sut.GoogleAuthenticateAsync(FakeAccessToken);

            Assert.Equal(expectedError, result.Error);
        }

        private AuthService CreateSutInstance()
        {
            return new AuthService(_googleApiMock.Object, _customerManagementClientMock.Object, _mapper);
        }
    }
}
