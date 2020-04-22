using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using MAVN.Service.CustomerAPI.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [Route("api/mobile")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly IMobileSettingsReader _settingsReader;
        private readonly ILog _log;

        public MobileController(IMobileSettingsReader settingsReader, ILogFactory logFactory)
        {
            _settingsReader = settingsReader;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Returns mobile application settings
        /// </summary>
        [HttpGet]
        [Route("settings")]
        [SwaggerOperation("GetMobileSettings")]
        [ProducesResponseType(typeof(JObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetSettings()
        {
            try
            {
                var result =  await _settingsReader.ReadJsonAsync();
                return Ok(result);
            }
            catch (JsonReaderException e)
            {
                const string errorMessage = "Settings content is not a valid json";

                _log.Error(e, errorMessage);

                return StatusCode((int)HttpStatusCode.InternalServerError, ErrorResponse.Create(errorMessage));
            }
        }
    }
}
