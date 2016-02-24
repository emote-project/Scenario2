using System.Collections.Generic;
using PS.Utilities.IO;
using PS.Utilities.Math;
using PS.Utilities.Serialization;

namespace EnercitiesAI.Domain
{
    public class GameValuesStatsCollection : StatisticsCollection
    {
        private const string ENVIRONMENT_SCORE = "EnvironmentScore";
        private const string ECONOMY_SCORE = "EconomyScore";
        private const string WELLBEING_SCORE = "WellbeingScore";
        private const string MONEY = "Money";
        private const string POPULATION = "Population";
        private const string OIL = "Oil";
        private const string POWER_CONSUMPTION = "PowerConsumption";
        private const string POWER_PRODUCTION = "PowerProduction";
        private const string SCORE_UNIFORMITY = "ScoreUniformity";
        private readonly string _prefix;
        private List<StatisticalQuantity> _allQuantities;

        public GameValuesStatsCollection(string prefix)
        {
            this._prefix = prefix;

            //creates stats and add to collection
            this.Add(this.GetStatName(POPULATION), this.Population = new StatisticalQuantity());
            this.Add(this.GetStatName(MONEY), this.Money = new StatisticalQuantity());
            this.Add(this.GetStatName(OIL), this.Oil = new StatisticalQuantity());
            this.Add(this.GetStatName(POWER_CONSUMPTION), this.PowerConsumption = new StatisticalQuantity());
            this.Add(this.GetStatName(POWER_PRODUCTION), this.PowerProduction = new StatisticalQuantity());
            this.Add(this.GetStatName(ENVIRONMENT_SCORE), this.EnvironmentScore = new StatisticalQuantity());
            this.Add(this.GetStatName(ECONOMY_SCORE), this.EconomyScore = new StatisticalQuantity());
            this.Add(this.GetStatName(WELLBEING_SCORE), this.WellbeingScore = new StatisticalQuantity());
            this.Add(this.GetStatName(SCORE_UNIFORMITY), this.ScoresUniformity = new StatisticalQuantity());

            this.GatherAllStats();
        }

        public ulong ValueCount
        {
            get { return this.Population.ValueCount; }
        }

        private void GatherAllStats()
        {
            this._allQuantities = new List<StatisticalQuantity>
                                  {
                                      this.EnvironmentScore,
                                      this.EconomyScore,
                                      this.WellbeingScore,
                                      this.Money,
                                      this.Population,
                                      this.Oil,
                                      this.PowerConsumption,
                                      this.PowerProduction,
                                      this.ScoresUniformity
                                  };
        }

        private static readonly string[] AllLabels =
        {
            ENVIRONMENT_SCORE, ECONOMY_SCORE, WELLBEING_SCORE, MONEY, POPULATION, OIL, POWER_CONSUMPTION,
            POWER_PRODUCTION, SCORE_UNIFORMITY
        };

        #region Statistical quantities

        public StatisticalQuantity Population { get; private set; }
        public StatisticalQuantity Money { get; private set; }
        public StatisticalQuantity Oil { get; private set; }
        public StatisticalQuantity PowerConsumption { get; private set; }
        public StatisticalQuantity PowerProduction { get; private set; }
        public StatisticalQuantity EnvironmentScore { get; private set; }
        public StatisticalQuantity EconomyScore { get; private set; }
        public StatisticalQuantity WellbeingScore { get; private set; }
        public StatisticalQuantity ScoresUniformity { get; private set; }

        #endregion

        #region Public Methods

        public override void Dispose()
        {
            base.Dispose();
            this._allQuantities.Clear();
        }

        public void Update(GameValuesElement values)
        {
            this.EnvironmentScore.Value = values.EnvironmentScore;
            this.EconomyScore.Value = values.EconomyScore;
            this.WellbeingScore.Value = values.WellbeingScore;
            this.Population.Value = values.Homes;
            this.Money.Value = values.Money;
            this.Oil.Value = values.Oil;
            this.PowerConsumption.Value = values.PowerConsumption;
            this.PowerProduction.Value = values.PowerProduction;
            this.ScoresUniformity.Value = values.ScoresUniformity;
        }

        public GameValuesElement GetAverageGameValues()
        {
            return new GameValuesElement
                   {
                       EnvironmentScore = this.EnvironmentScore.Avg,
                       EconomyScore = this.EconomyScore.Avg,
                       WellbeingScore = this.WellbeingScore.Avg,
                       Homes = (int) this.Population.Avg,
                       Money = this.Money.Avg,
                       Oil = this.Oil.Avg,
                       PowerConsumption = this.PowerConsumption.Avg,
                       PowerProduction = this.PowerProduction.Avg
                       //ScoresUniformity = this.ScoresUniformity.Avg,
                   };
        }

        #endregion

        #region Normalization Methods

