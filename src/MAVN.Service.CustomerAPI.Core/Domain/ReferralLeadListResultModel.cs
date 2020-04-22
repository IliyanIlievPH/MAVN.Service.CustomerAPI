using System.Collections.Generic;
using Lykke.Common.ApiLibrary.Contract;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class ReferralLeadListResultModel
    {
        public IReadOnlyCollection<ReferralLeadModel> ReferralLeads { get; set; }

        public ILykkeApiErrorCode ErrorCode { get; set; }
    }
}
