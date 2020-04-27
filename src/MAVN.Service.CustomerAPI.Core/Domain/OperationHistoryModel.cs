using System;
using Falcon.Numerics;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class OperationHistoryModel
    {
        public HistoryOperationType Type { get; set; }

        public DateTime Timestamp { get; set; }

        public Money18 Amount { get; set; }

        public string ActionRule { get; set; }

        public string OtherSideCustomerEmail { get; set; }

        public string OtherSideCustomerName { get; set; }

        public string PartnerName { get; set; }

        public string InstalmentName { get; set; }
    }
}
