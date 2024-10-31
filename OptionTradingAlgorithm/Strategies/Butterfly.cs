using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class ButterflyStrategy : Strategy
    {
        private double _lowerStrike;
        private double _middleStrike;
        private double _higherStrike;
        private DateTime _expirationDate;
        private DateTime _pricingDate;
        private Market _market;
        private PriceEngine _priceEngine;
        private string _pricingMethod;
        private int _steps;

        public ButterflyStrategy(
            double lowerStrike,
            double middleStrike,
            double higherStrike,
            DateTime expirationDate,
            DateTime pricingDate,
            Market market,
            string pricingMethod = "Auto",
            int steps = 50)
            : base("Butterfly Spread")
        {
            _lowerStrike = lowerStrike;
            _middleStrike = middleStrike;
            _higherStrike = higherStrike;
            _expirationDate = expirationDate;
            _pricingDate = pricingDate;
            _market = market;
            _pricingMethod = pricingMethod;
            _steps = steps;
            _priceEngine = new PriceEngine();

            PricingMethod = pricingMethod;

            SetupPositions();
        }

        public override void SetupPositions()
        {
            // Acheter une option Call avec le prix d'exercice le plus bas
            EuropeanOption longCallLow = new EuropeanOption(_lowerStrike, _expirationDate, "Call", _pricingDate);
            double premiumLongCallLow = _priceEngine.PriceOption(longCallLow, _market, _pricingMethod, _steps);
            AddPosition(new OptionPosition(longCallLow, 1, premiumLongCallLow));

            // Vendre deux options Call avec le prix d'exercice moyen
            EuropeanOption shortCallMiddle = new EuropeanOption(_middleStrike, _expirationDate, "Call", _pricingDate);
            double premiumShortCallMiddle = _priceEngine.PriceOption(shortCallMiddle, _market, _pricingMethod, _steps);
            AddPosition(new OptionPosition(shortCallMiddle, -2, premiumShortCallMiddle));

            // Acheter une option Call avec le prix d'exercice le plus haut
            EuropeanOption longCallHigh = new EuropeanOption(_higherStrike, _expirationDate, "Call", _pricingDate);
            double premiumLongCallHigh = _priceEngine.PriceOption(longCallHigh, _market, _pricingMethod, _steps);
            AddPosition(new OptionPosition(longCallHigh, 1, premiumLongCallHigh));
        }

        public override double CalculatePayoff(double underlyingPrice)
        {
            double totalPayoff = 0.0;

            foreach (var position in Positions)
            {
                double payoff = position.Payoff(underlyingPrice);
                totalPayoff += payoff;
            }
            return totalPayoff;
        }

        public override (double maxProfit, double maxLoss) CalculateMaxProfitAndLoss()
        {
            double initialCost = InitialCost();
            double maxProfit = (_middleStrike - _lowerStrike) - initialCost;
            double maxLoss = -initialCost;
            return (maxProfit, maxLoss);
        }
    }
}
