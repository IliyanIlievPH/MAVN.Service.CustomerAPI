using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client.Models.Requests;

namespace MAVN.Service.CustomerAPI.Services
{
    public class SigninThrottlingService : ISigninThrottlingService
    {
        private readonly IExpiringCountersService _expiringCountersService;
        private readonly IDistributedLocksService _locksService;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly SigninThrottlingConfiguration _config;
        private readonly ILog _log; 

        public SigninThrottlingService(
            IExpiringCountersService expiringCountersService, 
            IDistributedLocksServiceProvider distributedLocksServiceProvider, 
            IThrottlingSettingsService throttlingSettingsService,
            ILogFactory logFactory, 
            ICustomerProfileClient customerProfileClient)
        {
            _expiringCountersService = expiringCountersService;
            _customerProfileClient = customerProfileClient;
            _locksService = distributedLocksServiceProvider.Get(DistributedLockPurpose.SigninThrottling);
            _config = throttlingSettingsService.GetSigninSettings();
            _log = logFactory.CreateLog(this);
        }

        public async Task<FailedSigninResultModel> RegisterFailedSigninAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            var (customerIdentity, sanitizedIdentity) = await GetCustomerIdentityAsync(email);

            if (await _locksService.DoesLockExistAsync(customerIdentity))
            {
                _log.Warning($"The signin for [{sanitizedIdentity}] is already locked");
                
                return FailedSigninResultModel.SigninLocked((int) _config.AccountLockPeriod.TotalMinutes);
            }
            
            var counter = await _expiringCountersService.IncrementCounterAsync(_config.ThresholdPeriod,
                nameof(SigninThrottlingService), customerIdentity);

            if (counter >= _config.LockThreshold)
            {
                var signinLocked = await _locksService.TryAcquireLockAsync(
                    new {customerIdentity}.ToJson(),
                    DateTime.UtcNow.Add(_config.AccountLockPeriod),
                    customerIdentity);

                if (!signinLocked)
                {
                    _log.Error(message: $"Couldn't lock signin for [{sanitizedIdentity}]");
                    
                    throw new InvalidOperationException("Signin lock was unsuccessful"); 
                }
                
                _log.Warning($"The account [{sanitizedIdentity}] has been locked for signin attempts for {_config.AccountLockPeriod}");
                
                return FailedSigninResultModel.SigninLocked((int) _config.AccountLockPeriod.TotalMinutes);
            }
            
            var attemptsLeftBeforeLock = _config.LockThreshold - (int) counter;

            return counter >= _config.WarningThreshold
                ? FailedSigninResultModel.Warning(attemptsLeftBeforeLock)
                : FailedSigninResultModel.NoImpact(attemptsLeftBeforeLock);
        }
        
        public async Task FlushFailuresAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            
            var (customerIdentity, sanitizedIdentity) = await GetCustomerIdentityAsync(email);

            await _expiringCountersService.ResetCounterAsync(nameof(SigninThrottlingService), customerIdentity);
        }

        public async Task<SigninLockStatusResultModel> IsSigninLockedAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            
            var (customerIdentity, sanitizedIdentity) = await GetCustomerIdentityAsync(email);
            
            var isLocked = await _locksService.DoesLockExistAsync(customerIdentity);

            return isLocked
                ? SigninLockStatusResultModel.Locked((int) _config.AccountLockPeriod.TotalMinutes)
                : SigninLockStatusResultModel.NotLocked();
        }

        private async Task<(string, string)> GetCustomerIdentityAsync(string email)
        {
            var customerProfileResponse = await _customerProfileClient.CustomerProfiles.GetByEmailAsync(
                new GetByEmailRequestModel { Email = email, IncludeNotVerified = true });

            var result = email;

            if (customerProfileResponse.ErrorCode == CustomerProfileErrorCodes.None)
            {
                result = customerProfileResponse.Profile.CustomerId;
            }
            else
            {
                _log.Warning($"Customer profile was not found by email {email.SanitizeEmail()}",
                    context: new {error = customerProfileResponse.ErrorCode.ToString()});
            }

            var sanitizedResult = result.IsValidEmail() ? result.SanitizeEmail() : result;

            return (result, sanitizedResult);
        }
    }
}
