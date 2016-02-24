using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Thalamus.Conflicts
{
    public class ConflictSettings
    {
        public enum DefaultRuleType
        {
            All,
            Ignore,
            None
        }

        public Dictionary<string, Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>> ConflictRules { get; set; }
        public DefaultRuleType DefaultRuleOnActionPublishingConflict = DefaultRuleType.Ignore;
        public DefaultRuleType DefaultRuleOnActionSubscriptionConflict = DefaultRuleType.Ignore;
        public DefaultRuleType DefaultRuleOnPerceptionPublishingConflict = DefaultRuleType.Ignore;
        public DefaultRuleType DefaultRuleOnPerceptionSubscriptionConflict = DefaultRuleType.Ignore;
        #region File saving and loading

        public static ConflictSettings LoadConflictSettings(string filename)
        {
            using (StreamReader file = File.OpenText(filename))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    ConflictSettings c = (ConflictSettings)serializer.Deserialize(file, typeof(ConflictSettings));
                    return c;
                }
                catch 
                {
                    return new ConflictSettings();
                }
            }
        }
        public static void Save(string filename, ConflictSettings conflictSettings)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename))) Directory.CreateDirectory(Path.GetDirectoryName(filename));
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, conflictSettings);
            }
        }

        public ConflictSettings()
        {
            ConflictRules = new Dictionary<string, Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>>();
        }

        #endregion
    }
}
