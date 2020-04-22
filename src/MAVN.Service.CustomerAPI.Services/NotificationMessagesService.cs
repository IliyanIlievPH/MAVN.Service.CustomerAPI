using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.Service.PushNotifications.Client;
using Lykke.Service.PushNotifications.Client.Models.Requests;

namespace MAVN.Service.CustomerAPI.Services
{
    public class NotificationMessagesService : INotificationMessagesService
    {
        private readonly IPushNotificationsClient _pushNotificationsClient;
        private readonly IMapper _mapper;

        public NotificationMessagesService(IPushNotificationsClient pushNotificationsClient, IMapper mapper)
        {
            _pushNotificationsClient = pushNotificationsClient;
            _mapper = mapper;
        }

        public async Task<PaginatedNotificationMessagesModel> GetAsync(string customerId, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            var result =
                await _pushNotificationsClient.NotificationMessagesApi.GetNotificationMessagesForCustomerAsync(
                    new NotificationMessagesRequestModel
                    {
                        CurrentPage = currentPage,
                        PageSize = pageSize,
                        CustomerId = customerId
                    });

            return new PaginatedNotificationMessagesModel
            {
                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                NotificationMessages = _mapper.Map<IEnumerable<NotificationMessage>>(result.Data),
                TotalCount = result.TotalCount
            };
        }

        public async Task MarkMessageAsReadAsync(Guid messageGroupId)
        {
            await _pushNotificationsClient.NotificationMessagesApi.MarkMessageAsReadAsync(
                new MarkMessageAsReadRequestModel {MessageGroupId = messageGroupId});
        }

        public async Task<int> GetNumberOfUnreadMessagesAsync(string customerId)
        {
            return await _pushNotificationsClient.NotificationMessagesApi.GetUnreadMessagesCountAsync(customerId);
        }

        public async Task MarkAllCustomerMessagesAsReadAsync(string customerId)
        {
            await _pushNotificationsClient.NotificationMessagesApi.MarkAllMessagesAsReadAsync(
                new MarkAllMessagesAsReadRequestModel {CustomerId = customerId});
        }
    }
}
