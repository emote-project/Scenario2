using EmoteCommonMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmoteEvents.ComplexData;

namespace EmoteEvents
{
    public class LightUtteranceHistoryItem : JsonSerializable
    {
        public String LibraryId { get; set; }//the id of the uterance in the library
        public String Library { get; set; }


        public LightUtteranceHistoryItem()
        {
        }

    }

    public class UtteranceHistoryItem
    {
        public UtteranceHistoryItem()
        {
          
        }


        public int id { get; set; }//database id. 
        public String utteranceId { get; set; }//unique id for the instance
        public String learner { get; set; }
        public String utterance { get; set; }
        public String category { get; set; }
        public String subcategory { get; set; }
        public String start { get; set; }
        public String end { get; set; }
        public String canceledTime { get; set; }
        public Boolean canceled { get; set; }
        public Boolean question { get; set; }
        public String library { get; set; }
		public String repetitions { get; set; }
        public String libraryId { get; set; }//the id of the uterance in the library
        public String sessionNumber { get; set; }//the id of the uterance in the library
        
        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static UtteranceHistoryItem DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (UtteranceHistoryItem)serializer.Deserialize(textReader, typeof(UtteranceHistoryItem));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize Utterance from '" + serialized + "': " + e.Message);
            }
            return null;
        }
    }
}
