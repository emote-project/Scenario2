using CaseBasedController.Detection;
using EmoteEvents;
using Newtonsoft.Json;

namespace CaseBasedController.Behavior.Enercities
{
    /// <summary>
    ///     Tells the AI to perform planning (calculate current best actions) according to the specified strategies.
    /// </summary>
    public class UpdateAIStrategies : BaseBehavior
    {
        /// <summary>
        ///     The strategies of all players that are going to be used by the AI module to plan the next action.
        /// </summary>
        [JsonIgnore]
        public StrategiesSet Strategies { get; set; }

        public override void Execute(IFeatureDetector detector)
        {
            //send the strategies to the AI client
            lock (this.locker)
                if (this.Strategies != null)
                    this.actionPublisher.UpdateStrategies(this.Strategies.SerializeToJson());

            //TODO verify wait for something?
            this.RaiseFinishedEvent(detector);
        }

        public override void Cancel()
        {
            

        }
    }
}