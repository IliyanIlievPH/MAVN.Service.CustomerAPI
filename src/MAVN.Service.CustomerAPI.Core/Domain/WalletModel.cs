using MAVN.Numerics;
using MAVN.Service.CustomerAPI.Core.Constants;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class WalletModel
    {
        public WalletsErrorCodes Error { get; set; }

        public Money18? Balance { get; set; }

        public Money18? StakedBalance { get; set; }

        public string AssetSymbol { get; set; }

        public bool IsWalletBlocked { get; set; }
        
        public string Address { get; set; }
    }
}
