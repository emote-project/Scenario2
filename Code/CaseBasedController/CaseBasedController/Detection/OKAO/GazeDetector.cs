using CaseBasedController.Thalamus;
using MathNet.Numerics.LinearAlgebra;
using PS.Utilities.Math;

namespace CaseBasedController.Detection.OKAO
{
    public enum GazeDirection
    {
        Other,
        Robot,
        Table
    }

    public class GazeDetector : OKAOFeatureDetector
    {
        private const uint MAX_VALUE = 100;
        private const double LOOK_DOWN_THRESHOLD = -15d/MAX_VALUE;
        private const double LOOK_ROBOT_VERT_THRESHOLD = 15d/MAX_VALUE;
        private const double LOOK_ROBOT_HORIZ_THRESHOLD = 20d/MAX_VALUE;
        private const double LOOK_OTHER_BOTTOM_THRESHOLD = 15d/MAX_VALUE;
        private const double LOOK_OTHER_TOP_THRESHOLD = 40d/MAX_VALUE;
        private const double LOOK_OTHER_HORIZ_THRESHOLD = 35d/MAX_VALUE;
        private readonly KalmanFilter<double> _filterX = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0));
        private readonly KalmanFilter<double> _filterY = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0));
        private bool _isGazing;

        /// <summary>
        ///     The gaze direction we want to detect.
        /// </summary>
        public GazeDirection Direction { get; set; }

        /// <summary>
        ///     This detector becomes active when the gaze direction is pointing towards the specified object.
        /// </summary>
        public override bool IsActive
        {
            get { lock (this.locker) return this.isSubject && this._isGazing; }
        }

        protected override void OnOKAOPerceptionEvent(object sender, OKAOPerceptionArgs okaoPerceptionArgs)
        {
            base.OnOKAOPerceptionEvent(sender, okaoPerceptionArgs);

            lock (this.locker)
            {
                //updates the filter with normalized gaze values
                var perception = okaoPerceptionArgs.PerceptionLog;
                this._filterX.Update(GetMatrix(perception.gazeVectorX/MAX_VALUE));
                this._filterY.Update(GetMatrix(perception.gazeVectorY/MAX_VALUE));
                var gazeX = GetValue((Matrix<double>) this._filterX.State);
                var gazeY = GetValue((Matrix<double>) this._filterY.State);

                if (this.Direction.Equals(GazeDirection.Table))
                {
                    //if looking down, looking at the table
                    this._isGazing = gazeY <= LOOK_DOWN_THRESHOLD;
                    return;
                }
                if (this.Direction.Equals(GazeDirection.Robot))
                {
                    //if looking at center, looking at robot
                    this._isGazing = (gazeY > -LOOK_ROBOT_VERT_THRESHOLD) && (gazeY < LOOK_ROBOT_VERT_THRESHOLD) &&
                                     (gazeX < LOOK_ROBOT_HORIZ_THRESHOLD) && (gazeX > -LOOK_ROBOT_HORIZ_THRESHOLD);
                    return;
                }

                //if detecting look other, verifies if outside of center-top frame
                if ((gazeY < LOOK_OTHER_BOTTOM_THRESHOLD) || (gazeY > LOOK_OTHER_TOP_THRESHOLD) ||
                    (gazeX < -LOOK_OTHER_HORIZ_THRESHOLD) || (gazeX > LOOK_OTHER_HORIZ_THRESHOLD))
                {
                    this._isGazing = false;
                    return;
                }

                //left subject must be looking right
                this._isGazing = gazeX > 0 ? this.Subject.Equals(Subject.Left) : this.Subject.Equals(Subject.Right);
            }
        }
    }
}