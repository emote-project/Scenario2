using System.Globalization;

namespace EmotionalClimateClassification
{
    public class OkaoPerception
    {
        public double Time { get; set; }
        public uint Anger { get; set; }
        public uint Disgust { get; set; }
        public uint Fear { get; set; }
        public uint Joy { get; set; }
        public string LookAt { get; set; }
        public double LookAtX { get; set; }
        public double LookAtY { get; set; }
        public uint Neutral { get; set; }
        public uint Sadness { get; set; }
        public uint Surprise { get; set; }
        public uint Smile { get; set; }
        public uint SmileConfidence { get; set; }

        public static string[] HeaderStrings
        {
            get
            {
                var strs = new string[11];
                strs[0] = "Anger";
                strs[1] = "Disgust";
                strs[2] = "Fear";
                strs[3] = "Joy";
                strs[4] = "Sadness";
                strs[5] = "Surprise";
                strs[6] = "Neutral";
                strs[7] = "LookAt";
                strs[8] = "LookAtX";
                strs[9] = "LookAtY";
                strs[10] = "Smile";
                return strs;
            }
        }

        public string[] ToStrings()
        {
            var strs = new string[11];
            strs[0] = this.Anger.ToString(CultureInfo.InvariantCulture);
            strs[1] = this.Disgust.ToString(CultureInfo.InvariantCulture);
            strs[2] = this.Fear.ToString(CultureInfo.InvariantCulture);
            strs[3] = this.Joy.ToString(CultureInfo.InvariantCulture);
            strs[4] = this.Sadness.ToString(CultureInfo.InvariantCulture);
            strs[5] = this.Surprise.ToString(CultureInfo.InvariantCulture);
            strs[6] = this.Neutral.ToString(CultureInfo.InvariantCulture);
            strs[7] = this.LookAt.ToString(CultureInfo.InvariantCulture);
            strs[8] = this.LookAtX.ToString(CultureInfo.InvariantCulture);
            strs[9] = this.LookAtY.ToString(CultureInfo.InvariantCulture);
            strs[10] = this.Smile.ToString(CultureInfo.InvariantCulture);
            return strs;
        }
    }
}