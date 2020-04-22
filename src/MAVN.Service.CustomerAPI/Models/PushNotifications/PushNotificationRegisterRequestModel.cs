namespace MAVN.Service.CustomerAPI.Models.PushNotifications
{
    public class PushNotificationRegisterRequestModel
    {
        public string InfobipPushRegistrationId { get; set; }

        public string FirebaseToken { get; set; }

        public string AppleToken { get; set; }
    }
}
