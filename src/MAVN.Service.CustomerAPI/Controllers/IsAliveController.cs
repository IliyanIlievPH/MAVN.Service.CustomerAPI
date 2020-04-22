using System.Collections.Generic;
using System.Net;
using Lykke.Common;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    [Route("api/[controller]")]
    [ApiController]
    public class IsAliveController : Controller
    {
        /// <summary>
        /// Checks service is alive
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation("IsAlive")]
        [ProducesResponseType(typeof(IsAliveResponse), (int)HttpStatusCode.OK)]
        public IActionResult Get()
        {
            return Ok(new IsAliveResponse
            {
                Name = AppEnvironment.Name,
                Version = AppEnvironment.Version,
                Env = AppEnvironment.EnvInfo,
                IssueIndicators = new List<IsAliveResponse.IssueIndicator>(0),
#if DEBUG
                IsDebug = true,
#else
                IsDebug = false,
#endif
            });
        }
    }
}
