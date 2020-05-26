namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    public class GetSmartVoucherCampaignsRequest : PaginationRequestModel
    {
        /// <summary>
        /// Represents search field by campaign's name
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// Represents search field by Latitude
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Represents search field by Longitude
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Represents search field by Location which is in some radius
        /// </summary>
        public double? RadiusInKm { get; set; }

        /// <summary>
        /// Represents search field by Iso3 code of the country
        /// </summary>
        public string CountryIso3Code { get; set; }
    }
}
