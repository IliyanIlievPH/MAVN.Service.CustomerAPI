using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IPartnersMessagesService
    {
        Task<PartnerMessage> GetPartnerMessageAsync(string partnerMessageId);
    }
}
