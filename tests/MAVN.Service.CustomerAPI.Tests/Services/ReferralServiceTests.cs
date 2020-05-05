using System;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Logs;
using MAVN.Service.BonusEngine.Client;
using MAVN.Service.Campaign.Client;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Infrastructure.AutoMapperProfiles;
using MAVN.Service.CustomerAPI.Services;
using MAVN.Service.EligibilityEngine.Client;
using MAVN.Service.OperationsHistory.Client;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.Referral.Client;
using MAVN.Service.Referral.Client.Enums;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.Referral.Client.Models.Responses;
using MAVN.Service.Staking.Client;
using Moq;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Services
{
    public class ReferralServiceTests
    {
        private readonly IMapper _mapper;

        private readonly Mock<IReferralClient> _referralClient;
        private readonly Mock<ICampaignClient> _campaignClient;
        private readonly Mock<IBonusEngineClient> _bonusEngineClient;
        private readonly Mock<IStakingClient> _stakingClient;
        private readonly Mock<IPartnerManagementClient> _partnerManagementClient;
        private readonly Mock<IEligibilityEngineClient> _eligibilityEngine;
        private readonly Mock<ISettingsService> _settingsService;
        private readonly Mock<IOperationsHistoryClient> _operrationHistoryServiceMock;

        private readonly IReferralService _referralService;

        public ReferralServiceTests()
        {
            _referralClient = new Mock<IReferralClient>();
            _campaignClient = new Mock<ICampaignClient>();
            _bonusEngineClient = new Mock<IBonusEngineClient>();
            _stakingClient = new Mock<IStakingClient>();
            _partnerManagementClient = new Mock<IPartnerManagementClient>();
            _eligibilityEngine = new Mock<IEligibilityEngineClient>();
            _settingsService = new Mock<ISettingsService>();
            _operrationHistoryServiceMock = new Mock<IOperationsHistoryClient>();

            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();

            _referralService = new ReferralService(_referralClient.Object,
                _campaignClient.Object,
                _bonusEngineClient.Object,
                _stakingClient.Object,
                _partnerManagementClient.Object,
                _eligibilityEngine.Object,
                _settingsService.Object,
                _operrationHistoryServiceMock.Object,
                LogFactory.Create(),
                _mapper);
        }

        [Fact]
        public async Task ShouldGetReferralCode_WhenReferralCodeExits()
        {
            // Arrange
            var referralCode = "refcod";

            _referralClient.Setup(c => c.ReferralApi.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new ReferralResultResponse()
                {
                    ReferralCode = referralCode
                });

            // Act
            var result = await _referralService.GetOrCreateReferralCodeAsync("123");

            // Assert
            Assert.Equal(referralCode, result);
        }

        [Fact]
        public async Task ShouldCreateReferralCode_WhenReferralCodeDoesNotExists()
        {
            // Arrange
            var referralCode = "refcod";

            _referralClient.Setup(c => c.ReferralApi.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new ReferralResultResponse()
                {
                    ReferralCode = null,
                    ErrorCode = ReferralErrorCodes.ReferralNotFound
                });

            _referralClient.Setup(c => c.ReferralApi.PostAsync(It.IsAny<ReferralCreateRequest>()))
                .ReturnsAsync(new ReferralCreateResponse()
                {
                    ReferralCode = referralCode
                });

            // Act
            var result = await _referralService.GetOrCreateReferralCodeAsync("123");

            // Assert
            Assert.Equal(referralCode, result);
        }

        [Fact]
        public async Task ShouldReturnNull_WhenReferralCreateFails()
        {
            // Arrange
            _referralClient.Setup(c => c.ReferralApi.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new ReferralResultResponse()
                {
                    ReferralCode = null,
                    ErrorCode = ReferralErrorCodes.ReferralNotFound
                });

            _referralClient.Setup(c => c.ReferralApi.PostAsync(It.IsAny<ReferralCreateRequest>()))
                .ReturnsAsync(new ReferralCreateResponse()
                {
                    ReferralCode = null
                });

            // Act
            var result = await _referralService.GetOrCreateReferralCodeAsync("123");

            // Assert
            Assert.Null(result);
        }
    }
}
