using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Agents
{
    /// <summary>
    /// Represents a KYA form data.
    /// </summary>
    [PublicAPI]
    public class AgentRegistrationRequestModel
    {
        /// <summary>
        /// The customer first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The customer last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The agent country of residence identifier.
        /// </summary>
        public int CountryOfResidenceId { get; set; }

        /// <summary>
        /// The note to Emaar.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// A collection of agent photos required for KYA.
        /// </summary>
        public IReadOnlyList<ImageModel> Images { get; set; }
    }
}
