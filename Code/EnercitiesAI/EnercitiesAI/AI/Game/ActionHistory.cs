using System;
using System.Collections.Generic;
using System.Linq;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using Newtonsoft.Json;
using PS.Utilities;

namespace EnercitiesAI.AI.Game
{
    public class ActionHistory : List<ActionStatePair>, IDisposable
    {
        private const int DEF_MAX_PLAYER_HIST = 3;

        public ActionHistory()
        {
            LastPlayersActions = new Dictionary<EnercitiesRole, Queue<IPlayerAction>>();
            this.MaxPlayerHistory = DEF_MAX_PLAYER_HIST;
            foreach (var role in EnumUtil<EnercitiesRole>.GetTypes())
                this.LastPlayersActions.Add(role, new Queue<IPlayerAction>());
        }

        [JsonProperty]
        public int MaxPlayerHistory { get; set; }

        [JsonProperty]
        private Dictionary<EnercitiesRole, Queue<IPlayerAction>> LastPlayersActions { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (var queue in this.LastPlayersActions.Values)
                queue.Clear();
            this.LastPlayersActions.Clear();
        }

        #endregion

        public void Add(EnercitiesRole playerRole, ActionStatePair asp)
        {
            this.Add(asp);

            //adds action to player's last actions queue
            var lastPlayerActions = this.LastPlayersActions[playerRole];
            lastPlayerActions.Enqueue(asp.Action);

            //trims queue if necessary
            if (lastPlayerActions.Count > this.MaxPlayerHistory)
                lastPlayerActions.Dequeue();
        }

        public new void Clear()
        {
            base.Clear();
            foreach (var lastPlayerActions in this.LastPlayersActions.Values)
                lastPlayerActions.Clear();
        }

        public IPlayerAction GetLastAction(EnercitiesRole playerRole)
        {
            var lastPlayerActions = this.LastPlayersActions[playerRole];
            return lastPlayerActions.Count == 0
                ? null
                : lastPlayerActions.Last();
        }

        public int GetTimeSinceLastActionType(EnercitiesRole playerRole, IPlayerAction action)
        {
            var actionInfo = GetActionInfo(action);
            var lastPlayerActions = this.LastPlayersActions[playerRole];

            //traverses the queue (first action occured at longer time ago)
            var time = lastPlayerActions.Count - 1;
            foreach (var lastPlayerAction in lastPlayerActions)
            {
                //verifies action of the same type and sub-type
                var lastPlayerActionInfo = GetActionInfo(lastPlayerAction);
                if (actionInfo.ActionType.Equals(lastPlayerActionInfo.ActionType) &&
                    actionInfo.SubType.Equals(lastPlayerActionInfo.SubType))
                    return time;
                time--;
            }

            //not a recent action, return a high value
            return int.MaxValue;
        }

        private static EnercitiesActionInfo GetActionInfo(IPlayerAction action)
        {
            //if upgrades, chose first upgrade action
            return ((action is UpgradeStructures)
                ? ((UpgradeStructures) action).Upgrades[0]
                : action).ToEnercitiesActionInfo();
        }
    }
}