using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lykke.Common.ApiLibrary.Contract;
using MAVN.Service.CustomerAPI.Core.Constants;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Constants
{

    public class LykkeApiErrorCodesTests
    {
        private readonly HashSet<string> _expectedErrorCodes = new HashSet<string>
        {
            nameof(ApiErrorCodes.ModelValidation.ModelValidationFailed),
            nameof(ApiErrorCodes.Service.InvalidEmailFormat),
            nameof(ApiErrorCodes.Service.InvalidCredentials),
            nameof(ApiErrorCodes.Service.LoginAlreadyInUse),
            nameof(ApiErrorCodes.Service.InvalidPasswordFormat),
            nameof(ApiErrorCodes.Service.TransferSourceAndTargetMustBeDifferent),
            nameof(ApiErrorCodes.Service.SenderCustomerAssetNotFound),
            nameof(ApiErrorCodes.Service.SenderCustomerNotEnoughBalance),
            nameof(ApiErrorCodes.Service.SenderCustomerNotFound),
            nameof(ApiErrorCodes.Service.TargetCustomerNotFound),
            nameof(ApiErrorCodes.Service.ReferralNotFound),
            nameof(ApiErrorCodes.Service.ReferralLeadCustomerIdInvalid),
            nameof(ApiErrorCodes.Service.ReferralLeadCustomerNameInvalid),
            nameof(ApiErrorCodes.Service.ReferralLeadCustomerNumberInvalid),
            nameof(ApiErrorCodes.Service.ReferralLeadCustomerNoteInvalid),
            nameof(ApiErrorCodes.Service.ReferralLeadAlreadyExist),
            nameof(ApiErrorCodes.Service.ReferralLeadNotProcessed),
            nameof(ApiErrorCodes.Service.ReferralCampaignNotFound),
            nameof(ApiErrorCodes.Service.ReferralInvalidStake),
            nameof(ApiErrorCodes.Service.CustomerNotApprovedAgent),
            nameof(ApiErrorCodes.Service.NoCustomerWithSuchEmail),
            nameof(ApiErrorCodes.Service.IdentifierDoesNotExist),
            nameof(ApiErrorCodes.Service.ThereIsNoIdentifierForThisCustomer),
            nameof(ApiErrorCodes.Service.ReachedMaximumRequestForPeriod),
            nameof(ApiErrorCodes.Service.EmailIsAlreadyVerified),
            nameof(ApiErrorCodes.Service.ProvidedIdentifierHasExpired),
            nameof(ApiErrorCodes.Service.IdentifierMismatch),
            nameof(ApiErrorCodes.Service.CustomerDoesNotExist),
            nameof(ApiErrorCodes.Service.AgentAlreadyApproved),
            nameof(ApiErrorCodes.Service.SfAccountAlreadyExisting),
            nameof(ApiErrorCodes.Service.EmailNotVerified),
            nameof(ApiErrorCodes.Service.NotEnoughTokens),
            nameof(ApiErrorCodes.Service.CustomerProfileDoesNotExist),
            nameof(ApiErrorCodes.Service.ReferralExpired),
            nameof(ApiErrorCodes.Service.LeadAlreadyConfirmed),
            nameof(ApiErrorCodes.Service.InvalidCustomerId),
            nameof(ApiErrorCodes.Service.CustomerWalletMissing),
            nameof(ApiErrorCodes.Service.InvalidAmount),
            nameof(ApiErrorCodes.Service.InvalidReceiver),
            nameof(ApiErrorCodes.Service.LeadAlreadyConfirmed),
            nameof(ApiErrorCodes.Service.CountryPhoneCodeDoesNotExist),
            nameof(ApiErrorCodes.Service.CountryOfResidenceDoesNotExist),
            nameof(ApiErrorCodes.Service.ImageUploadError),
            nameof(ApiErrorCodes.Service.ConnectorRegistrationError),
            nameof(ApiErrorCodes.Service.InvalidCampaignId),
            nameof(ApiErrorCodes.Service.CampaignDoesNotExists),
            nameof(ApiErrorCodes.Service.CustomerWalletBlocked),
            nameof(ApiErrorCodes.Service.TransferSourceCustomerWalletBlocked),
            nameof(ApiErrorCodes.Service.TransferTargetCustomerWalletBlocked),
            nameof(ApiErrorCodes.Service.CustomerBlocked),
            nameof(ApiErrorCodes.Service.LoginExistsWithDifferentProvider),
            nameof(ApiErrorCodes.Service.InvalidOrExpiredGoogleAccessToken),
            nameof(ApiErrorCodes.Service.AlreadyRegisteredWithGoogle),
            nameof(ApiErrorCodes.Service.ReferralAlreadyConfirmed),
            nameof(ApiErrorCodes.Service.ReferralsLimitExceeded),
            nameof(ApiErrorCodes.Service.PaymentDoesNotExist),
            nameof(ApiErrorCodes.Service.PaymentIsNotInACorrectStatusToBeUpdated),
            nameof(ApiErrorCodes.Service.PaymentRequestsIsForAnotherCustomer),
            nameof(ApiErrorCodes.Service.CanNotReferYourself),
            nameof(ApiErrorCodes.Service.ReferralLeadAlreadyConfirmed),
            nameof(ApiErrorCodes.Service.MessageRequestsIsForAnotherCustomer),
            nameof(ApiErrorCodes.Service.PartnersPaymentNotFound),
            nameof(ApiErrorCodes.Service.EarnRuleNotFound),
            nameof(ApiErrorCodes.Service.SpendRuleNotFound),
            nameof(ApiErrorCodes.Service.PhoneIsAlreadyVerified),
            nameof(ApiErrorCodes.Service.CustomerPhoneIsMissing),
            nameof(ApiErrorCodes.Service.ReferralAlreadyExist),
            nameof(ApiErrorCodes.Service.CustomerIsNotVerified),
            nameof(ApiErrorCodes.Service.LinkingRequestAlreadyExist),
            nameof(ApiErrorCodes.Service.LinkingRequestDoesNotExist),
            nameof(ApiErrorCodes.Service.InvalidWalletLinkPublicAddress),
            nameof(ApiErrorCodes.Service.InvalidWalletLinkPrivateAddress),
            nameof(ApiErrorCodes.Service.InvalidWalletLinkSignature),
            nameof(ApiErrorCodes.Service.LinkingRequestAlreadyApproved),
            nameof(ApiErrorCodes.Service.PublicWalletIsNotLinked),
            nameof(ApiErrorCodes.Service.LinkingRequestIsBeingConfirmed),
            nameof(ApiErrorCodes.Service.InvalidCountryOfNationalityId),
            nameof(ApiErrorCodes.Service.PhoneAlreadyExists),
            nameof(ApiErrorCodes.Service.SalesForceError),
            nameof(ApiErrorCodes.Service.CannotPassBothFiatAndTokensAmount),
            nameof(ApiErrorCodes.Service.EitherFiatOrTokensAmountShouldBePassed),
            nameof(ApiErrorCodes.Service.SpendRuleNotFound),
            nameof(ApiErrorCodes.Service.ConversionRateNotFound),
            nameof(ApiErrorCodes.Service.InvalidRealEstateId),
            nameof(ApiErrorCodes.Service.InvalidVerticalInSpendRule),
            nameof(ApiErrorCodes.Service.EmailIsNotAllowed),
            nameof(ApiErrorCodes.Service.CustomerIsNotActive),
            nameof(ApiErrorCodes.Service.PinIsNotSet),
            nameof(ApiErrorCodes.Service.PinCodeMismatch),
            nameof(ApiErrorCodes.Service.InvalidPin),
            nameof(ApiErrorCodes.Service.PinAlreadySet),
            nameof(ApiErrorCodes.Service.InvalidPhoneNumber),
            nameof(ApiErrorCodes.Service.InvalidPriceInSpendRule),
            nameof(ApiErrorCodes.Service.NoVouchersInStock),
            nameof(ApiErrorCodes.Service.PublicBlockchainIsDisabled),
            nameof(ApiErrorCodes.Service.SmartVoucherCampaignNotFound),
            nameof(ApiErrorCodes.Service.SmartVoucherCampaignNotActive),
            nameof(ApiErrorCodes.Service.NoAvailableVouchers),
            nameof(ApiErrorCodes.Service.SmartVoucherNotFound),
            nameof(ApiErrorCodes.Service.PaymentProviderError),
            nameof(ApiErrorCodes.Service.PaymentInfoNotFound),
        };

        /// <summary>
        ///     Ensures each code is unique and it's value has not changed.
        ///     Verifies that newly added codes has test cases.
        ///     Verifies if codes were removed their test cases is removed too.
        ///     If for some reasons you have modified error codes contract,
        ///     please fix unit test cases too. This is needed to make sure you have changed error code knowingly.
        /// </summary>
        [Fact]
        public void ErrorCodes_WasNotModifiedAccidently()
        {
            var currentErrorCodes = GetAllCurrentApiErrorCodes(typeof(ApiErrorCodes));

            foreach (var expectedCode in _expectedErrorCodes)
            {
                Assert.True(currentErrorCodes.Contains(expectedCode),
                $"Error code: \"{expectedCode}\" was removed! But it still have a test. If you removed it knowingly please remove it from {nameof(_expectedErrorCodes)}.");
            }

            if (currentErrorCodes.Count > _expectedErrorCodes.Count)
            {
                var addedErrorCodes = currentErrorCodes.Except(_expectedErrorCodes);

                foreach (var addedErrorCode in addedErrorCodes)
                    Assert.True(false,
                         $"Code: \"{addedErrorCode}\" was added, but don't have a test. Please add it to {nameof(_expectedErrorCodes)}.");
            }
        }

        /// <summary>
        /// Get all current ErrorCodes defined in <see cref="ApiErrorCodes"/>
        /// </summary>
        private static HashSet<string> GetAllCurrentApiErrorCodes(Type type,
            HashSet<string> errorCodeNames = null)
        {
            if (errorCodeNames == null)
                errorCodeNames = new HashSet<string>();

            var errorCodes = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var propertyInfo in errorCodes)
            {
                var errorCodeKey = propertyInfo.Name;

                var errorCode = propertyInfo.GetValue(null) as ILykkeApiErrorCode;

                 Assert.False(errorCode == null, $"Error code: \"{errorCodeKey}\" is null!");

                var errorCodeName = errorCode.Name;

                Assert.True(string.Equals(errorCodeKey, errorCodeName), $"Error code: \"{errorCodeKey}\" name should match field name!");

                Assert.True(errorCodeNames.Add(errorCodeName), $"Error code: \"{errorCodeName}\" should have unique name!"); 
            }

            var typeGroups = type.GetNestedTypes();

            foreach (var typeGroup in typeGroups) GetAllCurrentApiErrorCodes(typeGroup, errorCodeNames);

            return errorCodeNames;
        }
    }
}
