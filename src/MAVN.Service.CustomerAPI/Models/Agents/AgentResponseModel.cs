using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.CustomerAPI.Models.Agents
{
    /// <summary>
    /// Represents an agent general information.
    /// </summary>
    [PublicAPI]
    public class AgentResponseModel
    {
        /// <summary>
        /// The customer email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The customer first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The customer last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The country phone code identifier.
        /// </summary>
        public int CountryPhoneCodeId { get; set; }

        /// <summary>
        /// The customer phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Indicates the agent status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AgentStatus Status { get; set; }

        /// <summary>
        /// If <c>true</c> all requirements completed and the customer could become an agent, otherwise <c>false</c>.
        /// </summary>
        public bool IsEligible { get; set; }

        /// <summary>
        /// Indicated that customer has enough tokens to become agent. 
        /// </summary>
        public bool HasEnoughTokens { get; set; }

        /// <summary>
        /// Indicated that customer email verified. 
        /// </summary>
        public bool HasVerifiedEmail { get; set; }

        /// <summary>
        /// The number of tokens that required to become an agent.
        /// </summary>
        public string RequiredNumberOfTokens { get; set; }
    }
}
