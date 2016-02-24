using EmoteCommonMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmoteEvents
{
    public class LearnerStateInfo
    {
        public LearnerStateInfo()
        {
            competencyItems = new List<CompetencyItem>();
        }


        public LearnerStateInfo(int learnerId, int stepId, int activityId, int scenarioId, int sessionId, bool correct, String mapEventId)
        {
            competencyItems = new List<CompetencyItem>();
            this.learnerId = learnerId;
            this.stepId = stepId;
            this.activityId = activityId;
            this.scenarioId = scenarioId;
            this.sessionId = sessionId;
            this.correct= correct;
            this.mapEventId = mapEventId;
        }

        public List<CompetencyItem> competencyItems { get; set; }
        public String mapEventId { get; set; }
        public int learnerId { get; set; }
        public String learnerName { get; set; } //For the IM to use for name of student. 
        public int stepId { get; set; }
        public int activityId { get; set; }
        public int scenarioId { get; set; }
        public int sessionId { get; set; }
        public EmoteCommonMessages.LearnerModelUpdateReason reasonForUpdate { get; set; }
        public Boolean correct { get; set; }

        public class CompetencyItem
        {
            
            public CompetencyItem(String competencyName, Boolean competencyCorrect,String competencyActual,String competencyExpected,
           double comptencyValue, int competencyConfidence, double oldCompetencyValue, double competencyDelta, EvidenceType competencyType, Impact impact)
            {
                this.competencyName = competencyName;
                
                this.competencyCorrect = competencyCorrect;
                this.competencyActual = competencyActual;
                this.competencyExpected = competencyExpected;
                this.comptencyValue = comptencyValue;
                this.competencyConfidence = competencyConfidence;
                this.oldCompetencyValue = oldCompetencyValue;
                this.competencyDelta = competencyDelta;
                this.competencyType = competencyType;
                this.impact = impact;
            }

            public CompetencyItem()
            {
              
            }
             

            public String competencyName { get; set; }
            public Boolean competencyCorrect { get; set; }
            public String competencyActual { get; set; }
            public String competencyExpected { get; set; }
            public double comptencyValue { get; set; }
            public int competencyConfidence { get; set; }
            public double oldCompetencyValue { get; set; }
            public double competencyDelta { get; set; }
            public EvidenceType competencyType { get; set; }
            public Impact impact { get; set; }

            public Delta working { get; set; } // this is in this session
            public Delta shortTerm { get; set; } // compared to last session
            public Delta longTerm { get; set; } // over all time

            public double finalvaluePreviousSession { get; set; }
            public double highestValuePreviousSessions { get; set; }
            public double lowestFinalValuePreviousSessions { get; set; }
        }

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static LearnerStateInfo DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (LearnerStateInfo)serializer.Deserialize(textReader, typeof(LearnerStateInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize LearnerStateInfo from '" + serialized + "': " + e.Message);
            }
            return null;
        }
    }
}
