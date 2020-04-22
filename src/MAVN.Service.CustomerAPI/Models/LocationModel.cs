using System;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models
{
    /// <summary>
    /// Represents a partner's location.
    /// </summary>
    [PublicAPI]
    public class LocationModel
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The locations name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The location address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The location creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}