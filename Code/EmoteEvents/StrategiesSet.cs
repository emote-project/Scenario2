using System;
using System.Collections.Generic;
using System.IO;
using EmoteEnercitiesMessages;
using Newtonsoft.Json;

namespace EmoteEvents
{
    public class StrategiesSet
    {
        public StrategiesSet(Dictionary<EnercitiesRole, double[]> strategies)
        {
            this.Strategies = strategies;
        }

        public Dictionary<EnercitiesRole, double[]> Strategies { get; set; }

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();

        }

        public static StrategiesSet DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (StrategiesSet) serializer.Deserialize(textReader, typeof (StrategiesSet));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize StrategiesSet from '" + serialized + "': " + e.Message);
            }
            return null;
        }
    }
}