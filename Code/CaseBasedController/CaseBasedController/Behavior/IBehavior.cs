using System;
using CaseBasedController.Detection;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Behavior
{

    #region Delegates

    public delegate void ExecutedEventHandler(IBehavior behavior, IFeatureDetector detector);

    #endregion

    /// <summary>
    ///     An action that is associated with a certain <see cref="CompositeBehavior" />.
    ///     Represents a specific response to the activation of some <see cref="Case" />.
    /// </summary>
    public interface IBehavior : IDisposable
    {
        /// <summary>
        ///     The priority level associated with this <see cref="IBehavior" />. Used to manage concurrent behavior execution.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        ///     Occurs when this <see cref="IBehavior" />'s execution finishes.
        /// </summary>
        event ExecutedEventHandler ExecutionFinished;

        /// <summary>
        ///     Executes the response encoded by this <see cref="IBehavior" />.
        /// </summary>
        /// <param name="detector">
        ///     the <see cref="IFeatureDetector" /> object that contains information about the event that activated the execution
        ///     of this <see cref="IBehavior" />.
        /// </param>
        void Execute(IFeatureDetector detector);

        /// <summary>
        ///     Cancels the (possibly) ongoing execution of this <see cref="IBehavior" />.
        /// </summary>
        void Cancel();

        /// <summary>
        ///     Initiates this <see cref="IBehavior" />.
        /// </summary>
        /// <param name="actionPublisher"> The publisher responsible for this <see cref="BaseBehavior" /> execution.</param>
        /// <param name="perceptionClient"> The perception client from whoom to attach events.</param>
        void Init(IAllActionPublisher actionPublisher, IAllPerceptionClient perceptionClient);
    }
}