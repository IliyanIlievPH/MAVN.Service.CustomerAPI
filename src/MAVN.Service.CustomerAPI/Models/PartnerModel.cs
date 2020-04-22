using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models
{
    /// <summary>
    /// Represents a partner.
    /// </summary>
    [PublicAPI]
    public class PartnerModel
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The partner name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The partner locations.
        /// </summary>
        public IReadOnlyCollection<LocationModel> Locations { get; set; }
    }
}
