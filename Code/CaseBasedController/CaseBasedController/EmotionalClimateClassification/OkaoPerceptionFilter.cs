using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Utilities.Math;

namespace EmotionalClimateClassification
{
    public class OkaoPerceptionFilter
    {
        private const double DEFAULT_Q = 0.1;
        private const int DEFAULT_R = 10;
        private uint _lastSmileConf;

        private readonly KalmanFilter<double> _filterAnger = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                             {
                                                                 Q = GetMatrix(DEFAULT_Q),
                                                                 R = GetMatrix(DEFAULT_R)
                                                             };

        private readonly KalmanFilter<double> _filterDisgust = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                               {
                                                                   Q = GetMatrix(DEFAULT_Q),
                                                                   R = GetMatrix(DEFAULT_R)
                                                               };

        private readonly KalmanFilter<double> _filterFear = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                            {
                                                                Q = GetMatrix(DEFAULT_Q),
                                                                R = GetMatrix(DEFAULT_R)
                                                            };

        private readonly KalmanFilter<double> _filterJoy = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                           {
                                                               Q = GetMatrix(DEFAULT_Q),
                                                               R = GetMatrix(DEFAULT_R)
                                                           };

        private readonly KalmanFilter<double> _filterLookAtX = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                               {
                                                                   Q = GetMatrix(DEFAULT_Q),
                                                                   R = GetMatrix(DEFAULT_R)
                                                               };

        private readonly KalmanFilter<double> _filterLookAtY = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                               {
                                                                   Q = GetMatrix(DEFAULT_Q),
                                                                   R = GetMatrix(DEFAULT_R)
                                                               };

        private readonly KalmanFilter<double> _filterNeutral = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                               {
                                                                   Q = GetMatrix(DEFAULT_Q),
                                                                   R = GetMatrix(DEFAULT_R)
                                                               };

        private readonly KalmanFilter<double> _filterSadness = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                               {
                                                                   Q = GetMatrix(DEFAULT_Q),
                                                                   R = GetMatrix(DEFAULT_R)
                                                               };

        private readonly KalmanFilter<double> _filterSmile = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                             {
                                                                 Q = GetMatrix(DEFAULT_Q),
                                                                 R = GetMatrix(DEFAULT_R)
                                                             };

        private readonly KalmanFilter<double> _filterSurprise = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0))
                                                                {
                                                                    Q = GetMatrix(DEFAULT_Q),
                                                                    R = GetMatrix(DEFAULT_R)
                                                                };

        public OkaoPerception FilteredPerception
        {
            get
            {
                var lookAtX = GetValue(this._filterLookAtX.State);
                var lookAtY = GetValue(this._filterLookAtY.State);
                var filteredPerception = new OkaoPerception
                                         {
                                             Anger = (uint) GetValue(this._filterAnger.State),
                                             Disgust = (uint) GetValue(this._filterDisgust.State),
                                             Fear = (uint) GetValue(this._filterFear.State),
                                             Joy = (uint) GetValue(this._filterJoy.State),
                                             Sadness = (uint) GetValue(this._filterSadness.State),
                                             Surprise = (uint) GetValue(this._filterSurprise.State),
                                             Neutral = (uint) GetValue(this._filterNeutral.State),
                                             Smile = (uint) GetValue(this._filterSmile.State),
                                             SmileConfidence = this._lastSmileConf,
                                             LookAtX = lookAtX,
                                             LookAtY = lookAtY,
                                             LookAt = GetLookAt(lookAtX, lookAtY)
                                         };
                return filteredPerception;
            }
        }

        private static string GetLookAt(double lookAtX, double lookAtY)
        {
            return lookAtY < 15
                ? (lookAtX > 0 ? "ScreenR" : "ScreenL")
                : ((lookAtY < 30) && (lookAtX < 20) && (lookAtX > -20) ? "Robot" : "Else");
        }

        public void UpdateFilters(OkaoPerception perception)
        {
            //update facial expression filters
            this._filterAnger.Update(GetMatrix(perception.Anger));
            this._filterDisgust.Update(GetMatrix(perception.Disgust));
            this._filterFear.Update(GetMatrix(perception.Fear));
            this._filterJoy.Update(GetMatrix(perception.Joy));
            this._filterSadness.Update(GetMatrix(perception.Sadness));
            this._filterSurprise.Update(GetMatrix(perception.Surprise));
            this._filterNeutral.Update(GetMatrix(perception.Neutral));

            ////for smile, confidence is used for the covariance
            //var smileConf = 1000 - (perception.SmileConfidence);
            //var smileR = perception.Smile*smileConf*smileConf;
            //this._filterSmile.R = GetMatrix(smileR);
            this._filterSmile.Update(GetMatrix(perception.Smile));
            this._lastSmileConf = perception.SmileConfidence;

            //updates gaze
            this._filterLookAtX.Update(GetMatrix(perception.LookAtX));
            this._filterLookAtY.Update(GetMatrix(perception.LookAtY));
        }

        private static DiagonalMatrix GetMatrix(double value)
        {
            return new DiagonalMatrix(1, 1, value);
        }

        private static double GetValue(Matrix<double> parameter)
        {
            return parameter[0, 0];
        }
    }
}