using System;
using OptionTradingAlgorithm.Modele;

namespace OptionTradingAlgorithm.Pricing
{
    public class Node
    {
        public double SpotPrice { get; set; }
        public double ContractPrice { get; set; } = 0.0;
        public bool? EarlyExercise { get; set; } = null;
        public Node? NextNodeUp { get; set; } = null;
        public Node? NextNodeMid { get; set; } = null;
        public Node? NextNodeDown { get; set; } = null;
        public double PCumulative { get; set; } = 0.0;
        public double PUp { get; set; } = 0.0;
        public double PMid { get; set; } = 0.0;
        public double PDown { get; set; } = 0.0;
        public Node? TopNode { get; set; } = null;
        public Node? BottomNode { get; set; } = null;
        public Node? PreviousNode { get; set; } = null;
        public Tree? Tree { get; set; } = null;

        public Node(double spotPrice, double pCumulative, Tree tree)
        {
            SpotPrice = spotPrice;
            PCumulative = pCumulative;
            Tree = tree;
        }

        public Node GetTopNode(double alpha)
        {
            if (PreviousNode == null)
            {
                throw new InvalidOperationException("PreviousNode should not be null");
            }

            if (TopNode != null)
            {
                return TopNode;
            }
            else
            {
                double p = 0.0;
                if (PreviousNode != null)
                {
                    p += PreviousNode.PCumulative * PreviousNode.PUp;
                    if (PreviousNode.TopNode != null)
                    {
                        p += PreviousNode.TopNode.PCumulative * PreviousNode.TopNode.PMid;
                        if (PreviousNode.TopNode.TopNode != null)
                        {
                            p += PreviousNode.TopNode.TopNode.PCumulative * PreviousNode.TopNode.TopNode.PDown;
                        }
                    }
                }

                if (p > Tree.PruningThreshold || PreviousNode.TopNode != null)
                {
                    TopNode = new Node(SpotPrice * alpha, 0.0, Tree);
                    TopNode.BottomNode = this;
                    return TopNode;
                }
                else
                {
                    return null;
                }
            }
        }

        public Node GetBottomNode(double alpha)
        {
            if (BottomNode != null)
            {
                return BottomNode;
            }
            else
            {
                double p = 0.0;
                if (PreviousNode != null)
                {
                    p += PreviousNode.PCumulative * PreviousNode.PDown;
                    if (PreviousNode.BottomNode != null)
                    {
                        p += PreviousNode.BottomNode.PCumulative * PreviousNode.BottomNode.PMid;
                        if (PreviousNode.BottomNode.BottomNode != null)
                        {
                            p += PreviousNode.BottomNode.BottomNode.PCumulative * PreviousNode.BottomNode.BottomNode.PUp;
                        }
                    }

                    if (p > Tree.PruningThreshold || PreviousNode.BottomNode != null)
                    {
                        BottomNode = new Node(SpotPrice / Tree.Alpha, 0.0, Tree);
                        BottomNode.TopNode = this;
                        return BottomNode;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public void ComputeProbability(double D)
        {
            double r = Tree.Market.RiskFreeRate;
            double dt = Tree.Dt;
            double alpha = Tree.Alpha;
            double sigma = Tree.Market.Sigma;

            double E = SpotPrice * Math.Exp(r * dt) - D;
            double V = Math.Pow(SpotPrice, 2) * Math.Exp(2 * r * dt) * (Math.Exp(Math.Pow(sigma, 2) * dt) - 1);

            double nextMidSpotPriceInv = 1.0 / NextNodeMid.SpotPrice;

            PDown = ((Math.Pow(nextMidSpotPriceInv, 2) * (V + E * E) - 1 - (alpha + 1) * (nextMidSpotPriceInv * E - 1)) / ((1 - alpha) * (Math.Pow(alpha, -2) - 1)));
            PUp = ((nextMidSpotPriceInv * E - 1 - (Math.Pow(alpha, -1) - 1) * PDown) / (alpha - 1));
            PMid = 1 - PUp - PDown;
        }

        public void ComputeCumulativeProbabilities()
        {
            if (PCumulative * Math.Max(PUp, PDown) < Tree.PruningThreshold)
            {
                PMid = 1.0;
                PUp = 0.0;
                PDown = 0.0;
            }
            else
            {
                if (NextNodeMid != null)
                {
                    if (NextNodeMid.TopNode != null)
                    {
                        NextNodeMid.TopNode.PCumulative += PCumulative * PUp;
                    }
                    if (NextNodeMid.BottomNode != null)
                    {
                        NextNodeMid.BottomNode.PCumulative += PCumulative * PDown;
                    }
                }
            }
            if (NextNodeMid != null)
            {
                NextNodeMid.PCumulative += PCumulative * PMid;
            }
        }

        public Node NextStepSetup(double forward, Node node, double D)
        {
            double alpha = Tree.Alpha;
            NextNodeMid = Node.GetNextMidNode(forward, alpha, node);
            NextNodeMid.PreviousNode = this;
            ComputeProbability(D);
            NextNodeMid.TopNode = NextNodeMid.GetTopNode(alpha);
            NextNodeMid.BottomNode = NextNodeMid.GetBottomNode(alpha);
            ComputeCumulativeProbabilities();
            return NextNodeMid;
        }

        public static Node GetNextMidNode(double forward, double alpha, Node node)
        {
            while (forward > node.SpotPrice * (1 + alpha) / 2)
            {
                if (node.PreviousNode == null)
                {
                    break;
                }
                else
                {
                    node = node.GetTopNode(alpha);
                }
            }
            while (forward <= node.SpotPrice * (1 + 1 / alpha) / 2)
            {
                if (forward < 0)
                {
                    break;
                }
                if (node.PreviousNode == null)
                {
                    break;
                }
                else
                {
                    node = node.GetBottomNode(alpha);
                }
            }
            return node;
        }
    }
}
