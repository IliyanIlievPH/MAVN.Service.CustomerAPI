using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;

namespace MAVN.Service.CustomerAPI.Core.Constants
{
    /// <summary>
    ///     Class for storing all possible error codes that may happen in Api.
    ///     Use it with <see cref="LykkeApiErrorException" />.
    /// </summary>
    public static class ApiErrorCodes
    {
        /// <summary>
        ///     Group for client and service related error codes.
        /// </summary>
        public static class Service
        {
            /// <summary>
            ///     Invalid email format.
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidEmailFormat =
                new LykkeApiErrorCode(nameof(InvalidEmailFormat), "Invalid email format.");

            /// <summary>
            ///     Login or password is not valid.
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidCredentials =
                new LykkeApiErrorCode(nameof(InvalidCredentials), "Login or password is not valid.");

            /// <summary>
            ///     This login is already in use.
            /// </summary>
            public static readonly ILykkeApiErrorCode LoginAlreadyInUse =
                new LykkeApiErrorCode(nameof(LoginAlreadyInUse), "This login is already in use.");

            /// <summary>
            ///     Invalid password format.
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidPasswordFormat =
                new LykkeApiErrorCode(nameof(InvalidPasswordFormat), "Invalid password format.");

            /// <summary>
            ///     The sender doesn't have any amount of the selected asset
            /// </summary>
            public static readonly ILykkeApiErrorCode SenderCustomerAssetNotFound =
                new LykkeApiErrorCode(nameof(SenderCustomerAssetNotFound), "The Sender doesn't have any amount of the asset.");
            /// <summary>
            ///     The sender doesn't have enough balance of the selected asset
            /// </summary>
            public static readonly ILykkeApiErrorCode SenderCustomerNotEnoughBalance =
                new LykkeApiErrorCode(nameof(SenderCustomerNotEnoughBalance), "The Sender doesn't have enough balance of the asset.");
            /// <summary>
            ///     The sender doesn't not exist in our Customer Database
            /// </summary>
            public static readonly ILykkeApiErrorCode SenderCustomerNotFound =
                new LykkeApiErrorCode(nameof(SenderCustomerNotFound), "The Sender does not exist");
            /// <summary>
            ///     The Receiver doesn't not exist in our Customer Database
            /// </summary>
            public static readonly ILykkeApiErrorCode TargetCustomerNotFound =
                new LykkeApiErrorCode(nameof(TargetCustomerNotFound), "The Receiver does not exist");
            /// <summary>
            ///     The Receiver doesn't not exist in our Customer Database
            /// </summary>
            public static readonly ILykkeApiErrorCode TransferSourceAndTargetMustBeDifferent =
                new LykkeApiErrorCode(nameof(TransferSourceAndTargetMustBeDifferent), "The Sender and the receiver cannot be the same Customers");
            /// <summary>
            ///     The source Customer's Wallet is blocked
            /// </summary>
            public static readonly ILykkeApiErrorCode TransferSourceCustomerWalletBlocked =
                new LykkeApiErrorCode(nameof(TransferSourceCustomerWalletBlocked), "The Sender Wallet is blocked");
            /// <summary>
            ///     The recipient Customer's Wallet is blocked
            /// </summary>
            public static readonly ILykkeApiErrorCode TransferTargetCustomerWalletBlocked =
                new LykkeApiErrorCode(nameof(TransferTargetCustomerWalletBlocked), "The Recipient Wallet is blocked");
            /// <summary>
            ///     Referral not found.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralNotFound =
                new LykkeApiErrorCode(nameof(ReferralNotFound), "Referral not found.");
            /// <summary>
            ///     Referral expired.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralExpired =
                new LykkeApiErrorCode(nameof(ReferralExpired), "Referral expired.");
            /// <summary>
            ///     Lead already confirmed.
            /// </summary>
            public static readonly ILykkeApiErrorCode LeadAlreadyConfirmed =
                new LykkeApiErrorCode(nameof(LeadAlreadyConfirmed), "Referral Lead already confirmed.");
            /// <summary>
            ///     Referred customer number invalid.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralLeadCustomerNumberInvalid =
                new LykkeApiErrorCode(nameof(ReferralLeadCustomerNumberInvalid), "Referred customer number invalid");
            /// <summary>
            ///     Referred customer name invalid.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralLeadCustomerNameInvalid =
                new LykkeApiErrorCode(nameof(ReferralLeadCustomerNameInvalid), "Referred customer name invalid");
            /// <summary>
            ///     Referred customer id invalid.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralLeadCustomerIdInvalid =
                new LykkeApiErrorCode(nameof(ReferralLeadCustomerIdInvalid), "Referred customer Id invalid");
            /// <summary>
            ///     Property Referral failed to be processed.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralLeadNotProcessed =
                new LykkeApiErrorCode(nameof(ReferralLeadNotProcessed), "Property Referral failed to be processed");
            /// <summary>
            ///     Property Referral failed to be processed.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralLeadCustomerNoteInvalid =
                new LykkeApiErrorCode(nameof(ReferralLeadCustomerNoteInvalid), "Note for the referred customer invalid");
            /// <summary>
            ///     Referred customer id invalid.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralLeadAlreadyExist =
                new LykkeApiErrorCode(nameof(ReferralLeadAlreadyExist), "Lead with the same Phone or/and Email is already referred.");
            /// <summary>
            ///     Customer with such email does not exist in our Customer Database
            /// </summary>
            public static readonly ILykkeApiErrorCode NoCustomerWithSuchEmail =
                new LykkeApiErrorCode(nameof(NoCustomerWithSuchEmail), "Customer with such email does not exist");
            /// <summary>
            ///     The provided identifier does not exist in our Customer Database
            /// </summary>
            public static readonly ILykkeApiErrorCode IdentifierDoesNotExist =
                new LykkeApiErrorCode(nameof(IdentifierDoesNotExist), "The provided identifier does not exist");
            /// <summary>
            ///     There is no identifier for this customer in our Customer Database
            /// </summary>
            public static readonly ILykkeApiErrorCode ThereIsNoIdentifierForThisCustomer =
                new LykkeApiErrorCode(nameof(ThereIsNoIdentifierForThisCustomer), "There is no identifier for this customer");
            /// <summary>
            ///     Reached maximum requests for period for this Customer in our Customer Database
            /// </summary>
            public static readonly ILykkeApiErrorCode ReachedMaximumRequestForPeriod =
                new LykkeApiErrorCode(nameof(ReachedMaximumRequestForPeriod), "Reached maximum requests for period for this Customer");
            /// <summary>
            ///     Email of the customer is already verified.
            /// </summary>
            public static readonly ILykkeApiErrorCode EmailIsAlreadyVerified =
                new LykkeApiErrorCode(nameof(EmailIsAlreadyVerified), "Email is already verified");
            /// <summary>
            ///     The identifier has expired
            /// </summary>
            public static readonly ILykkeApiErrorCode ProvidedIdentifierHasExpired =
                new LykkeApiErrorCode(nameof(ProvidedIdentifierHasExpired), "The provided identifier has expired and it is not valid anymore");
            /// <summary>
            ///     Identifier does not match the customer's one
            /// </summary>
            public static readonly ILykkeApiErrorCode IdentifierMismatch =
                new LykkeApiErrorCode(nameof(IdentifierMismatch), "The provided identifier does not match the customer's one");
            /// <summary>
            ///     Customer does not exist in the system
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerDoesNotExist =
                new LykkeApiErrorCode(nameof(CustomerDoesNotExist), "Customer does not exist");
            /// <summary>
            ///     Agent already registered and approved on the referral service
            /// </summary>
            public static readonly ILykkeApiErrorCode AgentAlreadyApproved =
                new LykkeApiErrorCode(nameof(AgentAlreadyApproved), "Customer already registered and approved as an agent");
            /// <summary>
            ///     Customer is not approved agent
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerNotApprovedAgent =
                new LykkeApiErrorCode(nameof(CustomerNotApprovedAgent), "Customer is not approved agent");
            /// <summary>
            ///     Customer already register as an agent.
            /// </summary>
            public static readonly ILykkeApiErrorCode SfAccountAlreadyExisting =
                new LykkeApiErrorCode(nameof(SfAccountAlreadyExisting), "Customer already register as an agent");
            /// <summary>
            ///     Email of the customer is not verified
            /// </summary>
            public static readonly ILykkeApiErrorCode EmailNotVerified =
                new LykkeApiErrorCode(nameof(EmailNotVerified), "Email is not verified");
            /// <summary>
            ///     Customer has not enough tokens
            /// </summary>
            public static readonly ILykkeApiErrorCode NotEnoughTokens =
                new LykkeApiErrorCode(nameof(NotEnoughTokens), "Not enough tokens");
            /// <summary>
            ///     Customer profile does not exist
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerProfileDoesNotExist =
                new LykkeApiErrorCode(nameof(CustomerProfileDoesNotExist), "Customer profile does not exist");
            /// <summary>
            /// Customer id is not valid
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidCustomerId =
                    new LykkeApiErrorCode(nameof(InvalidCustomerId), "Customer Id is not valid");
            /// <summary>
            /// The customer does not have a wallet
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerWalletMissing =
                new LykkeApiErrorCode(nameof(CustomerWalletMissing), "Customer does not have a wallet");
            /// <summary>
            /// The Customer's Wallet is blocked
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerWalletBlocked =
                new LykkeApiErrorCode(nameof(CustomerWalletBlocked), "Customer Wallet blocked");
            /// <summary>
            /// The Customer is blocked
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerBlocked =
                new LykkeApiErrorCode(nameof(CustomerBlocked), "Customer blocked");
            /// <summary>
            /// The value for amount is not valid
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidAmount =
                new LykkeApiErrorCode(nameof(InvalidAmount), "The value for amount is not valid");
            /// <summary>
            /// There is a problem with the receiver (invalid ID or missing wallet)
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidReceiver =
                new LykkeApiErrorCode(nameof(InvalidReceiver), "There is a problem with the transfers receiver");
            /// <summary>
            ///     Country phone code does not exist
            /// </summary>
            public static readonly ILykkeApiErrorCode CountryPhoneCodeDoesNotExist =
                new LykkeApiErrorCode(nameof(CountryPhoneCodeDoesNotExist), "Country phone code does not exist");
            /// <summary>
            ///     Country of residence does not exist
            /// </summary>
            public static readonly ILykkeApiErrorCode CountryOfResidenceDoesNotExist =
                new LykkeApiErrorCode(nameof(CountryOfResidenceDoesNotExist), "Country of residence does not exist");
            /// <summary>
            ///     An error occurred while uploading images
            /// </summary>
            public static readonly ILykkeApiErrorCode ImageUploadError =
                new LykkeApiErrorCode(nameof(ImageUploadError), "An error occurred while uploading images");
            /// <summary>
            ///     An error occurred while registering an agent account
            /// </summary>
            public static readonly ILykkeApiErrorCode ConnectorRegistrationError =
                new LykkeApiErrorCode(nameof(ConnectorRegistrationError), "An error occurred while registering an agent account");
            /// <summary>
            /// CampaignId is not a valid GUID
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidCampaignId =
                new LykkeApiErrorCode(nameof(InvalidCampaignId), "CampaignId is not a valid GUID");
            /// <summary>
            /// Campaign with the provided id does not exist
            /// </summary>
            public static readonly ILykkeApiErrorCode CampaignDoesNotExists =
                new LykkeApiErrorCode(nameof(CampaignDoesNotExists), "Campaign with the provided id does not exist");
            /// <summary>
            /// This customer is registered but with different login provider.
            /// For example if you are trying to login with google but you are registered with standard account
            /// </summary>
            public static readonly ILykkeApiErrorCode LoginExistsWithDifferentProvider =
                new LykkeApiErrorCode(nameof(LoginExistsWithDifferentProvider), "This customer is registered but with different login provider");
            /// <summary>
            /// The provided google access token is not valid or expired
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidOrExpiredGoogleAccessToken =
                new LykkeApiErrorCode(nameof(InvalidOrExpiredGoogleAccessToken), "The provided google access token is not valid or expired");
            /// <summary>
            /// The customer is already registered using google provider
            /// </summary>
            public static readonly ILykkeApiErrorCode AlreadyRegisteredWithGoogle =
                new LykkeApiErrorCode(nameof(AlreadyRegisteredWithGoogle), "The customer is already registered using google provider");
            /// <summary>
            /// The amount of referrals has reached the limit
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralsLimitExceeded =
                new LykkeApiErrorCode(nameof(ReferralsLimitExceeded), "The amount of referrals has reached the limit");
            /// <summary>
            /// The referral with given email has already been confirmed
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralAlreadyConfirmed =
                new LykkeApiErrorCode(nameof(ReferralAlreadyConfirmed), "The referral with given email has already been confirmed");
            /// <summary>
            /// This payment request is for another customer
            /// </summary>
            public static readonly ILykkeApiErrorCode PaymentRequestsIsForAnotherCustomer =
                new LykkeApiErrorCode(nameof(PaymentRequestsIsForAnotherCustomer), "This payment request is for another customer");
            /// <summary>
            /// The payment request is not in a correct status to be updated
            /// </summary>
            public static readonly ILykkeApiErrorCode PaymentIsNotInACorrectStatusToBeUpdated =
                new LykkeApiErrorCode(nameof(PaymentIsNotInACorrectStatusToBeUpdated), "The payment request is not in a correct status to be updated");
            /// <summary>
            /// Payment request with the provided details does not exist in the system
            /// </summary>
            public static readonly ILykkeApiErrorCode PaymentDoesNotExist =
                new LykkeApiErrorCode(nameof(PaymentDoesNotExist), "Payment request with the provided details does not exist in the system");

