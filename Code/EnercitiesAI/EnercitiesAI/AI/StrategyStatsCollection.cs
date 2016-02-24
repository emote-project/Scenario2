using System.Collections.Generic;
using EmoteEvents;
using PS.Utilities.IO;
using PS.Utilities.Math;
using PS.Utilities.Serialization;

namespace EnercitiesAI.AI
{
    public class StrategyStatsCollection : StatisticsCollection
    {
        private const string ENVIRONMENT_SCORE = "EnvironmentScore";
        private const string ECONOMY_SCORE = "EconomyScore";
        private const string WELLBEING_SCORE = "WellbeingScore";
        private const string MONEY = "Money";
        private const string HOMES = "Homes";
        private const string OIL = "Oil";
        private const string POWER = "Power";
        private const string SCORE_UNIFORMITY = "ScoreUniformity";
        private readonly string _prefix;
        private List<StatisticalQuantity> _allQuantities;

        public StrategyStatsCollection(string prefix)
        {
            this._prefix = prefix;

            //creates stats and add to collection
            this.Add(this.GetStatName(HOMES), this.Homes = new StatisticalQuantity());
            this.Add(this.GetStatName(MONEY), this.Money = new StatisticalQuantity());
            this.Add(this.GetStatName(OIL), this.Oil = new StatisticalQuantity());
            this.Add(this.GetStatName(POWER), this.Power = new StatisticalQuantity());
            this.Add(this.GetStatName(ENVIRONMENT_SCORE), this.EnvironmentScore = new StatisticalQuantity());
            this.Add(this.GetStatName(ECONOMY_SCORE), this.EconomyScore = new StatisticalQuantity());
            this.Add(this.GetStatName(WELLBEING_SCORE), this.WellbeingScore = new StatisticalQuantity());
            this.Add(this.GetStatName(SCORE_UNIFORMITY), this.ScoresUniformity = new StatisticalQuantity());

            this.GatherAllStats();
        }

        public ulong ValueCount
        {
            get { return this.Homes.ValueCount; }
        }

        private void GatherAllStats()
        {
            this._allQuantities = new List<StatisticalQuantity>
                                  {
                                      this.EnvironmentScore,
                                      this.EconomyScore,
                                      this.WellbeingScore,
                                      this.Money,
                                      this.Power,
                                      this.Oil,
                                      this.Homes,
                                      this.ScoresUniformity
                                  };
        }

        private static readonly string[] AllLabels =
        {
            ENVIRONMENT_SCORE, ECONOMY_SCORE, WELLBEING_SCORE, MONEY, HOMES, OIL, POWER, SCORE_UNIFORMITY
        };

        #region Statistical quantities

        public StatisticalQuantity Homes { get; private set; }
        public StatisticalQuantity Money { get; private set; }
        public StatisticalQuantity Oil { get; private set; }
        public StatisticalQuantity Power { get; private set; }
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

        public void Update(Strategy strategy)
        {
            this.EnvironmentScore.Value = strategy.EnvironmentWeight;
            this.EconomyScore.Value = strategy.EconomyWeight;
            this.WellbeingScore.Value = strategy.WellbeingWeight;
            this.Homes.Value = strategy.HomesWeight;
            this.Money.Value = strategy.MoneyWeight;
            this.Oil.Value = strategy.OilWeight;
            this.Power.Value = strategy.PowerWeight;
            this.ScoresUniformity.Value = strategy.ScoreUniformityWeight;
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
                this.PrintResultsTogether(basePath);
        }

        public void SaveToJson(string filePath)
        {
            this.SerializeJsonFile(filePath);
        }

        public static StrategyStatsCollection LoadFromJson(string filePath)
        {
            return JsonUtil.DeserializeJsonFile<StrategyStatsCollection>(filePath);
        }

        public void LoadResultsIndividualy(string basePath)
        {
            this.EnvironmentScore = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(ENVIRONMENT_SCORE)));
            this.EconomyScore = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(ECONOMY_SCORE)));
            this.WellbeingScore = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(WELLBEING_SCORE)));
            this.Homes = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(HOMES)));
            this.Money = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(MONEY)));
            this.Oil = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(OIL)));
            this.Power = StatisticalQuantity.LoadFromJson(
                string.Format("{0}/{1}.json", basePath, this.GetFileStatName(POWER)));

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

        private void PrintResultsTogether(string basePath)
        {
            var allQuantities = new Dictionary<string, StatisticalQuantity>
                                {
                                    {this.GetFileStatName(ECONOMY_SCORE), this.EconomyScore},
                                    {this.GetFileStatName(ENVIRONMENT_SCORE), this.EnvironmentScore},
                                    {this.GetFileStatName(WELLBEING_SCORE), this.WellbeingScore},
                                    {this.GetFileStatName(MONEY), this.Money},
                                    {this.GetFileStatName(POWER), this.Power},
                                    {this.GetFileStatName(OIL), this.Oil},
                                    {this.GetFileStatName(HOMES), this.Homes},
                                    {this.GetFileStatName(SCORE_UNIFORMITY), this.ScoresUniformity}
                                };
            StatisticsUtil.PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}strategy.csv", basePath, this._prefix), allQuantities);
        }

        #endregion
    }
}