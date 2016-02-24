using System;
using System.Threading.Tasks;
using System.Windows;
using CaseBasedController.GameInfo;
using EmoteCommonMessages;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EmoteEvents.ComplexData;

namespace CaseBasedController.Thalamus
{
    public partial class ControllerClient : IAllPerceptionClient
    {
        #region IAllPerceptionClient Members

        public void EndGameNoOil(int totalScore)
        {
            if (EndGameNoOilEvent != null) EndGameNoOilEvent(this, new EndGameEventArgs {TotalScore = totalScore});
        }

        public void EndGameSuccessfull(int totalScore)
        {
            if (EndGameSuccessfullEvent != null)
                EndGameSuccessfullEvent(this, new EndGameEventArgs {TotalScore = totalScore});
        }

        public void EndGameTimeOut(int totalScore)
        {
            if (EndGameTimeOutEvent != null)
                EndGameTimeOutEvent(this, new EndGameEventArgs {TotalScore = totalScore});
        }

        public void GameStarted(string player1Name, string player1Role, string player2Name, string player2Role)
        {
            var eventData = new GenericGameEventArgs
                            {
                                Player1Name = player1Name,
                                Player2Name = player2Name,
                                Player1Role = player1Role,
                                Player2Role = player2Role
                            };
            //console.writeline("Game started event: "+player1Name+","+player2Name);
            GameStatus.perceptionClient_GameStartedEvent(this, eventData);
            if (GameStartedEvent != null) GameStartedEvent(this, eventData);
        }

        public void PlayersGender(Gender player1Gender, Gender player2Gender)
        {
            if (PlayersGenderEvent != null)
                PlayersGenderEvent(this,
                    new PlayersGenderEventArgs {Player1Gender = player1Gender, Player2Gender = player2Gender});
        }

        public void PlayersGenderString(string player1Gender, string player2Gender)
        {
            var p1 = (Gender) Enum.Parse(typeof (Gender), player1Gender);
            var p2 = (Gender) Enum.Parse(typeof (Gender), player2Gender);
            if (PlayersGenderEvent != null)
                PlayersGenderEvent(this, new PlayersGenderEventArgs {Player1Gender = p1, Player2Gender = p2});
        }

        public void ReachedNewLevel(int level)
        {
            GameInfo.GameStatus.client_NewLevelEvent(null,new ReachedNewLevelEventArgs(){Level = level});
            if (ReachedNewLevelEvent != null)
                ReachedNewLevelEvent(this, new ReachedNewLevelEventArgs {Level = level});
        }

        public void ResumeGame(string player1Name, string player1Role, string player2Name, string player2Role,
            string serializedGameState)
        {
            var eventData = new GenericGameEventArgs
                            {
                                Player1Name = player1Name,
                                Player2Name = player2Name,
                                Player1Role = player1Role,
                                Player2Role = player2Role
                            };
            GameStatus.perceptionClient_GameStartedEvent(this, eventData);
            if (ResumeGameEvent != null)
                ResumeGameEvent(this, eventData);
        }

        public void TurnChanged(string serializedGameState)
        {
            var eventArgs = new GenericGameEventArgs {SerializedGameState = serializedGameState};
            GameStatus.client_TurnChangedEvent(this, eventArgs);

            AIGameActionPlayer.GetInstance()._client_TurnChangedEvent(this,eventArgs);

            if (TurnChangedEvent != null)
                TurnChangedEvent(this, eventArgs);
        }

        public void StrategyGameMoves(string environmentalistMove, string economistMove, string mayorMove,
            string globalMove)
        {
            if (StrategyGameMovesEvent != null)
                StrategyGameMovesEvent(this,
                    new StrategyGameMovesEventArgs
                    {
                        EnvironmentalistMove = environmentalistMove,
                        EconomistMove = economistMove,
                        MayorMove = mayorMove,
                        GlobalMove = globalMove
                    });
        }

        public void ExamineCell(double screenX, double screenY, int cellX, int cellY,
            StructureType structureType_structure, string structureType_translated)
        {
            if (ExamineCellEvent != null)
                ExamineCellEvent(this,
                    new ExamineCellEventArgs
                    {
                        ScreenX = screenX,
                        ScreenY = screenY,
                        CellX = cellX,
                        CellY = cellY,
                        Type = structureType_structure,
                        StructureType_translated = structureType_translated
                    });
        }