            /// <summary>
            /// Error when the agent tries to refer himself
            /// </summary>
            public static readonly  ILykkeApiErrorCode CanNotReferYourself =
                new LykkeApiErrorCode(nameof(CanNotReferYourself), "You can not refer yourself");

            /// <summary>
            /// Error when the agent tries to refer himself
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralLeadAlreadyConfirmed =
                new LykkeApiErrorCode(nameof(ReferralLeadAlreadyConfirmed), "Lead already confirmed");

            /// <summary>
            /// Error when the campaign for the lead not found
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralCampaignNotFound =
                new LykkeApiErrorCode(nameof(ReferralCampaignNotFound), "Lead campaign not found");

            /// <summary>
            /// Error when the staking for the campaign is invalid
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralInvalidStake =
                new LykkeApiErrorCode(nameof(ReferralInvalidStake), "Lead stake is invalid for the campaign");

            /// <summary>
            /// Error when the agent tries to refer himself
            /// </summary>
            public static readonly ILykkeApiErrorCode MessageRequestsIsForAnotherCustomer =
                new LykkeApiErrorCode(nameof(MessageRequestsIsForAnotherCustomer), "Message request is for another customer");

            /// <summary>
            /// Partners Payment was not found
            /// </summary>
            public static readonly ILykkeApiErrorCode PartnersPaymentNotFound =
                new LykkeApiErrorCode(nameof(PartnersPaymentNotFound), "Partners Payment was not found");
            
