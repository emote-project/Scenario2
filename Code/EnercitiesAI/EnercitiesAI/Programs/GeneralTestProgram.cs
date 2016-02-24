using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.AI;
using EnercitiesAI.AI.Game;

namespace EnercitiesAI.Programs
{
    internal class GeneralTestProgram
    {
        private const double POWER_STRAT_ADJUST_PARAM = 0.7;
        private const double MONEY_STRAT_ADJUST_PARAM = 0.99; // 0.9; //0.96;
        private const double OIL_STRAT_ADJUST_PARAM = 0.999; //0.999; //0.996;
        private const double SCORE_STRAT_ADJUST_PARAM = 0.7;
        private const double HOMES_STRAT_ADJUST_PARAM = 0.3; //0.3; //0.05; //0.0002;
        private const double ENVIRON_STRAT_ADJUST_PARAM = 0.5; //0.3; //0.05; //0.0002;

        private const double UNIFORM_SCORE_WEIGHT = 0.8; //0; //0.2;
        private const double NORMAL_SCORE_WEIGHT = 0; //0.2;
        private const double NORMAL_RESOURCE_WEIGHT = 0; //0.5;//0; // 0.4;
        private const double HOMES_WEIGHT = 1; //0.5;
        private const double OWN_SCORE_WEIGHT = 0.8; //0.7;
        private const double OWN_RESOURCE_WEIGHT = 0.3; //0; //0.6;


        private static void Main(string[] args)
        {
            //var statsCollection = new GameValuesStatsCollection("");
            //statsCollection.LoadResultsIndividualy(Path.GetFullPath("./Estimations"));
            //statsCollection.SaveToJson(Path.GetFullPath("./game-values-estimations.json"));

            var strategy = CreateStrategy(EnercitiesRole.Economist);
            strategy.Serialize(Game.ECO_STRATEGY_JSON);

            strategy = CreateStrategy(EnercitiesRole.Environmentalist);
            strategy.Serialize(Game.ENV_STRATEGY_JSON);

            strategy = CreateStrategy(EnercitiesRole.Mayor);
            strategy.Serialize(Game.MAY_STRATEGY_JSON);
        }

        private static Strategy CreateStrategy(EnercitiesRole role)
        {
            //sets default strategy
            var strategy = new Strategy
                           {
                               PowerAdjustParam = POWER_STRAT_ADJUST_PARAM,
                               MoneyAdjustParam = MONEY_STRAT_ADJUST_PARAM,
                               OilAdjustParam = OIL_STRAT_ADJUST_PARAM,
                               ScoreAdjustParam = SCORE_STRAT_ADJUST_PARAM,
                               HomesAdjustParam = HOMES_STRAT_ADJUST_PARAM,
                               EnvironmentAdjustParam = ENVIRON_STRAT_ADJUST_PARAM,
                               EconomyWeight = NORMAL_SCORE_WEIGHT,
                               EnvironmentWeight = NORMAL_SCORE_WEIGHT,
                               WellbeingWeight = NORMAL_SCORE_WEIGHT,
                               MoneyWeight = NORMAL_RESOURCE_WEIGHT,
                               OilWeight = NORMAL_RESOURCE_WEIGHT,
                               PowerWeight = NORMAL_RESOURCE_WEIGHT,
                               HomesWeight = HOMES_WEIGHT,
                               ScoreUniformityWeight = UNIFORM_SCORE_WEIGHT,
                           };

            //changes initial strategy according to role
            if (role.Equals(EnercitiesRole.Economist))
            {
                strategy.EconomyWeight = OWN_SCORE_WEIGHT;
                strategy.MoneyWeight = OWN_RESOURCE_WEIGHT;
            }
            else if (role.Equals(EnercitiesRole.Environmentalist))
            {
                strategy.EnvironmentWeight = OWN_SCORE_WEIGHT;
                strategy.OilWeight = OWN_RESOURCE_WEIGHT;
            }
            else if (role.Equals(EnercitiesRole.Mayor))
            {
                strategy.WellbeingWeight = OWN_SCORE_WEIGHT;
                strategy.PowerWeight = OWN_RESOURCE_WEIGHT;
            }

            strategy.Normalize();

            return strategy;
        }
    }
}