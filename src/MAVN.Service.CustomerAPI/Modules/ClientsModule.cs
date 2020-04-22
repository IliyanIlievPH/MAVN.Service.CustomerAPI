using Autofac;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.BonusEngine.Client;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Credentials.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CrossChainTransfers.Client;
using Lykke.Service.CrossChainWalletLinker.Client;
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
using Lykke.Service.Sessions.Client;
using Lykke.Service.Referral.Client;
using Lykke.Service.Staking.Client;
using Lykke.Service.Vouchers.Client.Extensions;
using Lykke.Service.WalletManagement.Client;
using Lykke.SettingsReader;

namespace MAVN.Service.CustomerAPI.Modules
{
    public class ClientsModule : Module
    {
        private readonly AppSettings _apiSettings;

        public ClientsModule(IReloadingManager<AppSettings> settings)
        {
            _apiSettings = settings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterBonusEngineClient(_apiSettings.BonusEngineServiceClient);

            builder.RegisterSessionsServiceClient(_apiSettings.SessionsServiceClient);

            builder.RegisterCustomerManagementClient(_apiSettings.CustomerManagementServiceClient, null);

            builder.RegisterCustomerProfileClient(_apiSettings.CustomerProfileServiceClient);

            builder.RegisterWalletManagementClient(_apiSettings.WalletManagementServiceClient, null);

            builder.RegisterReferralClient(_apiSettings.ReferralServiceClient, null);

            builder.RegisterOperationsHistoryClient(_apiSettings.OperationsHistoryServiceClient, null);

            builder.RegisterAgentManagementClient(_apiSettings.AgentManagementServiceClient);

            builder.RegisterPushNotificationsClient(_apiSettings.PushNotificationsServiceClient, null);

            builder.RegisterDictionariesClient(_apiSettings.DictionariesServiceClient);

            builder.RegisterPrivateBlockchainFacadeClient(_apiSettings.PrivateBlockchainFacadeServiceClient, null);

            builder.RegisterMaintenanceModeClient(_apiSettings.MaintenanceModeServiceClient, null);

            builder.RegisterCampaignClient(_apiSettings.CampaignServiceClient);

            builder.RegisterPartnersPaymentsClient(_apiSettings.PartnersPaymentsServiceClient, null);

            builder.RegisterPartnerManagementClient(_apiSettings.PartnerManagementServiceClient);

            builder.RegisterPartnersIntegrationClient(_apiSettings.PartnersIntegrationServiceClient, null);

            builder.RegisterEligibilityEngineClient(_apiSettings.EligibilityEngineServiceClient, null);

            builder.RegisterCurrencyConvertorClient(_apiSettings.CurrencyConverterServiceClient);

            builder.RegisterStakingClient(_apiSettings.StakingServiceClient, null);
            
            builder.RegisterCrossChainWalletLinkerClient(_apiSettings.CrossChainWalletLinkerServiceClient, null);
            
            builder.RegisterCrossChainTransfersClient(_apiSettings.CrossChainTransfersServiceClient, null);

            builder.RegisterEthereumBridgeClient(_apiSettings.EthereumBridgeServiceClient);

            builder.RegisterEmaarPropertyIntegrationClient(_apiSettings.EmaarPropertyIntegrationServiceClient, null);

            builder.RegisterPaymentTransfersClient(_apiSettings.PaymentTransfersServiceClient, null);

            builder.RegisterCredentialsClient(_apiSettings.CredentialsServiceClient);

            builder.RegisterVouchersClient(_apiSettings.VouchersServiceClient);
        }
    }
}
