using CaseBasedController.GameInfo;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    public enum ResourceType
    {
        Money,
        Oil,
        Power,
        Population
    }

    public class ResourceAboveAverageDetector : BinaryFeatureDetector
    {
        /// <summary>
        ///     The type of resource this detector refers to.
        /// </summary>
        public ResourceType ResourceType { get; set; }

        public override bool IsActive
        {
            get
            {
                lock (this.locker)
                    return
                        (GameStatus.CurrentState != null &&
                         ((this.ResourceType.Equals(ResourceType.Money) &&
                           GameStatus.CurrentState.NormalizedMoney > GameStatus.CurrentState.NormalizedResourcesAverage) ||
                          (this.ResourceType.Equals(ResourceType.Oil) &&
                           GameStatus.CurrentState.NormalizedOil > GameStatus.CurrentState.NormalizedResourcesAverage) ||
                           (this.ResourceType.Equals(ResourceType.Power) &&
                           GameStatus.CurrentState.NormalizedPower > GameStatus.CurrentState.NormalizedResourcesAverage) ||
                          (this.ResourceType.Equals(ResourceType.Population) &&
                           GameStatus.CurrentState.NormalizedPopulation >
                           GameStatus.CurrentState.NormalizedResourcesAverage)));
            }
        }

        public override void Dispose()
        {
            //detach event
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.TurnChangedEvent -= this.OnTurnChangedEvent;
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.TurnChangedEvent += this.OnTurnChangedEvent;
        }

        private void OnTurnChangedEvent(object sender, GenericGameEventArgs genericGameEventArgs)
        {
            //checks activation based on values in this turn, raise event if necessary
            lock (this.locker)
                this.CheckActivationChanged();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", base.ToString(), this.ResourceType);
        }
    }
}