            /// <summary>
            /// Error when non existing Earn Rule is request
            /// </summary>
            public static readonly ILykkeApiErrorCode EarnRuleNotFound =
                new LykkeApiErrorCode(nameof(EarnRuleNotFound), "Earn Rule not found");

            /// <summary>
            /// Error when non existing Spend Rule is request
            /// </summary>
            public static readonly ILykkeApiErrorCode SpendRuleNotFound =
                new LykkeApiErrorCode(nameof(SpendRuleNotFound), "Spend Rule not found");

            /// <summary>
            /// Customer's phone is not set in the system
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerPhoneIsMissing =
                new LykkeApiErrorCode(nameof(CustomerPhoneIsMissing), "Customer's phone is missing");

            /// <summary>
            /// Customer's phone is already verified
            /// </summary>
            public static readonly ILykkeApiErrorCode PhoneIsAlreadyVerified =
                new LykkeApiErrorCode(nameof(PhoneIsAlreadyVerified), "Customer's phone is already verified");
            
            /// <summary>
            /// Referral already exists
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralAlreadyExist =
                new LykkeApiErrorCode(nameof(ReferralAlreadyExist), "Referral already exists.");

            /// <summary>
            /// The customer is not verified
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerIsNotVerified =
                new LykkeApiErrorCode(nameof(CustomerIsNotVerified), "The customer is not verified");

