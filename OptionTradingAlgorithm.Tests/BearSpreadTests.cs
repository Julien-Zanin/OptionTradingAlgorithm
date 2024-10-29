using Xunit;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Strategies;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm.Tests
{
    public class BearSpreadTests
    {
        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceBelowLowerStrike_ReturnsMaximumPayoff()
        {
            // Arrange
            double higherStrike = 110;
            double lowerStrike = 100;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 100,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            BearSpread bearSpread = new BearSpread(
                higherStrikePrice: higherStrike,
                lowerStrikePrice: lowerStrike,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 90; // Prix du sous-jacent en dessous du strike inférieur

            // Expected payoff is higherStrike - lowerStrike
            double expectedPayoff = higherStrike - lowerStrike;

 
            double actualPayoff = bearSpread.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }

        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceAboveHigherStrike_ReturnsZeroPayoff()
        {
            // Arrange
            double higherStrike = 110;
            double lowerStrike = 100;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 120,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            BearSpread bearSpread = new BearSpread(
                higherStrikePrice: higherStrike,
                lowerStrikePrice: lowerStrike,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 120; // Prix du sous-jacent au-dessus du strike supérieur
            double expectedPayoff = 0; // Payoff nul

 
            double actualPayoff = bearSpread.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }
    }
}
