using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skene.Utterances.HistoryManager
{
    static class HistoryManagerFactory
    {

        static IUtterancesHistoryManager historyManager = null;

        public static IUtterancesHistoryManager GetHistoryManager()
        {
            if (historyManager == null) historyManager = new LearnerModelHistoryManager();
            return historyManager;
        }
    }
}
