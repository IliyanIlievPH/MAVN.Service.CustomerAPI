using Autofac;
using MAVN.Service.CustomerAPI.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.CustomerAPI.Modules
{
    public class RepositoriesModule : Module
    {
        private readonly IReloadingManager<DbSettings> _dbSettings;

        public RepositoriesModule(IReloadingManager<AppSettings> dbSettings)
        {
            _dbSettings = dbSettings.Nested(s => s.CustomerApiService.Db);
        }

        protected override void Load(ContainerBuilder builder)
        {
            
        }
    }
}
