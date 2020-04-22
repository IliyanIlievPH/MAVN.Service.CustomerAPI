using System;
using Falcon.Numerics;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class TransferInfoModel
    {
        public bool IsSender { get; set; }

        public string AssetSymbol { get; set; }

        public Money18 Amount { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
