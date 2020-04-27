using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.Dictionaries.Client;
using MAVN.Service.CustomerAPI.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/commonInformation")]
    [ApiController]
    public class CommonInformationController : ControllerBase
    {
        private readonly IDictionariesClient _dictionariesClient;
        private readonly IMapper _mapper;

        public CommonInformationController(IDictionariesClient dictionariesClient, IMapper mapper)
        {
            _dictionariesClient = dictionariesClient ??
                throw new ArgumentNullException(nameof(dictionariesClient));
            _mapper = mapper;
        }

        /// <summary>
        /// Returns common links information 
        /// </summary>
        /// <returns>
        /// 200 - Common Information Response
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(CommonInformationResponse), (int)HttpStatusCode.OK)]
        public async Task<CommonInformationResponse> GetCommonInformationAsync()
        {
            var result = await _dictionariesClient.CommonInformation.GetCommonInformationAsync();

            return _mapper.Map<CommonInformationResponse>(result);
        }
    }
}