        public void ImplementPolicy(PolicyType policy, string translation)
        {
            var eventArgs = new GameActionEventArgs {ActionTypeEnum = (int) policy, Translation = translation};
            GameStatus.client_ImplementPolicyEvent(this, eventArgs);
            AIGameActionPlayer.GetInstance().client_ImplementPolicyEvent(this, eventArgs);
            if (ImplementPolicyEvent != null)
                ImplementPolicyEvent(this, eventArgs);
        }

        public void ConfirmConstruction(StructureType structure, string translation, int cellX, int cellY)
        {
            var eventArgs = new GameActionEventArgs
                            {
                                ActionTypeEnum = (int) structure,
                                Translation = translation,
                                CellX = cellX,
                                CellY = cellY
                            };
            GameStatus.client_ConfirmConstructionEvent(this, eventArgs);
            AIGameActionPlayer.GetInstance().client_ConfirmConstructionEvent(this, eventArgs);
            if (ConfirmConstructionEvent != null)
                ConfirmConstructionEvent(this, eventArgs);
        }

        public void PerformUpgrade(UpgradeType upgrade, string translation, int cellX, int cellY)
        {
            var eventArgs = new GameActionEventArgs
                            {
                                ActionTypeEnum = (int) upgrade,
                                Translation = translation,
                                CellX = cellX,
                                CellY = cellY
                            };
            GameStatus.client_PerformUpgradeEvent(this, eventArgs);
            AIGameActionPlayer.GetInstance().client_PerformUpgradeEvent(this, eventArgs);
            if (PerformUpgradeEvent != null)
                PerformUpgradeEvent(this,
                    eventArgs);
        }

        public void SkipTurn()
        {
            if (SkipTurnEvent != null) SkipTurnEvent(this, new GameActionEventArgs());
        }

        public void BestActionPlanned(string[] enercitiesActionInfo_actionInfos)
        {
            if (BestActionPlannedEvent != null)
                BestActionPlannedEvent(this,
                    new IAEventArgs {EnercitiesActionInfo_bestActionInfos = enercitiesActionInfo_actionInfos});
        }

        public void BestActionsPlanned(EnercitiesRole currentPlayer, string[] enercitiesActionInfo_actionInfos)
            //public void BestActionsPlanned(string[] enercitiesActionInfo_actionInfos)
        {
            //var eventArgs = new IAEventArgs
            //                {
            //                    EnercitiesActionInfo_bestActionInfos = enercitiesActionInfo_actionInfos,
            //                    CurrentPlayer = currentPlayer
            //                };
            //GameStatus.perceptionClient_BestActionsPlannedEvent(this, eventArgs);
            //if (BestActionsPlannedEvent != null)
            //    BestActionsPlannedEvent(this, eventArgs);
        }

        public void ActionsPlanned(EnercitiesRole currentPlayer, string Strategy_planStrategy,
            string[] EnercitiesActionInfo_bestActionInfos, string[] EnercitiesActionInfo_worstActionInfos)
        {
            var eventArgs = new IAEventArgs
                            {
                                EnercitiesActionInfo_bestActionInfos = EnercitiesActionInfo_bestActionInfos,
                                EnercitiesActionInfo_worstActionInfos = EnercitiesActionInfo_worstActionInfos,
                                Strategy_planStrategy = Strategy_planStrategy,
                                CurrentPlayer = currentPlayer
                            };
            GameStatus.perceptionClient_BestActionsPlannedEvent(this, eventArgs);
            GameStatus.client_ActionsPlannedEvent(this, eventArgs); //TODO: Check why there are 2 similar events here

            AIGameActionPlayer.GetInstance()._client_ActionsPlannedEvent(this,eventArgs);

            if (ActionsPlannedEvent != null)
                ActionsPlannedEvent(this, eventArgs);
        }

        public void PredictedValuesUpdated(double[] values)
        {
            if (PredictedValuesUpdatedEvent != null)
                PredictedValuesUpdatedEvent(this, new IAEventArgs {Values = values});
        }

