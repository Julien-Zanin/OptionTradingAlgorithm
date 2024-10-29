using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OptionTradingAlgorithm.Modele;
using OptionTradingAlgorithm.Trading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace OptionTradingAlgorithm.ExcelExport
{
    public class ExcelExporter
    {
        public static string ExportStrategiesToExcel(
            string filePath,
            List<Strategy> strategies,
            Market market,
            double minPrice,
            double maxPrice,
            double step)
        {
            // Vérifier si le fichier existe déjà
            if (File.Exists(filePath))
            {
                // Avertir l'utilisateur
                Console.WriteLine($"Le fichier {filePath} existe déjà.");

                // Générer un nouveau nom de fichier avec un suffixe de version
                filePath = GetUniqueFilePath(filePath);

                Console.WriteLine($"Un nouveau fichier sera créé : {filePath}");
            }

            // Configurer EPPlus pour une utilisation non commerciale
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // 1. Feuille Récapitulative des Paramètres
                var summaryWorksheet = package.Workbook.Worksheets.Add("Résumé des Paramètres");
                int summaryRow = 1;

                // Écrire les paramètres du marché
                summaryWorksheet.Cells[summaryRow++, 1].Value = "Paramètres du Marché";
                summaryWorksheet.Cells[summaryRow, 1].Value = "Prix du Sous-Jacent";
                summaryWorksheet.Cells[summaryRow++, 2].Value = market.StockPrice;
                summaryWorksheet.Cells[summaryRow, 1].Value = "Taux Sans Risque";
                summaryWorksheet.Cells[summaryRow++, 2].Value = market.RiskFreeRate;
                summaryWorksheet.Cells[summaryRow, 1].Value = "Volatilité";
                summaryWorksheet.Cells[summaryRow++, 2].Value = market.Sigma;
                summaryWorksheet.Cells[summaryRow++, 1].Value = "";

                // Écrire les spécificités de chaque stratégie
                summaryWorksheet.Cells[summaryRow++, 1].Value = "Spécificités des Stratégies";

                foreach (var strategy in strategies)
                {
                    // En-tête pour la stratégie
                    summaryWorksheet.Cells[summaryRow, 1].Value = strategy.Name;
                    var strategyHeaderStyle = summaryWorksheet.Cells[summaryRow, 1].Style;
                    strategyHeaderStyle.Font.Bold = true;
                    strategyHeaderStyle.Fill.PatternType = ExcelFillStyle.Solid;
                    strategyHeaderStyle.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    summaryRow++;

                    // Méthode de pricing
                    summaryWorksheet.Cells[summaryRow, 1].Value = "Méthode de Pricing";
                    summaryWorksheet.Cells[summaryRow++, 2].Value = strategy.PricingMethod;

                    // Calcul des profits et pertes maximaux
                    var (maxProfit, maxLoss) = strategy.CalculateMaxProfitAndLoss();
                    summaryWorksheet.Cells[summaryRow, 1].Value = "Profit Maximal";
                    summaryWorksheet.Cells[summaryRow++, 2].Value = maxProfit;
                    summaryWorksheet.Cells[summaryRow, 1].Value = "Perte Maximale";
                    summaryWorksheet.Cells[summaryRow++, 2].Value = maxLoss;

                    // Détails des positions
                    summaryWorksheet.Cells[summaryRow++, 1].Value = "Positions :";
                    summaryWorksheet.Cells[summaryRow, 1].Value = "Type de Contrat";
                    summaryWorksheet.Cells[summaryRow, 2].Value = "Nature de l'Option";
                    summaryWorksheet.Cells[summaryRow, 3].Value = "Prix d'Exercice";
                    summaryWorksheet.Cells[summaryRow, 4].Value = "Date d'Expiration";
                    summaryWorksheet.Cells[summaryRow, 5].Value = "Quantité";
                    summaryWorksheet.Cells[summaryRow, 6].Value = "Prime";
                    var positionHeaderStyle = summaryWorksheet.Cells[summaryRow, 1, summaryRow, 6].Style;
                    positionHeaderStyle.Font.Bold = true;
                    positionHeaderStyle.Fill.PatternType = ExcelFillStyle.Solid;
                    positionHeaderStyle.Fill.BackgroundColor.SetColor(Color.LightGray);
                    summaryRow++;

                    foreach (var position in strategy.Positions)
                    {
                        summaryWorksheet.Cells[summaryRow, 1].Value = position.Option != null ? position.Option.ContractType : "Sous-Jacent";
                        summaryWorksheet.Cells[summaryRow, 2].Value = position.Option != null ? position.Option.Nature : "-";
                        summaryWorksheet.Cells[summaryRow, 3].Value = position.Option != null ? position.Option.StrikePrice : "-";
                        summaryWorksheet.Cells[summaryRow, 4].Value = position.Option != null ? position.Option.ExpirationDate.ToShortDateString() : "-";
                        summaryWorksheet.Cells[summaryRow, 5].Value = position.Quantity;
                        summaryWorksheet.Cells[summaryRow, 6].Value = position.Premium;
                        summaryRow++;
                    }

                    summaryRow++; // Ligne vide entre les stratégies
                }

                // Ajuster la largeur des colonnes
                summaryWorksheet.Cells.AutoFitColumns();

                // 2. Feuilles pour Chaque Stratégie
                foreach (var strategy in strategies)
                {
                    var worksheet = package.Workbook.Worksheets.Add(strategy.Name);

                    // Générer les données de payoff
                    var payoffData = strategy.GeneratePayoffData(minPrice, maxPrice, step);

                    // Écrire les données dans la feuille
                    worksheet.Cells[1, 1].Value = "Underlying Price";
                    worksheet.Cells[1, 2].Value = "Payoff";
                    worksheet.Cells[1, 3].Value = "Total Profit";

                    // Appliquer le style aux en-têtes
                    var headerStyle = worksheet.Cells[1, 1, 1, 3].Style;
                    headerStyle.Font.Bold = true;
                    headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
                    headerStyle.Fill.BackgroundColor.SetColor(Color.LightGray);

                    int row = 2;
                    foreach (var dataPoint in payoffData)
                    {
                        double underlyingPrice = dataPoint.UnderlyingPrice;
                        double payoff = dataPoint.Payoff;

                        // Calculer le profit total : Profit Total = Payoff - Coût Initial
                        double totalProfit = payoff - strategy.InitialCost();

                        worksheet.Cells[row, 1].Value = underlyingPrice;
                        worksheet.Cells[row, 2].Value = payoff;
                        worksheet.Cells[row, 3].Value = totalProfit;
                        row++;
                    }

                    // Créer un graphique pour le Payoff et le Profit Total
                    var chart = worksheet.Drawings.AddChart($"{strategy.Name} Chart", eChartType.Line);
                    chart.Title.Text = $"{strategy.Name} Payoff and Profit Diagram";

                    // Série pour le Payoff
                    var payoffSeries = chart.Series.Add(
                        worksheet.Cells[2, 2, row - 1, 2],
                        worksheet.Cells[2, 1, row - 1, 1]
                    );
                    payoffSeries.Header = "Payoff";

                    // Série pour le Profit Total
                    var profitSeries = chart.Series.Add(
                        worksheet.Cells[2, 3, row - 1, 3],
                        worksheet.Cells[2, 1, row - 1, 1]
                    );
                    profitSeries.Header = "Total Profit";

                    chart.XAxis.Title.Text = "Underlying Price";
                    chart.YAxis.Title.Text = "Value";

                    chart.SetPosition(0, 0, 4, 0);
                    chart.SetSize(800, 600);

                    // Ajuster la largeur des colonnes
                    worksheet.Cells.AutoFitColumns();
                }

                // 3. Feuille de Comparaison des Payoffs
                var comparisonWorksheet = package.Workbook.Worksheets.Add("Comparaison des Payoffs");

                // Écrire les en-têtes
                comparisonWorksheet.Cells[1, 1].Value = "Underlying Price";
                int col = 2;
                foreach (var strategy in strategies)
                {
                    comparisonWorksheet.Cells[1, col].Value = strategy.Name;
                    col++;
                }

                // Appliquer le style aux en-têtes
                var compHeaderStyle = comparisonWorksheet.Cells[1, 1, 1, col - 1].Style;
                compHeaderStyle.Font.Bold = true;
                compHeaderStyle.Fill.PatternType = ExcelFillStyle.Solid;
                compHeaderStyle.Fill.BackgroundColor.SetColor(Color.LightGray);

                // Générer les données de payoff
                int maxRows = 0;
                int dataRow = 2;
                for (double price = minPrice; price <= maxPrice; price += step)
                {
                    comparisonWorksheet.Cells[dataRow, 1].Value = price;
                    col = 2;
                    foreach (var strategy in strategies)
                    {
                        double payoff = strategy.CalculatePayoff(price);
                        comparisonWorksheet.Cells[dataRow, col].Value = payoff;
                        col++;
                    }
                    dataRow++;
                    maxRows++;
                }

                // Créer un graphique comparatif
                var compChart = comparisonWorksheet.Drawings.AddChart("Comparaison des Payoffs", eChartType.Line);
                compChart.Title.Text = "Comparaison des Payoffs des Stratégies";

                // Ajouter une série pour chaque stratégie
                col = 2;
                foreach (var strategy in strategies)
                {
                    var series = compChart.Series.Add(
                        comparisonWorksheet.Cells[2, col, dataRow - 1, col],
                        comparisonWorksheet.Cells[2, 1, dataRow - 1, 1]
                    );
                    series.Header = strategy.Name;
                    col++;
                }

                compChart.XAxis.Title.Text = "Underlying Price";
                compChart.YAxis.Title.Text = "Payoff";

                compChart.SetPosition(0, 0, strategies.Count + 2, 0);
                compChart.SetSize(800, 600);

                // Ajuster la largeur des colonnes
                comparisonWorksheet.Cells.AutoFitColumns();

                // Enregistrer le fichier Excel
                try
                {
                    package.SaveAs(new FileInfo(filePath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'enregistrement du fichier Excel : {ex.Message}");
                }
            }

            // Retourner le chemin du fichier créé
            return filePath;
        }

        private static string GetUniqueFilePath(string filePath)
        {
            int version = 1;
            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            string newFilePath = filePath;

            while (File.Exists(newFilePath))
            {
                newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}_v{version}{extension}");
                version++;
            }

            return newFilePath;
        }
    }
}
