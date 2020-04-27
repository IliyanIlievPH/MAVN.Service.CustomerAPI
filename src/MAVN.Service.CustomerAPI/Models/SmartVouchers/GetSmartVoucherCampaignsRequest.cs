namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    public class GetSmartVoucherCampaignsRequest : PaginationRequestModel
    {
        /// <summary>
        /// Represents search field by campaign's name
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// Only active campaigns flag
        /// </summary>
        public bool OnlyActive { get; set; }
    }
}
