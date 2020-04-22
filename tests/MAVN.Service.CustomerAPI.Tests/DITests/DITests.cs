using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.BonusEngine.Client;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Credentials.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CrossChainTransfers.Client;
using Lykke.Service.CrossChainWalletLinker.Client;
using MAVN.Service.CustomerAPI.Modules;
using MAVN.Service.CustomerAPI.Settings;
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.Dictionaries.Client;
using Lykke.Service.EligibilityEngine.Client;
using Lykke.Service.EthereumBridge.Client;
using Lykke.Service.EmaarPropertyIntegration.Client;
using Lykke.Service.MaintenanceMode.Client;
using Lykke.Service.OperationsHistory.Client;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.PartnersIntegration.Client;
using Lykke.Service.PartnersPayments.Client;
using Lykke.Service.PaymentTransfers.Client;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.PushNotifications.Client;
using Lykke.Service.Referral.Client;
using Lykke.Service.Sessions.Client;
using Lykke.Service.Staking.Client;
using Lykke.Service.Vouchers.Client;
using Lykke.Service.WalletManagement.Client;
using Lykke.SettingsReader.ReloadingManager;
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
                        FalconConstants = new FalconConstants {EmaarToken = "Emaar"},
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
                AgentManagementServiceClient = new AgentManagementServiceClientSettings {ServiceUrl = MockUrl},
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
                PaymentTransfersServiceClient = new PaymentTransfersServiceClientSettings { ServiceUrl = MockUrl},
                EmaarPropertyIntegrationServiceClient = new EmaarPropertyIntegrationServiceClientSettings() { ServiceUrl = MockUrl},
                CredentialsServiceClient = new CredentialsServiceClientSettings() { ServiceUrl = MockUrl},
                VouchersServiceClient = new VouchersServiceClientSettings { ServiceUrl = MockUrl}
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
