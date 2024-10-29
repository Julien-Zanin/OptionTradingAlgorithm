using OptionTradingAlgorithm.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionTradingAlgorithm.Trading
{
    public class OptionPosition
    {
        public Option Option { get; set; }
        public int Quantity { get; set; } // Positive pour une position longue, négative pour une position courte
        public double Premium { get; set; } // Prix de l'option

        public OptionPosition(Option option, int quantity, double premium)
        {
            Option = option;
            Quantity = quantity;
            Premium = premium;
        }

        public double Payoff(double underlyingPrice)
        {
            if (Option != null)
            {
                double intrinsicValue = Option.Payoff(underlyingPrice);
                // Si la position est vendue (quantité négative), inverser le payoff
                return intrinsicValue * Quantity;
            }
            else
            {
                // Pour les positions sur le sous-jacent
                return underlyingPrice * Quantity;
            }
        }

    }

}

