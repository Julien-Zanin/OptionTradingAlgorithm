using Xunit;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Strategies;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm.Tests
{
    public class ButterflyStrategyTests
    {
        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceEqualsMiddleStrike_ReturnsMaximumPayoff()
        {
            // Arrange
            double lowerStrike = 90;
            double middleStrike = 100;
            double higherStrike = 110;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 100,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            ButterflyStrategy butterfly = new ButterflyStrategy(
                lowerStrike: lowerStrike,
                middleStrike: middleStrike,
                higherStrike: higherStrike,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = middleStrike;

            // Expected payoff is middleStrike - lowerStrike
            double expectedPayoff = middleStrike - lowerStrike;

 
            double actualPayoff = butterfly.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }

        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceOutsideStrikes_ReturnsZeroPayoff()
        {
            // Arrange
            double lowerStrike = 90;
            double middleStrike = 100;
            double higherStrike = 110;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 80,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            ButterflyStrategy butterfly = new ButterflyStrategy(
                lowerStrike: lowerStrike,
                middleStrike: middleStrike,
                higherStrike: higherStrike,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 80;
            double expectedPayoff = 0;

 
            double actualPayoff = butterfly.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }
    }
}
