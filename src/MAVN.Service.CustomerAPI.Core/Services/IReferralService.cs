using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.ApiLibrary.Contract;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Domain.CommonReferral;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IReferralService
    {
        Task<string> GetOrCreateReferralCodeAsync(string customerId);
        Task<ILykkeApiErrorCode> AddReferralLeadAsync(string customerId, ReferralLeadCreateModel referralLeadCreateModel);
        Task<ReferralLeadListResultModel> GetLeadReferralsAsync(string agentId);
        Task<ILykkeApiErrorCode> ConfirmReferralAsync(string confirmationCode);
        Task<ILykkeApiErrorCode> AddHotelReferralAsync(string customerId, string email,
            Guid campaignId, string fullName, int phoneCountryCode, string phoneNumber);
        Task<IEnumerable<HotelReferralModel>> GetHotelReferralAsync(string customerId);
        Task<IReadOnlyList<CustomerCommonReferralModel>> PrepareReferralCommonData(
            IEnumerable<CommonReferralModel> paged, string customerId, Guid? earnRuleId);
        Task<ILykkeApiErrorCode> AddReferralFriendAsync(string customerId, ReferralFriendCreateModel referralFriendCreateModel);
    }
}
