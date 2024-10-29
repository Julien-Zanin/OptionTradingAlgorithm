using System.Text.RegularExpressions;

namespace OptionTradingAlgorithm.Modele
{
    public class AmericanOption : Option
    {
        public AmericanOption(double strikePrice, DateTime expirationDate, string contractType, DateTime pricingDate)
            : base(strikePrice, expirationDate, contractType, pricingDate)
        {
            Nature = "American";
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
