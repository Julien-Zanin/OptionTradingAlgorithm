<!DOCTYPE html>
<html lang="fr">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Option Trading Algorithm</title>
</head>
<body>

<h1>Option Trading Algorithm</h1>

<p>Un projet en C# qui implémente diverses stratégies de trading d'options, telles que le Straddle, le Bull Spread, le Bear Spread, le Butterfly Spread, le Strangle, le Covered Call, le Protective Put et l'Iron Condor. Le projet comprend également une fonctionnalité pour exporter les données de payoff et les graphiques associés vers un fichier Excel.</p>

<h2>Table des matières</h2>
<ul>
  <li><a href="#documentation-technique">Documentation Technique</a>
    <ul>
      <li><a href="#introduction">Introduction</a></li>
      <li><a href="#sélection-des-stratégies">Sélection des Stratégies</a></li>
      <li><a href="#choix-des-paramètres">Choix des Paramètres</a></li>
      <li><a href="#création-des-options">Création des Options</a></li>
      <li><a href="#gestion-des-positions">Gestion des Positions</a></li>
      <li><a href="#calcul-du-payoff-et-du-profit">Calcul du Payoff et du Profit</a></li>
      <li><a href="#stratégies-détaillées">Stratégies Détailées</a></li>
    </ul>
  </li>
  <li><a href="#structure-du-projet">Structure du Projet</a></li>
  <li><a href="#dépendances">Dépendances</a></li>
</ul>

<h2 id="documentation-technique">Documentation Technique</h2>

<h3 id="introduction">Introduction</h3>
<p>Ce projet vise à fournir une implémentation de diverses stratégies de trading d'options financières en utilisant le langage C#. Il permet aux utilisateurs de :</p>
<ul>
  <li>Représenter des stratégies d'options complexes.</li>
  <li>Calculer les payoffs et les profits associés à l'ensemble des stratégies demandées. Cela calcule également les payoff sur un intervalle de prix [MinPrice : Maxprice] </li>
  <li>Exporter les données et les graphiques vers un fichier pour une synthèse des profits et pertes maximales pour chacune des stratégies et feuille de synthèse globale des stratégies.</li>
</ul>

<h3 id="sélection-des-stratégies">Sélection des Stratégies</h3>
<p>Pour utiliser le programme et sélectionner les stratégies à analyser, vous devez modifier le fichier <code>Program.cs</code>. Dans ce fichier, vous trouverez une liste de stratégies initialisée comme suit :</p>

