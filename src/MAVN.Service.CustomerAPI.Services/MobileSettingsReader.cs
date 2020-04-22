using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common;
using Lykke.Common.Log;
using MAVN.Service.CustomerAPI.Core.Services;
using Newtonsoft.Json.Linq;

namespace MAVN.Service.CustomerAPI.Services
{
    public class MobileSettingsReader : IMobileSettingsReader, IStartStop
    {
        private readonly string _settingsUrl;
        private readonly HttpClient _httpClient;
        private readonly ILog _log;
        private readonly TimerTrigger _refreshTimer;

        private string _settingsValue;

        public MobileSettingsReader(
            string settingsUrl, 
            IHttpClientFactory httpClientFactory, 
            ILogFactory logFactory)
        {
            if (!settingsUrl.StartsWith("http"))
                throw new ArgumentException("Mobile application settings must be a url", nameof(settingsUrl));

            _settingsUrl = settingsUrl;
            _httpClient = httpClientFactory.CreateClient();
            _refreshTimer = new TimerTrigger(
                nameof(MobileSettingsReader),
                TimeSpan.FromMinutes(5),
                logFactory,
                TimerTriggeredEventHandler);
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            _refreshTimer.Start();
        }

        public void Dispose()
        {
            _refreshTimer.Dispose();
        }

        public void Stop()
        {
            _refreshTimer.Stop();
        }

        public async Task<JObject> ReadJsonAsync()
        {
            if (_settingsValue == null)
                _settingsValue = await DownloadSettingsAsync();
            return JObject.Parse(_settingsValue);
        }

        private async Task TimerTriggeredEventHandler(
            ITimerTrigger timer,
            TimerTriggeredHandlerArgs args,
            CancellationToken cancellationToken)
        {
            try
            {
                _settingsValue = await DownloadSettingsAsync();
            }
            catch (Exception e)
            {
                _log.Warning(e.Message, e);
            }
        }

        private async Task<string> DownloadSettingsAsync()
        {
            var response = await _httpClient.GetAsync(new Uri(_settingsUrl));

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
