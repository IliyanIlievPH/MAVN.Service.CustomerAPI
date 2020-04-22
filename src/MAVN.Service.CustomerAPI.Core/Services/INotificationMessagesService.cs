using System;
using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface INotificationMessagesService
    {
        Task<PaginatedNotificationMessagesModel> GetAsync(string customerId, int currentPage, int pageSize);

        Task MarkMessageAsReadAsync(Guid messageGroupId);

        Task<int> GetNumberOfUnreadMessagesAsync(string customerId);
        
        Task MarkAllCustomerMessagesAsReadAsync(string customerId);
    }
}