<pre><code>List&lt;Strategy&gt; strategies = new List&lt;Strategy&gt;
{
      new Straddle(100.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
      new BullSpread(95.0, 105.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "Tree"),
      new BearSpread(105.0, 95.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "Tree"),
      new ButterflyStrategy(80.0, 100.0, 120.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
      new Strangle(90.0, 110.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
      new CoveredCall(100.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
      new ProtectivePut(100.0, DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes"),
      new IronCondor(80.0,90.0,110.0,120.0,DateTime.Now.AddMonths(3), DateTime.Now, market, pricingMethod: "BlackScholes")
};
</code></pre>

<p>Par défaut les stratégies sont définies pour l'instant par des EuropeanOption, ici nous sommes dans <code>Butterfly.cs</code>  :</p>

<pre><code>SetupPositions()
        {
            // Acheter une option Call avec le prix d'exercice le plus bas
            EuropeanOption longCallLow = new EuropeanOption(_lowerStrike, _expirationDate, "Call", _pricingDate);
            double premiumLongCallLow = _priceEngine.PriceOption(longCallLow, _market, _pricingMethod, _steps);
            AddPosition(new OptionPosition(longCallLow, 1, premiumLongCallLow));

            // Vendre deux options Call avec le prix d'exercice moyen
            EuropeanOption shortCallMiddle = new EuropeanOption(_middleStrike, _expirationDate, "Call", _pricingDate);
            double premiumShortCallMiddle = _priceEngine.PriceOption(shortCallMiddle, _market, _pricingMethod, _steps);
            AddPosition(new OptionPosition(shortCallMiddle, -2, premiumShortCallMiddle));

            // Acheter une option Call avec le prix d'exercice le plus haut
            EuropeanOption longCallHigh = new EuropeanOption(_higherStrike, _expirationDate, "Call", _pricingDate);
            double premiumLongCallHigh = _priceEngine.PriceOption(longCallHigh, _market, _pricingMethod, _steps);
            AddPosition(new OptionPosition(longCallHigh, 1, premiumLongCallHigh));
        };
</code></pre>

<p> On utilise les options Européennes indifféremment des options Américaines car pour l'instant, il n'y a aucune différence entre les deux. En effet, le prix variant si et seulement si il y a un exercice anticipé, comme nous n'avons pas encore implémenter les dividendes en tant que données, nous n'avons pas encore implémenter le choix entre option américaine et européenne. Cependant il est important de noter que  <code>TrinomialTree.cs</code> peut les prendre en compte.  :</p>

<h3 id="choix-des-paramètres">Choix des Paramètres</h3>
<p>Lors de l'ajout d'une stratégie, vous pouvez personnaliser les paramètres suivants :</p>
<ul>
  <li><strong>Prix d'exercice (Strike Price)</strong> : Le prix auquel l'option peut être exercée.</li>
  <li><strong>Date d'expiration</strong> : La date à laquelle l'option expire.</li>
  <li><strong>Date de valorisation</strong> : La date actuelle ou la date à laquelle l'option est évaluée.</li>
  <li><strong>Méthode de pricing</strong> : La méthode utilisée pour calculer le prix de l'option (par exemple, Black-Scholes, Arbre Trinomial).</li>
  <li><strong>MinPrice</strong> : Sera le prix Spot minimum qui servira de borne inférieur dans la boucle de comparaison des prix.
  <li><strong>MaxPrice</strong> : Sera le prix Spot maximum qui servira de borne inférieur dans la boucle de comparaison des prix.
  <li><strong>Step</strong> : Sera le pas qui permettra d'itérer sur l'intervalle [MinPrice : Maxprice]
</ul>

<h3 id="création-des-options">Création des Options</h3>
<p>Les options sont modélisées par la classe <code>EuropeanOption</code>. Chaque option est caractérisée par :</p>
<ul>
  <li><strong>Type d'option</strong> : "Call" ou "Put".</li>
  <li><strong>Prix d'exercice (StrikePrice)</strong> : Le prix auquel l'option peut être exercée.</li>
  <li><strong>Date d'expiration (ExpirationDate)</strong> : La date à laquelle l'option expire.</li>
</ul>

<p>Exemple de création d'une option :</p>
<pre><code>EuropeanOption callOption = new EuropeanOption
{
    strikePrice: 100.0,
    expirationDate: DateTime.Now.AddMonths(3),
    nature: "Call",
    pricingDate: DateTime.Now
};
</code></pre>

<h3 id="gestion-des-positions">Gestion des Positions</h3>
<p>Une position représente l'achat ou la vente d'une option ou d'un actif sous-jacent. La classe <code>OptionPosition</code> est utilisée pour modéliser une position :</p>
<ul>
  <li><strong>Option</strong> : L'option associée à la position (ou <code>null</code> pour le sous-jacent).</li>
  <li><strong>Quantité (Quantity)</strong> : Le nombre d'options achetées (positif) ou vendues (négatif).</li>
  <li><strong>Prime (Premium)</strong> : Le prix payé ou reçu pour la position.</li>
</ul>

<p>Exemple de création d'une position :</p>
<pre><code>OptionPosition position = new OptionPosition
{
    option: callOption,
    quantity: 1,
    premium: premiumValue
};
</code></pre>

<h3 id="calcul-du-payoff-et-du-profit">Calcul du Payoff et du Profit</h3>
<p>Le payoff d'une position est le gain ou la perte réalisée à l'expiration de l'option en fonction du prix sous-jacent.</p>
<p>Calcul du Payoff :</p>
<ul>
    <li>Option Call : <img src="URL_IMAGE_PAYOFF_CALL" alt="Payoff pour Option Call"></li>
    <li>Option Put : <img src="URL_IMAGE_PAYOFF_PUT" alt="Payoff pour Option Put"></li>
    <li>Sous-jacent : <img src="URL_IMAGE_PAYOFF_SOUS_JACENT" alt="Payoff pour Sous-jacent"></li>
</ul>

<h3 id="stratégies-détaillées">Stratégies Détailées</h3>



<h4 id="bull-spread">Bull Spread</h4>
<p><strong>Description</strong> : Le Bull Spread est une stratégie haussière utilisant des options Call. Elle permet de limiter les pertes et les gains potentiels en utilisant deux options avec différents prix d’exercice.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Acheter une option Call (<code>lowerStrikeCall</code>)</li>
      <li>Vendre une option Call (<code>higherStrikeCall</code>), avec <code>higherStrikeCall</code> &gt; <code>lowerStrikeCall</code></li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : <p>Payoff = max(S - lowerStrikeCall, 0) - max(S - higherStrikeCall, 0)</p></li>
  <li><strong>Formule du Profit</strong> :<p>Profit = Payoff - (Prime Call Acheté - Prime Call Vendu)</p></li>
</ul>

<h4 id="bear-spread">Bear Spread</h4>
<p><strong>Description</strong> : Le Bear Spread est une stratégie baissière utilisant des options Put. Elle permet de limiter les pertes et les gains potentiels en utilisant deux options avec différents prix d’exercice.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Acheter une option Put (<code>higherStrikePut</code>)</li>
      <li>Vendre une option Put (<code>lowerStrikePut</code>), avec <code>higherStrikePut</code> &gt; <code>lowerStrikePut</code></li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> :<p>Payoff = max(higherStrikePut - S, 0) - max(lowerStrikePut - S, 0)</p></li>
  <li><strong>Formule du Profit</strong> :<p>Profit = Payoff - (Prime Put Acheté - Prime Put Vendu)</p></li>
</ul>

<h4 id="butterfly-spread">Butterfly Spread</h4>
<p><strong>Description</strong> : Le Butterfly Spread est une stratégie de faible volatilité qui utilise trois prix d’exercice différents pour créer un profil de profit limité, combinant des options achetées et vendues.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Acheter une option Call (<code>lowerStrikeCall</code>)</li>
      <li>Vendre deux options Call (<code>middleStrikeCall</code>)</li>
      <li>Acheter une option Call (<code>higherStrikeCall</code>) avec <code>higherStrikeCall</code> &gt; <code>middleStrikeCall</code> &gt; <code>lowerStrikeCall</code></li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : <p>Payoff = max(S - lowerStrikeCall, 0) - 2 * max(S - middleStrikeCall, 0) + max(S - higherStrikeCall, 0)</p></li>
  <li><strong>Formule du Profit</strong> :<p>Profit = Payoff - (Prime Call Acheté Bas + Prime Call Acheté Haut - 2 * Prime Call Vendu)</p></li>
</ul>

<h4 id="calendar-spread">Calendar Spread</h4>
<p><strong>Description</strong> : Le Calendar Spread est une stratégie de faible volatilité qui utilise des options avec le même prix d’exercice mais des dates d’expiration différentes.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Vendre une option à court terme</li>
      <li>Acheter une option à long terme avec le même prix d’exercice</li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : <p>Payoff = Valeur de l'option à long terme à l'échéance de la première option - Prime de l'option vendue</p></li>
  <li><strong>Formule du Profit</strong> :<p>Profit = Payoff - (Prime Option Long Terme - Prime Option Court Terme)</p></li>
</ul>

<h4 id="covered-call">Covered Call</h4>
<p><strong>Description</strong> : Le Covered Call est une stratégie où l'investisseur détient le sous-jacent et vend une option Call sur ce même actif.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Détenir l'actif sous-jacent</li>
      <li>Vendre une option Call (K)</li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : <p>Payoff = min(S, K)</p></li>
  <li><strong>Formule du Profit</strong> :<p>Profit = Payoff + Prime Reçue - Prix d'Achat de l'Actif</p></li>
</ul>

<h4 id="protective-put">Protective Put</h4>
<p><strong>Description</strong> : Le Protective Put est une stratégie de couverture où l'investisseur détient le sous-jacent et achète une option Put pour se protéger d'une baisse de prix.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Détenir l'actif sous-jacent</li>
      <li>Acheter une option Put (K)</li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : <p>Payoff = max(K, S)</p></li>
  <li><strong>Formule du Profit</strong> : <p>Profit = Payoff - Prix d'Achat de l'Actif - Prime Put</p></li>
</ul>

<h4 id="iron-condor">Iron Condor</h4>
<p><strong>Description</strong> : L'Iron Condor est une stratégie neutre qui combine un Bull Put Spread et un Bear Call Spread pour limiter les pertes et les gains.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Vendre une option Put (<code>higherStrikePut</code>)</li>
      <li>Acheter une option Put (<code>lowerStrikePut</code>), avec <code>lowerStrikePut</code> &lt; <code>higherStrikePut</code></li>
      <li>Vendre une option Call (<code>lowerStrikeCall</code>)</li>
      <li>Acheter une option Call (<code>higherStrikeCall</code>), avec <code>higherStrikeCall</code> &gt; <code>lowerStrikeCall</code></li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : <p>Payoff = Différence entre les primes - Pertes maximales des spreads</p></li>
  <li><strong>Formule du Profit</strong> : <p>Profit = Payoff - Coût Net des Options</p></li>
</ul>

<h4 id="straddle">Straddle</h4>
<p><strong>Description</strong> : Le Straddle est une stratégie neutre qui consiste à acheter une option Call et une option Put avec le même prix d'exercice et la même date d'expiration.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Acheter une option Call (K)</li>
      <li>Acheter une option Put (K)</li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : <p>Payoff = max(S - K, 0) + max(K - S, 0)</p></li>
  <li><strong>Formule du Profit</strong> : <p>Profit = Payoff - (Prime Call + Prime Put)</p></li>
</ul>

<h4 id="strangle">Strangle</h4>
<p><strong>Description</strong> : Le Strangle est une stratégie de volatilité qui consiste à acheter une option Call et une option Put avec des prix d’exercice différents.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Acheter une option Call (<code>higherStrikeCall</code>)</li>
      <li>Acheter une option Put (<code>lowerStrikePut</code>)</li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : <p>Payoff = max(S - higherStrikeCall, 0) + max(lowerStrikePut - S, 0)</p></li>
  <li><strong>Formule du Profit</strong> : <p>Profit = Payoff - (Prime Call + Prime Put)</p></li>
</ul>


<h3>Étapes d'Installation</h3>
<ol>
    <li>Cloner le dépôt GitHub :
        <pre><code>git clone https://github.com/Julien-Zanin/OptionTradingAlgorithm.git</code></pre>
    </li>
    <li>Naviguer vers le répertoire du projet :
        <pre><code>cd OptionTradingAlgorithm</code></pre>
    </li>
    <li>Restaurer les packages NuGet :
        <pre><code>dotnet restore</code></pre>
    </li>
</ol>

<h2>Structure du Projet</h2>
<ul>
    <li><strong>OptionTradingAlgorithm</strong> : Projet principal contenant le programme et les implémentations des stratégies.
        <ul>
            <li><code>Program.cs</code> : Point d'entrée du programme.</li>
            <li><code>Strategies</code> : Dossier contenant les classes pour chaque stratégie.
                <ul>
                    <li><code>Straddle.cs</code>, <code>BullSpread.cs</code>, etc. : Implémentations des stratégies spécifiques.</li>
                </ul>
            </li>
            <li><code>Modele</code> : Dossier contenant les modèles financiers.
                <ul>
                    <li><code>Market.cs</code> : Représente les conditions du marché. Cette classe récupère les paramètres liés au marché.</li>
                    <li><code>Contract.cs</code> : Représente les paramètres du contrat.</li>
                    <li><code>EuropeanOption.cs</code> : Modélisation des options européennes.</li>
                    <li><code>AmericanOption.cs</code> : Modélisation des options américaines.</li>
                </ul>
            </li>
            <li><code>Pricing</code> : Dossier contenant les méthodes de pricing.
                <ul>
                    <li><code>PriceEngine.cs</code> : Moteur de calcul des prix des options.</li>
                    <li><code>BlackScholesPricer.cs</code> : Implémentation du modèle Black-Scholes.</li>
                    <li><code>TrinomialTreePricer.cs</code> : Implémentation de l'arbre trinomial.</li>
                </ul>
            </li>
            <li><code>Trading</code> : Dossier contenant les classes relatives aux positions.
                <ul>
                    <li><code>PositionOption.cs</code> : Représente une position sur une option (Achat ou Vente).</li>
                    <li><code>TradingStrategy.cs</code> : Représente les conditions du marché. Cette classe récupère les paramètres liés au marché.</li>
                </ul>
            </li>
        </ul>
    </li>
    <li><strong>OptionTradingAlgorithmTests</strong> : Projet de tests unitaires pour valider le fonctionnement des stratégies.</li>
    <li><strong>ExcelExport</strong> : Dossier contenant la classe <code>ExcelExporter.cs</code> pour l'exportation des données vers Excel.</li>
    <li><strong>Output</strong> : Dossier généré contenant les fichiers Excel produits par le programme.</li>
</ul>

<h2>Dépendances</h2>
<ul>
    <li>.NET 6.0 : Framework nécessaire pour exécuter l'application.</li>
    <li>EPPlus : Bibliothèque pour manipuler les fichiers Excel.
        <ul>
            <li>Installée via NuGet :
                <pre><code>Install-Package EPPlus -Version 5.8.0</code></pre>
            </li>
        </ul>
    </li>
    <li>Microsoft.Extensions.Configuration.Json (optionnel) : Pour gérer les configurations via un fichier <code>appsettings.json</code>.</li>
</ul>

</body>
</html>
