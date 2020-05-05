using System;
using System.Linq;
using System.Threading.Tasks;
using MAVN.Service.Campaign.Client;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Services;
using MAVN.Service.OperationsHistory.Client;
using MAVN.Service.OperationsHistory.Client.Models.Requests;
using MAVN.Service.OperationsHistory.Client.Models.Responses;
using Moq;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Services
{
    public class OperationsHistoryServiceTests
    {
        private readonly Mock<IOperationsHistoryClient> _operationsHistoryClientMock = new Mock<IOperationsHistoryClient>();
        private readonly Mock<ICampaignClient> _campaignClientMock = new Mock<ICampaignClient>();

        private const string FakeCustomerId = "b1f34a48-eb1d-4d11-a32f-c2db45a9e892"; 
            
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetAsync_InvalidCustomerId_RaisesException(string customerId)
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetAsync(customerId, 1, 10));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        public async Task GetAsync_InvalidPaginationParameters_RaisesException(int currentPage, int pageSize)
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentException>(() => sut.GetAsync(FakeCustomerId, currentPage, pageSize));
        }

        [Fact]
        public async Task GetAsync_ReturnsTransfersAndBonusesTogether()
        {
            _operationsHistoryClientMock
                .Setup(x => x.OperationsHistoryApi.GetByCustomerIdAsync(
                    It.IsAny<string>(),
                    It.IsAny<PaginationModel>()))
                .ReturnsAsync(new PaginatedCustomerOperationsResponse
                {
                    BonusCashIns = new[] {new BonusCashInResponse()},
                    Transfers = new[]
                    {
                        new TransferResponse {ReceiverCustomerId = FakeCustomerId},
                        new TransferResponse {SenderCustomerId = FakeCustomerId}
                    },
                    TotalCount = 3
                });
            
            var sut = CreateSutInstance();

            var result = await sut.GetAsync(FakeCustomerId, 1, 10);
            
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Operations.Count());
            Assert.Single(result.Operations, o => o.Type == HistoryOperationType.ReceiveTransfer);
            Assert.Single(result.Operations, o => o.Type == HistoryOperationType.SendTransfer);
            Assert.Single(result.Operations, o => o.Type == HistoryOperationType.BonusReward);
        }
        
        private IOperationsHistoryService CreateSutInstance()
        {
            return new OperationsHistoryService(
                _operationsHistoryClientMock.Object,
                _campaignClientMock.Object);
        }
    }
}
