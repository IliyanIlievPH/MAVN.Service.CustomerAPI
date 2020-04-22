namespace MAVN.Service.CustomerAPI.Validation
{
    public static class Patterns
    {
        public const string NameValidationPattern =
            @"(^([a-zA-Z]+[\- ]?)*[a-zA-Z'’]+$)|(^([\u0600-\u065F\u066A-\u06EF\u06FA-\u06FF]+[\- ]?)*[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FF]+$)";

        public const string PhoneValidationPattern = @"^[0-9 A-Z a-z #;,()+*-]{1,30}$";

        public const string IbanValidationPattern = @"^[A-Z]{2}[A-Z\d]{15,32}$";

        public const string SwiftCodeValidationPattern = @"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$";

        public const string CustomerPhoneValidationPattern = "^[0-9]+$";

        public const string EmailValidationPattern =
            @"\A(?:[a-zA-Z0-9!#$%&'*/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?)\z";
    }
}
