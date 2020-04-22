using JetBrains.Annotations;

namespace MAVN.Service.CustomerAPI.Models.Lists
{
    /// <summary>
    /// Represents a country of residents. 
    /// </summary>
    [PublicAPI]
    public class CountryOfResidenceModel
    {
        /// <summary>
        /// The country unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The country localized name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The country ISO 3166-1 alpha-2 code.
        /// </summary>
        public string CountryIso2Code { get; set; }

        /// <summary>
        /// The country ISO 3166-1 alpha-3 code.
        /// </summary>
        public string CountryIso3Code { get; set; }
    }
}
