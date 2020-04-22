using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface ICustomerService
    {
       Task<RegistrationResultModel> RegisterAsync(RegistrationRequestDto model);

       Task<RegistrationResultModel> GoogleRegisterAsync(GoogleRegistrationRequestDto model);

       Task<CustomerInfoModel> GetCustomerInfoAsync(
           string customerId,
           bool includeNotVerified = false,
           bool includeNotActive = false);

       Task<ChangePasswordResultModel> ChangePasswordAsync(string customerId, string password);

       Task<ResetPasswordResultModel> ResetPasswordAsync(string email, string resetIdentifier, string password);
    }
}
