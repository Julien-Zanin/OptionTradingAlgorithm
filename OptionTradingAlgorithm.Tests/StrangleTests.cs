using Xunit;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Strategies;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm.Tests
{
    public class StrangleTests
    {
        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceBetweenStrikes_ReturnsZeroPayoff()
        {
            // Arrange
            double lowerStrikePrice = 95;
            double higherStrikePrice = 105;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 100,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            Strangle strangle = new Strangle(
                lowerStrikePrice: lowerStrikePrice,
                higherStrikePrice: higherStrikePrice,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 100;
            double expectedPayoff = 0;

 
            double actualPayoff = strangle.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }

        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceFarBelowLowerStrike_ReturnsPositivePayoff()
        {
            // Arrange
            double lowerStrikePrice = 95;
            double higherStrikePrice = 105;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 90,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            Strangle strangle = new Strangle(
                lowerStrikePrice: lowerStrikePrice,
                higherStrikePrice: higherStrikePrice,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 80;
            double expectedPayoff = lowerStrikePrice - underlyingPrice;

 
            double actualPayoff = strangle.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }
    }
}
