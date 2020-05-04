using System.Linq;
using Autofac;
using Common.Cache;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.SettingsReader;
using MAVN.Service.CustomerAPI.Services;
using MAVN.Service.CustomerAPI.Settings;
using Refit;
using StackExchange.Redis;

namespace MAVN.Service.CustomerAPI.Modules
{
    public class ServiceModule : Module
    {
        private readonly BaseSettings _settings;
        private readonly MobileAppSettings _mobileSettings;

        private const string RequestsRouteThrottlingByCustomerPattern = "c-api:throttling:route:{0}:{1}:{2}";
        private const string RequestsSignInThrottlingByLoginPattern = "c-api:throttling:signin:{0}";

        public ServiceModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.Nested(x => x.CustomerApiService).CurrentValue;
            _mobileSettings = settings.Nested(x => x.MobileAppSettings).CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StartupManager>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<CustomerService>()
                .As<ICustomerService>()
                .SingleInstance();

            builder.RegisterType<WalletOperationsService>()
                .As<IWalletOperationsService>()
                .WithParameter(TypedParameter.From(_settings.Constants.TokenSymbol))
                .SingleInstance();

            builder.RegisterType<RequestContext>().As<IRequestContext>().InstancePerLifetimeScope();
            builder.RegisterType<LykkePrincipal>().As<ILykkePrincipal>().InstancePerLifetimeScope();

            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>();  

            builder.RegisterType<ReferralService>().As<IReferralService>().SingleInstance();

            builder.RegisterType<MobileSettingsReader>()
                .WithParameter(TypedParameter.From(_mobileSettings.SettingsUrl))
                .As<IMobileSettingsReader>()
                .As<IStartStop>()
                .SingleInstance();

            builder.RegisterType<PushNotificationService>()
                .As<IPushNotificationService>()
                .SingleInstance();

            builder.RegisterType<PartnersMessagesService>()
                .As<IPartnersMessagesService>()
                .SingleInstance();

            var passwordRules = new PasswordValidationRulesDto
            {
                AllowWhiteSpaces = _settings.PasswordValidationSettings.AllowWhiteSpaces,
                MinLength = _settings.PasswordValidationSettings.MinLength,
                MinUpperCase = _settings.PasswordValidationSettings.MinUpperCase,
                MinLowerCase = _settings.PasswordValidationSettings.MinLowerCase,
                MinNumbers = _settings.PasswordValidationSettings.MinNumbers,
                AllowedSpecialSymbols = _settings.PasswordValidationSettings.AllowedSpecialSymbols,
                MinSpecialSymbols = _settings.PasswordValidationSettings.MinSpecialSymbols,
                MaxLength = _settings.PasswordValidationSettings.MaxLength,
            };
            builder.RegisterType<PasswordValidator>()
                .WithParameter(TypedParameter.From(passwordRules))
                .As<IPasswordValidator>()
                .SingleInstance();
                
            builder.RegisterType<OperationsHistoryService>().As<IOperationsHistoryService>();

            builder.RegisterInstance(RestService.For<IGoogleApi>(_settings.GoogleAuthSettings.GoogleApiUrl))
                .As<IGoogleApi>()
                .SingleInstance();

            builder.RegisterType<AuthService>()
                .As<IAuthService>()
                .SingleInstance();
            
            builder.Register(c => ConnectionMultiplexer.Connect(_settings.CacheSettings.RedisConfiguration))
                .As<IConnectionMultiplexer>()
                .SingleInstance();
            
            builder.RegisterType<RedisLocksService>()
                .WithParameter(TypedParameter.From(RequestsRouteThrottlingByCustomerPattern))
                .Keyed<IDistributedLocksService>(DistributedLockPurpose.RouteThrottling)
                .As<IDistributedLocksService>()
                .SingleInstance();
            
            builder.RegisterType<RedisLocksService>()
                .WithParameter(TypedParameter.From(RequestsSignInThrottlingByLoginPattern))
                .Keyed<IDistributedLocksService>(DistributedLockPurpose.SigninThrottling)
                .As<IDistributedLocksService>()
                .SingleInstance();
            
            builder.RegisterType<DistributedLocksServiceProvider>()
                .As<IDistributedLocksServiceProvider>();
            
            builder.RegisterType<ExpiringCountersService>()
                .As<IExpiringCountersService>()
                .SingleInstance();

            builder.RegisterType<SettingsService>()
                .WithParameter("tokenName", _settings.Constants.TokenSymbol)
                .WithParameter("usdAssetName", _settings.Constants.UsdAssetName)
                .WithParameter("baseCurrencyCode", _settings.BaseCurrencyCode)
                .WithParameter("transitAccountAddress", _settings.TransitAccountAddress)
                .WithParameter("isPublicBlockchainFeatureDisabled", _settings.IsPublicBlockchainFeatureDisabled)
                .As<ISettingsService>();

            builder.RegisterType<ThrottlingSettingsService>()
                .WithParameter(TypedParameter.From(_settings.RequestLimitsByCustomer.Select(x =>
                    new RouteThrottlingConfigurationItem
                    {
                        Verb = x.Verb, Route = x.Route, FrequencyInSeconds = x.FrequencyInSeconds
                    })))
                .WithParameter(TypedParameter.From(new SigninThrottlingConfiguration
                {
                    WarningThreshold = _settings.FailedLoginAttemptsThrottling.WarningThreshold,
                    LockThreshold = _settings.FailedLoginAttemptsThrottling.LockThreshold,
                    ThresholdPeriod = _settings.FailedLoginAttemptsThrottling.ThresholdPeriod,
                    AccountLockPeriod = _settings.FailedLoginAttemptsThrottling.AccountLockPeriod
                }))
                .As<IThrottlingSettingsService>()
                .SingleInstance();

            builder.RegisterType<SigninThrottlingService>()
                .As<ISigninThrottlingService>()
                .SingleInstance();

            builder.RegisterType<CurrencyConverter>()
                .As<ICurrencyConverter>()
                .SingleInstance();

            builder.RegisterType<PartnersPaymentsResponseFormatter>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<PublicWalletLinkingService>()
                .As<IPublicWalletLinkingService>()
                .SingleInstance();

            builder.RegisterType<PublicWalletTransferService>()
                .As<IPublicWalletTransferService>()
                .SingleInstance();

            builder.RegisterType<NotificationMessagesService>()
                .As<INotificationMessagesService>()
                .SingleInstance();
        }
    }
}
