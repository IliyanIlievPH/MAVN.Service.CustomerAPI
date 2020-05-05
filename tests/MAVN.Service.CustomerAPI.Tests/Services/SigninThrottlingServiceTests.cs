using System;
using System.Threading.Tasks;
using Lykke.Logs;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Services;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client.Models.Responses;
using Moq;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Services
{
    public class SigninThrottlingServiceTests
    {
        #region Mocks
        
        private readonly Mock<IExpiringCountersService> _expiringCountersServiceMock = new Mock<IExpiringCountersService>();
        private readonly Mock<IDistributedLocksServiceProvider> _distributedLocksServiceProviderMock = new Mock<IDistributedLocksServiceProvider>();
        private readonly Mock<IThrottlingSettingsService> _throttlingSettingsServiceMock = new Mock<IThrottlingSettingsService>();
        private readonly Mock<ICustomerProfileClient> _customerProfileClientMock = new Mock<ICustomerProfileClient>();
        private readonly Mock<IDistributedLocksService> _distributedLocksServiceMock = new Mock<IDistributedLocksService>();
        
        #endregion

        private const string FakeEmail = "fake@email.com";

        private const string FakeCustomerId = "243c7f7e-36a7-409b-b44b-4c5882b6b330";
        
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task RegisterFailedSigninAsync_InvalidParameters_ThrowsException(string email)
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.RegisterFailedSigninAsync(email));
        }

        [Fact]
        public async Task RegisterFailedSigninAsync_AlreadyLocked_ReturnsError()
        {
            _distributedLocksServiceProviderMock
                .Setup(x => x.Get(DistributedLockPurpose.SigninThrottling))
                .Returns(_distributedLocksServiceMock.Object);

            _distributedLocksServiceMock
                .Setup(x => x.DoesLockExistAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _customerProfileClientMock
                .Setup(x => x.CustomerProfiles.GetByEmailAsync(It.IsAny<GetByEmailRequestModel>()))
                .ReturnsAsync(new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.None,
                    Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile
                    {
                        CustomerId = FakeCustomerId
                    }
                });

            _throttlingSettingsServiceMock
                .Setup(x => x.GetSigninSettings())
                .Returns(new SigninThrottlingConfiguration{AccountLockPeriod = TimeSpan.FromMinutes(30)});

            var sut = CreateSutInstance();

            var result = await sut.RegisterFailedSigninAsync(FakeEmail);
            
            Assert.Equal(FailedSigninEffect.SigninLocked, result.Effect);
            Assert.Equal(0, result.AttemptsLeftBeforeLock);
            Assert.Equal(_throttlingSettingsServiceMock.Object.GetSigninSettings().AccountLockPeriod.TotalMinutes, result.RetryPeriodInMinutesWhenLocked);
            Assert.NotNull(result.Message);
        }

        [Fact]
        public async Task RegisterFailedSigninAsync_ReachedLockThreshold_ReturnsLockedError()
        {
            const int lockThreshold = 10;
            
            _distributedLocksServiceProviderMock
                .Setup(x => x.Get(DistributedLockPurpose.SigninThrottling))
                .Returns(_distributedLocksServiceMock.Object);

            _distributedLocksServiceMock
                .Setup(x => x.DoesLockExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            _customerProfileClientMock
                .Setup(x => x.CustomerProfiles.GetByEmailAsync(It.IsAny<GetByEmailRequestModel>()))
                .ReturnsAsync(new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.None,
                    Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile
                    {
                        CustomerId = FakeCustomerId
                    }
                });
            
            _throttlingSettingsServiceMock
                .Setup(x => x.GetSigninSettings())
                .Returns(new SigninThrottlingConfiguration{AccountLockPeriod = TimeSpan.FromMinutes(30), LockThreshold = lockThreshold});

            _expiringCountersServiceMock
                .Setup(x => x.IncrementCounterAsync(It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(lockThreshold + 1);

            _distributedLocksServiceMock
                .Setup(x => x.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var sut = CreateSutInstance();

            var result = await sut.RegisterFailedSigninAsync(FakeEmail);
            
            Assert.Equal(FailedSigninEffect.SigninLocked, result.Effect);
            Assert.Equal(0, result.AttemptsLeftBeforeLock);
            Assert.Equal(_throttlingSettingsServiceMock.Object.GetSigninSettings().AccountLockPeriod.TotalMinutes, result.RetryPeriodInMinutesWhenLocked);
            Assert.NotNull(result.Message);
        }

        [Fact]
        public async Task RegisterFailedSigninAsync_ReachedLockThreshold_CouldNotPutLock_RaisesException()
        {
            const int lockThreshold = 10;
            
            _distributedLocksServiceProviderMock
                .Setup(x => x.Get(DistributedLockPurpose.SigninThrottling))
                .Returns(_distributedLocksServiceMock.Object);

            _distributedLocksServiceMock
                .Setup(x => x.DoesLockExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            
            _customerProfileClientMock
                .Setup(x => x.CustomerProfiles.GetByEmailAsync(It.IsAny<GetByEmailRequestModel>()))
                .ReturnsAsync(new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.None,
                    Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile
                    {
                        CustomerId = FakeCustomerId
                    }
                });
            
            _throttlingSettingsServiceMock
                .Setup(x => x.GetSigninSettings())
                .Returns(new SigninThrottlingConfiguration{AccountLockPeriod = TimeSpan.FromMinutes(30), LockThreshold = lockThreshold});

            _expiringCountersServiceMock
                .Setup(x => x.IncrementCounterAsync(It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(lockThreshold + 1);
            
            _distributedLocksServiceMock
                .Setup(x => x.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var sut = CreateSutInstance();
            
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.RegisterFailedSigninAsync(FakeEmail));
        }

        [Theory]
        [InlineData(10, 8, 9, FailedSigninEffect.Warning)]
        [InlineData(10, 8, 8, FailedSigninEffect.Warning)]
        [InlineData(10, 8, 7, FailedSigninEffect.None)]
        public async Task RegisterFailedSigninAsync_ReturnsCorrespondingError(int lockThreshold, 
            int warningThreshold,
            int actualCounter, 
            FailedSigninEffect expected)
        {
            _distributedLocksServiceProviderMock
                .Setup(x => x.Get(DistributedLockPurpose.SigninThrottling))
                .Returns(_distributedLocksServiceMock.Object);

            _distributedLocksServiceMock
                .Setup(x => x.DoesLockExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _customerProfileClientMock
                .Setup(x => x.CustomerProfiles.GetByEmailAsync(It.IsAny<GetByEmailRequestModel>()))
                .ReturnsAsync(new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.None,
                    Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile
                    {
                        CustomerId = FakeCustomerId
                    }
                });

            _throttlingSettingsServiceMock
                .Setup(x => x.GetSigninSettings())
                .Returns(new SigninThrottlingConfiguration
                {
                    AccountLockPeriod = TimeSpan.FromMinutes(30),
                    LockThreshold = lockThreshold,
                    WarningThreshold = warningThreshold
                });

            _expiringCountersServiceMock
                .Setup(x => x.IncrementCounterAsync(It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(actualCounter);

            var sut = CreateSutInstance();

            var result = await sut.RegisterFailedSigninAsync(FakeEmail);

            Assert.Equal(expected, result.Effect);
            Assert.Equal(lockThreshold - actualCounter, result.AttemptsLeftBeforeLock);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FlushFailuresAsync_InvalidParameters_RaisesException(string email)
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.FlushFailuresAsync(email));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task IsSigninLockedAsync_ReturnsCorrespondingResult(bool isLocked)
        {
            var lockPeriod = TimeSpan.FromMinutes(30);
            
            _customerProfileClientMock
                .Setup(x => x.CustomerProfiles.GetByEmailAsync(It.IsAny<GetByEmailRequestModel>()))
                .ReturnsAsync(new CustomerProfileResponse
                {
                    ErrorCode = CustomerProfileErrorCodes.None,
                    Profile = new CustomerProfile.Client.Models.Responses.CustomerProfile
                    {
                        CustomerId = FakeCustomerId
                    }
                });
            
            _distributedLocksServiceProviderMock
                .Setup(x => x.Get(DistributedLockPurpose.SigninThrottling))
                .Returns(_distributedLocksServiceMock.Object);

            _distributedLocksServiceMock
                .Setup(x => x.DoesLockExistAsync(It.IsAny<string>()))
                .ReturnsAsync(isLocked);
            
            _throttlingSettingsServiceMock
                .Setup(x => x.GetSigninSettings())
                .Returns(new SigninThrottlingConfiguration
                {
                    AccountLockPeriod = lockPeriod,
                });
            
            var sut = CreateSutInstance();

            var result = await sut.IsSigninLockedAsync(FakeEmail);

            Assert.Equal(isLocked, result.IsLocked);
            Assert.Equal(isLocked ? lockPeriod.TotalMinutes : 0, result.RetryPeriodInMinutesWhenLocked);
        }

        private ISigninThrottlingService CreateSutInstance()
        {
            return new SigninThrottlingService(
                _expiringCountersServiceMock.Object,
                _distributedLocksServiceProviderMock.Object,
                _throttlingSettingsServiceMock.Object,
                EmptyLogFactory.Instance,
                _customerProfileClientMock.Object);
        }
    }
}
