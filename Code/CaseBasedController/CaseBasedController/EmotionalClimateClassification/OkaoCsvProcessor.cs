using System;
using System.Globalization;
using System.IO;

namespace EmotionalClimateClassification
{
    public class OkaoCsvProcessor : IDisposable
    {
        private const uint MAX_VALUE = 100;
        private const char SEPARATOR = ',';
        private readonly OkaoPerceptionFilter _perceptionFilter = new OkaoPerceptionFilter();
        private readonly StreamReader _reader;

        public OkaoCsvProcessor(string csvFile)
        {
            if (string.IsNullOrWhiteSpace(csvFile) || !File.Exists(csvFile))
                throw new ApplicationException(string.Format("Invalid csv file provided: {0}", csvFile));

            this._reader = new StreamReader(csvFile);
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._reader.Close();
            this._reader.Dispose();
        }

        #endregion

        public OkaoPerception ProcessLine()
        {
            //checks end of file
            var line = this._reader.ReadLine();
            if (line == null) return null;

            //reads elements
            var elems = line.Split(SEPARATOR);
            if (elems.Length < 20) return null;

            var perception = new OkaoPerception
                             {
                                 Time = double.Parse(elems[0], CultureInfo.InvariantCulture),
                                 Smile = uint.Parse(elems[9], CultureInfo.InvariantCulture),
                                 SmileConfidence = uint.Parse(elems[10], CultureInfo.InvariantCulture),
                                 Anger = uint.Parse(elems[11], CultureInfo.InvariantCulture),
                                 Disgust = uint.Parse(elems[12], CultureInfo.InvariantCulture),
                                 Fear = uint.Parse(elems[13], CultureInfo.InvariantCulture),
                                 Joy = uint.Parse(elems[14], CultureInfo.InvariantCulture),
                                 Sadness = uint.Parse(elems[15], CultureInfo.InvariantCulture),
                                 Surprise = uint.Parse(elems[16], CultureInfo.InvariantCulture),
                                 Neutral = uint.Parse(elems[17], CultureInfo.InvariantCulture),
                                 LookAtX = double.Parse(elems[18], CultureInfo.InvariantCulture),
                                 LookAtY = double.Parse(elems[19], CultureInfo.InvariantCulture),
                                 LookAt = elems[20]
                             };

            this._perceptionFilter.UpdateFilters(perception);
            var filtered = this._perceptionFilter.FilteredPerception;
            filtered.Time = perception.Time;
            return filtered;
        }
    }
}