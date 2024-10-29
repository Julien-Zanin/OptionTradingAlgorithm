using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class Strangle : Strategy
    {
        private double _lowerStrikePrice;
        private double _higherStrikePrice;
        private DateTime _expirationDate;
        private DateTime _pricingDate;
        private Market _market;
        private PriceEngine _priceEngine;

        private string _pricingMethod;
        private int _steps;

        public Strangle(double lowerStrikePrice, double higherStrikePrice, DateTime expirationDate, DateTime pricingDate, Market market, string pricingMethod = "Auto", int steps = 50)
            : base("Strangle")
        {
            _lowerStrikePrice = lowerStrikePrice;
            _higherStrikePrice = higherStrikePrice;
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
            // Acheter une option Put avec un strike plus bas
            EuropeanOption putOption = new EuropeanOption(_lowerStrikePrice, _expirationDate, "Put", _pricingDate);
            double putPremium = _priceEngine.PriceOption(putOption, _market, _pricingMethod);
            AddPosition(new OptionPosition(putOption, 1, putPremium));

            // Acheter une option Call avec un strike plus haut
            EuropeanOption callOption = new EuropeanOption(_higherStrikePrice, _expirationDate, "Call", _pricingDate);
            double callPremium = _priceEngine.PriceOption(callOption, _market, _pricingMethod);
            AddPosition(new OptionPosition(callOption, 1, callPremium));
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
            double maxProfit = double.PositiveInfinity; // Théoriquement illimité
            double maxLoss = -initialCost;
            return (maxProfit, maxLoss);
        }
    }
}
