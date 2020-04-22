using System.Collections.Generic;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IThrottlingSettingsService
    {
        IReadOnlyList<RouteThrottlingConfigurationItem> GetRouteSettings();

        SigninThrottlingConfiguration GetSigninSettings();
    }
}