            /// <summary>
            /// Error when there is already public wallet linking request exists
            /// </summary>
            public static readonly ILykkeApiErrorCode LinkingRequestAlreadyExist =
                new LykkeApiErrorCode(nameof(LinkingRequestAlreadyExist), "There is already wallet linking request for the customer");
            
            /// <summary>
            /// Error when there is no linking request for the customer
            /// </summary>
            public static readonly ILykkeApiErrorCode LinkingRequestDoesNotExist =
                new LykkeApiErrorCode(nameof(LinkingRequestDoesNotExist), "The wallet linking request does not exist");
            
            /// <summary>
            /// Error when public address provided is not valid
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidWalletLinkPublicAddress = 
                new LykkeApiErrorCode(nameof(InvalidWalletLinkPublicAddress), "The wallet linking public address is no valid");
            
            /// <summary>
            /// Error when private address provided is not valid
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidWalletLinkPrivateAddress = 
                new LykkeApiErrorCode(nameof(InvalidWalletLinkPrivateAddress), "The wallet linking private address is no valid");
            
            /// <summary>
            /// Error when signature provided is not valid
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidWalletLinkSignature = 
                new LykkeApiErrorCode(nameof(InvalidWalletLinkSignature), "The wallet linking signature is not valid");
            
            /// <summary>
            /// Error when the linking request has already been approved
            /// </summary>
            public static readonly ILykkeApiErrorCode LinkingRequestAlreadyApproved =
                new LykkeApiErrorCode(nameof(LinkingRequestAlreadyApproved), "The wallet linking request is already approved");
            
