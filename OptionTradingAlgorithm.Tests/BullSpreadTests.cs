using Xunit;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Strategies;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm.Tests
{
    public class BullSpreadTests
    {
        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceAboveHigherStrike_ReturnsMaximumPayoff()
        {
            // Arrange
            double lowerStrike = 100;
            double higherStrike = 110;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 120,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            BullSpread bullSpread = new BullSpread(
                lowerStrike: lowerStrike,
                higherStrike: higherStrike,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 120; // Prix du sous-jacent au-dessus du strike supérieur

            // Expected payoff is higherStrike - lowerStrike
            double expectedPayoff = higherStrike - lowerStrike;

 
            double actualPayoff = bullSpread.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }

        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceBelowLowerStrike_ReturnsZeroPayoff()
        {
            // Arrange
            double lowerStrike = 100;
            double higherStrike = 110;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 90,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            BullSpread bullSpread = new BullSpread(
                lowerStrike: lowerStrike,
                higherStrike: higherStrike,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 90; // Prix du sous-jacent en dessous du strike inférieur
            double expectedPayoff = 0; // Payoff nul

 
            double actualPayoff = bullSpread.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }
    }
}
