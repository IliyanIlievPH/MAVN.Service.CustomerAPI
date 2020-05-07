using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.SmartVouchers
{
    /// <summary>
    /// GeolocationModel
    /// </summary>
    [PublicAPI]
    public class GeolocationModel
    {
        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }
    }
}
