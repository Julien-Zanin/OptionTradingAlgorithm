using System;
using System.Collections.Generic;
using OptionTradingAlgorithm.Trading;

namespace OptionTradingAlgorithm.Strategies
{
    public class StrategyComparer
    {
        public List<Strategy> Strategies { get; set; }

        public StrategyComparer()
        {
            Strategies = new List<Strategy>();
        }

        public void AddStrategy(Strategy strategy)
        {
            Strategies.Add(strategy);
        }

        public void CompareStrategies(double underlyingPrice)
        {
            Console.WriteLine($"Comparaison des stratégies pour un prix du sous-jacent de {underlyingPrice:F2} :");
            foreach (var strategy in Strategies)
            {
                double payoff = strategy.CalculatePayoff(underlyingPrice);
                Console.WriteLine($" - {strategy.Name} : Payoff = {payoff:F2}");
            }
        }

        // Méthode pour comparer les stratégies sur une gamme de prix du sous-jacent
        public void CompareOverPriceRange(double startPrice, double endPrice, double step)
        {
            Console.WriteLine("Comparaison des stratégies sur une gamme de prix du sous-jacent :");
            for (double price = startPrice; price <= endPrice; price += step)
            {
                Console.WriteLine($"\nPrix du sous-jacent : {price:F2}");
                foreach (var strategy in Strategies)
                {
                    double payoff = strategy.CalculatePayoff(price);
                    Console.WriteLine($" - {strategy.Name} : Payoff = {payoff:F2}");
                }
            }
        }
    }
}
