using System;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    public class SmartVoucherCampaignContentResponseModel
    {
        /// <summary>Represents campaign's content id</summary>
        public Guid Id { get; set; }

        /// <summary>Represents the type of the content</summary>
        public SmartVoucherCampaignContentType ContentType { get; set; }

        /// <summary>Represents content's language</summary>
        public Localization Localization { get; set; }

        /// <summary>Represents content's value</summary>
        public string Value { get; set; }

        /// <summary>Represents content's image</summary>
        public FileResponseModel Image { get; set; }
    }
}
