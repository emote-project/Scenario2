using System.Collections.Generic;
using System.Linq;
using LogicWebLib;

namespace InOutEmote.behaviours
{
    public class PerformWrapup : Behaviour
    {
        private int _session;
        private string[] _tagNames;
        private string[] _tagValues;
        private PerformUtterance _performMeUtt;

        public PerformWrapup()
        {
            var gs = GameState.GetInstance();
            _session = gs.Session;
            var tagsAndValues = gs.GetTagNamesAndValues();
            _tagNames = tagsAndValues.Keys.ToArray();
            _tagValues = tagsAndValues.Values.ToArray();
        }

        public override void BehaviourTask()
        {
            string subcategory = null;
            string category = null;
            if (_session == 3)
            {
                category = UtterancesMapping.WRAPUP_SESS3_NOOIL.Key;
                subcategory = UtterancesMapping.WRAPUP_SESS3_NOOIL.Value;

            } 
            if (_session == 4)
            {
                category = UtterancesMapping.WRAPUP_SESS4_NOOIL.Key;
                subcategory = UtterancesMapping.WRAPUP_SESS4_NOOIL.Value;
            }
            _performMeUtt = new PerformUtterance(new KeyValuePair<string, string>(category, subcategory), _tagNames, _tagValues);
            _performMeUtt.BehaviourStateChangedEvent += performMEUtt_BehaviourStateChangedEvent;
            _performMeUtt.Execute();
        }

        private void performMEUtt_BehaviourStateChangedEvent(Behaviour behaviour)
        {
            ExecutionEnded();
        }
    }
}
