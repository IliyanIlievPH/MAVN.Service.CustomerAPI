using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.Middleware.Authentication;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Models;
using MAVN.Service.CustomerAPI.Models.History;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/history")]
    public class HistoryController : ControllerBase
    {
        private readonly IOperationsHistoryService _operationsHistoryService;
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;

        public HistoryController(
            IOperationsHistoryService operationsHistoryService,
            IRequestContext requestContext,
            IMapper mapper)
        {
            _operationsHistoryService = operationsHistoryService;
            _requestContext = requestContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Get operations history for the authorized user
        /// </summary>
        /// <param name="request">The request details</param>
        /// <returns></returns>
        [HttpGet("operations")]
        [SwaggerOperation("GetCustomerOperationsHistory")]
        [ProducesResponseType(typeof(PaginatedOperationsHistoryResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<PaginatedOperationsHistoryResponseModel> GetCustomerOperationsHistoryAsync([FromQuery] PaginationRequestModel request)
        {
            var result = await _operationsHistoryService.GetAsync(
                _requestContext.UserId, 
                request.CurrentPage,
                request.PageSize);

            return _mapper.Map<PaginatedOperationsHistoryResponseModel>(result);
        }
    }
}
