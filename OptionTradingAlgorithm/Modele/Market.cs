namespace OptionTradingAlgorithm.Modele
{
    public class Market
    {
        public double StockPrice { get; set; }
        public double RiskFreeRate { get; set; }
        public double Sigma {  get; set; }
        public List<Tuple<DateTime,double>> Dividends { get; set; }
        public DateTime PricingDate { get; set; } // Date de pricing


        public Market(double stockPrice, double riskFreeRate, double sigma, List<Tuple<DateTime, double>> dividends, DateTime pricingDate)
        {
            StockPrice = stockPrice;
            RiskFreeRate = riskFreeRate;
            Sigma = sigma;
            Dividends = dividends;
            PricingDate = pricingDate;
        }
        public double TotalDividend(DateTime pricingDate, double tStart, double tEnd)
        {
            double totalDividend = 0.0;
            foreach (var (divDate, divAmount) in Dividends)
            {
                double tDiv = (divDate - pricingDate).TotalDays / 365.0;
                if (tDiv > tStart && tDiv <= tEnd)
                {
                    totalDividend += divAmount;
                }
            }
            return totalDividend;
        }
    }
}
