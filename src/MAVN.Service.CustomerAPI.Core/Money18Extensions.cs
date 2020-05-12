using System.Globalization;
using MAVN.Numerics;

namespace MAVN.Service.CustomerAPI.Core
{
    public static class Money18Extensions
    {
        public static int NumberDecimalPlaces { get; set; } = 2; 
        private static CultureInfo DefaultCulture
        {
            get => CultureInfo.InvariantCulture;
        }

        public static string ToDisplayString(this Money18 value)
        {
            return ToDisplayString(value, DefaultCulture.NumberFormat, NumberDecimalPlaces);
        }

        public static string ToDisplayString(this Money18? value)
        {
            return value.HasValue ? ToDisplayString(value.Value, DefaultCulture.NumberFormat, NumberDecimalPlaces) : null;
        }

        public static string ToDisplayString(this Money18 value, NumberFormatInfo numberFormat, int numberDecimalPlaces)
        {
            return value.ToString("N0", numberDecimalPlaces, numberFormat);
        }
    }
}
