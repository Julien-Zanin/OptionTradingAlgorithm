using OptionTradingAlgorithm.ExcelExport;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Pricing;
using OptionTradingAlgorithm.Strategies;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;

namespace OptionTradingAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            // Définir les paramètres du marché
            Market market = new Market(
                stockPrice: 100.0,
                riskFreeRate: 0.05,
                sigma: 0.2,
                dividends: new List<Tuple<DateTime, double>>(), // Liste vide de dividendes
                pricingDate: DateTime.Now
            );

            // Définir la plage de prix du sous-jacent
            double minPrice = 50.0;
            double maxPrice = 150.0;
            double step = 1.0;

            // Créer la liste des stratégies
            List<Strategy> strategies = new List<Strategy>
            {
                new Straddle(100.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
                new BullSpread(95.0, 105.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "Tree"),
                new BearSpread(105.0, 95.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "Tree"),
                new ButterflyStrategy(80.0, 100.0, 120.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
                new Strangle(90.0, 110.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
                new CoveredCall(100.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
                new ProtectivePut(100.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
                new IronCondor(80.0,90.0,110.0,120.0,DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes")
                // On peut mettre d'autres stratégies
            };

            // Exporter les données vers un fichier Excel

            // Obtenir le répertoire de base de l'application
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Chemin du dossier Output
            string outputDirectory = Path.Combine(baseDirectory, "Output");

            // Vérifier si le dossier Output existe, sinon le créer
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Définir le chemin du fichier dans le dossier Output
            string fileName = "Strategies_Payoff_Analysis.xlsx";
            string filePath = Path.Combine(outputDirectory, fileName);
            ExcelExporter.ExportStrategiesToExcel(filePath, strategies,market, minPrice, maxPrice, step);

            Console.WriteLine($"Les données de payoff ont été exportées vers {filePath}");
        }
    }
}