            /// <summary>
            /// Error when public wallet address has not been linked yet
            /// </summary>
            public static readonly ILykkeApiErrorCode PublicWalletIsNotLinked = 
                new LykkeApiErrorCode(nameof(PublicWalletIsNotLinked), "The public wallet address has not been linked yet");
            
            /// <summary>
            /// Error when deleting linking request which is being confirmed yet
            /// </summary>
            public static readonly ILykkeApiErrorCode LinkingRequestIsBeingConfirmed =
                new LykkeApiErrorCode(nameof(LinkingRequestIsBeingConfirmed), "The linking request can not be deleted while there is an ongoing confirmation");

            /// <summary>
            /// The provided country of nationality id is not valid
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidCountryOfNationalityId =
                new LykkeApiErrorCode(nameof(InvalidCountryOfNationalityId), "The provided country of nationality id is not valid");

            /// <summary>
            /// This phone is already set and verified for another customer
            /// </summary>
            public static readonly ILykkeApiErrorCode PhoneAlreadyExists =
                new LykkeApiErrorCode(nameof(PhoneAlreadyExists), "This phone is already set and verified for another customer");

            /// <summary>
            /// Some error was returned from salesforce
            /// </summary>
            public static readonly ILykkeApiErrorCode SalesForceError =
                new LykkeApiErrorCode(nameof(SalesForceError), "SalesForce returned an error");

