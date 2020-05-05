using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Infrastructure.AutoMapperProfiles;
using MAVN.Service.CustomerAPI.Services;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using MAVN.Service.PartnersIntegration.Client;
using MAVN.Service.PartnersIntegration.Client.Models;
using Moq;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Services
{
    public class PartnersMessagesServiceTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IPartnersIntegrationClient> _partnersIntegrationClientMock =
            new Mock<IPartnersIntegrationClient>();

        private readonly Mock<IPartnerManagementClient> _partnerManagementClientMock =
            new Mock<IPartnerManagementClient>();

        private readonly IPartnersMessagesService _service;

        public PartnersMessagesServiceTests()
        {
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });

            var mapper = mockMapper.CreateMapper();

            _service = new PartnersMessagesService(mapper, _partnersIntegrationClientMock.Object,
                _partnerManagementClientMock.Object);
        }

        [Fact]
        public async Task
            When_Get_Partner_Message_Async_Is_Executed_Then_Partner_Management_And_Partners_Integration_Clients_Are_Called()
        {
            _partnersIntegrationClientMock.Setup(x => x.MessagesApi.GetMessageAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<MessageGetResponseModel>()
                    .With(x => x.PartnerId, Guid.NewGuid().ToString()).Create()));

            _partnerManagementClientMock.Setup(x => x.Partners.GetByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(_fixture.Create<PartnerDetailsModel>()));

            await _service.GetPartnerMessageAsync(It.IsAny<string>());

            _partnersIntegrationClientMock.Verify(x => x.MessagesApi.GetMessageAsync(It.IsAny<string>()), Times.Once);
            _partnerManagementClientMock.Verify(x => x.Partners.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
