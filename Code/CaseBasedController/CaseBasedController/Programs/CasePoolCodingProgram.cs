using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CaseBasedController.Behavior;
using CaseBasedController.Behavior.Enercities;
using CaseBasedController.Behavior.Enercities.InteractiveTutorial;
using CaseBasedController.Detection;
using CaseBasedController.Detection.Composition;
using CaseBasedController.Detection.Enercities;
using CaseBasedController.Detection.Enercities.FirstActions;
using CaseBasedController.Detection.OKAO;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;
using PS.Utilities;

namespace CaseBasedController.Programs
{
    static public class CasePoolCodingProgram
    {
        private static void Main(string[] args)
        {

            var casePool = TestPool();
            string path = @"../../../Tests/Test.json";

            //serialize
            casePool.SerializeToJson(path);

            //test deserialization
            casePool = CasePool.DeserializeFromJson(path);

            Console.WriteLine("Case pool creation finished, press any key...");
            //Console.ReadKey();
        }

        #region EnercitiesDemo

        static public CasePool EnercitiesDemo()
        {
            var casePool = new CasePool();

            #region  --------------------------------- BASE DETECTORS
            IFeatureDetector gameStartBinaryFeatureDetector = new GameStartedBinaryFeatureDetector();
            IFeatureDetector turnChangedInstantFeatureDetector = new TurnChangedInstantFeatureDetector();
            IFeatureDetector newLevelInstantFeatureDetector = new NewLevelInstantFeatureDetector();

            IFeatureDetector level1BaseFeatureDetector = new LevelBaseFeatureDetector(1);

            IFeatureDetector aiTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Mayor, Description = "AI turn detector" };
            IFeatureDetector envTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Environmentalist, Description = "Environmentalist turn detector" };
            IFeatureDetector ecoTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Economist, Description = "Economist turn detector" };

            IFeatureDetector aiPlannedAction = new AIActionPlannedDetector();

            IFeatureDetector gameActionDoneInstantFeatureDetector = new GameActionInstantFeatureDetector() { Description = "Tells when something have been built/uptadet/implemented" };
            IFeatureDetector didPlayerInteractBinaryFeatureDetector = new PlayerInteractionBinaryFeatureDetector() { Description = "Tells when any action was done on the game" };

            IFeatureDetector isRobotSpeakingBinaryFeatureDetector = new RobotSpeakingBinaryFeatureDetector();
            IFeatureDetector isHumanSpeakingBinaryFeatureDetector = new HumanPlayerSpeakingBinaryFeatureDetector();

            IFeatureDetector gameEndedNoOilBinaryFeatureDetector = new GameEndedBinaryDetector()
            {
                GameEndType = GameEndedBinaryDetector.EndingType.NoOil,
                Description = "Detects when the game ends because of No Oil"
            };

            IFeatureDetector gameEndedWinBinaryFeatureDetector = new GameEndedBinaryDetector()
            {
                GameEndType = GameEndedBinaryDetector.EndingType.Win,
                Description = "Detects when the game ends successfully"
            };

            IFeatureDetector gameEndedTimeUpBinaryFeatureDetector = new GameEndedBinaryDetector()
            {
                GameEndType = GameEndedBinaryDetector.EndingType.TimeUp,
                Description = "Detects when the game ends because the time is up"
            };

            IFeatureDetector gameEndedAnyReasonFeatureDetector = new GameEndedBinaryDetector()
            {
                GameEndType = GameEndedBinaryDetector.EndingType.Any,
                Description = "Detects when the game ends"
            };

            IFeatureDetector learnerModelMemoryEventInstantFeatureDetector = new LearnerModelEventInstantFeatureDetector()
            {
                Description = "Detects when the learner model detects an important event",
            };

            IFeatureDetector newLevelMenuShowedBinaryFeatureDetector = new NewLevelMenuShowedBinaryFeatureDetector();

            #endregion

            #region --------------------------------- COMPOSITE DETECTORS

            IFeatureDetector isHumanTurnBinaryFeatureDetector = new OrFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    envTurnBinaryFeatureDetector,
                    ecoTurnBinaryFeatureDetector
                }
            };

            IFeatureDetector aiPlannedActionDuringMayrTurnDetector = new AndFeatureDetector()
            {
                Description = "AI planned action in Mayor turn",
                Detectors = new List<IFeatureDetector>()
                {
                    aiTurnBinaryFeatureDetector,
                    aiPlannedAction,
                    new NotFeatureDetector() { WatchedDetector = newLevelMenuShowedBinaryFeatureDetector}
                }
            };

            IFeatureDetector aiPlayedGameActionDetector = new AndFeatureDetector()
            {
                Description = "AI playing a game action",
                Detectors =
                {
                    aiTurnBinaryFeatureDetector,
                    gameActionDoneInstantFeatureDetector
                }
            };

            IFeatureDetector aiPlayedGameActionStayAliveDetector = new StayActiveDetector()
            {
                WatchedDetector = aiPlayedGameActionDetector,
                Delay = 5,
            };

            IFeatureDetector turnChangedToHumanPlayersDetector = new AndFeatureDetector()
            {
                Description = "Turn changed to human players",
                Detectors = new List<IFeatureDetector>()
                {
                    new NotFeatureDetector() { WatchedDetector = aiTurnBinaryFeatureDetector},
                    new StayActiveDetector() { 
                        WatchedDetector =  new TurnChangedInstantFeatureDetector(),
                        Delay = 2000 
                    }
                }
            };

            IFeatureDetector turnChangedToAiPlayerDetector = new AndFeatureDetector()
            {
                Description = "Turn changed to AI player",
                Detectors = new List<IFeatureDetector>()
                {
                    aiTurnBinaryFeatureDetector,
                    new TurnChangedInstantFeatureDetector()
                }
            };

            var newLevelStayActiveDetector = new StayActiveDetector()
            {
                WatchedDetector = newLevelInstantFeatureDetector,
                Delay = 10000,
                Description = "Detects when a new level is reached and stay active for some time to be sure that the new level is announced"
            };

            var aiTakingTooLongDuringAiTurn = new AndFeatureDetector
            {
                Detectors = new List<IFeatureDetector>() 
                { 
                    aiTurnBinaryFeatureDetector,
                    new DelayFromTurnStartDetector() { Delay = 9000 },                                      // AFTER x SECONDS FROM THE START OF THE TURN
                    new NotFeatureDetector(){ WatchedDetector = level1BaseFeatureDetector},                 // NOT DURING THE FIRST LEVEL
                    new NotFeatureDetector() { WatchedDetector = aiPlayedGameActionDetector},                // NOT AFTER THE AI ALREADY DID AN ACTION
                    new NotFeatureDetector() { WatchedDetector = gameEndedAnyReasonFeatureDetector}
                }
            };

            var isHumanIdleBinaryFeatureDetector = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    new DelayFromTurnStartDetector() { Delay = 10000 },  
                    new NotFeatureDetector() { WatchedDetector = didPlayerInteractBinaryFeatureDetector},
                    isHumanTurnBinaryFeatureDetector,
                    new NotFeatureDetector() { WatchedDetector = gameEndedAnyReasonFeatureDetector},
                },
                Description = "isHumanIdleBinaryFeatureDetector",
                ActivationsMinDelayMilliseconds = 60000,
            };

            var afterGameEndedBinaryDetector1 = new HasFiredFeatureDetector()
            {
                WatchedDetector = gameEndedAnyReasonFeatureDetector,
                Description = "Detects when the end-game utterance is been said"
            };

            // ############### CLASSIFIER SHOULD ACT
            var classifierShouldAct = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    new DelayFromTurnStartDetector(){ Delay = 3000 },
                    new NotFeatureDetector() { WatchedDetector = new StayActiveDetector() { WatchedDetector = new RobotSpeakingBinaryFeatureDetector(), Delay = 5000 }},
                    new NotFeatureDetector() { WatchedDetector = gameEndedAnyReasonFeatureDetector},
                },
                Description = "classifierShouldAct"
            };


            // ############### CLASSIFIER SHOULD NOT ACT
            var classifierShouldNotAct = new NotFeatureDetector
            {
                WatchedDetector = classifierShouldAct,
                Description = "classifierShouldNOTAct",
            };

            #endregion






            #region --------------------------------- BEHAVIOURS
            // ############### ANOUNCE AI TURN
            var bUttTurnchangedSelf = new PerformUtterance() { Category = "turnchanged", Subcategory = "self" };
            // ############### ANOUNCE HUMAN PLAYER TURN
            var bUttTurnchangedOther = new PerformUtterance() { Category = "turnchanged", Subcategory = "other" };
            // ############### SAY TUTORIAL
            var b_utt_tutorial_and_first_action = new PerformTutorial();
            // ############### MAKE GAME MOVE 
            var bMakeRecomGameMove = new ExecuteAIRecomGameAction();
            // ############### COMMENT AI TAKING TOO LONG
            var bCommentAiTakingTooLong = new PerformUtterance() { Category = "feedback", Subcategory = "thinking" };
            // ############### COMMENT NEW LEVEL
            var bCommentNewLevel = new PerformUtterance() { Category = "reachednewlevel", Subcategory = "-", Priority = 5 };
            // ############### TALK ABOUT RULES
            var bTalkAboutRules = new PerformUtterance() { Category = "Gamerules", Subcategory = "General" };
            // ############### GAME ENDED
            var bGameEndedNoOil = new PerformUtterance() { Category = "endgame", Subcategory = "nooil", Priority = 10 };
            var bGameEndedWin = new PerformUtterance() { Category = "endgame", Subcategory = "successful", Priority = 10 };
            var bGameEndedTimeUp = new PerformUtterance() { Category = "gamestate", Subcategory = "timeup", Priority = 10 };
            // ############### WRAPUP
            var bWrapupAndGoodbye = new PerformWrapupAndGoodbye();

            // ############### ENABLE / DISABLE CLASSIFIER
            var bEnableClassifier = new ClassifierControlBehavior { UseClassifier = true };
            var bDisableClassifier = new ClassifierControlBehavior { UseClassifier = false };


            // ############### LEARNER MODEL
            var bLearnerModelMemoryEvent = new PerformUtteranceForLatestMemoryEvent();

            #endregion

            //var bFakeTutorial = new PerformCustomUtterance()
            //{
            //    Utterance =
            //        "I should say the tutorial now, but since i'm sick of repeating the same phrase over and over, I'm just sayng a shorter one for testing this behaviour."
            //};

            //casePool.Add(new Case(gameStartBinaryFeatureDetector, b_utt_tutorial_and_first_action) { Description = "Tell tutorial" });
            //// #################################### GAME MECANICS
            //casePool.Add(new Case(turnChangedToAiPlayerDetector, bUttTurnchangedSelf) { Description = "Say when turn changes to self" });
            //casePool.Add(new Case(turnChangedToHumanPlayersDetector, bUttTurnchangedOther) { Description = "Say when turn changes human players" });
            //casePool.Add(new Case(aiPlannedActionDuringMayrTurnDetector, bMakeRecomGameMove, false) { Description = "Play recomended game move", });
            //casePool.Add(new Case(newLevelStayActiveDetector, bCommentNewLevel) { Description = "Say when a new level is reached" });

            //casePool.Add(new Case(gameEndedNoOilBinaryFeatureDetector, bGameEndedNoOil) { Description = "Say when a the game ended because of no oil" });
            //casePool.Add(new Case(gameEndedWinBinaryFeatureDetector, bGameEndedWin) { Description = "Say when a the game ended successfully" });
            //casePool.Add(new Case(gameEndedTimeUpBinaryFeatureDetector, bGameEndedTimeUp) { Description = "Say when a the game ended because the time is up" });

            //casePool.Add(new Case(afterGameEndedBinaryDetector1, bWrapupAndGoodbye) { Description = "Tell a wrapup of the game session and goodbye" });
            //// ####################################################

            //casePool.Add(new Case(aiTakingTooLongDuringAiTurn, bCommentAiTakingTooLong) { Description = "Comment AI taking too long" });
            //casePool.Add(new Case(isHumanIdleBinaryFeatureDetector, bTalkAboutRules) { Description = "Helps the human player in the early game if he doesn't act" });

            //casePool.Add(new Case(classifierShouldAct, bEnableClassifier) { Description = "Classifier Enabled" });
            //casePool.Add(new Case(classifierShouldNotAct, bDisableClassifier) { Description = "Classifier disabled" });

            //casePool.Add(new Case(learnerModelMemoryEventInstantFeatureDetector, bLearnerModelMemoryEvent) { Description = "Learner model events" });


            return casePool;
        }

        static public CasePool EnercitiesDemoEmpathic()
        {
            var casePool = new CasePool();

            #region  --------------------------------- BASE DETECTORS
            IFeatureDetector gameStartBinaryFeatureDetector = new GameStartedBinaryFeatureDetector();
            IFeatureDetector turnChangedInstantFeatureDetector = new TurnChangedInstantFeatureDetector();
            IFeatureDetector reachedNewLevelBinaryFeatureDetector = new NewLevelReachedBinaryFeatureDetector();

            IFeatureDetector level1BaseFeatureDetector = new LevelBaseFeatureDetector(1);

            IFeatureDetector aiTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Mayor, Description = "AI turn detector" };
            IFeatureDetector envTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Environmentalist, Description = "Environmentalist turn detector" };
            IFeatureDetector ecoTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Economist, Description = "Economist turn detector" };

            IFeatureDetector aiPlannedAction = new AIActionPlannedDetector();

            IFeatureDetector gameActionDoneInstantFeatureDetector = new GameActionInstantFeatureDetector() { Description = "Tells when something have been built/uptadet/implemented" };
            IFeatureDetector didPlayerInteractBinaryFeatureDetector = new PlayerInteractionBinaryFeatureDetector() { Description = "Tells when any action was done on the game" };

            IFeatureDetector isRobotSpeakingBinaryFeatureDetector = new RobotSpeakingBinaryFeatureDetector();
            IFeatureDetector isHumanSpeakingBinaryFeatureDetector = new HumanPlayerSpeakingBinaryFeatureDetector();

            IFeatureDetector gameEndedNoOilBinaryFeatureDetector = new GameEndedBinaryDetector()
            {
                GameEndType = GameEndedBinaryDetector.EndingType.NoOil,
                Description = "Detects when the game ends because of No Oil"
            };

            IFeatureDetector gameEndedWinBinaryFeatureDetector = new GameEndedBinaryDetector()
            {
                GameEndType = GameEndedBinaryDetector.EndingType.Win,
                Description = "Detects when the game ends successfully"
            };

            IFeatureDetector gameEndedTimeUpBinaryFeatureDetector = new GameEndedBinaryDetector()
            {
                GameEndType = GameEndedBinaryDetector.EndingType.TimeUp,
                Description = "Detects when the game ends because the time is up"
            };

            IFeatureDetector gameEndedAnyReasonFeatureDetector = new GameEndedBinaryDetector()
            {
                GameEndType = GameEndedBinaryDetector.EndingType.Any,
                Description = "Detects when the game ends"
            };

            IFeatureDetector learnerModelMemoryEventInstantFeatureDetector = new LearnerModelEventInstantFeatureDetector()
            {
                Description = "Detects when the learner model detects an important event",
            };

            IFeatureDetector isTurn0BinaryFeatureDetector = new TurnNumberBinaryFeatureDetector(0);
            IFeatureDetector isTurn1BinaryFeatureDetector = new TurnNumberBinaryFeatureDetector(1);
            IFeatureDetector isTurn2BinaryFeatureDetector = new TurnNumberBinaryFeatureDetector(2);

            IFeatureDetector newLevelMenuShowedBinaryFeatureDetector = new NewLevelMenuShowedBinaryFeatureDetector();

            IFeatureDetector firstUpgradeDoneBinaryFeatureDetector = new AfterFirstUpgradeDoneByMayorBinaryFeatureDetector();
            IFeatureDetector firstPolicyAppliedBinaryFeatureDetector = new AfterFirstPolicyAppliedByMaiorBinaryFeatureDetector();
            IFeatureDetector firstSkipDoneBinaryFeatureDetector = new AfterFirstSkipDoneByMayorBinaryFeatureDetector();

            IFeatureDetector isFirstSessionBinaryFeatureDetector = new SessionBinaryFeatureDetector(1);

            IFeatureDetector aiPlannedActionDuringMayrTurnDetector = new AiPlannedActionForMayorBinaryFeatureDetector();

           

            #endregion

            #region --------------------------------- COMPOSITE DETECTORS

            IFeatureDetector isHumanTurnBinaryFeatureDetector = new OrFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    envTurnBinaryFeatureDetector,
                    ecoTurnBinaryFeatureDetector
                }
            };


            IFeatureDetector aiPlayedGameActionDetector = new AndFeatureDetector()
            {
                Description = "AI playing a game action",
                Detectors =
                {
                    aiTurnBinaryFeatureDetector,
                    gameActionDoneInstantFeatureDetector
                }
            };

            IFeatureDetector aiPlayedGameActionStayAliveDetector = new StayActiveDetector()
            {
                WatchedDetector = aiPlayedGameActionDetector,
                Delay = 5,
            };

            IFeatureDetector turnChangedToHumanPlayersDetector = new AndFeatureDetector()
            {
                Description = "Turn changed to human players",
                Detectors = new List<IFeatureDetector>()
                {
                    new NotFeatureDetector() { WatchedDetector = aiTurnBinaryFeatureDetector},
                    new StayActiveDetector() { 
                        WatchedDetector =  new TurnChangedInstantFeatureDetector(),
                        Delay = 2000 
                    }
                }
            };

            IFeatureDetector turnChangedToAiPlayerDetector = new AndFeatureDetector()
            {
                Description = "Turn changed to AI player",
                Detectors = new List<IFeatureDetector>()
                {
                    aiTurnBinaryFeatureDetector,
                    new TurnChangedInstantFeatureDetector(),
                    new NotFeatureDetector() {WatchedDetector = isTurn0BinaryFeatureDetector},
                    new NotFeatureDetector() {WatchedDetector = isTurn1BinaryFeatureDetector},
                    new NotFeatureDetector() {WatchedDetector = isTurn2BinaryFeatureDetector},
                }
            };

            var aiTakingTooLongDuringAiTurn = new AndFeatureDetector
            {
                Detectors = new List<IFeatureDetector>() 
                { 
                    aiTurnBinaryFeatureDetector,
                    new DelayFromTurnStartDetector() { Delay = 9000 },                                      // AFTER x SECONDS FROM THE START OF THE TURN
                    new NotFeatureDetector(){ WatchedDetector = level1BaseFeatureDetector},                 // NOT DURING THE FIRST LEVEL
                    new NotFeatureDetector() { WatchedDetector = aiPlayedGameActionDetector},                // NOT AFTER THE AI ALREADY DID AN ACTION
                    new NotFeatureDetector() { WatchedDetector = gameEndedAnyReasonFeatureDetector}
                }
            };

            var isHumanIdleBinaryFeatureDetector = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    new DelayFromTurnStartDetector() { Delay = 10000 },  
                    new NotFeatureDetector() { WatchedDetector = didPlayerInteractBinaryFeatureDetector},
                    isHumanTurnBinaryFeatureDetector,
                    new NotFeatureDetector() { WatchedDetector = gameEndedAnyReasonFeatureDetector},
                    new StayActiveDetector(){ WatchedDetector = new NotFeatureDetector() { WatchedDetector = isRobotSpeakingBinaryFeatureDetector },Delay = 5000}
                },
                Description = "isHumanIdleBinaryFeatureDetector",
                ActivationsMinDelayMilliseconds = 60000,
            };

            var afterGameEndedBinaryDetector1 = new HasFiredFeatureDetector()
            {
                WatchedDetector = gameEndedAnyReasonFeatureDetector,
                Description = "Detects when the end-game utterance is been said"
            };


            #region detectors for interactive tutorial

            var gameStartedAndFirstSession = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    gameStartBinaryFeatureDetector,
                    isFirstSessionBinaryFeatureDetector
                }
            };

            

            #endregion


            // ############### CLASSIFIER SHOULD ACT
            var classifierShouldAct = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    new DelayFromTurnStartDetector(){ Delay = 3000 },
                    new NotFeatureDetector() { WatchedDetector = new StayActiveDetector() { WatchedDetector = new RobotSpeakingBinaryFeatureDetector(), Delay = 5000 }},
                    new NotFeatureDetector() { WatchedDetector = gameEndedAnyReasonFeatureDetector},
                },
                Description = "classifierShouldAct"
            };


            // ############### CLASSIFIER SHOULD NOT ACT
            var classifierShouldNotAct = new NotFeatureDetector
            {
                WatchedDetector = classifierShouldAct,
                Description = "classifierShouldNOTAct",
            };

            #endregion



            #region --------------------------------- BEHAVIOURS
            // ############### ANOUNCE AI TURN
            var bUttTurnchangedSelf = new PerformUtterance() { Category = "turntaking", Subcategory = "robot" };
            // ############### ANOUNCE HUMAN PLAYER TURN
            var bUttTurnchangedOther = new PerformUtterance() { Category = "turntaking", Subcategory = "learner" };
            // ############### INTERACTIVE TUTORIAL
            var b_tutorial_1session = new PerformTutorialSession1();
            var b_tutorial_upgrades = new InteractiveTutorialUtterances("Tutorial", "OwnUpgrade", "Tutorial","OtherUpgrade") { Priority = 4};
            var b_tutorial_policy = new InteractiveTutorialUtterances("Tutorial", "OwnPolicy", "Tutorial", "OtherPolicy") { Priority = 4 };
            var b_tutorial_skip = new InteractiveTutorialUtterances("Tutorial", "OwnSkip", "Tutorial", "OtherSkip") { Priority = 4 };
            // ############### MAKE GAME MOVE 
            var bMakeRecomGameMove = new ExecuteAIRecomGameAction();
            // ############### COMMENT AI TAKING TOO LONG
            var bCommentAiTakingTooLong = new PerformUtterance() { Category = "feedback", Subcategory = "thinking" };
            // ############### COMMENT NEW LEVEL
            var bCommentNewLevel = new PerformUtterance() { Category = "reachednewlevel", Subcategory = "-", Priority = 5 };
            // ############### TALK ABOUT RULES
            var bTalkAboutRules = new PerformUtterance() { Category = "Gamerules", Subcategory = "General" };
            // ############### GAME ENDED
            var bGameEndedNoOil = new PerformUtterance() { Category = "endgame", Subcategory = "nooil", Priority = 10 };
            var bGameEndedWin = new PerformUtterance() { Category = "endgame", Subcategory = "successful", Priority = 10 };
            var bGameEndedTimeUp = new PerformUtterance() { Category = "gamestate", Subcategory = "timeup", Priority = 10 };
            // ############### WRAPUP
            var bWrapupAndGoodbye = new PerformWrapupAndGoodbye();

            // ############### ENABLE / DISABLE CLASSIFIER
            var bEnableClassifier = new ClassifierControlBehavior { UseClassifier = true };
            var bDisableClassifier = new ClassifierControlBehavior { UseClassifier = false };


            // ############### LEARNER MODEL
            var bLearnerModelMemoryEvent = new PerformUtteranceForLatestMemoryEvent();

            #endregion

            //var bFakeTutorial = new PerformCustomUtterance()
            //{
            //    Utterance =
            //        "I should say the tutorial now, but since i'm sick of repeating the same phrase over and over, I'm just sayng a shorter one for testing this behaviour."
            //};


            //casePool.Add(new Case(gameStartBinaryFeatureDetector, new BadTrickBehaviour(), false) { Description = "Play game moves"});
            
            // #################################### GAME MECANICS
            casePool.Add(new Case(turnChangedToAiPlayerDetector, bUttTurnchangedSelf) { Description = "Say when turn changes to self" });
            casePool.Add(new Case(turnChangedToHumanPlayersDetector, bUttTurnchangedOther) { Description = "Say when turn changes human players" });
            
            //casePool.Add(new Case(aiPlannedActionDuringMayrTurnDetector, bMakeRecomGameMove, false) { Description = "Play recomended game move", });
            
            casePool.Add(new Case(reachedNewLevelBinaryFeatureDetector, bCommentNewLevel) { Description = "Say when a new level is reached" });

            casePool.Add(new Case(gameEndedNoOilBinaryFeatureDetector, bGameEndedNoOil) { Description = "Say when a the game ended because of no oil" });
            casePool.Add(new Case(gameEndedWinBinaryFeatureDetector, bGameEndedWin) { Description = "Say when a the game ended successfully" });
            casePool.Add(new Case(gameEndedTimeUpBinaryFeatureDetector, bGameEndedTimeUp) { Description = "Say when a the game ended because the time is up" });

            //casePool.Add(new Case(afterGameEndedBinaryDetector1, bWrapupAndGoodbye) { Description = "Tell a wrapup of the game session and goodbye" });

            // #################################### INTERCTIVE TUTORIAL
            casePool.Add(new Case(gameStartedAndFirstSession, b_tutorial_1session, false) { Description = "Tell tutorial" });
            casePool.Add(new Case(firstUpgradeDoneBinaryFeatureDetector, b_tutorial_upgrades, false) { Description = "Tell upgrades tutorial" });
            casePool.Add(new Case(firstPolicyAppliedBinaryFeatureDetector, b_tutorial_policy, false) { Description = "Tell policies tutorial" });
            casePool.Add(new Case(firstSkipDoneBinaryFeatureDetector, b_tutorial_skip, false) { Description = "Tell skip tutorial" });
            
            // ####################################################

            casePool.Add(new Case(aiTakingTooLongDuringAiTurn, bCommentAiTakingTooLong) { Description = "Comment AI taking too long" });
            casePool.Add(new Case(isHumanIdleBinaryFeatureDetector, bTalkAboutRules) { Description = "Helps the human player in the early game if he doesn't act" });

            casePool.Add(new Case(classifierShouldAct, bEnableClassifier) { Description = "Classifier Enabled" });
            casePool.Add(new Case(classifierShouldNotAct, bDisableClassifier) { Description = "Classifier disabled" });

            casePool.Add(new Case(learnerModelMemoryEventInstantFeatureDetector, bLearnerModelMemoryEvent) { Description = "Learner model events" });


            return casePool;
        }

        #endregion

        #region MachineLearning features CasePool

        static public CasePool OkaoCasePool()
        {
            var angerEnvBinaryFeatureDetector = new FacialExpressionDetector() {Expression = Expression.Anger, Subject = Subject.Left};



            List<BaseFeatureDetector> detectors = new List<BaseFeatureDetector>();
            detectors.Add(angerEnvBinaryFeatureDetector);

            // -------------- Fake behaviour
            var fakeBehaviour = new EmptyBehaviour();

            var casePool = new CasePool();
            foreach (var det in detectors)
            {
                casePool.Add(new Case(det, fakeBehaviour) { Description = det.Description });
            }
            return casePool;
        }

        static public CasePool MLPool()
        {
            // --------------------------------- DETECTORS
            List<BaseFeatureDetector> detectors = new List<BaseFeatureDetector>();
            // ############### AI TURN DET
            var aiTurnDetector = new PlayerTurnBinaryFeatureDetector { Player = EnercitiesRole.Mayor, Description = "aiTurnDetector" };
            // ############### HUMAN TURN DET
            var ecoTurnDetector = new PlayerTurnBinaryFeatureDetector { Player = EnercitiesRole.Economist, Description = "ecoTurnDetector" };
            var envTurnDetector = new PlayerTurnBinaryFeatureDetector { Player = EnercitiesRole.Environmentalist, Description = "envTurnDetector" };
            var subDects = new List<IFeatureDetector>() { ecoTurnDetector, envTurnDetector };
            var humanTurnDetector = new OrFeatureDetector() { Description = "humanTurnDetector" };
            humanTurnDetector.AddRange(subDects);
            
            // ############### TURN CHANGE DET
            var turnChangedDetector = new TurnChangedInstantFeatureDetector() { Description = "turnChangedDetector" };
            
            // ############### TURN CHANGED & HUMAN TURN DET
            var turnChangedToHumanPlayerDetector = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { humanTurnDetector, turnChangedDetector } };
            turnChangedToHumanPlayerDetector.Description = "turnChangedToHumanPlayerDetector";
            // ############### AI RECOM MOVE & AI TURN
            var aiActionDetector = new AIActionPlannedDetector() { Description = "aiActionDetector" };
            var aiActionDuringAiTurnDetector = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { aiTurnDetector, aiActionDetector } };
            aiActionDuringAiTurnDetector.Description = "aiActionDuringAiTurnDetector";
            
            // ############### GAME ACTION PLAYED
            var gameActionPlayedDetector = new GameActionInstantFeatureDetector() { Description = "gameActionPlayedDetector" };
            detectors.Add(gameActionPlayedDetector);
            // ############### AI DID GAME ACTION
            var aiDidGameActionDetector = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { gameActionPlayedDetector, aiTurnDetector } };
            aiDidGameActionDetector.Description = "aiDidGameActionDetector";
            
            // ############### NEW LEVEL
            var newLevelDetector = new NewLevelInstantFeatureDetector() { Description = "newLevelDetector" };
            
            // ############### PLAYER GAME INTERACTION (clicks or actions)
            var playerInteractionInThisTurn = new PlayerInteractionBinaryFeatureDetector() { Description = "playerInteractionInThisTurn" };
            
            // ############### AI TAKING TOO LONG
            var aiTakingTooLongDetector = new DelayFromTurnStartDetector() { Delay = 9000, Description = "aiTakingTooLongDetector" };
            
            // ############### AI TAKING TOO LONG & AI TURN
            var aiTakingTooLongDuringAITurn = new AndFeatureDetector { Detectors = new List<IFeatureDetector>() { aiTurnDetector, aiTakingTooLongDetector } };
            aiTakingTooLongDuringAITurn.Description = "aiTakingTooLongDuringAITurn";
            
            // ############### GAME JUST STARTED
            var earlyGameDetector = new EarlyGameDetector() { Description = "earlyGameDetector" };
            
            // ############### PLAYER TURN LASTING TOO LONG
            var playerTurnLastingTooLong = new DelayFromTurnStartDetector() { Delay = 15000, Description = "playerTurnLastingTooLong" };
            
            // ############### PLAYER NOT INTERACTING FOR TOO LONG
            var playerNotInteractingForTooLongDetector = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>() { 
                    new NotFeatureDetector() { WatchedDetector = playerInteractionInThisTurn}, 
                    playerTurnLastingTooLong, 
                    humanTurnDetector, 
                    earlyGameDetector,
                    //new NotFeatureDetector() { WatchedDetector = playerNotInteractingForTooLongDetectorAlreadyFired}
                }
            };
            playerNotInteractingForTooLongDetector.Description = "playerNotInteractingForTooLongDetector";
            var playerNotInteractingForTooLongDetectorAlreadyFired = new HasFiredFeatureDetector() { WatchedDetector = playerNotInteractingForTooLongDetector, Description = "playerNotInteractingForTooLongDetectorAlreadyFired " };
            var playerNotInteractingForToLongAndNotAlreadyFired = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { playerNotInteractingForTooLongDetector, new NotFeatureDetector() { WatchedDetector = playerNotInteractingForTooLongDetectorAlreadyFired } } };
            playerNotInteractingForToLongAndNotAlreadyFired.Description = "playerNotInteractingForToLongAndNotAlreadyFired";
            
            // ############## RULES SAID DETECTOR
            var rulesSaidDetector = new RulesSaidDetector() { Description = "rulesSaidDetector" };
            
            // ############## LAST ACTION TYPE DETECTOR
            var lastActionUpgradeDetector = new LastActionTypeDetector() { ActionType = ActionType.UpgradeStructures, Description = "lastActionUpgradeDetector" };
            var lastActionConstructionDetector = new LastActionTypeDetector() { ActionType = ActionType.BuildStructure, Description = "lastActionConstructionDetector" };
            var lastActionPolicyDetector = new LastActionTypeDetector() { ActionType = ActionType.ImplementPolicy, Description = "lastActionPolicyDetector" };
            var lastActionSkipDetector = new LastActionTypeDetector() { ActionType = ActionType.SkipTurn, Description = "lastActionSkipDetector" };

            // ############## HUMANS SPEAKING
            var ecoSpeakingDetector = new HumanPlayerSpeakingBinaryFeatureDetector() { HumanPlayer = EmoteCommonMessages.ActiveUser.Right, Description = "ecoSpeakingDetector" };
            var envSpeakingDetector = new HumanPlayerSpeakingBinaryFeatureDetector() { HumanPlayer = EmoteCommonMessages.ActiveUser.Left, Description = "envSpeakingDetector" };

            // ############## ROBOT SPEAKING
            var robotSpeakingDetector = new RobotSpeakingBinaryFeatureDetector() { Description = "robotSpeakingDetector" };

            // ############## TURN DETECTORS
            var level1Detector = new LevelBaseFeatureDetector(1) { Description = "level1detector" };
            var level2Detector = new LevelBaseFeatureDetector(2) { Description = "level2detector" };
            var level3Detector = new LevelBaseFeatureDetector(3) { Description = "level3detector" };
            var level4Detector = new LevelBaseFeatureDetector(4) { Description = "level4detector" };

            // ############# OPENED MENUS DETECTORS
            var exploringMenuConstructionDetector = new ExploringMenuDetector(ExploringMenuDetector.MenuType.CONSTRUCTION) { Description = "exploringMenuConstructionDetector" };
            var exploringMenuPolicyDetector = new ExploringMenuDetector(ExploringMenuDetector.MenuType.POLICY) { Description = "exploringMenuPolicyDetector" };
            var exploringMenuUpgradeDetector = new ExploringMenuDetector(ExploringMenuDetector.MenuType.UPGRADE) { Description = "exploringMenuUpgradeDetector" };

            // ############# RESOURCES ABOVE AVERAGE
            var oilAboveAverage = new ResourceAboveAverageDetector() { ResourceType = ResourceType.Oil, Description = "oilAboveAverage" };
            var powerAboveAverage = new ResourceAboveAverageDetector() { ResourceType = ResourceType.Power, Description = "powerAboveAverage" };
            var populationAboveAverage = new ResourceAboveAverageDetector() { ResourceType = ResourceType.Population, Description = "populationAboveAverage" };
            var moneyAboveAverage = new ResourceAboveAverageDetector() { ResourceType = ResourceType.Money, Description = "moneyAboveAverages" };



            detectors.Add(oilAboveAverage);
            detectors.Add(powerAboveAverage);
            detectors.Add(populationAboveAverage);
            detectors.Add(moneyAboveAverage);
            detectors.Add(exploringMenuConstructionDetector);
            detectors.Add(exploringMenuPolicyDetector);
            detectors.Add(exploringMenuUpgradeDetector);
            detectors.Add(robotSpeakingDetector);
            detectors.Add(ecoSpeakingDetector);
            detectors.Add(envSpeakingDetector);
            detectors.Add(lastActionUpgradeDetector);
            detectors.Add(lastActionConstructionDetector);
            detectors.Add(lastActionPolicyDetector);
            detectors.Add(lastActionSkipDetector);
            detectors.Add(rulesSaidDetector);
            detectors.Add(playerNotInteractingForToLongAndNotAlreadyFired);
            detectors.Add(earlyGameDetector);
            detectors.Add(aiTakingTooLongDuringAITurn);
            detectors.Add(playerInteractionInThisTurn);
            detectors.Add(ecoTurnDetector);
            detectors.Add(envTurnDetector);
            detectors.Add(aiTurnDetector);
            //detectors.Add(playerTurnLastingTooLong);
            //detectors.Add(aiTakingTooLongDetector);
            //detectors.Add(newLevelDetector);
            //detectors.Add(level1Detector);
            //detectors.Add(level2Detector);
            //detectors.Add(level3Detector);
            //detectors.Add(level4Detector);
            //detectors.Add(aiDidGameActionDetector);
            //detectors.Add(aiActionDuringAiTurnDetector);
            //detectors.Add(turnChangedDetector);
            //detectors.Add(humanTurnDetector);


            // -------------- Fake behaviour
            var fakeBehaviour = new EmptyBehaviour();


            var casePool = new CasePool();
            foreach (var det in detectors)
            {
                casePool.Add(new Case(det, fakeBehaviour) { Description = det.Description });
            }
            return casePool;
        }

        #endregion

        #region EC Case Pool
        public static CasePool ECPool()
        {
            // --------------------------------- DETECTORS
            var detectors = new List<BaseFeatureDetector>();


            //duplicate for each subject (left/right)
            foreach (var subject in EnumUtil<Subject>.GetTypes())
            {
                // ############### SMILE
                detectors.Add(new SmileDetector
                {
                    Subject = subject,
                    Description = string.Format("{0}Smile", subject)
                });

                // ############### FACIAL EXPRESSIONS
                detectors.AddRange(EnumUtil<Expression>.GetTypes().Select(
                    expression => new FacialExpressionDetector
                    {
                        Subject = subject,
                        Expression = expression,
                        Description =
                            string.Format("{0}{1}", subject, expression)
                    }));

                // ############### GAZE LOOK-TOs
                var gazeDetectors = new List<GazeDetector>(EnumUtil<GazeDirection>.GetTypes().Select(
                    direction => new GazeDetector
                    {
                        Subject = subject,
                        Direction = direction,
                        Description =
                            string.Format("{0}Look{1}", subject, direction)
                    }));
                detectors.AddRange(gazeDetectors);

                // ############### GAZE DISTRACTED
                var orFeatureDetector = new OrFeatureDetector();
                foreach (var gazeDetector in gazeDetectors)
                    orFeatureDetector.Add(gazeDetector);
                var distracted = new NotFeatureDetector
                {
                    WatchedDetector = orFeatureDetector,
                    Description = string.Format("{0}Distracted", subject)
                };
                detectors.Add(distracted);
            }


            // -------------- Fake behaviour
            var fakeBehaviour = new EmptyBehaviour();


            var casePool = new CasePool();
            foreach (var det in detectors)
                casePool.Add(new Case(det, fakeBehaviour) { Description = det.Description });
            return casePool;
        }
        #endregion

        #region TestPool

        static public CasePool TestPool()
        {
            var casePool = new CasePool();

            #region  --------------------------------- BASE DETECTORS
            IFeatureDetector gameStartBinaryFeatureDetector = new GameStartedBinaryFeatureDetector();
            IFeatureDetector turnChangedInstantFeatureDetector = new TurnChangedInstantFeatureDetector();
            IFeatureDetector newLevelInstantFeatureDetector = new NewLevelInstantFeatureDetector();

            IFeatureDetector level1BaseFeatureDetector = new LevelBaseFeatureDetector(1);

            IFeatureDetector aiTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Mayor, Description =  "AI turn detector"};
            IFeatureDetector envTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Environmentalist, Description = "Environmentalist turn detector" };
            IFeatureDetector ecoTurnBinaryFeatureDetector = new PlayerTurnBinaryFeatureDetector() { Player = EnercitiesRole.Economist, Description = "Economist turn detector"  };

            IFeatureDetector aiPlannedAction = new AIActionPlannedDetector();

            IFeatureDetector gameActionDoneInstantFeatureDetector = new GameActionInstantFeatureDetector() { Description = "Tells when something have been built/uptadet/implemented"};
            IFeatureDetector didPlayerInteractBinaryFeatureDetector = new PlayerInteractionBinaryFeatureDetector() { Description = "Tells when any action was done on the game"};

            IFeatureDetector isRobotSpeakingBinaryFeatureDetector = new RobotSpeakingBinaryFeatureDetector();
            IFeatureDetector isHumanSpeakingBinaryFeatureDetector = new HumanPlayerSpeakingBinaryFeatureDetector();

            

            #endregion

            #region --------------------------------- COMPOSITE DETECTORS

            IFeatureDetector isHumanTurnBinaryFeatureDetector = new OrFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    envTurnBinaryFeatureDetector,
                    ecoTurnBinaryFeatureDetector
                }
            };

            IFeatureDetector aiPlannedActionDuringMayrTurnDetector = new AndFeatureDetector()
            {
                Description = "AI planned action in Mayor turn",
                Detectors = new List<IFeatureDetector>()
                {
                    aiTurnBinaryFeatureDetector,
                    aiPlannedAction
                }
            };

            IFeatureDetector aiPlayedGameActionDetector = new AndFeatureDetector()
            {
                Description = "AI playing a game action",
                Detectors =
                {
                    aiTurnBinaryFeatureDetector,
                    gameActionDoneInstantFeatureDetector
                }
            };

            IFeatureDetector turnChangedToHumanPlayersDetector = new AndFeatureDetector()
            {
                Description = "Turn changed to human players",
                Detectors = new List<IFeatureDetector>()
                {
                    new NotFeatureDetector() { WatchedDetector = aiTurnBinaryFeatureDetector},
                    new StayActiveDetector() { 
                        WatchedDetector =  new TurnChangedInstantFeatureDetector(),
                        Delay = 2000 
                    }
                }
            };

            IFeatureDetector turnChangedToAiPlayerDetector = new AndFeatureDetector()
            {
                Description = "Turn changed to AI player",
                Detectors = new List<IFeatureDetector>()
                {
                    aiTurnBinaryFeatureDetector,
                    new TurnChangedInstantFeatureDetector()
                }
            };

            var aiTakingTooLongDuringAiTurn = new AndFeatureDetector
            {
                Detectors = new List<IFeatureDetector>() 
                { 
                    aiTurnBinaryFeatureDetector,
                    new DelayFromTurnStartDetector() { Delay = 9000 },                                      // AFTER x SECONDS FROM THE START OF THE TURN
                    new NotFeatureDetector(){ WatchedDetector = level1BaseFeatureDetector},                 // NOT DURING THE FIRST LEVEL
                    new NotFeatureDetector() { WatchedDetector = aiPlayedGameActionDetector}                // NOT AFTER THE AI ALREADY DID AN ACTION
                }
            };

            var isHumanIdleBinaryFeatureDetector = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    new DelayFromTurnStartDetector() { Delay = 10000 },  
                    new NotFeatureDetector() { WatchedDetector = didPlayerInteractBinaryFeatureDetector},
                    isHumanTurnBinaryFeatureDetector,
                },
                Description = "isHumanIdleBinaryFeatureDetector",
                ActivationsMinDelayMilliseconds = 60000,
            };

            // ############### CLASSIFIER SHOULD ACT
            var classifierShouldAct = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    new DelayFromTurnStartDetector(){ Delay = 8000 },
                    new NotFeatureDetector() { WatchedDetector = new StayActiveDetector() { WatchedDetector = new RobotSpeakingBinaryFeatureDetector(), Delay = 5000 }},
                },
                Description = "classifierShouldAct"
            };

            // ############### CLASSIFIER SHOULD NOT ACT
            var classifierShouldNotAct = new NotFeatureDetector
            {
                WatchedDetector = classifierShouldAct,
                Description = "classifierShouldNOTAct",
            };

            #endregion



            


            #region --------------------------------- BEHAVIOURS
            // ############### ANOUNCE AI TURN
            var b_utt_turnchanged_self = new PerformUtterance() { Category = "turnchanged", Subcategory = "self" };
            // ############### ANOUNCE HUMAN PLAYER TURN
            var b_utt_turnchanged_other = new PerformUtterance() { Category = "turnchanged", Subcategory = "other" };
            // ############### SAY TUTORIAL
            var b_utt_tutorial = new PerformUtterance { Category = "gamestatus", Subcategory = "tutorial" };
            // ############### MAKE GAME MOVE 
            var b_makeRecomGameMove = new ExecuteAIRecomGameAction();
            // ############### COMMENT AI GAME ACTION
            var b_commentAIGameAction = new PerformGameActionUtterance() { };
            // ############### COMMENT AI TAKING TOO LONG
            var b_commentAITakingTooLong = new PerformUtterance() { Category = "feedback", Subcategory = "thinking" };
            // ############### COMMENT NEW LEVEL
            var b_commentNewLevel = new PerformUtterance() { Category = "reachednewlevel", Subcategory = "-" };
            // ############### COMMENT PLAYER TAKING TOO LONG WITHOUT INTERACTING DURING EARLY GAME
            var b_commentPlayerTakingLongEarlyGame = new PerformUtterance(true) { Category = "gamerules", Subcategory = "general" };
            // ############### TALK ABOUT RULES
            var b_talkAboutRules = new PerformUtterance() { Category = "Gamerules", Subcategory = "General" };
            //new RandomBehavior()
            //{
            //    Behaviors = new List<BaseBehavior>()
            //    {
            //        new PerformUtterance(true) { Category = "Gamerules", Subcategory = "General" },
            //        new PerformUtterance(true) { Category = "Gamerules", Subcategory = "Indicators" },
            //        new PerformUtterance(true) { Category = "Gamerules", Subcategory = "LevelUp" },
            //        new PerformUtterance(true) { Category = "Gamerules", Subcategory = "Policy" },
            //        new PerformUtterance(true) { Category = "Gamerules", Subcategory = "NonRenewable" },
            //        new PerformUtterance(true) { Category = "Gamerules", Subcategory = "Residential" },
            //    }
            //};

            // ############### ENABLE / DISABLE CLASSIFIER
            var b_enableClassifier = new ClassifierControlBehavior { UseClassifier = true };
            var b_disableClassifier = new ClassifierControlBehavior { UseClassifier = false, Priority = 1 };

            #endregion

            

            return casePool;
        }

        #endregion
    }
}