namespace MAVN.Service.CustomerAPI.Core.Domain
{
    /// <summary>
    /// Specifies status of referral lead. 
    /// </summary>
    public enum ReferralLeadStatus
    {
        /// <summary>
        /// Indicates that the referral lead waiting for approval. 
        /// </summary>
        Sent,
        
        /// <summary>
        /// Indicates that the referral lead confirmed.
        /// </summary>
        Accepted,
        
        /// <summary>
        /// Indicates that the referral lead approved.
        /// </summary>
        Approved,
        
        /// <summary>
        /// Indicates that the referral lead rejected.
        /// </summary>
        Rejected
    }
}
