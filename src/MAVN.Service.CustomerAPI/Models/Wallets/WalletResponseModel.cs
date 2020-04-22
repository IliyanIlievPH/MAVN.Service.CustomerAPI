using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Wallets
{
    [PublicAPI]
    public class WalletResponseModel
    {
        public string Balance { get; set; }

        public string ExternalBalance { get; set; }

        public string AssetSymbol { get; set; }

        public bool IsWalletBlocked { get; set; }

        public string TotalEarned { get; set; }

        public string TotalSpent { get; set; }

        public string StakedBalance { get; set; }
        
        public string PrivateWalletAddress { get; set; }
        
        public string PublicWalletAddress { get; set; }
        
        public PublicAddressLinkingStatus PublicAddressLinkingStatus { get; set; }

        public string TransitAccountAddress { get; set; }
    }
}
