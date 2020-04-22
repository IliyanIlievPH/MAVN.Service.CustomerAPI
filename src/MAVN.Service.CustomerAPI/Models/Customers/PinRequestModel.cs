using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.CustomerAPI.Models.Customers
{
    /// <summary>
    /// Request model
    /// </summary>
    public class PinRequestModel
    {
        /// <summary>
        /// PIN code
        /// </summary>
        [Required]
        public string Pin { get; set; }
    }
}
