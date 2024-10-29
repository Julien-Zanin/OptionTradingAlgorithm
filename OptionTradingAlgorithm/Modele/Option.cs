using System;

namespace OptionTradingAlgorithm.Modele
{
    public abstract class Option
    {
        public double StrikePrice { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ContractType { get; set; } // "Call" ou "Put"
        public DateTime PricingDate { get; set; }

        public string Nature { get; set; }

        public double Maturity
        {
            get
            {
                return (ExpirationDate - PricingDate).TotalDays / 365.0;
            }
        }

        public Option(double strikePrice, DateTime expirationDate, string contractType, DateTime pricingDate)
        {
            StrikePrice = strikePrice;
            ExpirationDate = expirationDate;
            ContractType = contractType;
            PricingDate = pricingDate;
            Nature = "Option"; 
        }
 
    public abstract double Payoff(double spotPrice);
    }
}
