using MAVN.Service.CustomerAPI.Core.Constants;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class TransferResultModel
    {
        /// <summary>
        /// Transfer transaction Id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public TransferErrorCodes ErrorCode { get; set; }
    }
}
