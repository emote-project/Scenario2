using EmoteCommonMessages;
using EmoteEnercitiesMessages;
using Thalamus;
using Thalamus.BML;

namespace Simulator
{
    public partial class THClient : ThalamusClient
    {
        public const string CLIENT_NAME = "Simulator";
        public const string CASE_POOL_FILE = "casepool.json";
        public const string DEFAULT_CHARACTER_NAME = "Eux";

        public interface IAllActionPublisher :
       IThalamusPublisher,
       IEnercitiesGameStateEvents,
       IEnercitiesAIPerceptions,
       IEnercitiesTaskActions,
       IGazeStateActions,
       IHeadActions,
       IFMLSpeech,
       IAnimationActions
        {
        }

        public class ControllerPublisher : IAllActionPublisher
        {
            private readonly dynamic _publisher;

            public ControllerPublisher(dynamic publisher)
            {
                this._publisher = publisher;
            }

            #region IControllerPublisher Members

            public void EndGameNoOil(int totalScore)
            {
                this._publisher.EndGameNoOil(totalScore);
            }

            public void EndGameSuccessfull(int totalScore)
            {
                this._publisher.EndGameSuccessfull(totalScore);
            }

            public void EndGameTimeOut(int totalScore)
            {
                this._publisher.EndGameTimeOut(totalScore);
            }

            public void GameStarted(string player1Name, string player1Role, string player2Name, string player2Role)
            {
                this._publisher.GameStarted(player1Name, player1Role, player2Name, player2Role);
            }

            public void PlayersGender(Gender player1Gender, Gender player2Gender)
            {
                this._publisher.PlayersGender(player1Gender, player2Gender);
            }

            public void PlayersGenderString(string player1Gender, string player2Gender)
            {
                this._publisher.PlayersGenderString(player1Gender, player2Gender);
            }

            public void ReachedNewLevel(int level)
            {
                this._publisher.ReachedNewLevel(level);
            }

            public void ResumeGame(
                string player1Name, string player1Role, string player2Name, string player2Role, string serializedGameState)
            {
                this._publisher.ResumeGame(player1Name, player1Role, player2Name, player2Role, serializedGameState);
            }

            public void StrategyGameMoves(
                string environmentalistMove, string economistMove, string mayorMove, string globalMove)
            {
                this._publisher.StrategyGameMoves(environmentalistMove, economistMove, mayorMove, globalMove);
            }

            public void TurnChanged(string serializedGameState)
            {
                this._publisher.TurnChanged(serializedGameState);
            }

            public void UpdateStrategies(string StrategiesSet_strategies)
            {
                this._publisher.UpdateStrategies(StrategiesSet_strategies);
            }

            public void ConfirmConstruction(StructureType structure, int cellX, int cellY)
            {
                this._publisher.ConfirmConstruction(structure, cellX, cellY);
            }

            public void ImplementPolicy(PolicyType policy)
            {
                this._publisher.ImplementPolicy(policy);
            }

            public void PerformUpgrade(UpgradeType upgrade, int cellX, int cellY)
            {
                this._publisher.PerformUpgrade(upgrade, cellX, cellY);
            }

            public void PlayStrategy(EnercitiesStrategy strategy)
            {
                this._publisher.PlayStrategy(strategy);
            }

            public void SkipTurn()
            {
                this._publisher.SkipTurn();
            }

            public void GazeAtScreen(double x, double y)
            {
                this._publisher.GazeAtScreen(x, y);
            }

            public void GazeAtTarget(string targetName)
            {
                this._publisher.GazeAtTarget(targetName);
            }

            public void GlanceAtScreen(double x, double y)
            {
                this._publisher.GlanceAtScreen(x, y);
            }

            public void GlanceAtTarget(string targetName)
            {
                this._publisher.GlanceAtTarget(targetName);
            }

            public void Head(string id, string lexeme, int repetitions, double amplitude = 20.0f, double frequency = 1.0f)
            {
                this._publisher.Head(id, lexeme, repetitions, amplitude, frequency);
            }

            public void CancelUtterance(string id)
            {
                this._publisher.CancelUtterance(id);
            }

            public void PerformUtterance(string id, string utterance, string category)
            {
                this._publisher.PerformUtterance(id, utterance, category);
            }

            public void PerformUtteranceFromLibrary(
                string id, string category, string subcategory, string[] tagNames, string[] tagValues)
            {
                this._publisher.PerformUtteranceFromLibrary(id, category, subcategory, tagNames, tagValues);
            }

            public void ReplaceTagsAndPerform(string utterance, string category)
            {
                this._publisher.ReplaceTagsAndPerform(utterance, category);
            }

            public void PlayAnimation(string id, string animation)
            {
                this._publisher.PlayAnimation(id, animation);
            }

            public void PlayAnimationQueued(string id, string animation)
            {
                this._publisher.PlayAnimationQueued(id, animation);
            }

            public void StopAnimation(string id)
            {
                this._publisher.StopAnimation(id);
            }

            #endregion
        }

        public THClient(string character = DEFAULT_CHARACTER_NAME)
            : base(CLIENT_NAME, character)
        {
            this.SetPublisher<ControllerPublisher>();
            

        }


    }
}