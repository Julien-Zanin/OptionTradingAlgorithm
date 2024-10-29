using Xunit;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Strategies;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm.Tests
{
    public class ProtectivePutTests
    {
        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceBelowStrike_ReturnsLimitedLoss()
        {
            // Arrange
            double strikePrice = 100;
            double initialStockPrice = 100;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: initialStockPrice,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            ProtectivePut protectivePut = new ProtectivePut(
                strikePrice: strikePrice,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 80;

            // Expected payoff is (underlyingPrice - initialStockPrice) + (strikePrice - underlyingPrice)
            double expectedPayoff = (underlyingPrice - initialStockPrice) + (strikePrice - underlyingPrice);

 
            double actualPayoff = protectivePut.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }

        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceAboveStrike_ReturnsUnlimitedProfit()
        {
            // Arrange
            double strikePrice = 100;
            double initialStockPrice = 100;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: initialStockPrice,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            ProtectivePut protectivePut = new ProtectivePut(
                strikePrice: strikePrice,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 120;

            // Expected payoff is (underlyingPrice - initialStockPrice)
            double expectedPayoff = underlyingPrice - initialStockPrice;

 
            double actualPayoff = protectivePut.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }
    }
}
