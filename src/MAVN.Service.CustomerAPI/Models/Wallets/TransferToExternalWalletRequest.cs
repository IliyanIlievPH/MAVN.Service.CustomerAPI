using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Wallets
{
    [PublicAPI]
    public class TransferToExternalWalletRequest
    {
        [Required]
        public string Amount { get; set; }
    }
}
