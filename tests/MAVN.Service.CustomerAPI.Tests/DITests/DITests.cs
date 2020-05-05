using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using MAVN.Service.BonusEngine.Client;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Credentials.Client;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CrossChainTransfers.Client;
using MAVN.Service.CrossChainWalletLinker.Client;
using MAVN.Service.CustomerAPI.Modules;
using MAVN.Service.CustomerAPI.Settings;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.Dictionaries.Client;
using MAVN.Service.EligibilityEngine.Client;
using MAVN.Service.EthereumBridge.Client;
using MAVN.Service.MaintenanceMode.Client;
using MAVN.Service.OperationsHistory.Client;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnersIntegration.Client;
using MAVN.Service.PartnersPayments.Client;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.PushNotifications.Client;
using MAVN.Service.Referral.Client;
using MAVN.Service.Sessions.Client;
using MAVN.Service.Staking.Client;
using MAVN.Service.Vouchers.Client;
using MAVN.Service.WalletManagement.Client;
using Lykke.SettingsReader.ReloadingManager;
using MAVN.Service.SmartVouchers.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.DITests
{
    public class DiTests
    {
        private const string MockUrl = "http://localhost";
        private readonly IContainer _container;
        private readonly ILogFactory _logFactory;

        public DiTests()
        {
            var settings = new AppSettings
            {
                CustomerApiService =
                    new BaseSettings
                    {
                        Db = new DbSettings(),
                        CacheSettings = new CacheSettings(),
                        PasswordValidationSettings = new PasswordValidationSettings(),
                        Constants = new Settings.Constants {TokenSymbol = "MVN"},
                        GoogleAuthSettings = new GoogleAuthSettings {GoogleApiUrl = MockUrl},
                        RequestLimitsByCustomer = new List<ThrottlingSettings>(),
                        FailedLoginAttemptsThrottling = new FailedSigninAttemptsThrottlingSettings()
                    },
                SessionsServiceClient = new SessionsServiceClientSettings {ServiceUrl = MockUrl},
                CustomerManagementServiceClient =
                    new CustomerManagementServiceClientSettings {ServiceUrl = MockUrl},
                PrivateBlockchainFacadeServiceClient =
                    new PrivateBlockchainFacadeServiceClientSettings {ServiceUrl = MockUrl},
                CustomerProfileServiceClient =
                    new CustomerProfileServiceClientSettings {ServiceUrl = MockUrl},
                WalletManagementServiceClient = new WalletManagementServiceClientSettings {ServiceUrl = MockUrl},
                ReferralServiceClient = new ReferralServiceClientSettings {ServiceUrl = MockUrl},
                MobileAppSettings = new MobileAppSettings {SettingsUrl = MockUrl},
                OperationsHistoryServiceClient = new OperationsHistoryServiceClientSettings {ServiceUrl = MockUrl},
                PushNotificationsServiceClient = new PushNotificationsServiceClientSettings {ServiceUrl = MockUrl},
                DictionariesServiceClient = new DictionariesServiceClientSettings {ServiceUrl = MockUrl},
                MaintenanceModeServiceClient = new MaintenanceModeServiceClientSettings {ServiceUrl = MockUrl},
                CampaignServiceClient = new CampaignServiceClientSettings {ServiceUrl = MockUrl},
                PartnersPaymentsServiceClient = new PartnersPaymentsServiceClientSettings {ServiceUrl = MockUrl},
                PartnerManagementServiceClient = new PartnerManagementServiceClientSettings {ServiceUrl = MockUrl},
                BonusEngineServiceClient = new BonusEngineServiceClientSettings {ServiceUrl = MockUrl},
                PartnersIntegrationServiceClient = new PartnersIntegrationServiceClientSettings { ServiceUrl = MockUrl },
                EligibilityEngineServiceClient = new EligibilityEngineServiceClientSettings{ServiceUrl = MockUrl},
                CurrencyConverterServiceClient = new CurrencyConvertorServiceClientSettings{ServiceUrl = MockUrl},
                StakingServiceClient = new StakingServiceClientSettings { ServiceUrl = MockUrl },
                CrossChainWalletLinkerServiceClient = new CrossChainWalletLinkerServiceClientSettings {ServiceUrl = MockUrl},
                CrossChainTransfersServiceClient = new CrossChainTransfersServiceClientSettings{ServiceUrl = MockUrl},
                EthereumBridgeServiceClient = new EthereumBridgeServiceClientSettings{ServiceUrl = MockUrl},
                CredentialsServiceClient = new CredentialsServiceClientSettings() { ServiceUrl = MockUrl},
                VouchersServiceClient = new VouchersServiceClientSettings { ServiceUrl = MockUrl},
                SmartVouchersServiceClient = new SmartVouchersServiceClientSettings { ServiceUrl = MockUrl},
            };

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(new ServiceModule(ConstantReloadingManager.From(settings)));
            containerBuilder.RegisterModule(new ClientsModule(ConstantReloadingManager.From(settings)));
            containerBuilder.RegisterModule(new AspNetCoreModule());
            _logFactory = LogFactory.Create().AddUnbufferedConsole();
            containerBuilder.RegisterInstance(_logFactory).As<ILogFactory>();
            var sc = new ServiceCollection();
            sc.AddHttpClient();
            containerBuilder.Populate(sc);

            //register your controller class here to test
            _container = containerBuilder.Build();
        }

        [Fact]
        public void Test_InstantiateControllers()
        {
            //Arrange
            var controllersToTest = _container.ComponentRegistry.Registrations
                .Where(r => typeof(Controller).IsAssignableFrom(r.Activator.LimitType))
                .Select(r => r.Activator.LimitType)
                .ToList();
            controllersToTest.ForEach(controller =>
            {
                //Act-Assert - ok if no exception
                _container.Resolve(controller);
            });
        }
    }
}
