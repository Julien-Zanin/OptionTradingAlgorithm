using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm.Trading
{
    public abstract class Strategy
    {
        public string Name { get; set; }
        public List<OptionPosition> Positions { get; set; }
        public string PricingMethod { get; set; } // Ajouter la méthode de pricing utilisée

        public Strategy(string name)
        {
            Name = name;
            Positions = new List<OptionPosition>();
            PricingMethod = "Auto"; // Valeur par défaut
        }

        public abstract void SetupPositions();
        public abstract double CalculatePayoff(double underlyingPrice);
        public abstract (double maxProfit, double maxLoss) CalculateMaxProfitAndLoss();

        public void AddPosition(OptionPosition position)
        {
            Positions.Add(position);
        }

        public virtual double InitialCost()
        {
            double totalCost = 0.0;
            foreach (var position in Positions)
            {
                totalCost += position.Premium * position.Quantity;
            }
            return totalCost;
        }

        // Méthode pour générer les données de payoff
        public List<(double UnderlyingPrice, double Payoff)> GeneratePayoffData(double minPrice, double maxPrice, double step)
        {
            List<(double UnderlyingPrice, double Payoff)> payoffData = new List<(double, double)>();

            for (double price = minPrice; price <= maxPrice; price += step)
            {
                double payoff = CalculatePayoff(price);
                payoffData.Add((price, payoff));
            }

            return payoffData;
        }
    }
}
