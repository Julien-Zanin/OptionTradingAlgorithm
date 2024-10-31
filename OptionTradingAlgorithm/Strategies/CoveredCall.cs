using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class CoveredCall : Strategy
    {
        private double _strikePrice;
        private DateTime _expirationDate;
        private DateTime _pricingDate;
        private Market _market;
        private PriceEngine _priceEngine;

        private string _pricingMethod;
        private int _steps;

        public CoveredCall(double strikePrice, DateTime expirationDate, DateTime pricingDate, Market market, string pricingMethod = "Auto", int steps = 50)
            : base("Covered Call")
        {
            _strikePrice = strikePrice;
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
            // Acheter l'actif sous-jacent
            AddPosition(new OptionPosition(null, 1, -_market.StockPrice)); // Null pour l'option, -Prix car c'est un coût

            // Vendre une option Call
            EuropeanOption shortCall = new EuropeanOption(_strikePrice, _expirationDate, "Call", _pricingDate);
            double premiumShortCall = _priceEngine.PriceOption(shortCall, _market, _pricingMethod);
            AddPosition(new OptionPosition(shortCall, -1, premiumShortCall));
        }

        public override double CalculatePayoff(double underlyingPrice)
        {
            double totalPayoff = 0.0;

            // Payoff de la position sur le sous-jacent
            double stockPayoff = underlyingPrice - _market.StockPrice;

            // Payoff de l'option vendue
            double optionPayoff = Positions[1].Payoff(underlyingPrice);

            totalPayoff = stockPayoff + optionPayoff;
            return totalPayoff;
        }

        public override (double maxProfit, double maxLoss) CalculateMaxProfitAndLoss()
        {
            double initialCost = InitialCost();
            double maxProfit = (_strikePrice - _market.StockPrice) + Positions[1].Premium;
            double maxLoss = -(_market.StockPrice - Positions[1].Premium);
            return (maxProfit, maxLoss);
        }

        public override double InitialCost()
        {
            double totalCost = 0.0;

            // Coût de l'achat du sous-jacent
            totalCost += _market.StockPrice * -1; // Négatif car c'est un coût

            // Prime reçue de l'option vendue
            totalCost += Positions[1].Premium * Positions[1].Quantity;

            return totalCost;
        }
    }
}