        public void StrategiesUpdated(string strategiesSet_strategies)
        {
            if (StrategiesUpdatedEvent != null)
                StrategiesUpdatedEvent(this, new IAEventArgs {StrategiesSet_strategies = strategiesSet_strategies});
        }

        public void WordDetected(string[] words)
        {
            if (WordDetectedEvent != null) WordDetectedEvent(this, new WordDetectedEventArgs {Words = words});
        }

        public void TargetLink(string targetName, string linkedTargetName)
        {
            if (TargetLinkEvent != null)
                TargetLinkEvent(this,
                    new TargetsEventArgs {TargetName = targetName, LinkedTargetName = linkedTargetName});
        }

        public void TargetScreenInfo(string targetName, int x, int y)
        {
            if (TargetScreenInfoEvent != null)
                TargetScreenInfoEvent(this, new TargetsEventArgs {TargetName = targetName, X = x, Y = y});
        }

        public void EyebrowsAU(double au2User1, double au4User1, double au2User2, double au4User2)
        {
            if (EyebrowsAUEvent != null)
                EyebrowsAUEvent(this,
                    new EyebrowsAUEventArgs
                    {
                        Au2User1 = au2User1,
                        Au2User2 = au2User2,
                        Au4User1 = au4User1,
                        Au4User2 = au4User2
                    });
        }

        public void EyebrowsAU2(double au4LeftUser1, double au4RightUser1, double au4LeftUser2, double au4RightUser2)
        {
            //if (EyebrowsAUEvent != null) EyebrowsAUEvent(this, new EyebrowsAUEventArgs() { AU2_user1 = au2_user1, AU2_user2 = au2_user2, AU4_user1 = au4_user1, AU4_user2 = au4_user2 });
            throw new NotImplementedException();
        }

        public void GazeTracking(int userID, GazeEnum direction, int ConfidenceVal)
        {
            if (GazeTrackingEvent != null)
                GazeTrackingEvent(this, new PerceptionEventArgs
                                        {
                                            UserID = userID,
                                            Direction = direction,
                                            ConfidenceVal = ConfidenceVal
                                        });
        }

        public void HeadTracking(int userID, double x, double y, double z, bool detectedSkeleton)
        {
            if (HeadTrackingEvent != null)
                HeadTrackingEvent(this,
                    new PerceptionEventArgs
                    {
                        UserID = userID,
                        X = x,
                        Y = y,
                        Z = z,
                        DetectedSkeleton = detectedSkeleton
                    });
        }

        public void PointingPosition(int userID, Hand hand, double x, double y, double z)
        {
            if (PointingPositionEvent != null)
                PointingPositionEvent(this,
                    new PerceptionEventArgs {UserID = userID, X = x, Y = y, Z = z, Hand = hand});
        }

        public void UserMutualGaze(bool value)
        {
            if (UserMutualGazeEvent != null) UserMutualGazeEvent(this, new PerceptionEventArgs {Value = value});
        }

        public void UserMutualPoint(bool value, double avegX, double avegY)
        {
            if (UserMutualPointEvent != null)
                UserMutualPointEvent(this, new PerceptionEventArgs {Value = value, X = avegX, Y = avegY});
        }

        public void UserTouchChin(int userID, bool value)
        {
            if (UserTouchChinEvent != null)
                UserTouchChinEvent(this, new PerceptionEventArgs {UserID = userID, Value = value});
        }

        public void ActiveSoundUser(ActiveUser userAct, double leftValue, double rightValue)
        {
            if (ActiveSoundUserEvent != null)
                ActiveSoundUserEvent(this,
                    new ActiveSoundUserEventArgs {UserAct = userAct, LeftValue = leftValue, RightValue = rightValue});
        }

        public void SpeakStarted(string id)
        {
            if (SpeakStartedEvent != null) SpeakStartedEvent(this, new SpeechEventArgs {ID = id});
        }

        public void SpeakFinished(string id)
        {
            if (SpeakFinishedEvent != null) SpeakFinishedEvent(this, new SpeechEventArgs {ID = id});
        }

        public void Speak(string id, string text)
        {
            if (SpeakEvent != null) SpeakEvent(this, new SpeechEventArgs {ID = id, Text = text});
        }

