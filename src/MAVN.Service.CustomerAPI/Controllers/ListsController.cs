using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MAVN.Common.Middleware.Authentication;
using MAVN.Common.Middleware.Version;
using MAVN.Service.Dictionaries.Client;
using MAVN.Service.CustomerAPI.Models.Lists;
using MAVN.Service.PartnerManagement.Client;
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
        private readonly IPartnerManagementClient _partnerManagementClient;
        private readonly IMapper _mapper;

        public ListsController(IDictionariesClient dictionariesClient, IPartnerManagementClient partnerManagementClient, IMapper mapper)
        {
            _dictionariesClient = dictionariesClient;
            _partnerManagementClient = partnerManagementClient;
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
        [ProducesResponseType(typeof(IReadOnlyList<CountryOfResidenceModel>), (int)HttpStatusCode.OK)]
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
        [ProducesResponseType(typeof(IReadOnlyList<CountryPhoneCodeModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<CountryPhoneCodeModel>> GetCountryPhoneCodesAsync()
        {
            var countryPhoneCodes = await _dictionariesClient.Salesforce.GetCountryPhoneCodesAsync();

            return _mapper.Map<IReadOnlyList<CountryPhoneCodeModel>>(countryPhoneCodes);
        }

        /// <summary>
        /// Returns a collection of country info
        /// </summary>
        /// <remarks>
        /// Used to get a collection of country info
        /// </remarks>
        /// <returns>
        /// 200 - a collection of country info.
        /// </returns>
        [LykkeAuthorize]
        [HttpGet("partnersCountries")]
        [ProducesResponseType(typeof(IReadOnlyList<CountryInfoModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyList<CountryInfoModel>> GetPartnersCountriesAsync()
        {
            var iso3CodesResponse = await _partnerManagementClient.Locations.GetCountryIso3CodeForAllLocations();

            var result = iso3CodesResponse.CountriesIso3Codes
                .Select(iso3Code =>
                    new CountryInfoModel
                    {
                        Iso3Code = iso3Code,
                        Name = CountryManager.GetCountryNameByIso3(iso3Code),
                    })
                .ToList();

            return result;
        }
    }
}
