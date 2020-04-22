using System.Collections.Generic;
using System.Linq;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;

namespace MAVN.Service.CustomerAPI.Services
{
    public class ThrottlingSettingsService : IThrottlingSettingsService
    {
        private readonly IEnumerable<RouteThrottlingConfigurationItem> _routeSettings;
        private readonly SigninThrottlingConfiguration _signinSettings;
        
        public ThrottlingSettingsService(
            IEnumerable<RouteThrottlingConfigurationItem> routeSettings, 
            SigninThrottlingConfiguration signinSettings)
        {
            _routeSettings = routeSettings;
            _signinSettings = signinSettings;
        }
        
        public IReadOnlyList<RouteThrottlingConfigurationItem> GetRouteSettings()
        {
            return _routeSettings.ToList();
        }

        public SigninThrottlingConfiguration GetSigninSettings()
        {
            return _signinSettings;
        }
    }
}
