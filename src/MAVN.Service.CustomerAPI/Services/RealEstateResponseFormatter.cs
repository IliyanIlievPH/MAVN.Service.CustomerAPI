using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MAVN.Service.CustomerAPI.Models.RealEstate;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;

namespace MAVN.Service.CustomerAPI.Services
{
    public class RealEstateResponseFormatter
    {
        private const string RealEstateIdDelimiter = "###";

        private readonly ICustomerProfileClient _cpClient;

        public RealEstateResponseFormatter(ICustomerProfileClient cpClient)
        {
            _cpClient = cpClient;
        }

        public async Task<(RealEstatePropertiesResponse, RealEstateErrorCodes)> FormatAsync(string customerId, string spendRuleId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));

            if (string.IsNullOrEmpty(spendRuleId))
                throw new ArgumentNullException(nameof(spendRuleId));

            var customerProfile = await _cpClient.CustomerProfiles.GetByCustomerIdAsync(customerId);

            if (customerProfile.ErrorCode == CustomerProfileErrorCodes.CustomerProfileDoesNotExist)
                return (null, RealEstateErrorCodes.CustomerProfileDoesNotExist);

            return (new RealEstatePropertiesResponse { RealEstateProperties = new List<RealEstateProperty>() },
                RealEstateErrorCodes.None);
        }

        public RealEstateIdDto DecomposeRealEstateId(string id)
        {
            var idBytes = Convert.FromBase64String(id);
            var decomposedId = Encoding.UTF8.GetString(idBytes).Split(RealEstateIdDelimiter);

            return new RealEstateIdDto
            {
                LocationCode = decomposedId[0],
                AccountNumber = decomposedId[1],
                CustomerTrxId = decomposedId[2],
                Type = decomposedId[3],
                OrgId = decomposedId[4]
            };
        }
    }
}