        public GameValuesElement GetNormalizedGameValues(GameValuesElement values)
        {
            var normValues = (GameValuesElement) values.Clone();
            normValues.EconomyScore = GetNormalizedGameValue(values.EconomyScore, this.EconomyScore);
            normValues.EnvironmentScore = GetNormalizedGameValue(values.EnvironmentScore, this.EnvironmentScore);
            normValues.WellbeingScore = GetNormalizedGameValue(values.WellbeingScore, this.WellbeingScore);
            normValues.Money = GetNormalizedGameValue(values.Money, this.Money);
            normValues.Oil = GetNormalizedGameValue(values.Oil, this.Oil);
            normValues.Homes = GetNormalizedGameValue(values.Homes, this.Population);
            normValues.PowerConsumption = GetNormalizedGameValue(values.PowerConsumption, this.PowerConsumption);
            normValues.PowerProduction = GetNormalizedGameValue(values.PowerProduction, this.PowerProduction);
            return normValues;
        }

        public double GetNormalizedScoresUniformity(GameValuesElement values)
        {
            return GetNormalizedGameValue(values.ScoresUniformity, this.ScoresUniformity);
        }

        private static double GetNormalizedGameValue(double value, StatisticalQuantity valueQuantity)
        {
            if (valueQuantity.ValueCount == 0) return value;

            //creates artificial boundaries
            var stdDev = 2*valueQuantity.StdDev;
            var max = 0.5*(valueQuantity.Avg + stdDev + valueQuantity.Max);
            var min = 0.5*(valueQuantity.Avg - stdDev + valueQuantity.Min);

            //checks out of boundaries
            if (value >= max) return 1.0;
            if (value <= min) return -1.0;
            if (value.Equals(0.0)) return 0.0;

            var retVal = (value - valueQuantity.Avg)/(0.5*(max - min));
            return retVal;// > 1.0 ? 1.0 : (retVal < -1.0 ? -1.0 : retVal);
        }

        #endregion

        #region Serialization Methods

        private string GetStatName(string statName)
        {
            return string.Format("{0}-{1}", this._prefix, statName);
        }

        private string GetFileStatName(string statName)
        {
            return this._prefix.Equals(string.Empty) ? statName : statName.Replace(this._prefix, string.Empty);
        }

        public void PrintResults(string basePath, bool separateFiles = true)
        {
            PathUtil.CreateDirectory(basePath);

            if (separateFiles)
                this.PrintResultsIndividualy(basePath);
            else
                this.PrintResultsByGroup(basePath);
        }

        public void SaveToJson(string filePath)
        {
            this.SerializeJsonFile(filePath);
        }

        public static GameValuesStatsCollection LoadFromJson(string filePath)
        {
            return JsonUtil.DeserializeJsonFile<GameValuesStatsCollection>(filePath);
        }

        public void LoadResultsIndividualy(string basePath)
        {
            this.EnvironmentScore = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(ENVIRONMENT_SCORE)));
            this.EconomyScore = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(ECONOMY_SCORE)));
            this.WellbeingScore = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(WELLBEING_SCORE)));
            this.Population = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(POPULATION)));
            this.Money = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(MONEY)));
            this.Oil = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(OIL)));
            this.PowerConsumption = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(POWER_CONSUMPTION)));
            this.PowerProduction = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(POWER_PRODUCTION)));

            this.GatherAllStats();
        }

        public void PrintResultsIndividualy(string basePath)
        {
            for (var i = 0; i < this._allQuantities.Count; i++)
            {
                var quantity = this._allQuantities[i];
                quantity.SaveToJson(string.Format("{0}/{1}.json", basePath, this.GetFileStatName(AllLabels[i])));
                quantity.PrintStatisticsToCSV(
                    string.Format("{0}/{1}.csv", basePath, this.GetFileStatName(AllLabels[i])), true, true);
            }
        }

        private void PrintResultsByGroup(string basePath)
        {
            //prints scores
            var scoresQuantList = new Dictionary<string, StatisticalQuantity>
                                  {
                                      {this.GetFileStatName(ECONOMY_SCORE), this.EconomyScore},
                                      {this.GetFileStatName(ENVIRONMENT_SCORE), this.EnvironmentScore},
                                      {this.GetFileStatName(WELLBEING_SCORE), this.WellbeingScore}
                                  };
            StatisticsUtil.PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}scores.csv", basePath, this._prefix), scoresQuantList);

            //prints resources
            var resourcesQuantList = new Dictionary<string, StatisticalQuantity>
                                     {
                                         {this.GetFileStatName(MONEY), this.Money},
                                         {this.GetFileStatName(POPULATION), this.Population},
                                         {this.GetFileStatName(OIL), this.Oil},
                                         {this.GetFileStatName(POWER_CONSUMPTION), this.PowerConsumption},
                                         {this.GetFileStatName(POWER_PRODUCTION), this.PowerProduction}
                                     };
            StatisticsUtil.PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}resources.csv", basePath, this._prefix), resourcesQuantList);
        }

        #endregion
    }
}