using System;
using System.Collections.Generic;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.BonusEngine.Client;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Credentials.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CrossChainTransfers.Client;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.Dictionaries.Client;
using Lykke.Service.EligibilityEngine.Client;
using Lykke.Service.EthereumBridge.Client;
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
using Lykke.SettingsReader.Attributes;
using MAVN.Service.SmartVouchers.Client;

namespace MAVN.Service.CustomerAPI.Settings
{
    public class AppSettings
    {
        public BaseSettings CustomerApiService { get; set; }
        public SessionsServiceClientSettings SessionsServiceClient { get; set; }
        public CustomerManagementServiceClientSettings CustomerManagementServiceClient { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }
        public WalletManagementServiceClientSettings WalletManagementServiceClient { get; set; }
        public ReferralServiceClientSettings ReferralServiceClient { get; set; }
        public MobileAppSettings MobileAppSettings { get; set; }
        public OperationsHistoryServiceClientSettings OperationsHistoryServiceClient { get; set; }
        public AgentManagementServiceClientSettings AgentManagementServiceClient { get; set; }
        public PushNotificationsServiceClientSettings PushNotificationsServiceClient { get; set; }
        public DictionariesServiceClientSettings DictionariesServiceClient { get; set; }
        public PrivateBlockchainFacadeServiceClientSettings PrivateBlockchainFacadeServiceClient { get; set; }
        public MaintenanceModeServiceClientSettings MaintenanceModeServiceClient { get; set; }
        public CampaignServiceClientSettings CampaignServiceClient { get; set; }
        public PartnersPaymentsServiceClientSettings PartnersPaymentsServiceClient { get; set; }
        public BonusEngineServiceClientSettings BonusEngineServiceClient { get; set; }
        public PartnerManagementServiceClientSettings PartnerManagementServiceClient { get; set; }
        public PartnersIntegrationServiceClientSettings PartnersIntegrationServiceClient { get; set; }
        public EligibilityEngineServiceClientSettings EligibilityEngineServiceClient { get; set; }
        public CurrencyConvertorServiceClientSettings CurrencyConverterServiceClient { get; set; }
        public StakingServiceClientSettings StakingServiceClient { get; set; }
        public CrossChainWalletLinkerServiceClientSettings CrossChainWalletLinkerServiceClient { get; set; }
        public CrossChainTransfersServiceClientSettings CrossChainTransfersServiceClient { get; set; }
        public EthereumBridgeServiceClientSettings EthereumBridgeServiceClient { get; set; }
        public PaymentTransfersServiceClientSettings PaymentTransfersServiceClient { get; set; }
        public CredentialsServiceClientSettings CredentialsServiceClient { get; set; }
        public VouchersServiceClientSettings VouchersServiceClient { get; set; }
        public SmartVouchersServiceClientSettings SmartVouchersServiceClient { get; set; }
    }

    public class MonitoringServiceClientSettings    
    {
        public string MonitoringServiceUrl { get; set; }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }

    public class BaseSettings
    {
        public DbSettings Db { get; set; }
        public CacheSettings CacheSettings { get; set; }
        public PasswordValidationSettings PasswordValidationSettings { get; set; }
        public Constants Constants { get; set; }
        public GoogleAuthSettings GoogleAuthSettings { get; set; }
        public List<ThrottlingSettings> RequestLimitsByCustomer { get; set; }
        public int NumberDecimalPlaces { get; set; }
        public string BaseCurrencyCode { get; set; }
        public FailedSigninAttemptsThrottlingSettings FailedLoginAttemptsThrottling { get; set; }
        public string TransitAccountAddress { get; set; }
        [Optional]
        public bool IsPublicBlockchainFeatureDisabled { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
    }

    public class CacheSettings
    {
        public string DataCacheInstance { get; set; }
        public string RedisConfiguration { get; set; }
    }

    public class MobileAppSettings
    {
        public string SettingsUrl { get; set; }
    }

    public class PasswordValidationSettings
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public int MinUpperCase { get; set; }
        public int MinLowerCase { get; set; }
        public int MinSpecialSymbols { get; set; }
        public int MinNumbers { get; set; }
        public string AllowedSpecialSymbols { get; set; }
        public bool AllowWhiteSpaces { get; set; }
    }

    public class Constants
    {
        public string TokenSymbol { get; set; }

        public string UsdAssetName => "USD";
    }

    public class GoogleAuthSettings
    {
        public string GoogleApiUrl { get; set; }
    }

    public class ThrottlingSettings
    {
        public string Verb { get; set; }
        
        public string Route { get; set; }
        
        public decimal FrequencyInSeconds { get; set; }
    }

    public class FailedSigninAttemptsThrottlingSettings
    {
        public int WarningThreshold { get; set; }
        
        public int LockThreshold { get; set; }
        
        public TimeSpan ThresholdPeriod { get; set; }
        
        public TimeSpan AccountLockPeriod { get; set; }
    }
}

