using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Infrastructure.AutoMapperProfiles;
using MAVN.Service.CustomerAPI.Services;
using MAVN.Service.PushNotifications.Client;
using MAVN.Service.PushNotifications.Client.Models.Requests;
using MAVN.Service.PushNotifications.Client.Models.Responses;
using Moq;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Services
{
    public class NotificationMessagesServiceTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IPushNotificationsClient> _pushNotificationsClientMock =
            new Mock<IPushNotificationsClient>();

        private readonly INotificationMessagesService _service;

        public NotificationMessagesServiceTests()
        {
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });

            var mapper = mockMapper.CreateMapper();

            _service = new NotificationMessagesService(_pushNotificationsClientMock.Object, mapper);
        }

        [Fact]
        public async Task When_MarkMessageAsReadAsync_Is_Executed_Then_Push_Notifications_Client_Is_Called()
        {
            _pushNotificationsClientMock.Setup(x =>
                    x.NotificationMessagesApi.MarkMessageAsReadAsync(It.IsAny<MarkMessageAsReadRequestModel>()))
                .Returns(Task.CompletedTask);

            await _service.MarkMessageAsReadAsync(It.IsAny<Guid>());

            _pushNotificationsClientMock.Verify(
                x => x.NotificationMessagesApi.MarkMessageAsReadAsync(It.IsAny<MarkMessageAsReadRequestModel>()),
                Times.Once);
        }

        [Fact]
        public async Task When_MarkAllCustomerMessagesAsReadAsync_Is_Executed_Then_Push_Notifications_Client_Is_Called()
        {
            _pushNotificationsClientMock
                .Setup(x =>
                    x.NotificationMessagesApi.MarkAllMessagesAsReadAsync(It.IsAny<MarkAllMessagesAsReadRequestModel>()))
                .Returns(Task.CompletedTask);

            await _service.MarkAllCustomerMessagesAsReadAsync(It.IsAny<string>());

            _pushNotificationsClientMock.Verify(
                x => x.NotificationMessagesApi.MarkAllMessagesAsReadAsync(
                    It.IsAny<MarkAllMessagesAsReadRequestModel>()), Times.Once);
        }

        [Fact]
        public async Task When_GetNumberOfUnreadMessagesAsync_Is_Executed_Then_Push_Notifications_Client_Is_Called()
        {
            _pushNotificationsClientMock
                .Setup(x => x.NotificationMessagesApi.GetUnreadMessagesCountAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(1));

            await _service.GetNumberOfUnreadMessagesAsync(It.IsAny<string>());

            _pushNotificationsClientMock.Verify(
                x => x.NotificationMessagesApi.GetUnreadMessagesCountAsync(It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task When_GetAsync_Is_Executed_Then_Push_Notifications_Client_Is_Called()
        {
            _pushNotificationsClientMock.Setup(x =>
                    x.NotificationMessagesApi.GetNotificationMessagesForCustomerAsync(
                        It.IsAny<NotificationMessagesRequestModel>()))
                .Returns(Task.FromResult(_fixture.Build<PaginatedResponseModel<NotificationMessageResponseModel>>()
                    .Create()));

            await _service.GetAsync("dummy customer id", It.IsAny<int>(), It.IsAny<int>());

            _pushNotificationsClientMock.Verify(
                x => x.NotificationMessagesApi.GetNotificationMessagesForCustomerAsync(
                    It.IsAny<NotificationMessagesRequestModel>()), Times.Once);
        }

        [Fact]
        public async Task When_GetAsync_Is_Executed_For_Invalid_Customer_Id_Then_Exception_Is_Thrown()
        {
            _pushNotificationsClientMock.Setup(x =>
                    x.NotificationMessagesApi.GetNotificationMessagesForCustomerAsync(
                        It.IsAny<NotificationMessagesRequestModel>()))
                .Returns(Task.FromResult(_fixture.Build<PaginatedResponseModel<NotificationMessageResponseModel>>()
                    .Create()));

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.GetAsync(string.Empty, It.IsAny<int>(), It.IsAny<int>()));
        }
    }
}