            /// <summary>
            /// Conversion Rate was not found
            /// </summary>
            public static readonly ILykkeApiErrorCode ConversionRateNotFound =
                new LykkeApiErrorCode(nameof(ConversionRateNotFound), "Conversion Rate was not found");

            /// <summary>
            /// The provided real estate id was not valid
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidRealEstateId =
                new LykkeApiErrorCode(nameof(InvalidRealEstateId), "The provided real estate id was not valid");

            /// <summary>
            /// Only one of them should be passed
            /// </summary>
            public static readonly ILykkeApiErrorCode CannotPassBothFiatAndTokensAmount =
                new LykkeApiErrorCode(nameof(CannotPassBothFiatAndTokensAmount), "You cannot pass both fiat and tokens amount");

            /// <summary>
            /// One f them should be passed
            /// </summary>
            public static readonly ILykkeApiErrorCode EitherFiatOrTokensAmountShouldBePassed =
                new LykkeApiErrorCode(nameof(EitherFiatOrTokensAmountShouldBePassed), "You must pas fiat or tokens amount");

            /// <summary>
            /// The vertical in the spend rule is not valid for this kind of operation
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidVerticalInSpendRule =
                new LykkeApiErrorCode(nameof(InvalidVerticalInSpendRule), "The vertical in the spend rule is not valid for this kind of operation");

            /// <summary>
            /// This email is not allowed
            /// </summary>
            public static readonly ILykkeApiErrorCode EmailIsNotAllowed =
                new LykkeApiErrorCode(nameof(EmailIsNotAllowed), "This email is not allowed");

            /// <summary>
            /// The customer has already been deactivated
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerIsNotActive =
                new LykkeApiErrorCode(nameof(CustomerIsNotActive), "The customer is not active");

            /// <summary>
            /// The customer has not set PIN code
            /// </summary>
            public static readonly ILykkeApiErrorCode PinIsNotSet =
                new LykkeApiErrorCode(nameof(PinIsNotSet), "The customer has not set PIN code");

            /// <summary>
            /// The provided PIN does not match the customer's one
            /// </summary>
            public static readonly ILykkeApiErrorCode PinCodeMismatch =
                new LykkeApiErrorCode(nameof(PinCodeMismatch), "The provided PIN does not match the customer's one");

            /// <summary>
            /// The provided PIN is not valid
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidPin =
                new LykkeApiErrorCode(nameof(InvalidPin), "The provided PIN is not valid");

            /// <summary>
            /// The customer has already set a PIN
            /// </summary>
            public static readonly ILykkeApiErrorCode PinAlreadySet =
                new LykkeApiErrorCode(nameof(PinAlreadySet), "The customer has already set a PIN");

            /// <summary>
            /// The provided phone number is not a valid one
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidPhoneNumber =
                new LykkeApiErrorCode(nameof(InvalidPhoneNumber), "The provided phone number is not a valid one");
            
            /// <summary>
            /// The price in the spend rule is not valid for this kind of operation
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidPriceInSpendRule =
                new LykkeApiErrorCode(nameof(InvalidPriceInSpendRule), "The price in the spend rule is not valid for this kind of operation");

            /// <summary>
            /// No vouchers in stock
            /// </summary>
            public static readonly ILykkeApiErrorCode NoVouchersInStock  =
                new LykkeApiErrorCode(nameof(NoVouchersInStock), "No vouchers in stock");

            /// <summary>
            /// Error when there is no linking request for the customer
            /// </summary>
            public static readonly ILykkeApiErrorCode PublicBlockchainIsDisabled =
                new LykkeApiErrorCode(nameof(PublicBlockchainIsDisabled), "Interaction with the public blockchain is disabled");