        public void SpeakBookmarks(string id, string[] text, string[] bookmarks)
        {
            var eventArgs = new SpeechEventArgs {ID = id, Texts = text, Bookmarks = bookmarks};

            AIGameActionPlayer.GetInstance()._client_SpeakBookmarksEvent(this,eventArgs);

            if (SpeakBookmarksEvent != null)
                SpeakBookmarksEvent(this, new SpeechEventArgs {ID = id, Texts = text, Bookmarks = bookmarks});
        }

        public void SpeakStop()
        {
            if (SpeakStopEvent != null)
                SpeakStopEvent(this, null);
        }

        public void AnimationStarted(string id)
        {
            if (AnimationStartedEvent != null) AnimationStartedEvent(this, new AnimationEventArgs {ID = id});
        }

        public void AnimationFinished(string id)
        {
            if (AnimationFinishedEvent != null) AnimationFinishedEvent(this, new AnimationEventArgs {ID = id});
        }

        public void BuildMenuTooltipShowed(StructureCategory category, string translation)
        {
            if (BuildMenuTooltipShowedEvent != null)
                BuildMenuTooltipShowedEvent(this,
                    new MenuEventArgs {EnumValue = (int) category, Translation = translation});
        }

        public void BuildMenuTooltipClosed(StructureCategory category, string translation)
        {
            if (BuildMenuTooltipClosedEvent != null)
                BuildMenuTooltipClosedEvent(this,
                    new MenuEventArgs {EnumValue = (int) category, Translation = translation});
        }

        public void BuildingMenuToolSelected(StructureType structure, string translation)
        {
            if (BuildingMenuToolSelectedEvent != null)
                BuildingMenuToolSelectedEvent(this,
                    new MenuEventArgs {EnumValue = (int) structure, Translation = translation});
        }

        public void BuildingMenuToolUnselected(StructureType structure, string translation)
        {
            if (BuildingMenuToolUnselectedEvent != null)
                BuildingMenuToolUnselectedEvent(this,
                    new MenuEventArgs {EnumValue = (int) structure, Translation = translation});
        }

        public void PoliciesMenuShowed()
        {
            if (PoliciesMenuShowedEvent != null) PoliciesMenuShowedEvent(this, null);
        }

        public void PoliciesMenuClosed()
        {
            if (PoliciesMenuClosedEvent != null) PoliciesMenuClosedEvent(this, null);
        }

        public void PolicyTooltipShowed(PolicyType policy, string translation)
        {
            if (PolicyTooltipShowedEvent != null)
                PolicyTooltipShowedEvent(this, new MenuEventArgs {EnumValue = (int) policy, Translation = translation});
        }

        public void PolicyTooltipClosed()
        {
            if (PolicyTooltipClosedEvent != null) PolicyTooltipClosedEvent(this, null);
        }

        public void UpgradesMenuShowed()
        {
            if (UpgradesMenuShowedEvent != null) UpgradesMenuShowedEvent(this, null);
        }

        public void UpgradesMenuClosed()
        {
            if (UpgradesMenuClosedEvent != null) UpgradesMenuClosedEvent(this, null);
        }

        public void UpgradeTooltipShowed(UpgradeType upgrade, string translation)
        {
            if (UpgradeTooltipShowedEvent != null)
                UpgradeTooltipShowedEvent(this, new MenuEventArgs {EnumValue = (int) upgrade, Translation = translation});
        }

        public void UpgradeTooltipClosed()
        {
            if (UpgradeTooltipClosedEvent != null) UpgradeTooltipClosedEvent(this, null);
        }

        public void PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames,
            string[] tagValues)
        {
            if (PerformUtteranceFromLibraryEvent != null)
                PerformUtteranceFromLibraryEvent(this,
                    new PerformUtteranceFromLibraryEventArgs
                    {
                        ID = id,
                        Category = category,
                        Subcategory = subcategory,
                        TagNames = tagNames,
                        TagValues = tagValues
                    });
        }

        public void OKAOMessage(int userID, double smile, double confidence, double anger, double disgust, double fear,
            double joy,
            double sadness, double surprise, double neutral, string gazeDirection)
        {
        }

        public void QSensorMessage(int userID, double Z, double Y, double X, double Temp, double EDA)
        {
        }

