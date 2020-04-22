using System;
using MAVN.Service.CustomerAPI.Models.Enums;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    public class ReferralPaginationRequestModel : PaginationRequestModel
    {
        public CommonReferralStatus Status { get; set; }

        public Guid? EarnRuleId { get; set; }
    }
}
