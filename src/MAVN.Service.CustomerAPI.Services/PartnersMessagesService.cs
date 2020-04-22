using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.PartnersIntegration.Client;

namespace MAVN.Service.CustomerAPI.Services
{
    public class PartnersMessagesService : IPartnersMessagesService
    {
        private readonly IMapper _mapper;
        private readonly IPartnersIntegrationClient _partnersIntegrationClient;
        private readonly IPartnerManagementClient _partnerManagementClient;

        public PartnersMessagesService(IMapper mapper, IPartnersIntegrationClient partnersIntegrationClient,
            IPartnerManagementClient partnerManagementClient)
        {
            _mapper = mapper;
            _partnersIntegrationClient = partnersIntegrationClient;
            _partnerManagementClient = partnerManagementClient;
        }

        public async Task<PartnerMessage> GetPartnerMessageAsync(string partnerMessageId)
        {
            var result = await _partnersIntegrationClient.MessagesApi.GetMessageAsync(partnerMessageId);

            var response = _mapper.Map<PartnerMessage>(result);

            response.PartnerMessageId = partnerMessageId;

            var partnerInfo = await _partnerManagementClient.Partners.GetByIdAsync(Guid.Parse(response.PartnerId));
            
            response.PartnerName = partnerInfo.Name;

            var location = partnerInfo.Locations.FirstOrDefault(x => x.ExternalId == result.ExternalLocationId);

            response.LocationId = location?.Id.ToString();
            response.LocationName = location?.Name;

            return response;
        }
    }
}
