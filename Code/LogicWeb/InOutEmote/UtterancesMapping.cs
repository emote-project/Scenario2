using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote
{
    static class UtterancesMapping
    {
        public static readonly KeyValuePair<string, string> TURNCHANGED_OTHER = new KeyValuePair<string, string>("turntaking", "learner");
        public static readonly KeyValuePair<string, string> TURNCHANGED_SELF = new KeyValuePair<string, string>("turntaking", "robot");
        
        public static readonly KeyValuePair<string, string> GAMERULES_LEVELUP = new KeyValuePair<string, string>("gamerules", "levelup");
        public static readonly KeyValuePair<string,string> GAMERULES_GENERAL = new KeyValuePair<string, string>("gamerules","general");

        public static readonly KeyValuePair<string, string> GAMEENDED_NOOIL = new KeyValuePair<string, string>("endgame", "nooil");
        public static readonly KeyValuePair<string, string> GAMEENDED_WIN = new KeyValuePair<string, string>("endgame", "successful");
        public static readonly KeyValuePair<string, string> GAMEENDED_TIMEUP = new KeyValuePair<string, string>("gamestatus", "timeup");

        public static readonly KeyValuePair<string, string> WRAPUP_SESS3_NOOIL = new KeyValuePair<string, string>("wrapup", "5");
        public static readonly KeyValuePair<string, string> WRAPUP_SESS4_NOOIL = new KeyValuePair<string, string>("wrapup", "8");

        public static readonly KeyValuePair<string, string> TUTORIAL_BASE = new KeyValuePair<string, string>("greeting", "welcome");
        public static readonly KeyValuePair<string, string> TUTORIAL_OWNCONSTRUCTION = new KeyValuePair<string, string>("tutorial", "ownconstruction");
        public static readonly KeyValuePair<string, string> TUTORIAL_OWNPOLICY = new KeyValuePair<string, string>("tutorial", "ownpolicy");
        public static readonly KeyValuePair<string, string> TUTORIAL_OWNSKIP = new KeyValuePair<string, string>("tutorial", "ownskip");
        public static readonly KeyValuePair<string, string> TUTORIAL_OWNUPGRADE = new KeyValuePair<string, string>("tutorial", "ownupgrade");

        public static readonly KeyValuePair<string, string> CONFIRMCONSTRUCTION_SELF = new KeyValuePair<string, string>("confirmconstruction", "self");
        public static readonly KeyValuePair<string, string> PERFORMUPGRADE_SELF = new KeyValuePair<string, string>("performupgrade", "self");
        public static readonly KeyValuePair<string, string> IMPLEMENTPOLICY_SELF = new KeyValuePair<string, string>("implementpolicy", "self");


    }
}
