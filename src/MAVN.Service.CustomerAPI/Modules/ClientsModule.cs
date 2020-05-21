using Autofac;
using MAVN.Service.BonusEngine.Client;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Credentials.Client;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CrossChainTransfers.Client;
using MAVN.Service.CrossChainWalletLinker.Client;
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
using MAVN.Service.Sessions.Client;
using MAVN.Service.Referral.Client;
using MAVN.Service.Staking.Client;
using MAVN.Service.Vouchers.Client.Extensions;
using MAVN.Service.WalletManagement.Client;
using Lykke.SettingsReader;
using MAVN.Service.PaymentManagement.Client;
using MAVN.Service.SmartVouchers.Client;

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

            builder.RegisterCredentialsClient(_apiSettings.CredentialsServiceClient);

            builder.RegisterVouchersClient(_apiSettings.VouchersServiceClient);

            builder.RegisterSmartVouchersClient(_apiSettings.SmartVouchersServiceClient, null);

            builder.RegisterPaymentManagementClient(_apiSettings.PaymentManagementServiceClient, null);
        }
    }
}
