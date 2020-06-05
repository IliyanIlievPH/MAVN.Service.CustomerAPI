using System;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    public class SmartVoucherCampaignDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal VoucherPrice { get; set; }
        public Guid PartnerId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Currency { get; set; }
    }
}
