# Option Trading Algorithm

Un projet en C# qui implémente diverses stratégies de trading d'options, telles que le Straddle, le Bull Spread, le Bear Spread, le Butterfly Spread, le Strangle, le Covered Call, le Protective Put et l'Iron Condor. Le projet comprend également une fonctionnalité pour exporter les données de payoff et les graphiques associés vers un fichier Excel.

## Table des matières
- [Option Trading Algorithm](#option-trading-algorithm)
  - [Table des matières](#table-des-matières)
- [Documentation Technique](#documentation-technique)
  - [Introduction](#introduction)
    - [Sélection des Stratégies](#sélection-des-stratégies)
    - [Choix des Paramètres](#choix-des-paramètres)
    - [Création des Options](#création-des-options)
    - [Gestion des Positions](#gestion-des-positions)
  - [Calcul du Payoff et du Profit](#calcul-du-payoff-et-du-profit)
    - [Calcul du Payoff pour une Option](#calcul-du-payoff-pour-une-option)
    - [Calcul du Payoff Total d'une Stratégie](#calcul-du-payoff-total-dune-stratégie)
    - [Calcul du Profit Total](#calcul-du-profit-total)
- [Détail des stratégies pour les mettre en place](#détail-des-stratégies-pour-les-mettre-en-place)
  - [Bull Spread](#bull-spread)
  - [Bear Spread](#bear-spread)
  - [Butterfly Spread](#butterfly-spread)
  - [Calendar Spread](#calendar-spread)
  - [Covered Call](#covered-call)
  - [Protective Put](#protective-put)
  - [Iron Condor](#iron-condor)
  - [Straddle](#straddle)
  - [Strangle](#strangle)
- [Étapes d'Installation](#étapes-dinstallation)
- [Structure du Projet](#structure-du-projet)
- [Dépendances](#dépendances)
- [Considérations Futures](#considérations-futures)
  - [Difficultées rencontrées](#difficultées-rencontrées)

# Documentation Technique

## Introduction
Ce projet vise à fournir une implémentation de diverses stratégies de trading d'options financières en utilisant le langage C#. Il permet aux utilisateurs de :
- Représenter des stratégies d'options complexes.
- Calculer les payoffs et les profits associés à l'ensemble des stratégies demandées. Cela calcule également les payoffs sur un intervalle de prix \[MinPrice : Maxprice\].
- Exporter les données et les graphiques vers un fichier pour une synthèse des profits et pertes maximales pour chacune des stratégies et feuille de synthèse globale des stratégies.

### Sélection des Stratégies
Pour utiliser le programme et sélectionner les stratégies à analyser, vous devez modifier le fichier `Program.cs`. Dans ce fichier, vous trouverez une liste de stratégies initialisée comme suit :

```csharp
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
};
```

Par défaut, les stratégies sont définies pour l'instant par des EuropeanOption, ici nous sommes dans `Butterfly.cs` :


```csharp
SetupPositions()
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
```

On utilise les options Européennes indifféremment des options Américaines car pour l'instant, il n'y a aucune différence entre les deux. En effet, le prix variant si et seulement si il y a un exercice anticipé, comme nous n'avons pas encore implémenté les dividendes en tant que données, nous n'avons pas encore implémenté le choix entre option américaine et européenne. Cependant, il est important de noter que `TrinomialTree.cs` peut les prendre en compte.

### Choix des Paramètres
Lors de l'ajout d'une stratégie, vous pouvez personnaliser les paramètres suivants :
- **Prix d'exercice (Strike Price)** : Le prix auquel l'option peut être exercée.
- **Date d'expiration** : La date à laquelle l'option expire.
- **Date de valorisation** : La date actuelle ou la date à laquelle l'option est évaluée.
- **Méthode de pricing** : La méthode utilisée pour calculer le prix de l'option (par exemple, Black-Scholes, Arbre Trinomial).
- **MinPrice** : Sera le prix Spot minimum qui servira de borne inférieure dans la boucle de comparaison des prix.
- **MaxPrice** : Sera le prix Spot maximum qui servira de borne supérieure dans la boucle de comparaison des prix.
- **Step** : Le pas d'itération sur l'intervalle \[MinPrice : Maxprice\].

### Création des Options
Les options sont modélisées par la classe `EuropeanOption`. Chaque option est caractérisée par :
- **Type d'option** : "Call" ou "Put".
- **Prix d'exercice (StrikePrice)** : Le prix auquel l'option peut être exercée.
- **Date d'expiration (ExpirationDate)** : La date à laquelle l'option expire.

Exemple de création d'une option :


```csharp
EuropeanOption callOption = new EuropeanOption
{
    strikePrice: 100.0,
    expirationDate: DateTime.Now.AddMonths(3),
    nature: "Call",
    pricingDate: DateTime.Now
};
```

### Gestion des Positions
Une position représente l'achat ou la vente d'une option ou d'un actif sous-jacent. La classe `OptionPosition` est utilisée pour modéliser une position :
- **Option** : L'option associée à la position (ou `null` pour le sous-jacent).
- **Quantité (Quantity)** : Le nombre d'options achetées (positif) ou vendues (négatif).
- **Prime (Premium)** : Le prix payé ou reçu pour la position.

Exemple de création d'une position :

```csharp
OptionPosition position = new OptionPosition
{
    option: callOption,
    quantity: 1,
    premium: premiumValue
};
```

## Calcul du Payoff et du Profit

Le payoff d'une position est le gain ou la perte réalisée à l'expiration de l'option en fonction du prix du sous-jacent.

### Calcul du Payoff pour une Option

- **Option Call** :
  \[
  \text{Payoff} = \max(S - K, 0)
  \]

- **Option Put** :
  \[
  \text{Payoff} = \max(K - S, 0)
  \]

- **Position Vendue** : Le payoff est multiplié par -1.

### Calcul du Payoff Total d'une Stratégie

Le payoff total est la somme des payoffs de toutes les positions de la stratégie :


```csharp
        public override double CalculatePayoff(double underlyingPrice)
        {
            double totalPayoff = 0.0;
            foreach (var position in Positions)
            {
                double positionPayoff = position.Payoff(underlyingPrice);
                totalPayoff += positionPayoff;
            }
            return totalPayoff;
        }
```

### Calcul du Profit Total

Le profit total est calculé en tenant compte du coût initial (la somme des primes payées ou reçues) :


```csharp
double totalProit = totalPayoff - InitialCost();
```





# Détail des stratégies pour les mettre en place

## Bull Spread

**Description** : Le Bull Spread est une stratégie haussière utilisant des options Call. Elle permet de limiter les pertes et les gains potentiels en utilisant deux options avec différents prix d’exercice.

- **Composition** :
  - Acheter une option Call ( `lowerStrikeCall` )
  - Vendre une option Call ( `higherStrikeCall` ), avec `higherStrikeCall > lowerStrikeCall`

- **Payoff Théorique** :
  \[
  \text{Payoff} = \max(S - \text{lowerStrikeCall}, 0) - \max(S - \text{higherStrikeCall}, 0)
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} - (\text{Prime Call Acheté} - \text{Prime Call Vendu})
  \]

---

## Bear Spread

**Description** : Le Bear Spread est une stratégie baissière utilisant des options Put. Elle permet de limiter les pertes et les gains potentiels en utilisant deux options avec différents prix d’exercice.

- **Composition** :
  - Acheter une option Put ( `higherStrikePut` )
  - Vendre une option Put ( `lowerStrikePut` ), avec `higherStrikePut > lowerStrikePut`

- **Payoff Théorique** :
  \[
  \text{Payoff} = \max(\text{higherStrikePut} - S, 0) - \max(\text{lowerStrikePut} - S, 0)
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} - (\text{Prime Put Acheté} - \text{Prime Put Vendu})
  \]

---

## Butterfly Spread

**Description** : Le Butterfly Spread est une stratégie de faible volatilité qui utilise trois prix d’exercice différents pour créer un profil de profit limité, combinant des options achetées et vendues.

- **Composition** :
  - Acheter une option Call ( `lowerStrikeCall` )
  - Vendre deux options Call ( `middleStrikeCall` )
  - Acheter une option Call ( `higherStrikeCall` ) avec `higherStrikeCall > middleStrikeCall > lowerStrikeCall`

- **Payoff Théorique** :
  \[
  \text{Payoff} = \max(S - \text{lowerStrikeCall}, 0) - 2 \times \max(S - \text{middleStrikeCall}, 0) + \max(S - \text{higherStrikeCall}, 0)
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} - (\text{Prime Call Acheté Bas} + \text{Prime Call Acheté Haut} - 2 \times \text{Prime Call Vendu})
  \]

---

## Calendar Spread

**Description** : Le Calendar Spread est une stratégie de faible volatilité qui utilise des options avec le même prix d’exercice mais des dates d’expiration différentes.

- **Composition** :
  - Vendre   une option à court terme
  - Acheter une option à long terme avec le même prix d’exercice

- **Payoff Théorique** :
  \[
  \text{Payoff} = \text{Valeur de l'option à long terme à l'échéance de la première option} - \text{Prime de l'option vendue}
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} - (\text{Prime Option Long Terme} - \text{Prime Option Court Terme})
  \]

---

## Covered Call

**Description** : Le Covered Call est une stratégie où l'investisseur détient le sous-jacent et vend une option Call sur ce même actif.

- **Composition** :
  - Détenir l'actif sous-jacent
  - Vendre une option Call (K)

- **Payoff Théorique** :
  \[
  \text{Payoff} = \min(S, K)
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} + \text{Prime Reçue} - \text{Prix d'Achat de l'Actif}
  \]

---

## Protective Put

**Description** : Le Protective Put est une stratégie de couverture où l'investisseur détient le sous-jacent et achète une option Put pour se protéger d'une baisse de prix.

- **Composition** :
  - Détenir l'actif sous-jacent
  - Acheter une option Put (K)

- **Payoff Théorique** :
  \[
  \text{Payoff} = \max(K, S)
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} - \text{Prix d'Achat de l'Actif} - \text{Prime Put}
  \]

---

## Iron Condor

**Description** : L'Iron Condor est une stratégie neutre qui combine un Bull Put Spread et un Bear Call Spread pour limiter les pertes et les gains.

- **Composition** :
  - Vendre une option Put ( `higherStrikePut` )
  - Acheter une option Put ( `lowerStrikePut` ), avec `lowerStrikePut < higherStrikePut`
  - Vendre une option Call ( `lowerStrikeCall` )
  - Acheter une option Call ( `higherStrikeCall` ), avec `higherStrikeCall > lowerStrikeCall`

- **Payoff Théorique** :
  \[
  \text{Payoff} = \text{Différence entre les primes} - \text{Pertes maximales des spreads}
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} - \text{Coût Net des Options}
  \]

---

## Straddle

**Description** : Le Straddle est une stratégie neutre qui consiste à acheter une option Call et une option Put avec le même prix d'exercice et la même date d'expiration.

- **Composition** :
  - Acheter une option Call (K)
  - Acheter une option Put (K)

- **Payoff Théorique** :
  \[
  \text{Payoff} = \max(S - K, 0) + \max(K - S, 0)
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} - (\text{Prime Call} + \text{Prime Put})
  \]

---

## Strangle

**Description** : Le Strangle est une stratégie de volatilité qui consiste à acheter une option Call et une option Put avec des prix d’exercice différents.

- **Composition** :
  - Acheter une option Call ( `higherStrikeCall` )
  - Acheter une option Put ( `lowerStrikePut` )

- **Payoff Théorique** :
  \[
  \text{Payoff} = \max(S - \text{higherStrikeCall}, 0) + \max(\text{lowerStrikePut} - S, 0)
  \]

- **Formule du Profit** :
  \[
  \text{Profit} = \text{Payoff} - (\text{Prime Call} + \text{Prime Put})
  \]




# Étapes d'Installation
1. Cloner le dépôt GitHub :
    ```bash
    git clone https://github.com/Julien-Zanin/OptionTradingAlgorithm.git
    ```
2. Naviguer vers le répertoire du projet :
    ```bash
    cd OptionTradingAlgorithm
    ```
3. Restaurer les packages NuGet :
    ```bash
    dotnet restore
    ```



# Structure du Projet

- **OptionTradingAlgorithm** : Projet principal contenant le programme et les implémentations des stratégies.
  - `Program.cs` : Point d'entrée du programme.
  - `Strategies` : Dossier contenant les classes pour chaque stratégie.
    - `Straddle.cs`, `BullSpread.cs`, etc. : Implémentations des stratégies spécifiques.
  - `Modele` : Dossier contenant les modèles financiers.
    - `Market.cs` : Représente les conditions du marché. Cette classe récupère les paramètres liés au marché.
    - `Contract.cs` : Représente les paramètres du contrat.
    - `EuropeanOption.cs` : Modélisation des options européennes.
    - `AmericanOption.cs` : Modélisation des options américaines.
  - `Pricing` : Dossier contenant les méthodes de pricing.
    - `PriceEngine.cs` : Moteur de calcul des prix des options.
    - `BlackScholesPricer.cs` : Implémentation du modèle Black-Scholes.
    - `TrinomialTreePricer.cs` : Implémentation de l'arbre trinomial.
  - `Trading` : Dossier contenant les classes relatives aux positions.
    - `PositionOption.cs` : Représente une position sur une option (Achat ou Vente).
    - `TradingStrategy.cs` : Représente les conditions du marché. Cette classe récupère les paramètres liés au marché.

- **OptionTradingAlgorithmTests** : Projet de tests unitaires pour valider le fonctionnement des stratégies.
- **ExcelExport** : Dossier contenant la classe `ExcelExporter.cs` pour l'exportation des données vers Excel.
- **Output** : Dossier généré contenant les fichiers Excel produits par le programme.




# Dépendances
- **.NET 6.0** : Framework nécessaire pour exécuter l'application.
- **EPPlus** : Bibliothèque pour manipuler les fichiers Excel.
    - Installée via NuGet :
      ```bash
      Install-Package EPPlus -Version 5.8.0
      ```
- **Microsoft.Extensions.Configuration.Json (optionnel)** : Pour gérer les configurations via un fichier `appsettings.json`.
- Bibliothèques de test :
  - `coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit`, et `xunit.runner.visualstudio` pour exécuter et analyser les tests.



# Considérations Futures

- **Extension des Stratégies** :
  - Ajout de nouvelles stratégies d'options.
  - Implémentation totale des Options Américaines avec l'implémentation des dividendes

- **Interface Utilisateur** :
  - Développement d'une interface graphique pour faciliter l'utilisation du programme.

- **Intégration de Données Réelles** :
  - Connexion à des API financières pour obtenir des données de marché en temps réel (style Yahoo Finance).

- **Intégration de modèles de volatilité** : 
  - Implémentation d'une classe volatilité qui permettrait de choisir un modèle de calcul de volatilité tel que le modèle d'Heston mentionné lors de nos échanges.  


## Difficultées rencontrées 

L'implémentation de certaines stratégies telle que l'Iron Condor a pris plus de temps et nous a permis de réaliser certaines erreurs d'implémentations dans le calcul des payoffs. La mise en place ces tests unitaires, bien que laborieux nous a également beaucoup servi à débugger. Le manque de temps nous a fait faillite pour l'intégration de données réelles, qui n'aurait pas rajouté grand chose en lui-même et l'intégration d'une petite interface utilisateur pour guider l'implémentation des données de marché et mise en place des stratégies. 

