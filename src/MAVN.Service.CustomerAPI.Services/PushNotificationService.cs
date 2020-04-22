using System;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using Lykke.Service.PushNotifications.Client;
using Lykke.Service.PushNotifications.Client.Models.Requests;

namespace MAVN.Service.CustomerAPI.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IPushNotificationsClient _pushNotificationsClient;
        private readonly IMapper _mapper;

        public PushNotificationService(IPushNotificationsClient pushNotificationsClient, IMapper mapper)
        {
            _pushNotificationsClient = pushNotificationsClient;
            _mapper = mapper;
        }

        public async Task<PushNotificationRegistrationResult> RegisterForPushNotificationsAsync(string customerId,
            PushNotificationRegistrationCreateModel model)
        {
            var result = await _pushNotificationsClient.PushRegistrationsApi.RegisterForPushNotificationsAsync(
                new CreatePushRegistrationRequestModel
                {
                    AppleToken = model.AppleToken,
                    CustomerId = customerId,
                    FirebaseToken = model.FirebaseToken,
                    InfobipToken = model.InfobipPushRegistrationId
                });

            return _mapper.Map<PushNotificationRegistrationResult>(result);
        }

        public  Task CancelPushRegistrationNotificationsAsync(string infobipPushRegistrationId)
        {
            return _pushNotificationsClient.PushRegistrationsApi.DeleteRegistrationByInfobipTokenAsync(infobipPushRegistrationId);
        }
    }
}
