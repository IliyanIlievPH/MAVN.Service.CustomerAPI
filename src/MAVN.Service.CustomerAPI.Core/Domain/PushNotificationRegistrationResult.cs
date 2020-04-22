namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public enum PushNotificationRegistrationResult
    {
        Ok,
        InfobipPushRegistrationAlreadyExists,
        FirebaseTokenAlreadyExists,
        AppleTokenAlreadyExists
    }
}