        public void PerformUtterance(string id, string utterance, string category)
        {
            if (PerformUtteranceEvent != null)
                PerformUtteranceEvent(this,
                    new PerformUtteranceEventArgs {ID = id, Category = category, Utterance = utterance});
        }

        public void CancelUtterance(string id)
        {
            //throw new NotImplementedException();
        }

        public void PerceptionLog(double time, double faceUpDownDegrees, double faceLeftRightDegrees, double eyesUpdown,
            double eyesLeftRight, double headPositionY, double headPositionX, double closeRatioLeftEye,
            double closeRatioRightEye, double smile, double confidence, double anger, double disgust, double fear,
            double joy, double sadness, double surprise, double neutral, double gazeVectorX, double gazeVectorY,
            string gazeDirection, string subject)
        {
            if (this.OKAOPerceptionEvent != null)
                this.OKAOPerceptionEvent(this,
                    new OKAOPerceptionArgs
                    {
                        PerceptionLog = new OKAOScenario2Perception(
                            time, faceUpDownDegrees, faceLeftRightDegrees, eyesUpdown,
                            eyesLeftRight, headPositionY, headPositionX, closeRatioLeftEye,
                            closeRatioRightEye, smile, confidence, anger, disgust, fear, joy,
                            sadness, surprise, neutral, gazeVectorX, gazeVectorY, gazeDirection, subject)
                    });
        }

        #endregion

        public void GazeOKAO(int userID, bool gazeAtRobot)
        {
            if (GazeOKAOEvent != null)
                GazeOKAOEvent(this, new PerceptionEventArgs {UserID = userID, Value = gazeAtRobot});
        }

        public void learnerModelMemoryEvent(string MemoryEvent_memoryEvent)
        {
            LearnerModelMemoryEventArgs eventArgs = new LearnerModelMemoryEventArgs();
            eventArgs.MemoryEvent = MemoryEvent_memoryEvent != "" ? MemoryEvent.DeserializeFromJson(MemoryEvent_memoryEvent) : new MemoryEvent();
            GameStatus.client_learnerModelMemoryEvent(eventArgs.MemoryEvent); 
            if (LearnerModelMemoryEvent!=null)
                LearnerModelMemoryEvent(this, eventArgs);
        }

        public void UtteranceStarted(string id)
        {
            if (UtteranceStartedEvent != null)
                UtteranceStartedEvent(this, new IFMLUtteranceEventArgs { Id = id });
        }

        public void UtteranceFinished(string id)
        {
            UtteranceFinishedDelayed(id);
        }
        private async void UtteranceFinishedDelayed(string id)
        {
            await Task.Delay(500);
            if (UtteranceFinishedEvent != null)
                UtteranceFinishedEvent(this, new IFMLUtteranceEventArgs { Id = id });
            
        }

        public void EndOfLevelShow()
        {
            if (EndOfLevelShowEvent != null) EndOfLevelShowEvent(this, null);
        }

        public void EndOfLevelHide()
        {
            if (EndOfLevelHideEvent != null) EndOfLevelHideEvent(this, null);

        }

        public void StartGameShow()
        {
        }

        public void StartGameHide()
        {
        }

        public void EndOfGameShow()
        {
        }

        public void EndOfGameHide()
        {
        }

        public void Start(string StartMessageInfo_info)
        {
            var startMessageInfo = JsonSerializable.DeserializeFromJson<StartMessageInfo>(StartMessageInfo_info);
            var startEventArgs = new StartEventArgs() {StartMessageInfo = startMessageInfo};
            GameInfo.GameStatus.client_start(startEventArgs);
            if (StartEvent != null) 
                StartEvent(this, startEventArgs);
        }

        public void Stop()
        {
        }

        public void Reset()
        {
        }

        public void SetLearnerInfo(string LearnerInfo_learnerInfo)
        {
        }

        #region Events

