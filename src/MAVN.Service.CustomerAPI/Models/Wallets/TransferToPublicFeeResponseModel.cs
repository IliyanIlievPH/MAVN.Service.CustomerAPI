using MAVN.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Wallets
{
    [PublicAPI]
    public class TransferToPublicFeeResponseModel
    {
        /// <summary>
        /// Fee for public transfers
        /// </summary>
        public Money18 Fee { get; set; }
    }
}
