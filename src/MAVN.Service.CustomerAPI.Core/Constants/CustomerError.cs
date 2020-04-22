namespace MAVN.Service.CustomerAPI.Core.Constants
{
    public enum CustomerError
    {
        None,
        LoginNotFound,
        PasswordMismatch,
        RegisteredWithAnotherPassword,
        AlreadyRegistered,
        InvalidLoginFormat,
        InvalidPasswordFormat,
        LoginExistsWithDifferentProvider,
        AlreadyRegisteredWithGoogle,
        CustomerBlocked,
        InvalidCountryOfNationalityId,
        EmailIsNotAllowed,
        CustomerProfileDeactivated,
        InvalidOrExpiredGoogleAccessToken,
    }
}
