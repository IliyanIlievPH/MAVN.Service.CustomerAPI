using System;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Referral
{
    [PublicAPI]
    public class ReferralFriendRequestModel
    {
        /// <summary>
        /// The full name of the referred friend.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The email of the referred lead.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Campaign's id
        /// </summary>
        public Guid CampaignId { get; set; }
    }
}
