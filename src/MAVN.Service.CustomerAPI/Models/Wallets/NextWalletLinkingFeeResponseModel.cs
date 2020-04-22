using Falcon.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Wallets
{
    [PublicAPI]
    public class NextWalletLinkingFeeResponseModel
    {
        public Money18 Fee { get; set; }
    }
}
