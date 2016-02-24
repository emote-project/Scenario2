using System.Threading;
using EmoteEnercitiesMessages;
using Thalamus;

namespace EnercitiesAI
{
    internal interface IEnercitiesAIPublisher : IThalamusPublisher, IEnercitiesAIActions, IEnercitiesTaskActions
    {
    }

    internal class EnercitiesAIPublisher : IEnercitiesAIPublisher
    {
        private const int SEND_ACTION_INTRV = 2000;
        private readonly dynamic _publisher;

        public EnercitiesAIPublisher(dynamic publisher)
        {
            this._publisher = publisher;
        }

        #region IEnercitiesAIPublisher Members

        public void StrategiesUpdated(string StrategiesSet_strategies)
        {
            this._publisher.StrategiesUpdated(StrategiesSet_strategies);
        }

        public void BestActionPlanned(string[] EnercitiesActionInfo_actionInfos)
        {
            this._publisher.BestActionPlanned(EnercitiesActionInfo_actionInfos);
        }

        public void BestActionsPlanned(EnercitiesRole currentPlayer, string[] EnercitiesActionInfo_actionInfos)
        {
            this._publisher.BestActionsPlanned(currentPlayer, EnercitiesActionInfo_actionInfos);
        }

        public void ActionsPlanned(EnercitiesRole currentPlayer, string Strategy_planStrategy,
            string[] EnercitiesActionInfo_bestActionInfos, string[] EnercitiesActionInfo_worstActionInfos)
        {
            this._publisher.ActionsPlanned(currentPlayer, Strategy_planStrategy,
                EnercitiesActionInfo_bestActionInfos, EnercitiesActionInfo_worstActionInfos);
        }

        public void PredictedValuesUpdated(double[] values)
        {
            this._publisher.PredictedValuesUpdated(values);
        }

        public void PlayStrategy(EnercitiesStrategy strategy)
        {
            this._publisher.PlayStrategy(strategy);
        }

        public void ConfirmConstruction(StructureType structure, int cellX, int cellY)
        {
            this._publisher.ConfirmConstruction(structure, cellX, cellY);
            Thread.Sleep(SEND_ACTION_INTRV);
        }

        public void ImplementPolicy(PolicyType policy)
        {
            this._publisher.ImplementPolicy(policy);
            Thread.Sleep(SEND_ACTION_INTRV);
        }

        public void PerformUpgrade(UpgradeType upgrade, int cellX, int cellY)
        {
            this._publisher.PerformUpgrade(upgrade, cellX, cellY);
            Thread.Sleep(SEND_ACTION_INTRV);
        }

        public void SkipTurn()
        {
            this._publisher.SkipTurn();
            Thread.Sleep(SEND_ACTION_INTRV);
        }

        #endregion
    }
}