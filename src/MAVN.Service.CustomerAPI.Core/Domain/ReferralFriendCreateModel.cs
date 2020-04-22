using System;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class ReferralFriendCreateModel
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public Guid CampaignId { get; set; }
    }
}
