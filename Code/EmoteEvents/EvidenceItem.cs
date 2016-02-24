using EmoteCommonMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmoteEvents
{
    public class EvidenceItem
    {
        public EvidenceItem()
        {
          
        }
        public EvidenceItem(int learnerId, int stepId, int activityId, int scenarioId, int emoteScenarioId, string evidenceName,double value,EvidenceType evidenceType, int sessionId)
        {
            this.evidenceName = evidenceName;
            this.evidenceType = evidenceType;
            this.actual = value.ToString();
            this.learnerId = learnerId;
            this.stepId = stepId;
            this.activityId = activityId;
            this.scenarioId = scenarioId;
            this.emoteScenarioId = emoteScenarioId;
            this.sessionId = sessionId;
        }
     
        public EvidenceType evidenceType { get; set; }
        public String mapEventId { get; set; }
        public String evidenceName { get; set; }
        public int learnerId { get; set; }    
        public int stepId { get; set; }
        public int activityId { get; set; }
        public int scenarioId { get; set; }
        public int sessionId { get; set; }
        public int emoteScenarioId { get; set; }
        public Boolean correct { get; set; }
        public String actual { get; set; }
        public String expected { get; set; }
        public String action { get; set; }
      
        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static EvidenceItem DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (EvidenceItem)serializer.Deserialize(textReader, typeof(EvidenceItem));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize EvidenceItem from '" + serialized + "': " + e.Message);
            }
            return null;
        }
    }
}
