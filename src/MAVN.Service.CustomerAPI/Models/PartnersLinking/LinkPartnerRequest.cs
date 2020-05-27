using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.CustomerAPI.Models.PartnersLinking
{
    /// <summary>Request model used to link partner to a customer</summary>
    public class LinkPartnerRequest
    {
        /// <summary>Code of the partner</summary>
        [Required]
        public string PartnerCode { get; set; }

        /// <summary>Linking code of the partner</summary>
        [Required]
        public string PartnerLinkingCode { get; set; }
    }
}
