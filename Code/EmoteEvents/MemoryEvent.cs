using EmoteCommonMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmoteEvents
{
    public class MemoryEvent
    {
        public MemoryEvent()
        {
            memoryEventItems = new List<MemoryEventItem>();
        }

        public List<MemoryEventItem> memoryEventItems { get; set; }
        public int learnerId { get; set; }
        public int stepId { get; set; }
        public int activityId { get; set; }
        public int scenarioId { get; set; }
        public int sessionId { get; set; }
        public EmoteCommonMessages.LearnerModelUpdateReason reasonForUpdate { get; set; }


        public class MemoryEventItem
        {
            
            public MemoryEventItem()
            {
              
            }
             
            public String name { get; set; }
            public string category { get; set; }
            public string subcategory { get; set; }
            public string[] tagNames { get; set; }
            public string[] tagValues { get; set; }
            
          //  public Delta working { get; set; }
          //  public Delta shortTerm { get; set; }
          //  public Delta longTerm { get; set; }

          //  public double finalvaluePreviousSession { get; set; }
          //  public double highestValuePreviousSessions { get; set; }
          //  public double lowestFinalValuePreviousSessions { get; set; }
        }

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static MemoryEvent DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (MemoryEvent)serializer.Deserialize(textReader, typeof(MemoryEvent));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize MemoryEvent from '" + serialized + "': " + e.Message);
            }
            return null;
        }
    }
}
