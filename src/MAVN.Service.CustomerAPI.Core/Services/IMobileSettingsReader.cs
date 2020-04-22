using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IMobileSettingsReader
    {
        /// <summary>
        /// Reads settings as json
        /// </summary>
        /// <returns>JObject</returns>
        /// <exception cref="JsonReaderException">Thrown when settings content is not a valid json</exception>
        Task<JObject> ReadJsonAsync();
    }
}