            /// <summary>
            /// Error for non existing Smart voucher campaign
            /// </summary>
            public static readonly ILykkeApiErrorCode SmartVoucherCampaignNotFound =
                new LykkeApiErrorCode(nameof(SmartVoucherCampaignNotFound), "Smart voucher campaign not found");

            /// <summary>
            /// Error for not active smart voucher campaign
            /// </summary>
            public static readonly ILykkeApiErrorCode SmartVoucherCampaignNotActive =
                new LykkeApiErrorCode(nameof(SmartVoucherCampaignNotActive), "Smart voucher campaign not active");

            /// <summary>
            /// Error for no available vouchers for a smart voucher campaign
            /// </summary>
            public static readonly ILykkeApiErrorCode NoAvailableVouchers =
                new LykkeApiErrorCode(nameof(NoAvailableVouchers), "Smart voucher campaign does not have any available voucher");

            /// <summary>
            /// Error for problem with payment provider for a smart voucher campaign
            /// </summary>
            public static readonly ILykkeApiErrorCode PaymentProviderError =
                new LykkeApiErrorCode(nameof(PaymentProviderError), "There is problem with payment provider.");

            /// <summary>
            /// Error for not existing smart voucher
            /// </summary>
            public static readonly ILykkeApiErrorCode SmartVoucherNotFound =
                new LykkeApiErrorCode(nameof(SmartVoucherNotFound), "Smart voucher could not be found");

            /// <summary>
            /// Payment info is missing
            /// </summary>
            public static readonly ILykkeApiErrorCode PaymentInfoNotFound =
                new LykkeApiErrorCode(nameof(PaymentInfoNotFound), "Payment info for smart voucher could not be found");

            /// <summary>
            /// The customer is already linked to a partner
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerAlreadyLinkedToAPartner =
                new LykkeApiErrorCode(nameof(CustomerAlreadyLinkedToAPartner), "The customer is already linked to a partner");

            /// <summary>
            /// Partner linking info is missing
            /// </summary>
            public static readonly ILykkeApiErrorCode PartnerLinkingInfoDoesNotExist =
                new LykkeApiErrorCode(nameof(PartnerLinkingInfoDoesNotExist), "Partner linking info is missing");

            /// <summary>
            /// Partner linking info does not match
            /// </summary>
            public static readonly ILykkeApiErrorCode PartnerLinkingInfoDoesNotMatch =
                new LykkeApiErrorCode(nameof(PartnerLinkingInfoDoesNotMatch), "Partner linking info does not match");

            /// <summary>
            /// Wrong smart voucher validation code
            /// </summary>
            public static readonly ILykkeApiErrorCode WrongSmartVoucherValidationCode =
                new LykkeApiErrorCode(nameof(WrongSmartVoucherValidationCode), "Wrong smart voucher validation code");

            /// <summary>
            /// There is no linked partner to the seller customer
            /// </summary>
            public static readonly ILykkeApiErrorCode SellerCustomerIsNotALinkedPartner =
                new LykkeApiErrorCode(nameof(SellerCustomerIsNotALinkedPartner), "There is no linked partner to the seller customer");

            /// <summary>
            /// The linked partner to the seller customer is not the issuer of the voucher
            /// </summary>
            public static readonly ILykkeApiErrorCode SellerCustomerIsNotTheVoucherIssuer =
                new LykkeApiErrorCode(nameof(SellerCustomerIsNotTheVoucherIssuer), "The linked partner to the seller customer is not the issuer of the voucher");
        }
        
        /// <summary>
        ///     Group for all model validation error codes.
        /// </summary>
        public static class ModelValidation
        {
            /// <summary>
            ///     Common error code for any failed validation.
            ///     Use it as default validation error code if specific code is not required.
            /// </summary>
            public static readonly ILykkeApiErrorCode ModelValidationFailed =
                new LykkeApiErrorCode(nameof(ModelValidationFailed), "The model is invalid.");
        }
    }
}
