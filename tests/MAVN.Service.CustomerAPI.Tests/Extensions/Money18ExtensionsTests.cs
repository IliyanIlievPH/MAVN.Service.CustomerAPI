using System.Globalization;
using Falcon.Numerics;
using MAVN.Service.CustomerAPI.Core;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Extensions
{
    public class Money18ExtensionsTests
    {
        [Theory]
        [InlineData("123456789.632342", "en-US", "123 456 789.632342", 6)]
        [InlineData("123456789.632342", "en-US", "123 456 789.63", 2)]
        [InlineData("123456789.636342", "en-US", "123 456 789.63", 2)] // we slice, don't round
        [InlineData("123456789.333333333333333333", "en-US", "123 456 789.333333333333333333", 18)]
        public void Returns_Expected_Display_Value(string value, string cultureName, string expectedResult,
            int numberDecimalPlaces)
        {
            // arrange

            var number = Money18.Parse(value);
            var numberFormat = new CultureInfo(cultureName).NumberFormat;

            // act

            var actualResult = number.ToDisplayString(numberFormat, numberDecimalPlaces);

            // assert

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
