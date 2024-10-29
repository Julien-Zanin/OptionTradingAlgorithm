using Xunit;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Strategies;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm.Tests
{
    public class CoveredCallTests
    {
        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceBelowStrike_ReturnsExpectedPayoff()
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

            CoveredCall coveredCall = new CoveredCall(
                strikePrice: strikePrice,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 90;

            // Expected payoff is underlyingPrice - initialStockPrice
            double expectedPayoff = underlyingPrice - initialStockPrice;

 
            double actualPayoff = coveredCall.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }

        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceAboveStrike_ReturnsCappedPayoff()
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

            CoveredCall coveredCall = new CoveredCall(
                strikePrice: strikePrice,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 110;

            // Expected payoff is strikePrice - initialStockPrice
            double expectedPayoff = strikePrice - initialStockPrice;

 
            double actualPayoff = coveredCall.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }
    }
}
