using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmoteEvents.ComplexData
{
    public enum ScenarioLanguages
    {
        Portuguese,
        English,
        Swedish
    };

    public class StartMessageInfo : JsonSerializable
    {
        public List<LearnerInfo> Students { get; set; }
        public int SessionId { get; set; }
        public ScenarioLanguages Language { get; set; }
        public string ScenarioXmlName { get; set; }
        public bool IsEmpathic { get; set; }
    }
}
