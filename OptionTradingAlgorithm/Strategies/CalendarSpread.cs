using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Trading;
using System;

namespace OptionTradingAlgorithm.Strategies
{
    public class CalendarSpread : Strategy
    {
        private double _strikePrice;
        private DateTime _nearExpirationDate;
        private DateTime _farExpirationDate;
        private DateTime _pricingDate;
        private string _optionType; // "Call" ou "Put"
        private Market _market;
        private PriceEngine _priceEngine;

        private string _pricingMethod;
        private int _steps;

        public CalendarSpread(double strikePrice, DateTime nearExpirationDate, DateTime farExpirationDate, DateTime pricingDate, string optionType, Market market, string pricingMethod = "Auto", int steps = 50)
            : base("Calendar Spread")
        {
            _strikePrice = strikePrice;
            _nearExpirationDate = nearExpirationDate;
            _farExpirationDate = farExpirationDate;
            _pricingDate = pricingDate;
            _optionType = optionType;
            _market = market;
            _priceEngine = new PriceEngine();

            _pricingMethod = pricingMethod;
            _steps = steps;

            PricingMethod = pricingMethod; // Stocker la méthode de pricing utilisée

            SetupPositions();
        }

        public override void SetupPositions()
        {
            // Vendre une option proche de l'échéance
            EuropeanOption shortOption = new EuropeanOption(_strikePrice, _nearExpirationDate, _optionType, _pricingDate);
            double premiumShort = _priceEngine.PriceOption(shortOption, _market, _pricingMethod);
            AddPosition(new OptionPosition(shortOption, -1, premiumShort));

            // Acheter une option avec une échéance plus lointaine
            EuropeanOption longOption = new EuropeanOption(_strikePrice, _farExpirationDate, _optionType, _pricingDate);
            double premiumLong = _priceEngine.PriceOption(longOption, _market, _pricingMethod);
            AddPosition(new OptionPosition(longOption, 1, premiumLong));
        }

        public override double CalculatePayoff(double underlyingPrice)
        {
            double totalPayoff = 0.0;

            // Payoff de l'option courte à son expiration
            OptionPosition shortPosition = Positions[0];
            double shortOptionPayoff = shortPosition.Payoff(underlyingPrice);
            totalPayoff += shortOptionPayoff;

            // Valeur de l'option longue à la date d'expiration de l'option courte
            OptionPosition longPosition = Positions[1];

            // Mettre à jour la date de pricing du marché
            Market updatedMarket = new Market(
                stockPrice: underlyingPrice,
                riskFreeRate: _market.RiskFreeRate,
                sigma: _market.Sigma,
                dividends: _market.Dividends,
                pricingDate: _nearExpirationDate // Date actuelle est la date d'expiration de l'option courte
            );

            // Calculer la valeur de l'option longue restante
            EuropeanOption remainingLongOption = new EuropeanOption(
                longPosition.Option.StrikePrice,
                longPosition.Option.ExpirationDate,
                longPosition.Option.ContractType,
                _nearExpirationDate // Nouvelle date de pricing
            );

            double remainingLongOptionValue = _priceEngine.PriceOption(remainingLongOption, updatedMarket, _pricingMethod);

            totalPayoff += remainingLongOptionValue * longPosition.Quantity;
            return totalPayoff;
        }

        public override (double maxProfit, double maxLoss) CalculateMaxProfitAndLoss()
        {
            // Le calcul du profit et de la perte maximale pour un Calendar Spread est complexe
            // et dépend de plusieurs facteurs, y compris la volatilité future.
            // Pour simplifier, la perte maximale est généralement le coût net initial.

            double initialCost = InitialCost();
            double maxLoss = -initialCost;
            double maxProfit = double.PositiveInfinity; // Difficile à estimer précisément

            return (maxProfit, maxLoss);
        }
    }
}
