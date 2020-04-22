using Lykke.Service.CustomerManagement.Client;

namespace MAVN.Service.CustomerAPI.Infrastructure.Extensions
{
    public static class CustomerManagementErrorExtensions
    {
        public static bool IsPasswordWrong(this CustomerManagementError err)
        {
            return err == CustomerManagementError.PasswordMismatch ||
                   err == CustomerManagementError.RegisteredWithAnotherPassword ||
                   err == CustomerManagementError.InvalidPasswordFormat;
        }
    }
}
