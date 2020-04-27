using System;
using System.Threading.Tasks;
using Lykke.Logs;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CurrencyConvertor.Client.Models.Enums;
using Lykke.Service.CurrencyConvertor.Client.Models.Responses;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Services;
using Lykke.Service.EligibilityEngine.Client;
using Lykke.Service.EligibilityEngine.Client.Enums;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Requests;
using Lykke.Service.EligibilityEngine.Client.Models.ConversionRate.Responses;
using Moq;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests.Services
{
    public class CurrencyConverterTests
    {
        private readonly Mock<IEligibilityEngineClient> _eligibilityEngineClientMock =
            new Mock<IEligibilityEngineClient>();

        private readonly Mock<ISettingsService> _settingsServiceMock = new Mock<ISettingsService>();

        [Fact]
        public async Task GetCurrencyAmountInBaseCurrencyAsync_InvalidAmount_RaisesException()
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                sut.GetCurrencyAmountInBaseCurrencyAsync(-1, "any", "any", "any"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetCurrencyAmountInBaseCurrencyAsync_InvalidCurrency_RaisesException(string currency)
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                sut.GetCurrencyAmountInBaseCurrencyAsync(100, currency, "any", "any"));
        }

        [Fact]
        public async Task GetCurrencyAmountInBaseCurrencyAsync_ZeroAmount_ZeroResult()
        {
            var sut = CreateSutInstance();

            var result = await sut.GetCurrencyAmountInBaseCurrencyAsync(0, "any", "any", "any");

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetCurrencyAmountInBaseCurrencyAsync_BaseCurrencyIsTheSame_ReturnsSameAmount()
        {
            const string currency = "whatever";

            const decimal amount = 100;

            _settingsServiceMock
                .Setup(x => x.GetBaseCurrencyCode())
                .Returns(currency);

            _eligibilityEngineClientMock
                .Setup(x => x.ConversionRate.ConvertOptimalByPartnerAsync(It.IsAny<ConvertOptimalByPartnerRequest>()))
                .Verifiable();

            var sut = CreateSutInstance();

            var result = await sut.GetCurrencyAmountInBaseCurrencyAsync(amount, currency, "any", "any");

            _eligibilityEngineClientMock.Verify(
                x => x.ConversionRate.ConvertOptimalByPartnerAsync(It.IsAny<ConvertOptimalByPartnerRequest>()),
                Times.Never);

            Assert.Equal(amount, result);
        }

        [Fact]
        public async Task GetCurrencyAmountInBaseCurrencyAsync_BaseCurrencyIsDifferent_ReturnsResultFromConverter()
        {
            const decimal sourceAmount = 50;

            const decimal baseCurrencyAmount = 100;

            _eligibilityEngineClientMock
                .Setup(x => x.ConversionRate.ConvertOptimalByPartnerAsync(It.IsAny<ConvertOptimalByPartnerRequest>()))
                .ReturnsAsync(new ConvertOptimalByPartnerResponse {ErrorCode = EligibilityEngineErrors.None, Amount = baseCurrencyAmount})
                .Verifiable();

            var sut = CreateSutInstance();

            var result = await sut.GetCurrencyAmountInBaseCurrencyAsync(sourceAmount, "whatever", Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            _eligibilityEngineClientMock.Verify(
                x => x.ConversionRate.ConvertOptimalByPartnerAsync(It.IsAny<ConvertOptimalByPartnerRequest>()),
                Times.Once);

            Assert.Equal(baseCurrencyAmount, result);
        }

        [Fact]
        public async Task GetCurrencyAmountInTokensAsync_InvalidAmount_RaisesException()
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.GetCurrencyAmountInTokensAsync(-1, "any", "any", "any"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetCurrencyAmountInTokensAsync_InvalidCurrency_RaisesException(string currency)
        {
            var sut = CreateSutInstance();

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetCurrencyAmountInTokensAsync(100, currency, "any", "any"));
        }

        [Fact]
        public async Task GetCurrencyAmountInTokensAsync_ZeroAmount_ZeroResult()
        {
            var sut = CreateSutInstance();

            var result = await sut.GetCurrencyAmountInTokensAsync(0, "any", "any", "any");

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetCurrencyAmountInTokensAsync_CurrenciesAreTheSame_ReturnsSameAmount()
        {
            const string token = "whatever";

            const decimal amount = 100;

            _settingsServiceMock
                .Setup(x => x.GetTokenName())
                .Returns(token);

            _eligibilityEngineClientMock
                .Setup(x => x.ConversionRate.ConvertOptimalByPartnerAsync(It.IsAny<ConvertOptimalByPartnerRequest>()))
                .Verifiable();

            var sut = CreateSutInstance();

            var result = await sut.GetCurrencyAmountInTokensAsync(amount, token, "whatever", "whatever");

            _eligibilityEngineClientMock.Verify(
                x => x.ConversionRate.ConvertOptimalByPartnerAsync(It.IsAny<ConvertOptimalByPartnerRequest>()),
                Times.Never);

            Assert.Equal(amount, result);
        }

        [Fact]
        public async Task GetCurrencyAmountInTokensAsync_ReturnsResultFromConverter()
        {
            const decimal sourceAmount = 50;

            const decimal tokenAmount = 100;

            _eligibilityEngineClientMock
                .Setup(x => x.ConversionRate.ConvertOptimalByPartnerAsync(It.IsAny<ConvertOptimalByPartnerRequest>()))
                .ReturnsAsync(new ConvertOptimalByPartnerResponse {ErrorCode = EligibilityEngineErrors.None, Amount = tokenAmount})
                .Verifiable();

            _settingsServiceMock
                .Setup(x => x.GetTokenName())
                .Returns("any token");

            var sut = CreateSutInstance();

            var result = await sut.GetCurrencyAmountInTokensAsync(sourceAmount, "any fiat currency", Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            _eligibilityEngineClientMock.Verify(
                x => x.ConversionRate.ConvertOptimalByPartnerAsync(It.IsAny<ConvertOptimalByPartnerRequest>()),
                Times.Once);

            Assert.Equal(tokenAmount, result);
        }

        [Fact]
        public void GetBaseCurrencyCode_ReturnsFromSettings()
        {
            const string baseCurrencyCode = "base currency code";
            
            _settingsServiceMock
                .Setup(x => x.GetBaseCurrencyCode())
                .Returns(baseCurrencyCode)
                .Verifiable();

            var sut = CreateSutInstance();

            var result = sut.GetBaseCurrencyCode();
            
            _settingsServiceMock.Verify(x => x.GetBaseCurrencyCode(), Times.Once);
            
            Assert.Equal(baseCurrencyCode, result);
        }
        
        private ICurrencyConverter CreateSutInstance()
        {
            return new CurrencyConverter(
                _settingsServiceMock.Object,
                EmptyLogFactory.Instance,
                _eligibilityEngineClientMock.Object);
        }
    }
}
