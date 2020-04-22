using System;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class PartnerMessage
    {
        public string PartnerMessageId { get; set; }

        public string PartnerId { get; set; }

        public string PartnerName { get; set; }

        public string LocationId { get; set; }

        public string LocationName { get; set; }

        public string CustomerId { get; set; }

        public DateTime Timestamp { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}
