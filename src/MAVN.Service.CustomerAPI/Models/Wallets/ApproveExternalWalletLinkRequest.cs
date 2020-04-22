using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Wallets
{
    [PublicAPI]
    public class ApproveExternalWalletLinkRequest
    {
        [Required]
        public string PrivateAddress { get; set; }
        
        [Required]
        public string PublicAddress { get; set; }
        
        [Required]
        public string Signature { get; set; }
    }
}
