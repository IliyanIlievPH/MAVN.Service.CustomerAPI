namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class PushNotificationRegistrationCreateModel
    {
        public string InfobipPushRegistrationId { get; set; }

        public string FirebaseToken { get; set; }

        public string AppleToken { get; set; }
    }
}
