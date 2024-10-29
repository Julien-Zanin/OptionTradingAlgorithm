using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionTradingAlgorithm.Modele
{
     public class Contract
    {
        public double StrikePrice { get; set; }
        public DateTime ExpirationDate  { get; set; }
        public string ContractType { get; set; } // Call ou Put 
        public string Nature { get; set; } // Européenne ou Américaine 
        public DateTime PricingDate { get; set; }
        public double Maturity
        {
            get
            {
                return (ExpirationDate - PricingDate).TotalDays / 365.0;
            }
        }
        public Contract(double strikePrice, DateTime expirationDate, string contractType, string nature, DateTime pricingDate)
        {
            StrikePrice = strikePrice;
            ExpirationDate = expirationDate;
            ContractType = contractType;
            Nature = nature;
            PricingDate = pricingDate;  
        }

        public double Payoff(double spotPrice)
        {
            if (ContractType == "Call")
                return Math.Max(spotPrice - StrikePrice, 0);
            else // Cas du Put
                return Math.Max(StrikePrice - spotPrice, 0);
        }
    }
}
