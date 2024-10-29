using System;
using OptionTradingAlgorithm.Modele;

namespace OptionTradingAlgorithm.Pricing
{
    public class Tree
    {
        public int N { get; set; }
        public Option Option { get; set; } // Changement ici
        public Market Market { get; set; }
        public Node Root { get; set; } = null; // Initialisation de Root
        public double Dt { get; set; }
        public double Alpha { get; set; }
        public double PruningThreshold { get; set; } = 1e-9;

        public Tree(int n, Option option, Market market)
        {
            N = n;
            Option = option;
            Market = market;
            Dt = Option.Maturity / N;
            Alpha = Math.Exp(Market.Sigma * Math.Sqrt(3 * Dt));
            TreeConstructor();
        }

        public void TreeConstructor()
        {
            double S = Market.StockPrice;
            double r = Market.RiskFreeRate;
            double dt = Dt;
            int N = this.N;
            double t = 0.0;
            Node node = new Node(S, 1.0, this); // p_cumulative=1
            Root = node;
            Node trunk = node;

            for (int j = 0; j < N; j++)
            {
                double D = Market.TotalDividend(Option.PricingDate, t, t + dt);

                Node nextMidNode = new Node(trunk.SpotPrice * Math.Exp(r * dt) - D, 0.0, this);

                node = trunk;

                while (node != null)
                {
                    nextMidNode = node.NextStepSetup(node.SpotPrice * Math.Exp(r * dt) - D, nextMidNode, D);
                    node = node.TopNode;
                }

                node = trunk.BottomNode;
                nextMidNode = trunk.NextNodeMid;

                while (node != null)
                {
                    nextMidNode = node.NextStepSetup(node.SpotPrice * Math.Exp(r * dt) - D, nextMidNode, D);
                    node = node.BottomNode;
                }

                node = trunk.NextNodeMid;
                trunk = node;
                t += dt;
            }
        }
        // Cette fonction permet de récupérer valeur la plus haute et la plus basse en terme de prix en (sachant qu'on utilise la méthode du pruning)
        public (double maxPrice, double minPrice) GetExtremePricesAtMaturity()
        {
            List<double> pricesAtMaturity = new List<double>();

            // Naviguer jusqu'au dernier niveau de l'arbre
            Node currentNode = Root;
            while (currentNode.NextNodeMid != null)
            {
                currentNode = currentNode.NextNodeMid;
            }

            // Parcourir tous les nœuds du dernier niveau
            Stack<Node> stack = new Stack<Node>();
            stack.Push(currentNode);

            while (stack.Count > 0)
            {
                Node node = stack.Pop();
                pricesAtMaturity.Add(node.SpotPrice);

                if (node.TopNode != null)
                    stack.Push(node.TopNode);
                if (node.BottomNode != null)
                    stack.Push(node.BottomNode);
            }

            double maxPrice = pricesAtMaturity.Max();
            double minPrice = pricesAtMaturity.Min();

            return (maxPrice, minPrice);
        }
    }
}
