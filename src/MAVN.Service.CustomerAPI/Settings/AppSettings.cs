using System;
using System.Collections.Generic;
using MAVN.Service.BonusEngine.Client;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Credentials.Client;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CrossChainTransfers.Client;
using MAVN.Service.CrossChainWalletLinker.Client;
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
using Lykke.SettingsReader.Attributes;
using MAVN.Service.PaymentManagement.Client;
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
        public CredentialsServiceClientSettings CredentialsServiceClient { get; set; }
        public VouchersServiceClientSettings VouchersServiceClient { get; set; }
        public SmartVouchersServiceClientSettings SmartVouchersServiceClient { get; set; }
        public PaymentManagementServiceClientSettings PaymentManagementServiceClient { get; set; }
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

