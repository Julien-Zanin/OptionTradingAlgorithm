using Xunit;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Strategies;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm.Tests
{
    public class IronCondorTests
    {
        [Fact]
        public void CalculatePayoff_WhenUnderlyingPriceBetweenMiddleStrikes_ReturnsZeroPayoff()
        {
            // Arrange
            double lowerPutStrike = 90;
            double higherPutStrike = 95;
            double lowerCallStrike = 105;
            double higherCallStrike = 110;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 100,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            IronCondor ironCondor = new IronCondor(
                lowerPutStrike: lowerPutStrike,
                higherPutStrike: higherPutStrike,
                lowerCallStrike: lowerCallStrike,
                higherCallStrike: higherCallStrike,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 100;
            double expectedPayoff = 0;

 
            double actualPayoff = ironCondor.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }

        [Fact]        
        public void CalculatePayoff_WhenUnderlyingPriceBelowLowerPutStrike_ReturnsExpectedPayoff_test()
        {
            // Arrange
            double lowerPutStrike = 90;
            double higherPutStrike = 95;
            double lowerCallStrike = 105;
            double higherCallStrike = 110;
            DateTime expirationDate = DateTime.Today.AddMonths(3);
            DateTime pricingDate = DateTime.Today;

            Market market = new Market(
                stockPrice: 85,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(),
                pricingDate: pricingDate
            );

            IronCondor ironCondor = new IronCondor(
                lowerPutStrike: lowerPutStrike,
                higherPutStrike: higherPutStrike,
                lowerCallStrike: lowerCallStrike,
                higherCallStrike: higherCallStrike,
                expirationDate: expirationDate,
                pricingDate: pricingDate,
                market: market
            );

            double underlyingPrice = 85;

            // Calculer le payoff attendu en utilisant la formule
            // Payoff lorsque S_t < K1 = K1 - K2 
            double expectedPayoff = lowerPutStrike - higherPutStrike;

            double actualPayoff = ironCondor.CalculatePayoff(underlyingPrice);

            // Assert
            Assert.Equal(expectedPayoff, actualPayoff, 2);
        }

    }
}
