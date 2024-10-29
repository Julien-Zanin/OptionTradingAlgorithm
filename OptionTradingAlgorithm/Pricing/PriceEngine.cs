using MathNet.Numerics.Distributions;
using OptionTradingAlgorithm.Modele;
namespace OptionTradingAlgorithm.Pricing
{
    public class PriceEngine
    {
        public double PriceOption(Option option, Market market, string pricingMethod = "Auto", int steps = 2000)
        {
            if (pricingMethod == "Tree")
            {
                return PriceOptionWithTree(option, market, steps);
            }
            else if (pricingMethod == "BlackScholes")
            {
                if (option is EuropeanOption europeanOption)
                {
                    return BlackScholesPrice(europeanOption, market);
                }
                else
                {
                    throw new NotSupportedException("Black-Scholes is only applicable to European options.");
                }
            }
            else // "Auto"
            {
                if (option is AmericanOption)
                {
                    return PriceOptionWithTree(option, market, steps);
                }
                else if (option is EuropeanOption europeanOption)
                {
                    return BlackScholesPrice(europeanOption, market);
                }
                else
                {
                    throw new NotSupportedException("Option type not supported.");
                }
            }
        }

        public double PriceOptionWithTree(Option option, Market market, int steps)
        {
            Tree tree = new Tree(steps, option, market);
            return PriceOptionWithTree(tree);
        }

        private double PriceOptionWithTree(Tree tree)
        {
            double r = tree.Market.RiskFreeRate;
            double dt = tree.Dt;

            // Naviguer jusqu'au dernier nœud milieu
            Node midNodeN = tree.Root;
            while (midNodeN.NextNodeMid != null)
            {
                midNodeN = midNodeN.NextNodeMid;
            }

            // Calculer les payoffs aux derniers nœuds
            Node node = midNodeN;

            // Parcours des nœuds vers le haut
            while (node != null)
            {
                node.ContractPrice = tree.Option.Payoff(node.SpotPrice);
                node = node.TopNode;
            }

            // Parcours des nœuds vers le bas
            node = midNodeN.BottomNode;
            while (node != null)
            {
                node.ContractPrice = tree.Option.Payoff(node.SpotPrice);
                node = node.BottomNode;
            }

            // Effectuer le backward pricing
            BackwardPricing(midNodeN, tree.Option, dt, r);

            return tree.Root.ContractPrice;
        }

        private void BackwardPricing(Node lastMidNode, Option option, double dt, double r)
        {
            // Parcourir l'arbre en arrière pour calculer le prix de l'option
            while (lastMidNode.PreviousNode != null)
            {
                lastMidNode = lastMidNode.PreviousNode;

                Node node = lastMidNode;

                // Parcours des nœuds vers le haut
                while (node != null)
                {
                    double presentValue = node.PMid * node.NextNodeMid.ContractPrice;

                    if (node.NextNodeMid.TopNode != null)
                    {
                        presentValue += node.PUp * node.NextNodeMid.TopNode.ContractPrice;
                    }

                    if (node.NextNodeMid.BottomNode != null)
                    {
                        presentValue += node.PDown * node.NextNodeMid.BottomNode.ContractPrice;
                    }

                    presentValue *= Math.Exp(-r * dt);

                    if (option is AmericanOption)
                    {
                        double payoff = option.Payoff(node.SpotPrice);
                        node.ContractPrice = Math.Max(presentValue, payoff);
                        node.EarlyExercise = node.ContractPrice == payoff;
                    }
                    else
                    {
                        node.ContractPrice = presentValue;
                    }

                    node = node.TopNode;
                }

                // Parcours des nœuds vers le bas
                node = lastMidNode.BottomNode;
                while (node != null)
                {
                    double presentValue = node.PMid * node.NextNodeMid.ContractPrice;

                    if (node.NextNodeMid.TopNode != null)
                    {
                        presentValue += node.PUp * node.NextNodeMid.TopNode.ContractPrice;
                    }

                    if (node.NextNodeMid.BottomNode != null)
                    {
                        presentValue += node.PDown * node.NextNodeMid.BottomNode.ContractPrice;
                    }

                    presentValue *= Math.Exp(-r * dt);

                    if (option is AmericanOption)
                    {
                        double payoff = option.Payoff(node.SpotPrice);
                        node.ContractPrice = Math.Max(presentValue, payoff);
                        node.EarlyExercise = node.ContractPrice == payoff;
                    }
                    else
                    {
                        node.ContractPrice = presentValue;
                    }

                    node = node.BottomNode;
                }
            }
        }

        public double BlackScholesPrice(EuropeanOption option, Market market)
        {
            double S = market.StockPrice;
            double K = option.StrikePrice;
            double T = option.Maturity;
            double r = market.RiskFreeRate;
            double sigma = market.Sigma;

            double d1 = (Math.Log(S / K) + (r + 0.5 * sigma * sigma) * T) / (sigma * Math.Sqrt(T));
            double d2 = d1 - sigma * Math.Sqrt(T);

            if (option.ContractType == "Call")
            {
                return S * Normal.CDF(0, 1, d1) - K * Math.Exp(-r * T) * Normal.CDF(0, 1, d2);
            }
            else // Put
            {
                return K * Math.Exp(-r * T) * Normal.CDF(0, 1, -d2) - S * Normal.CDF(0, 1, -d1);
            }
        }
    }
}
