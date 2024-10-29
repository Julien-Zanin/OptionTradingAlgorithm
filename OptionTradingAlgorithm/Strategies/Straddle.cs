using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class Straddle : Strategy
    {
        private double _strikePrice;
        private DateTime _expirationDate;
        private DateTime _pricingDate;
        private Market _market;
        private PriceEngine _priceEngine;

        private string _pricingMethod;
        private int _steps;

        public Straddle(double strikePrice, DateTime expirationDate, DateTime pricingDate, Market market, string pricingMethod = "Auto", int steps = 50)
            : base("Straddle")
        {
            _strikePrice = strikePrice;
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
            // Créer une option call
            EuropeanOption callOption = new EuropeanOption(_strikePrice, _expirationDate, "Call", _pricingDate);
            double callPremium = _priceEngine.PriceOption(callOption, _market, _pricingMethod);

            // Créer une option put
            EuropeanOption putOption = new EuropeanOption(_strikePrice, _expirationDate, "Put", _pricingDate);
            double putPremium = _priceEngine.PriceOption(putOption, _market, _pricingMethod);

            // Ajouter les positions
            AddPosition(new OptionPosition(callOption, 1, callPremium));
            AddPosition(new OptionPosition(putOption, 1, putPremium));
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
