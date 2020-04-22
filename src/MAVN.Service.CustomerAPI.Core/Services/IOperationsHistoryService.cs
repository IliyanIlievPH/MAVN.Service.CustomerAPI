using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IOperationsHistoryService
    {
        Task<PaginatedOperationsHistoryModel> GetAsync(string customerId, int currentPage, int pageSize);
    }
}
