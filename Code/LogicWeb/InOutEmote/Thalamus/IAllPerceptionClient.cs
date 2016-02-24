using System;
using EmoteCommonMessages;
using EmoteEnercitiesMessages;
using Thalamus.BML;

namespace InOutEmote.Thalamus
{
    public interface IAllPerceptionClient :
        IEnercitiesExamineEvents,
        IEnercitiesGameStateEvents,
        IEnercitiesTaskEvents,
        IEnercitiesAIActions,
        ISpeechDetectionEvents,
        ITargetEvents,
        IPerceptionEvents,
        ISoundLocalizationEvents,
        ISpeakEvents,
        ISpeakActions,
        IAnimationEvents,
        IFMLSpeech,
        IScenario2Perception,
        IFMLSpeechEvents,
        IMenuEvents,
        IEmoteActions,
        ILearnerModelMemoryEvents,
        IEnercitiesClassificationEvent,
        IEmotionalClimate
    {
        event EventHandler<EndGameEventArgs> EndGameNoOilEvent;
        event EventHandler<EndGameEventArgs> EndGameSuccessfullEvent;
        event EventHandler<EndGameEventArgs> EndGameTimeOutEvent;
        event EventHandler<GenericGameEventArgs> GameStartedEvent;
        event EventHandler<PlayersGenderEventArgs> PlayersGenderEvent;
        event EventHandler<ReachedNewLevelEventArgs> ReachedNewLevelEvent;
        event EventHandler<GenericGameEventArgs> ResumeGameEvent;
        event EventHandler<GenericGameEventArgs> TurnChangedEvent;
        event EventHandler<StrategyGameMovesEventArgs> StrategyGameMovesEvent;
        event EventHandler<ExamineCellEventArgs> ExamineCellEvent;
        event EventHandler<GameActionEventArgs> ImplementPolicyEvent;
        event EventHandler<GameActionEventArgs> ConfirmConstructionEvent;
        event EventHandler<GameActionEventArgs> PerformUpgradeEvent;
        event EventHandler<GameActionEventArgs> SkipTurnEvent;
        event EventHandler<IAEventArgs> BestActionPlannedEvent;
        event EventHandler<IAEventArgs> BestActionsPlannedEvent;
        event EventHandler<IAEventArgs> ActionsPlannedEvent;
        event EventHandler<IAEventArgs> PredictedValuesUpdatedEvent;
        event EventHandler<IAEventArgs> StrategiesUpdatedEvent;
        event EventHandler<WordDetectedEventArgs> WordDetectedEvent;
        event EventHandler<TargetsEventArgs> TargetLinkEvent;
        event EventHandler<TargetsEventArgs> TargetScreenInfoEvent;
        event EventHandler<EyebrowsAUEventArgs> EyebrowsAUEvent;
        event EventHandler<PerceptionEventArgs> GazeTrackingEvent;
        event EventHandler<PerceptionEventArgs> HeadTrackingEvent;
        event EventHandler<PerceptionEventArgs> PointingPositionEvent;
        event EventHandler<PerceptionEventArgs> UserMutualGazeEvent;
        event EventHandler<PerceptionEventArgs> UserMutualPointEvent;
        event EventHandler<PerceptionEventArgs> GazeOKAOEvent;
        event EventHandler<PerceptionEventArgs> UserTouchChinEvent;
        event EventHandler<ActiveSoundUserEventArgs> ActiveSoundUserEvent;
        event EventHandler<SpeechEventArgs> SpeakStartedEvent;
        event EventHandler<SpeechEventArgs> SpeakFinishedEvent;
        event EventHandler<SpeechEventArgs> SpeakEvent;
        event EventHandler<SpeechEventArgs> SpeakBookmarksEvent;
        event EventHandler SpeakStopEvent;
        event EventHandler<AnimationEventArgs> AnimationStartedEvent;
        event EventHandler<AnimationEventArgs> AnimationFinishedEvent;
        event EventHandler<MenuEventArgs> BuildMenuTooltipShowedEvent;
        event EventHandler<MenuEventArgs> BuildMenuTooltipClosedEvent;
        event EventHandler<MenuEventArgs> BuildingMenuToolSelectedEvent;
        event EventHandler<MenuEventArgs> BuildingMenuToolUnselectedEvent;
        event EventHandler PoliciesMenuShowedEvent;
        event EventHandler PoliciesMenuClosedEvent;
        event EventHandler<MenuEventArgs> PolicyTooltipShowedEvent;
        event EventHandler PolicyTooltipClosedEvent;
        event EventHandler UpgradesMenuShowedEvent;
        event EventHandler UpgradesMenuClosedEvent;
        event EventHandler<MenuEventArgs> UpgradeTooltipShowedEvent;
        event EventHandler UpgradeTooltipClosedEvent;
        event EventHandler<PerformUtteranceFromLibraryEventArgs> PerformUtteranceFromLibraryEvent;
        event EventHandler<PerformUtteranceEventArgs> PerformUtteranceEvent;
        event EventHandler<OKAOPerceptionArgs> OKAOPerceptionEvent;
        event EventHandler<LearnerModelMemoryEventArgs> LearnerModelMemoryEvent;
        event EventHandler<IFMLUtteranceEventArgs> UtteranceStartedEvent;
        event EventHandler<IFMLUtteranceEventArgs> UtteranceFinishedEvent;
        event EventHandler EndOfLevelHideEvent;
        event EventHandler EndOfLevelShowEvent;
        event EventHandler<StartEventArgs> StartEvent;
        event EventHandler<ClassificationEventArgs> ClassifierResultEvent;
        event EventHandler<EmotionalClimateChangedEventArgs> EmotionalClimateChangedEvent;

    }
}