using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Falcon.Common.Middleware.Authentication;
using Falcon.Common.Middleware.Version;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.AgentManagement.Client.Models;
using Lykke.Service.AgentManagement.Client.Models.Agents;
using MAVN.Service.CustomerAPI.Core;
using MAVN.Service.CustomerAPI.Core.Constants;
using MAVN.Service.CustomerAPI.Models.Agents;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.CustomerAPI.Controllers
{
    [ApiController]
    [LykkeAuthorize]
    [Route("api/agents")]
    [LowerVersion(Devices = "android", LowerVersion = 659)]
    [LowerVersion(Devices = "IPhone,IPad", LowerVersion = 181)]
    public class AgentsController : ControllerBase
    {
        private readonly IRequestContext _requestContext;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IAgentManagementClient _agentManagementClient;

        public AgentsController(
            IRequestContext requestContext,
            ICustomerProfileClient customerProfileClient,
            IAgentManagementClient agentManagementClient)
        {
            _requestContext = requestContext;
            _customerProfileClient = customerProfileClient;
            _agentManagementClient = agentManagementClient;
        }

        /// <summary>
        /// Returns agent general information.
        /// </summary>
        /// <remarks>
        /// Used to get agent status of current customer.
        /// 
        /// Error codes:
        /// - **CustomerProfileDoesNotExist**
        /// </remarks>
        /// <returns>
        /// 200 - agent general information.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(AgentResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<AgentResponseModel> GetAgentAsync()
        {
            string customerId = _requestContext.UserId;

            var customerProfileResponse = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(customerId);

            switch (customerProfileResponse.ErrorCode)
            {
                case CustomerProfileErrorCodes.CustomerProfileDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerProfileDoesNotExist);
            }

            var customerProfile = customerProfileResponse.Profile;

            var agentTask = _agentManagementClient.Agents.GetByCustomerIdAsync(Guid.Parse(customerId));
            var customerRequirementsTask =
                _agentManagementClient.Requirements.GetByCustomerIdAsync(Guid.Parse(customerId));
            var numberOfTokensTask = _agentManagementClient.Requirements.GetTokensRequirementsAsync();

            await Task.WhenAll(agentTask, customerRequirementsTask, numberOfTokensTask);

            var agent = agentTask.Result;
            var customerRequirements = customerRequirementsTask.Result;
            var numberOfTokens = numberOfTokensTask.Result;

            return new AgentResponseModel
            {
                Email = customerProfile.Email,
                FirstName = customerProfile.FirstName,
                LastName = customerProfile.LastName,
                CountryPhoneCodeId = customerProfile.CountryPhoneCodeId ?? 0,
                PhoneNumber = customerProfile.ShortPhoneNumber,
                Status = agent != null
                    ? Enum.Parse<Models.Agents.AgentStatus>(agent.Status.ToString())
                    : Models.Agents.AgentStatus.NotAgent,
                IsEligible = customerRequirements.IsEligible,
                HasEnoughTokens = customerRequirements.HasEnoughTokens,
                HasVerifiedEmail = customerRequirements.HasVerifiedEmail,
                RequiredNumberOfTokens = numberOfTokens.RequiredNumberOfTokens.ToDisplayString()
            };
        }

        /// <summary>
        /// Registers an agent.
        /// </summary>
        /// <remarks>
        /// Used to register current customer as an agent.
        /// 
        /// Error codes:
        /// - **AgentAlreadyApproved**
        /// - **SfAccountAlreadyExisting**
        /// - **EmailNotVerified**
        /// - **NotEnoughTokens**
        /// - **CustomerProfileDoesNotExist**
        /// - **CountryPhoneCodeDoesNotExist**
        /// - **CountryOfResidenceDoesNotExist**
        /// - **ImageUploadError**
        /// - **ConnectorRegistrationError**
        /// </remarks>
        /// <param name="model">The KYA form data.</param>
        /// <returns>
        /// 204 - registration process successfully started.
        /// 400 - an error occurred while validating KYA data.
        /// </returns>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task RegisterAsync([FromBody] AgentRegistrationRequestModel model)
        {
            var result = await _agentManagementClient.Agents.RegisterAsync(new RegistrationModel
            {
                CustomerId = Guid.Parse(_requestContext.UserId),
                FirstName = model.FirstName,
                LastName = model.LastName,
                CountryOfResidenceId = model.CountryOfResidenceId,
                Note = model.Note,
                Images = model.Images.Select(o => new Lykke.Service.AgentManagement.Client.Models.Agents.ImageModel
                {
                    DocumentType =
                        Enum.Parse<Lykke.Service.AgentManagement.Client.Models.Agents.DocumentType>(
                            o.DocumentType.ToString()),
                    Name = o.Name,
                    Content = o.Content
                }).ToList()
            });

            switch (result.ErrorCode)
            {
                case AgentManagementErrorCode.None:
                    // Agent successfully registered
                    break;
                case AgentManagementErrorCode.AgentAlreadyApproved:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AgentAlreadyApproved);
                case AgentManagementErrorCode.AccountAlreadyExists:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.SfAccountAlreadyExisting);
                case AgentManagementErrorCode.EmailNotVerified:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.EmailNotVerified);
                case AgentManagementErrorCode.NotEnoughTokens:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NotEnoughTokens);
                case AgentManagementErrorCode.CustomerProfileDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerProfileDoesNotExist);
                case AgentManagementErrorCode.CountryPhoneCodeDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CountryPhoneCodeDoesNotExist);
                case AgentManagementErrorCode.CountryOfResidenceDoesNotExist:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CountryOfResidenceDoesNotExist);
                case AgentManagementErrorCode.ImageUploadFail:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ImageUploadError);
                case AgentManagementErrorCode.AccountRegistrationFail:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.ConnectorRegistrationError);
                default:
                    throw new InvalidOperationException
                        ($"Unexpected error during agent registration {_requestContext.UserId} - {result.ErrorCode}");
            }
        }
    }
}
