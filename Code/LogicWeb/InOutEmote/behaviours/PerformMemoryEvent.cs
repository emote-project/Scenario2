using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.behaviours
{
    public class PerformMemoryEvent : Behaviour
    {
        private string _subcategory = "";
        private string _category = "";
        private string[] _tagNames;
        private string[] _tagValues;
        private PerformUtterance _performMeUtt;


        public PerformMemoryEvent()
        {
            var gameState = GameState.GetInstance();
            if (gameState.CurrentState.MemoryEventData.memoryEventItems != null &&
                gameState.CurrentState.MemoryEventData.memoryEventItems.Any())
            {
                var lastMemoryEvent = gameState.CurrentState.MemoryEventData.memoryEventItems[0];
                _category = lastMemoryEvent.category;
                _subcategory = lastMemoryEvent.subcategory;
                _tagNames = lastMemoryEvent.tagNames;
                _tagValues = lastMemoryEvent.tagValues;
                _performMeUtt = new PerformUtterance(new KeyValuePair<string, string>(_category, _subcategory), _tagNames, _tagValues);
                _performMeUtt.BehaviourStateChangedEvent += performMEUtt_BehaviourStateChangedEvent;
            }
        }

        public override void BehaviourTask()
        {
            _performMeUtt.Execute();
        }

        void performMEUtt_BehaviourStateChangedEvent(Behaviour behaviour)
        {
            if (behaviour.BehaviourState == BehaviourStateType.Executed)
            {
                ExecutionEnded();
            }
        }
    }
}
