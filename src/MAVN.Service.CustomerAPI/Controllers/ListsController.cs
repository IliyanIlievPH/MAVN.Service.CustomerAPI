using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Version;
using MAVN.Service.CustomerAPI.Models.Lists;
using Lykke.Service.Dictionaries.Client;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [Route("api/lists")]
    [LowerVersion(Devices = "android", LowerVersion = 659)]
    [LowerVersion(Devices = "IPhone,IPad", LowerVersion = 181)]
    public class ListsController : ControllerBase
    {
        private readonly IDictionariesClient _dictionariesClient;
        private readonly IMapper _mapper;

        public ListsController(IDictionariesClient dictionariesClient, IMapper mapper)
        {
            _dictionariesClient = dictionariesClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of countries of residence.
        /// </summary>
        /// <remarks>
        /// Used to get a collection of countries of residence.
        /// </remarks>
        /// <returns>
        /// 200 - a collection of countries of residence.
        /// </returns>
        [HttpGet("countriesOfResidence")]
        [ProducesResponseType(typeof(IReadOnlyList<CountryOfResidenceModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<CountryOfResidenceModel>> GetCountriesOfResidenceAsync()
        {
            var countriesOfResidence = await _dictionariesClient.Salesforce.GetCountriesOfResidenceAsync();

            return _mapper.Map<IReadOnlyList<CountryOfResidenceModel>>(countriesOfResidence);
        }

        /// <summary>
        /// Returns a collection of country dialing codes.
        /// </summary>
        /// <remarks>
        /// Used to get a collection of country dialing codes.
        /// </remarks>
        /// <returns>
        /// 200 - a collection of country dialing codes.
        /// </returns>
        [HttpGet("countryPhoneCodes")]
        [ProducesResponseType(typeof(IReadOnlyList<CountryPhoneCodeModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<CountryPhoneCodeModel>> GetCountryPhoneCodesAsync()
        {
            var countryPhoneCodes = await _dictionariesClient.Salesforce.GetCountryPhoneCodesAsync();

            return _mapper.Map<IReadOnlyList<CountryPhoneCodeModel>>(countryPhoneCodes);
        }
    }
}
