using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class ProtectivePut : Strategy
    {
        private double _strikePrice;
        private DateTime _expirationDate;
        private DateTime _pricingDate;
        private Market _market;
        private PriceEngine _priceEngine;

        private string _pricingMethod;
        private int _steps;

        public ProtectivePut(double strikePrice, DateTime expirationDate, DateTime pricingDate, Market market, string pricingMethod = "Auto", int steps = 50)
            : base("Protective Put")
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
            // Acheter le sous-jacent
            AddPosition(new OptionPosition(null, 1, -_market.StockPrice)); // Null pour l'option, -Prix car c'est un coût

            // Acheter une option Put
            EuropeanOption longPut = new EuropeanOption(_strikePrice, _expirationDate, "Put", _pricingDate);
            double premiumLongPut = _priceEngine.PriceOption(longPut, _market, _pricingMethod);
            AddPosition(new OptionPosition(longPut, 1, premiumLongPut));
        }

        public override double CalculatePayoff(double underlyingPrice)
        {
            double totalPayoff = 0.0;

            // Payoff de la position sur le sous-jacent
            double stockPayoff = underlyingPrice - _market.StockPrice;

            // Payoff de l'option achetée
            double optionPayoff = Positions[1].Payoff(underlyingPrice);

            totalPayoff = stockPayoff + optionPayoff;
            return totalPayoff;
        }

        public override (double maxProfit, double maxLoss) CalculateMaxProfitAndLoss()
        {
            double initialCost = InitialCost();
            double maxProfit = double.PositiveInfinity; // Potentiel de gain illimité si le prix du sous-jacent augmente
            double maxLoss = (_market.StockPrice - _strikePrice) + Positions[1].Premium;
            return (maxProfit, maxLoss);
        }
    }
}
