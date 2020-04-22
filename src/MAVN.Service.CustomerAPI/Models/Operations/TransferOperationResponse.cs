using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Operations
{
    [PublicAPI]
    public class TransferOperationResponse
    {
        public string TransactionId { get; set; }
    }
}
