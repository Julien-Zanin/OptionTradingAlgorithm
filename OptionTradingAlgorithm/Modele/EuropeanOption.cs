namespace OptionTradingAlgorithm.Modele
{
    public class EuropeanOption : Option
    {
        public EuropeanOption(double strikePrice, DateTime expirationDate, string contractType, DateTime pricingDate)
            : base(strikePrice, expirationDate, contractType, pricingDate)
        {
            Nature = "European";
        }

        public override double Payoff(double spotPrice)
        {
            if (ContractType == "Call")
            {
                return Math.Max(spotPrice - StrikePrice, 0);
            }
            else // Put
            {
                return Math.Max(StrikePrice - spotPrice, 0);
            }
        }
    }
}
