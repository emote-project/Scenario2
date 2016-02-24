using EmoteCommonMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmoteEvents
{
    public class LearnerInfo
    {
        public LearnerInfo()
        {
          
        }


        public LearnerInfo(String firstName, String middleName, String lastName, int mapApplicationId, String sex, String birth, int thalamusLearnerId)
        {
            this.firstName = firstName;
            this.middleName = middleName;
            this.lastName = lastName;
            this.mapApplicationId = mapApplicationId;
            this.sex = sex;
            this.birth = birth;
            this.thalamusLearnerId = thalamusLearnerId;
        }

       	public String firstName { get; set; }
        public String middleName { get; set; }
        public String lastName { get; set; }
	    public int mapApplicationId { get; set; } //id for the map application 
	    public String sex { get; set; }
	    public String birth { get; set; }
        public int thalamusLearnerId { get; set; } //main thalamus id
        public int scenario1Difficulty { get; set; } //difficulty for s1 scenario 1-3... 
        
        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static LearnerInfo DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (LearnerInfo)serializer.Deserialize(textReader, typeof(LearnerInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize LearnerInfo from '" + serialized + "': " + e.Message);
            }
            return null;
        }
    }
}
