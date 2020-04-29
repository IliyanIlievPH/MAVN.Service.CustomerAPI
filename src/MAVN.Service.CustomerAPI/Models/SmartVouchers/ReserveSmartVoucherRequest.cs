using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// Request model to reserve a smart voucher
    /// </summary>
    [PublicAPI]
    public class ReserveSmartVoucherRequest
    {
        /// <summary>
        /// The id of the smart voucher campaign
        /// </summary>
        [Required]
        public Guid SmartVoucherCampaignId { get; set; }
    }
}
