<h1>Option Trading Algorithm</h1>

<p>Un projet en C# qui implémente diverses stratégies de trading d'options, telles que le Straddle, le Bull Spread, le Bear Spread, le Butterfly Spread, le Strangle, le Covered Call, le Protective Put et l'Iron Condor. Le projet comprend également une fonctionnalité pour exporter les données de payoff et les graphiques associés vers un fichier Excel.</p>

<h2>Table des matières</h2>
<ul>
  <li><a href="#introduction">Introduction</a></li>
  <li><a href="#utilisation-du-programme">Utilisation du Programme</a>
    <ul>
      <li><a href="#sélection-des-stratégies">Sélection des Stratégies</a></li>
      <li><a href="#choix-des-paramètres">Choix des Paramètres</a></li>
      <li><a href="#références-pour-les-stratégies">Références pour les Stratégies</a></li>
    </ul>
  </li>
  <li><a href="#fonctionnement-interne">Fonctionnement Interne</a>
    <ul>
      <li><a href="#création-des-options">Création des Options</a></li>
      <li><a href="#gestion-des-positions">Gestion des Positions</a></li>
      <li><a href="#calcul-du-payoff-et-du-profit">Calcul du Payoff et du Profit</a></li>
    </ul>
  </li>
  <li><a href="#stratégies-détaillées">Stratégies Détailées</a>
    <ul>
      <li><a href="#straddle">Straddle</a></li>
      <li><a href="#bull-spread">Bull Spread</a></li>
      <li><a href="#bear-spread">Bear Spread</a></li>
      <li><a href="#butterfly-spread">Butterfly Spread</a></li>
      <li><a href="#strangle">Strangle</a></li>
      <li><a href="#covered-call">Covered Call</a></li>
      <li><a href="#protective-put">Protective Put</a></li>
      <li><a href="#iron-condor">Iron Condor</a></li>
    </ul>
  </li>
  <li><a href="#arbre-trinomial">Arbre Trinomial</a>
    <ul>
      <li><a href="#principe-de-larbre-trinomial">Principe de l'Arbre Trinomial</a></li>
      <li><a href="#fonctionnement-de-limplémentation">Fonctionnement de l'Implémentation</a></li>
    </ul>
  </li>
  <li><a href="#installation">Installation</a></li>
  <li><a href="#structure-du-projet">Structure du Projet</a></li>
  <li><a href="#dépendances">Dépendances</a></li>
</ul>

<h2 id="introduction">Introduction</h2>
<p>Ce projet vise à fournir une implémentation de diverses stratégies de trading d'options financières en utilisant le langage C#. Il permet aux utilisateurs de :</p>
<ul>
  <li>Modifier des stratégies d'options complexes.</li>
  <li>Calculer les payoffs et les profits associés aux stratégies.</li>
  <li>Exporter les données et les graphiques vers un fichier Excel pour une analyse approfondie.</li>
</ul>

<h2 id="utilisation-du-programme">Utilisation du Programme</h2>

<h3 id="sélection-des-stratégies">Sélection des Stratégies</h3>
<p>Pour utiliser le programme et sélectionner les stratégies à analyser, vous devez modifier le fichier <code>Program.cs</code>. Dans ce fichier, vous trouverez une liste de stratégies initialisée comme suit :</p>

<pre><code>List&lt;Strategy&gt; strategies = new List&lt;Strategy&gt;
{
    new Straddle(80.0, expirationDate, pricingDate, market),
    new BullSpread(85.0, 105.0, expirationDate, pricingDate, market),
    // Ajoutez ou modifiez les stratégies ici
};
</code></pre>

<h3 id="choix-des-paramètres">Choix des Paramètres</h3>
<p>Lors de l'ajout d'une stratégie, vous pouvez personnaliser les paramètres suivants :</p>
<ul>
  <li><strong>Prix d'exercice (Strike Price)</strong> : Le prix auquel l'option peut être exercée.</li>
  <li><strong>Date d'expiration</strong> : La date à laquelle l'option expire.</li>
  <li><strong>Date de valorisation</strong> : La date actuelle ou la date à laquelle l'option est évaluée.</li>
  <li><strong>Méthode de pricing</strong> : La méthode utilisée pour calculer le prix de l'option (par exemple, Black-Scholes, Arbre Trinomial).</li>
</ul>

<h3 id="références-pour-les-stratégies">Références pour les Stratégies</h3>
<p>Pour choisir les options qui composent les stratégies, vous pouvez vous référer à la section <a href="#stratégies-détaillées">Stratégies Détailées</a> de ce document, qui décrit en détail les options et la composition de chaque stratégie.</p>

<h2 id="fonctionnement-interne">Fonctionnement Interne</h2>

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

<h4>Calcul du Payoff pour une Option</h4>
<ul>
  <li><strong>Option Call</strong> : \( \text{Payoff} = \max(S - K, 0) \)</li>
  <li><strong>Option Put</strong> : \( \text{Payoff} = \max(K - S, 0) \)</li>
</ul>

<h2 id="stratégies-détaillées">Stratégies Détailées</h2>

<h3 id="straddle">Straddle</h3>
<p><strong>Description</strong> : Le Straddle est une stratégie neutre qui consiste à acheter une option Call et une option Put avec le même prix d'exercice et la même date d'expiration.</p>

<ul>
  <li><strong>Composition</strong> :
    <ul>
      <li>Acheter une option Call (K)</li>
      <li>Acheter une option Put (K)</li>
    </ul>
  </li>
  <li><strong>Payoff Théorique</strong> : \( \text{Payoff} = \max(S - K, 0) + \max(K - S, 0) \)</li>
  <li><strong>Formule du Profit</strong> : \( \text{Profit} = \text{Payoff} - \text{Prime Call} - \text{Prime Put} \)</li>
</ul>

<!-- Continue de cette manière pour chaque stratégie (Bull Spread, Bear Spread, etc.) -->

<h2 id="arbre-trinomial">Arbre Trinomial</h2>
<h3 id="principe-de-larbre-trinomial">Principe de l'Arbre Trinomial</h3>
<p>L'arbre trinomial permet d'estimer les prix des options en représentant différentes étapes de variation du prix sous-jacent.</p>

<h3 id="fonctionnement-de-limplémentation">Fonctionnement de l'Implémentation</h3>
<p>L'algorithme utilise une approche de répartition des probabilités pour les trois branches possibles : montée, descente, et stabilité.</p>

<h2 id="installation">Installation</h2>
<p>Instructions pour installer les dépendances et compiler le projet.</p>

<h2 id="structure-du-projet">Structure du Projet</h2>
<ul>
  <li><code>Program.cs</code> : Point d'entrée du programme.</li>
  <li><code>EuropeanOption.cs</code> : Classe définissant les options européennes.</li>
  <!-- Autres fichiers de structure du projet -->
</ul>

<h2 id="dépendances">Dépendances</h2>
<p>Liste des bibliothèques requises.</p>

