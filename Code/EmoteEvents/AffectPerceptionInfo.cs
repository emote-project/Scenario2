using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmoteEvents
{
    public class AffectPerceptionInfo
    {
        public AffectPerceptionInfo()
        {
            mAffectiveStates = new List<AffectType>();
        }

        public List<AffectType> mAffectiveStates { get; set; }
        public String mMapEventId { get; set; }

        public class AffectType
        {
            public AffectType(EmoteCommonMessages.AffectPerceptionState state, EmoteCommonMessages.Charge charge, EmoteCommonMessages.PointofFocus focus, int confidence)
            {
                mState = state;
                mStateCharge = charge;
                mFocus = focus;
                mStateConfidence = confidence;
            }

            public EmoteCommonMessages.AffectPerceptionState mState { get; set; }
            public EmoteCommonMessages.Charge mStateCharge { get; set; }
            public int mStateConfidence { get; set; }
            public EmoteCommonMessages.PointofFocus mFocus { get; set; }
        }

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static AffectPerceptionInfo DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (AffectPerceptionInfo)serializer.Deserialize(textReader, typeof(AffectPerceptionInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize AffectPerceptionInfo from '" + serialized + "': " + e.Message);
            }
            return null;
        }
    }
}
