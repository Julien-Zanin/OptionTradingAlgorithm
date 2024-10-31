using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class BearSpread : Strategy
    {
        private double _higherStrikePrice;
        private double _lowerStrikePrice;
        private DateTime _expirationDate;
        private DateTime _pricingDate;
        private Market _market;
        private PriceEngine _priceEngine;

        private string _pricingMethod;
        private int _steps;

        public BearSpread(double higherStrikePrice, double lowerStrikePrice, DateTime expirationDate, DateTime pricingDate, Market market, string pricingMethod = "Auto", int steps = 50)
            : base("Bear Spread")
        {
            _higherStrikePrice = higherStrikePrice;
            _lowerStrikePrice = lowerStrikePrice;
            _expirationDate = expirationDate;
            _pricingDate = pricingDate;
            _market = market;
            _priceEngine = new PriceEngine();

            _pricingMethod = pricingMethod;
            _steps = steps;

            PricingMethod = pricingMethod;

            SetupPositions();
        }

        public override void SetupPositions()
        {
            // Acheter une option Put avec le strike le plus haut
            EuropeanOption longPutHigh = new EuropeanOption(_higherStrikePrice, _expirationDate, "Put", _pricingDate);
            double premiumLongHigh = _priceEngine.PriceOption(longPutHigh, _market, _pricingMethod);
            AddPosition(new OptionPosition(longPutHigh, +1, premiumLongHigh));

            // Vendre une option Put avec le strike le plus bas
            EuropeanOption shortPutLow = new EuropeanOption(_lowerStrikePrice, _expirationDate, "Put", _pricingDate);
            double premiumShortLow = _priceEngine.PriceOption(shortPutLow, _market, _pricingMethod);
            AddPosition(new OptionPosition(shortPutLow, -1, premiumShortLow));
        }

        public override double CalculatePayoff(double underlyingPrice)
        {
            double totalPayoff = 0.0;
            foreach (var position in Positions)
            {
                double positionPayoff = position.Payoff(underlyingPrice);
                totalPayoff += positionPayoff;
            }
            return totalPayoff;
        }

        public override (double maxProfit, double maxLoss) CalculateMaxProfitAndLoss()
        {
            double initialCost = InitialCost();
            double maxProfit = (_higherStrikePrice - _lowerStrikePrice) - initialCost;
            double maxLoss = -initialCost;
            return (maxProfit, maxLoss);
        }
    }
}
