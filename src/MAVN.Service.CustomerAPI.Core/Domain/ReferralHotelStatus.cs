namespace MAVN.Service.CustomerAPI.Core.Domain
{
    //
    // Summary:
    //     Represents state of the hotel referral
    public enum ReferralHotelStatus
    {
        /// <summary>
        /// Referral has been created
        /// </summary>       
        Pending,
        /// <summary>
        ///  Referral has been confirmed
        /// </summary>
        Confirmed,
        /// <summary>
        /// Referral has been used
        /// </summary>
        Used,
        /// <summary>
        /// Referral has expired
        /// </summary>
        Expired
    }
}