        public event EventHandler<EndGameEventArgs> EndGameNoOilEvent;
        public event EventHandler<EndGameEventArgs> EndGameSuccessfullEvent;
        public event EventHandler<EndGameEventArgs> EndGameTimeOutEvent;
        public event EventHandler<GenericGameEventArgs> GameStartedEvent;
        public event EventHandler<PlayersGenderEventArgs> PlayersGenderEvent;
        public event EventHandler<ReachedNewLevelEventArgs> ReachedNewLevelEvent;
        public event EventHandler<GenericGameEventArgs> ResumeGameEvent;
        public event EventHandler<GenericGameEventArgs> TurnChangedEvent;
        public event EventHandler<StrategyGameMovesEventArgs> StrategyGameMovesEvent;
        public event EventHandler<ExamineCellEventArgs> ExamineCellEvent;
        public event EventHandler<GameActionEventArgs> ImplementPolicyEvent;
        public event EventHandler<GameActionEventArgs> ConfirmConstructionEvent;
        public event EventHandler<GameActionEventArgs> PerformUpgradeEvent;
        public event EventHandler<GameActionEventArgs> SkipTurnEvent;
        public event EventHandler<IAEventArgs> BestActionPlannedEvent;
        public event EventHandler<IAEventArgs> BestActionsPlannedEvent;
        public event EventHandler<IAEventArgs> ActionsPlannedEvent;
        public event EventHandler<IAEventArgs> PredictedValuesUpdatedEvent;
        public event EventHandler<IAEventArgs> StrategiesUpdatedEvent;
        public event EventHandler<WordDetectedEventArgs> WordDetectedEvent;
        public event EventHandler<TargetsEventArgs> TargetLinkEvent;
        public event EventHandler<TargetsEventArgs> TargetScreenInfoEvent;
        public event EventHandler<EyebrowsAUEventArgs> EyebrowsAUEvent;
        public event EventHandler<PerceptionEventArgs> GazeTrackingEvent;
        public event EventHandler<PerceptionEventArgs> HeadTrackingEvent;
        public event EventHandler<PerceptionEventArgs> PointingPositionEvent;
        public event EventHandler<PerceptionEventArgs> UserMutualGazeEvent;
        public event EventHandler<PerceptionEventArgs> UserMutualPointEvent;
        public event EventHandler<PerceptionEventArgs> GazeOKAOEvent;
        public event EventHandler<PerceptionEventArgs> UserTouchChinEvent;
        public event EventHandler<ActiveSoundUserEventArgs> ActiveSoundUserEvent;
        public event EventHandler<SpeechEventArgs> SpeakStartedEvent;
        public event EventHandler<SpeechEventArgs> SpeakFinishedEvent;
        public event EventHandler<SpeechEventArgs> SpeakEvent;
        public event EventHandler<SpeechEventArgs> SpeakBookmarksEvent;
        public event EventHandler SpeakStopEvent;
        public event EventHandler<AnimationEventArgs> AnimationStartedEvent;
        public event EventHandler<AnimationEventArgs> AnimationFinishedEvent;
        public event EventHandler<MenuEventArgs> BuildMenuTooltipShowedEvent;
        public event EventHandler<MenuEventArgs> BuildMenuTooltipClosedEvent;
        public event EventHandler<MenuEventArgs> BuildingMenuToolSelectedEvent;
        public event EventHandler<MenuEventArgs> BuildingMenuToolUnselectedEvent;
        public event EventHandler PoliciesMenuShowedEvent;
        public event EventHandler PoliciesMenuClosedEvent;
        public event EventHandler<MenuEventArgs> PolicyTooltipShowedEvent;
        public event EventHandler PolicyTooltipClosedEvent;
        public event EventHandler UpgradesMenuShowedEvent;
        public event EventHandler UpgradesMenuClosedEvent;
        public event EventHandler<MenuEventArgs> UpgradeTooltipShowedEvent;
        public event EventHandler UpgradeTooltipClosedEvent;
        public event EventHandler<PerformUtteranceFromLibraryEventArgs> PerformUtteranceFromLibraryEvent;
        public event EventHandler<PerformUtteranceEventArgs> PerformUtteranceEvent;
        public event EventHandler<OKAOPerceptionArgs> OKAOPerceptionEvent;
        public event EventHandler<LearnerModelMemoryEventArgs> LearnerModelMemoryEvent;
        public event EventHandler<IFMLUtteranceEventArgs> UtteranceStartedEvent;
        public event EventHandler<IFMLUtteranceEventArgs> UtteranceFinishedEvent;
        public event EventHandler EndOfLevelHideEvent;
        public event EventHandler EndOfLevelShowEvent;
        public event EventHandler<StartEventArgs> StartEvent;

        #endregion


    }
}