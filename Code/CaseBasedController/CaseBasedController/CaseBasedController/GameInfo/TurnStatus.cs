using System;
using System.Collections.Generic;
using EmoteEnercitiesMessages;
using EmoteEvents;

namespace CaseBasedController.GameInfo
{
    /// <summary>
    ///     Represents the state of the game in one specific turn
    /// </summary>
    public class TurnStatus : ICloneable
    {
        public TurnStatus()
        {
            TurnNumber = 0;
        }

        public double NormalizedPopulation { get;  set; }

        public double NormalizedPower { get;  set; }

        public double NormalizedOil { get;  set; }

        public double NormalizedMoney { get;  set; }

        public double NormalizedResourcesAverage { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public Player PlayerAI { get; set; }

        public Player CurrentPlayer { get; set; }

        public TranslatebleEnum PlayedPolicy { get; set; }

        public TranslatebleEnum PlayedStructure { get; set; }

        public TranslatebleEnum PlayedUpgrade { get; set; }

        public EnercitiesGameInfo GameScores { get; set; }

        public int TurnNumber { get; set; }

        public List<EnercitiesActionInfo> BestActionsForThisTurn { get; set; }

        public Strategy StrategiesUsedForLastBestAction { get; set; }

        public MemoryEvent MemoryEventData { get; set; }

        public int  CurrentLevel { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        public string GetCurrentPlayerSide()
        {
            if (CurrentPlayer != null && !CurrentPlayer.IsAI())
            {
                if (CurrentPlayer.Role == EnercitiesRole.Environmentalist) return "Left";
                return "Right";
            }
            return null;
        }
    }
}