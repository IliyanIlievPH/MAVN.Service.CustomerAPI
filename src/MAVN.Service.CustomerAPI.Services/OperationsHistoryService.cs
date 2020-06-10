using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Campaign.Client.Models.Enums;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.OperationsHistory.Client;
using MAVN.Service.OperationsHistory.Client.Models.Requests;
using MAVN.Service.OperationsHistory.Client.Models.Responses;
using MoreLinq;

namespace MAVN.Service.CustomerAPI.Services
{
    public class OperationsHistoryService : IOperationsHistoryService
    {
        private readonly IOperationsHistoryClient _operationsHistoryClient;
        private readonly ICampaignClient _campaignClient;

        public OperationsHistoryService(
            IOperationsHistoryClient operationsHistoryClient,
            ICampaignClient campaignClient)
        {
            _operationsHistoryClient = operationsHistoryClient;
            _campaignClient = campaignClient;
        }

        public async Task<PaginatedOperationsHistoryModel> GetAsync(string customerId, int currentPage, int pageSize)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            if (currentPage <= 0)
            {
                throw new ArgumentException("Current page must be greater than 0.", nameof(currentPage));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));
            }

            var response = await _operationsHistoryClient.OperationsHistoryApi.GetByCustomerIdAsync(
                customerId,
                new PaginationModel {CurrentPage = currentPage, PageSize = pageSize});

            var voucherPurchasePaymentOperations = await ConvertAsync(response.VoucherPurchasePayments?.ToList());

            var allOperations = (response.Transfers?.Select(x => Convert(x, customerId)) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.BonusCashIns?.Select(Convert) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.PartnersPayments?.Select(x => Convert(x, HistoryOperationType.PartnerPayment)) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.RefundedPartnersPayments?.Select(x => Convert(x, HistoryOperationType.PartnerPaymentRefund)) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.LinkedWalletTransfers?.Select(Convert) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.ReferralStakes?.Select(x => Convert(x, HistoryOperationType.ReferralStake)) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.ReleasedReferralStakes?.Select(x => Convert(x, HistoryOperationType.ReleasedReferralStake)) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.FeeCollectedOperations?.Select(Convert) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.SmartVoucherPayments?.Select(Convert) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.SmartVoucherUses?.Select(Convert) ?? Array.Empty<OperationHistoryModel>())
                .Concat(response.SmartVoucherTransfers?.Select(x => Convert(x, customerId)) ?? Array.Empty<OperationHistoryModel>())
                .Concat(voucherPurchasePaymentOperations)
                .OrderByDescending(x => x.Timestamp);

            return new PaginatedOperationsHistoryModel {Operations = allOperations, TotalCount = response.TotalCount};
        }

        private async Task<IReadOnlyList<OperationHistoryModel>> ConvertAsync(
            IReadOnlyList<VoucherPurchasePaymentResponse> src)
        {
            if (src == null || !src.Any())
                return new OperationHistoryModel[0];

            var spendRuleIdentifiers = src
                .Select(o => o.SpendRuleId)
                .Distinct()
                .ToList();

            var spendRuleTitleMap = new Dictionary<Guid, string>();
            
            foreach (var group in spendRuleIdentifiers.Batch(5))
            {
                var tasks = group
                    .Select(spendRuleId => _campaignClient.Mobile.GetSpendRuleAsync(spendRuleId, Localization.En, true))
                    .ToList();

                await Task.WhenAll(tasks);

                foreach (var task in tasks.Where(task => task.Result != null))
                    spendRuleTitleMap.Add(task.Result.Id, task.Result.Title);
            }

            return src
                .Select(o => new OperationHistoryModel
                {
                    Type = HistoryOperationType.VoucherPurchasePayment,
                    Timestamp = o.Timestamp,
                    Amount = o.Amount,
                    CampaignName = spendRuleTitleMap.ContainsKey(o.SpendRuleId)
                        ? spendRuleTitleMap[o.SpendRuleId]
                        : null
                })
                .ToList();
        }

        private static OperationHistoryModel Convert(BonusCashInResponse src)
        {
            return new OperationHistoryModel
            {
                Type = HistoryOperationType.BonusReward,
                Timestamp = src.Timestamp,
                Amount = src.Amount,
                CampaignName = src.ConditionName ?? src.CampaignName,
            };
        }

        private static OperationHistoryModel Convert(TransferResponse src, string customerId)
        {
            return new OperationHistoryModel
            {
                Type = src.ReceiverCustomerId == customerId
                    ? HistoryOperationType.ReceiveTransfer
                    : HistoryOperationType.SendTransfer,
                Timestamp = src.Timestamp,
                Amount = src.Amount,
                OtherSideCustomerEmail = src.ReceiverCustomerId == customerId
                    ? src.SenderCustomerEmail
                    : src.ReceiverCustomerEmail,
            };
        }

        private static OperationHistoryModel Convert(PartnersPaymentResponse src, HistoryOperationType type)
        {
            return new OperationHistoryModel
            {
                Type = type,
                Timestamp = src.Timestamp,
                Amount = src.Amount,
                PartnerName = src.PartnerName,
            };
        }

        private static OperationHistoryModel Convert(LinkedWalletTransferResponse src)
        {
            return new OperationHistoryModel
            {
                Type = src.Direction == LinkedWalletTransferDirection.Incoming
                    ? HistoryOperationType.LinkedWalletReceiveTransfer
                    : HistoryOperationType.LinkedWalletSendTransfer,
                Timestamp = src.Timestamp,
                Amount = src.Amount
            };
        }

        private static OperationHistoryModel Convert(ReferralStakeResponse src, HistoryOperationType type)
        {
            return new OperationHistoryModel
            {
                Type = type,
                Timestamp = src.Timestamp,
                Amount = src.Amount,
                CampaignName = src.CampaignName,
            };
        }

        private static OperationHistoryModel Convert(FeeCollectedOperationResponse src)
        {
            HistoryOperationType type;
            switch (src.Reason)
            {
                case FeeCollectionReason.TransferToPublic:
                    type = HistoryOperationType.TransferToPublicFee;
                    break;
                case FeeCollectionReason.WalletLinking:
                    type = HistoryOperationType.WalletLinkingFee;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(src.Reason));
            }

            return new OperationHistoryModel
            {
                Type = type,
                Timestamp = src.Timestamp,
                Amount = src.Fee,
            };
        }

        private static OperationHistoryModel Convert(SmartVoucherUseResponse src)
        {
            return new OperationHistoryModel
            {
                Type = HistoryOperationType.SmartVoucherUse,
                Timestamp = src.Timestamp,
                Amount = src.Amount,
                CampaignName = src.CampaignName,
                Vertical = src.Vertical,
                Currency = src.AssetSymbol,
                PartnerName = src.PartnerName,
            };
        }

        private static OperationHistoryModel Convert(SmartVoucherPaymentResponse src)
        {
            return new OperationHistoryModel
            {
                Type = HistoryOperationType.SmartVoucherPayment,
                Timestamp = src.Timestamp,
                Amount = src.Amount,
                CampaignName = src.CampaignName,
                Vertical = src.Vertical,
                Currency = src.AssetSymbol,
                PartnerName = src.PartnerName,
            };
        }

        private static OperationHistoryModel Convert(SmartVoucherTransferResponse src, string customerId)
        {
            var customerIdGuid = Guid.Parse(customerId);
            return new OperationHistoryModel
            {
                Type = src.OldCustomerId == customerIdGuid ? HistoryOperationType.SmartVoucherTransferSend : HistoryOperationType.SmartVoucherTransferReceive,
                Timestamp = src.Timestamp,
                Amount = src.Amount,
                CampaignName = src.CampaignName,
                Vertical = src.Vertical,
                Currency = src.AssetSymbol,
                PartnerName = src.PartnerName,
                OtherSideCustomerEmail = src.OldCustomerId == customerIdGuid ? src.NewCustomerEmail : src.OldCustomerEmail,
            };
        }
    }
}
