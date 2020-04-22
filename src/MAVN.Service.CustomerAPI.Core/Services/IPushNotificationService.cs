using System;
using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IPushNotificationService
    {
        Task<PushNotificationRegistrationResult> RegisterForPushNotificationsAsync(string customerId,
            PushNotificationRegistrationCreateModel model);

        Task CancelPushRegistrationNotificationsAsync(string infobipPushRegistrationId);
    }
}
