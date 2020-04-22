using System;

namespace MAVN.Service.CustomerAPI.Core.Domain.CommonReferral
{
    public class CommonReferralModel
    {
        public ReferralType ReferralType { get; set; }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime TimeStamp { get; set; }

        public CommonReferralStatus Status { get; set; }

        public Guid? CampaignId { get; set; }

        public string PartnerId { get; set; }

        public bool IsApproximate { get; set; }
    }
}
