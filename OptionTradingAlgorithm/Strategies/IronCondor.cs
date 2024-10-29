using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class IronCondor : Strategy
    {
        private double _lowerPutStrike;
        private double _higherPutStrike;
        private double _lowerCallStrike;
        private double _higherCallStrike;
        private DateTime _expirationDate;
        private DateTime _pricingDate;
        private Market _market;
        private PriceEngine _priceEngine;

        private string _pricingMethod;
        private int _steps;

        public IronCondor(double lowerPutStrike, double higherPutStrike, double lowerCallStrike, double higherCallStrike, DateTime expirationDate, DateTime pricingDate, Market market, string pricingMethod = "Auto", int steps = 50)
            : base("Iron Condor")
        {
            _lowerPutStrike = lowerPutStrike;
            _higherPutStrike = higherPutStrike;
            _lowerCallStrike = lowerCallStrike;
            _higherCallStrike = higherCallStrike;
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
            // Acheter une option Put avec le strike inférieur (_lowerPutStrike)
            EuropeanOption longPut = new EuropeanOption(_lowerPutStrike, _expirationDate, "Put", _pricingDate);
            double premiumLongPut = _priceEngine.PriceOption(longPut, _market, _pricingMethod);
            AddPosition(new OptionPosition(longPut, 1, premiumLongPut));

            // Vendre une option Put avec le strike supérieur (_higherPutStrike)
            EuropeanOption shortPut = new EuropeanOption(_higherPutStrike, _expirationDate, "Put", _pricingDate);
            double premiumShortPut = _priceEngine.PriceOption(shortPut, _market, _pricingMethod);
            AddPosition(new OptionPosition(shortPut, -1, premiumShortPut));

            // Vendre une option Call avec le strike inférieur (_lowerCallStrike)
            EuropeanOption shortCall = new EuropeanOption(_lowerCallStrike, _expirationDate, "Call", _pricingDate);
            double premiumShortCall = _priceEngine.PriceOption(shortCall, _market, _pricingMethod);
            AddPosition(new OptionPosition(shortCall, -1, premiumShortCall));

            // Acheter une option Call avec le strike supérieur (_higherCallStrike)
            EuropeanOption longCall = new EuropeanOption(_higherCallStrike, _expirationDate, "Call", _pricingDate);
            double premiumLongCall = _priceEngine.PriceOption(longCall, _market, _pricingMethod);
            AddPosition(new OptionPosition(longCall, 1, premiumLongCall));
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
            double initialCost = +InitialCost();
            double maxProfit = +initialCost;
            double maxLoss = (_higherPutStrike - _lowerPutStrike) - initialCost;
            return (maxProfit, maxLoss);
        }
    }
}
