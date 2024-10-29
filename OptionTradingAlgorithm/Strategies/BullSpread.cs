using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class BullSpread : Strategy
    {
        private double _lowerStrike;
        private double _higherStrike;
        private DateTime _expirationDate;
        private DateTime _pricingDate;
        private Market _market;
        private PriceEngine _priceEngine;

        private string _pricingMethod;
        private int _steps;

        public BullSpread(
            double lowerStrike,
            double higherStrike,
            DateTime expirationDate,
            DateTime pricingDate,
            Market market,
            string pricingMethod = "Auto",
            int steps = 50)
            : base("Bull Spread")
        {
            _lowerStrike = lowerStrike;
            _higherStrike = higherStrike;
            _expirationDate = expirationDate;
            _pricingDate = pricingDate;
            _market = market;
            _priceEngine = new PriceEngine();

            _pricingMethod = pricingMethod;
            _steps = steps;

            PricingMethod = pricingMethod; // Stocker la méthode de pricing utilisée

            SetupPositions();
        }

        public override void SetupPositions()
        {
            // Acheter une option Call avec le strike le plus bas
            EuropeanOption longCall = new EuropeanOption(_lowerStrike, _expirationDate, "Call", _pricingDate);
            double longCallPremium = _priceEngine.PriceOption(longCall, _market, _pricingMethod);
            AddPosition(new OptionPosition(longCall, 1, longCallPremium));

            // Vendre une option Call avec le strike le plus haut
            EuropeanOption shortCall = new EuropeanOption(_higherStrike, _expirationDate, "Call", _pricingDate);
            double shortCallPremium = _priceEngine.PriceOption(shortCall, _market, _pricingMethod);
            AddPosition(new OptionPosition(shortCall, -1, shortCallPremium));
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

            double maxProfit = (_higherStrike - _lowerStrike) - initialCost;
            double maxLoss = -initialCost;

            return (maxProfit, maxLoss);
        }
    }
}
