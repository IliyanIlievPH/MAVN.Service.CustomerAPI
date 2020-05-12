using MAVN.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Operations
{
    [PublicAPI]
    public class TransferOperationRequest
    {
        public string ReceiverEmail { get; set; }

        public Money18 Amount { get; set; }
    }
}
