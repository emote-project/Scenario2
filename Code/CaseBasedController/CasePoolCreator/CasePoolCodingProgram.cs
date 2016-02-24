using System;
using System.Collections.Generic;
using CaseBasedController;
using CaseBasedController.Behavior;
using CaseBasedController.Behavior.Enercities;
using CaseBasedController.Detection;
using CaseBasedController.Detection.Composition;
using CaseBasedController.Detection.Enercities;
using EmoteEnercitiesMessages;

namespace CasePoolCreator
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

        public static CasePool EnercitiesDemo()
        {
            var casePool = new CasePool();

            // --------------------------------- DETECTORS
            // ############### FIRST TURN DETECTOR
            var firstTurnDetector = new TurnDetector(1) { Description = "firstTurnDetector" };
            var gameStartedDetector = new GameStartedMessageDetector() { Description = "gameStartedDetector" };
            // ############### TUTORIAL
            var tutorialTimeDetector = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    gameStartedDetector,
                },
                Description = "tutorialTimeDetector"
            };
            var tutorialDoneDetector = new OrFeatureDetector
            {
                Detectors = new List<IFeatureDetector>(){
                    new HasFiredFeatureDetector() { WatchedDetector = tutorialTimeDetector, Description = "TutorialDoneDetector" },
                    new NotFeatureDetector() { WatchedDetector =  new TurnDetector(1) }
                },
                Description = "tutorialDoneDetector"
            };
        

            // ############### AI TURN DET
            var aiTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Mayor, Description = "aiTurnDetector"};
            var turnChangedToAI = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    aiTurnDetector,
                    tutorialDoneDetector,
                },
                Description = "turnChangedToAI"
            };
            // ############### HUMAN TURN DET
            var ecoTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Economist, Description = "ecoTurnDetector" };
            var envTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Environmentalist, Description = "envTurnDetector" };

            var humanTurnDetector = new OrFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    ecoTurnDetector,
                    envTurnDetector
                },
                Description = "humanTurnDetector"
            };
            // ############### TURN CHANGE DET
            var turnChangedDetector = new TurnChangedDetector() { Description = "turnChangedDetector" };
            // ############### TURN CHANGED & HUMAN TURN DET
            var turnChangedToHumanPlayerDetector = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { humanTurnDetector, turnChangedDetector } };
            turnChangedToHumanPlayerDetector.Description = "turnChangedToHumanPlayerDetector";
            // ############### AI RECOM MOVE DET
            var aiActionDetector = new AIActionPlannedDetector() { Description = "aiActionDetector" };
            // ############### AI RECOM MOVE & AI TURN
            var aiActionDuringAiTurnDetector = new AndFeatureDetector() { 
                Detectors = new List<IFeatureDetector>() { 
                    aiTurnDetector, 
                    aiActionDetector,
                    tutorialDoneDetector,
                }};
            aiActionDuringAiTurnDetector.Description = "aiActionDuringAiTurnDetector";
            // ############### GAME ACTION PLAYED
            var gameActionPlayedDetector = new GameActionDetector() { Description = "gameActionPlayedDetector" };
            // ############### AI DID GAME ACTION
            var aiDidGameActionDetector = new AndFeatureDetector() 
            { 
                Detectors = new List<IFeatureDetector>() 
                {  
                    gameActionPlayedDetector,
                    aiTurnDetector,
                    tutorialDoneDetector
                }
            };
            aiDidGameActionDetector.Description = "aiDidGameActionDetector";
            // ############### AI TAKING TOO LONG & AI TURN
            var aiTakingTooLongDuringAITurn = new AndFeatureDetector 
            { 
                Detectors = new List<IFeatureDetector>() 
                { 
                    aiTurnDetector,
                    new DelayFromTurnStartDetector() { Delay = 9000 },
                    new NotFeatureDetector(){ WatchedDetector = firstTurnDetector},
                    new NotFeatureDetector() { WatchedDetector = aiDidGameActionDetector}
                }
            };
            aiTakingTooLongDuringAITurn.Description = "aiTakingTooLongDuringAITurn";
            // ############### NEW LEVEL
            var newLevelDetector = new NewLevelDetector() { Description = "newLevelDetector" };
            // ############### GAME JUST STARTED
            var earlyGameDetector = new EarlyGameDetector() { Description = "earlyGameDetector" };
            // ############### PLAYER GAME INTERACTION (clicks or actions)
            var playerInteractionInThisTurn = new PlayerInteractionDetector() { Description = "playerInteractionInThisTurn" };
            // ############### PLAYER TURN LASTING TOO LONG
            var playerTurnLastingTooLong = new DelayFromTurnStartDetector() { Delay = 10000, Description = "playerTurnLastingTooLong" };
            // ############### PLAYER NOT INTERACTING FOR TOO LONG
            var noSpeakingForTooLong = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>() 
                { 
                    new NotFeatureDetector() { WatchedDetector = new RobotSpeakingDetector()}, 
                    new DelayFromTurnStartDetector() { Delay = 7000 },
                    humanTurnDetector, 
                },
                Description = "noSpeakingForTooLong"
            };
            var doActionsForNoSpeakingForTooLong = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    noSpeakingForTooLong,
                    new NotFeatureDetector() {
                        WatchedDetector = new WasActiveDetector(){ 
                            WatchedDetector = noSpeakingForTooLong,  
                            Delay = 60000
                        }
                    },
                    new OrFeatureDetector() { 
                        Detectors= new List<IFeatureDetector>(){
                            new TurnDetector(1),
                            new TurnDetector(2),
                            new TurnDetector(3),
                            new TurnDetector(4)
                        }
                    },
                },
                Description = "doActionsForNoSpeakingForTooLong"
            };


            // ############### TALKING ABOUT RULES IN THIS TURN
            var shouldTalkAboutRules = new AndFeatureDetector() {
                Detectors = new List<IFeatureDetector>() {
                    new NotFeatureDetector() { WatchedDetector = new RobotSpeakingDetector() },
                    new WasActiveDetector() {WatchedDetector = new RobotSpeakingDetector() , Delay=5000},
                },
                Description = "shouldTalkAboutRules"
            };

            var doTalkAboutRulesThisTurn = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    shouldTalkAboutRules,
                },
                Description = "doTalkAboutRulesThisTurn"
            };

            



            // ############### CLASSIFIER SHOULD ACT
            var classifierShouldAct = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    new DelayFromTurnStartDetector(){ Delay = 8000 },
                    new NotFeatureDetector() { WatchedDetector = new StayActiveDetector() { WatchedDetector = new RobotSpeakingDetector(), Delay = 5000 }},
                },
                Description = "classifierShouldAct"
            };
            
            classifierShouldAct.Description = "classifierShouldAct";
            // ############### CLASSIFIER SHOULD NOT ACT
            var classifierShouldNOTAct = new NotFeatureDetector()
            {
                WatchedDetector = classifierShouldAct,
            };
            classifierShouldNOTAct.Description = "classifierShouldNOTAct";


            // --------------------------------- BEHAVIOURS
            // ############### ANOUNCE AI TURN
            var b_utt_turnchanged_self = new PerformUtterance() { Category = "turnchanged", Subcategory = "self", Priority = 0 };
            // ############### ANOUNCE HUMAN PLAYER TURN
            var b_utt_turnchanged_other = new PerformUtterance() { Category = "turnchanged", Subcategory = "other", Priority = 0 };
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
            var b_disableClassifier = new ClassifierControlBehavior { UseClassifier = false, Priority=1 };

            casePool.Add(new Case(tutorialTimeDetector, b_utt_tutorial) { Description = "Tell tutorial" });
            casePool.Add(new Case(turnChangedToAI, b_utt_turnchanged_self) { Description = "Say when turn changes to self" });
            casePool.Add(new Case(ecoTurnDetector, b_utt_turnchanged_other) { Description = "Say when turn changes to economist" });
            casePool.Add(new Case(envTurnDetector, b_utt_turnchanged_other) { Description = "Say when turn changes to environmentalist" });
            casePool.Add(new Case(aiActionDuringAiTurnDetector, b_makeRecomGameMove) { Description = "Play recomended game move" });
            casePool.Add(new Case(aiDidGameActionDetector, b_commentAIGameAction) { Description = "Comment AI game action" });
            casePool.Add(new Case(aiTakingTooLongDuringAITurn, b_commentAITakingTooLong) { Description = "Comment AI taking too long" });
            casePool.Add(new Case(newLevelDetector, b_commentNewLevel) { Description = "Say when a new level is reached" });
            casePool.Add(new Case(doActionsForNoSpeakingForTooLong, b_talkAboutRules) { Description = "Helps the human player in the early game if he doesn't act" });
            //casePool.Add(new Case(doTalkAboutRulesThisTurn, b_talkAboutRules) { Description = "Talk about rules when nothing is being said"});


            casePool.Add(new Case(classifierShouldAct, b_enableClassifier) { Description = "Classifier Enabled" });
            casePool.Add(new Case(classifierShouldNOTAct, b_disableClassifier) { Description = "Classifier disabled" });
            
            return casePool;
        }

        #endregion

        #region MachineLearning features CasePool

        static public CasePool MLPool()
        {
            // --------------------------------- DETECTORS
            List<BaseFeatureDetector> detectors = new List<BaseFeatureDetector>();
            // ############### AI TURN DET
            var aiTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Mayor, Description = "aiTurnDetector" };
            // ############### HUMAN TURN DET
            var ecoTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Economist, Description = "ecoTurnDetector" };
            var envTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Environmentalist, Description = "envTurnDetector" };
            var subDects = new List<IFeatureDetector>() { ecoTurnDetector, envTurnDetector };
            var humanTurnDetector = new OrFeatureDetector() { Description = "humanTurnDetector" };
            humanTurnDetector.AddRange(subDects);
            
            // ############### TURN CHANGE DET
            var turnChangedDetector = new TurnChangedDetector() { Description = "turnChangedDetector" };
            
            // ############### TURN CHANGED & HUMAN TURN DET
            var turnChangedToHumanPlayerDetector = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { humanTurnDetector, turnChangedDetector } };
            turnChangedToHumanPlayerDetector.Description = "turnChangedToHumanPlayerDetector";
            // ############### AI RECOM MOVE & AI TURN
            var aiActionDetector = new AIActionPlannedDetector() { Description = "aiActionDetector" };
            var aiActionDuringAiTurnDetector = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { aiTurnDetector, aiActionDetector } };
            aiActionDuringAiTurnDetector.Description = "aiActionDuringAiTurnDetector";
            
            // ############### GAME ACTION PLAYED
            var gameActionPlayedDetector = new GameActionDetector() { Description = "gameActionPlayedDetector" };
            detectors.Add(gameActionPlayedDetector);
            // ############### AI DID GAME ACTION
            var aiDidGameActionDetector = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { gameActionPlayedDetector, aiTurnDetector } };
            aiDidGameActionDetector.Description = "aiDidGameActionDetector";
            
            // ############### NEW LEVEL
            var newLevelDetector = new NewLevelDetector() { Description = "newLevelDetector" };
            
            // ############### PLAYER GAME INTERACTION (clicks or actions)
            var playerInteractionInThisTurn = new PlayerInteractionDetector() { Description = "playerInteractionInThisTurn" };
            
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
            var ecoSpeakingDetector = new HumanPlayerSpeakingDetector() { HumanPlayer = EmoteCommonMessages.ActiveUser.Right, Description = "ecoSpeakingDetector" };
            var envSpeakingDetector = new HumanPlayerSpeakingDetector() { HumanPlayer = EmoteCommonMessages.ActiveUser.Left, Description = "envSpeakingDetector" };

            // ############## ROBOT SPEAKING
            var robotSpeakingDetector = new RobotSpeakingDetector() { Description = "robotSpeakingDetector" };

            // ############## TURN DETECTORS
            var level1Detector = new TurnDetector(1) { Description = "level1detector" };
            var level2Detector = new TurnDetector(2) { Description = "level2detector" };
            var level3Detector = new TurnDetector(3) { Description = "level3detector" };
            var level4Detector = new TurnDetector(4) { Description = "level4detector" };

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

        #region TestPool

        static public CasePool TestPool()
        {
            var casePool = new CasePool();

            // --------------------------------- DETECTORS
            // ############### FIRST TURN DETECTOR
            var firstTurnDetector = new TurnDetector(1) { Description = "firstTurnDetector" };
            var gameStartedDetector = new GameStartedMessageDetector() { Description = "gameStartedDetector" };
            // ############### TUTORIAL
            var tutorialTimeDetector = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    gameStartedDetector,
                },
                Description = "tutorialTimeDetector"
            };
            var tutorialDoneDetector = new OrFeatureDetector
            {
                Detectors = new List<IFeatureDetector>(){
                    new HasFiredFeatureDetector() { WatchedDetector = tutorialTimeDetector, Description = "TutorialDoneDetector" },
                    new NotFeatureDetector() { WatchedDetector =  new TurnDetector(1) }
                },
                Description = "tutorialDoneDetector"
            };


            // ############### AI TURN DET
            var aiTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Mayor, Description = "aiTurnDetector" };
            var turnChangedToAI = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    aiTurnDetector,
                    tutorialDoneDetector,
                },
                Description = "turnChangedToAI"
            };
            // ############### HUMAN TURN DET
            var ecoTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Economist, Description = "ecoTurnDetector" };
            var envTurnDetector = new PlayerTurnDetector { Player = EnercitiesRole.Environmentalist, Description = "envTurnDetector" };

            var humanTurnDetector = new OrFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    ecoTurnDetector,
                    envTurnDetector
                },
                Description = "humanTurnDetector"
            };
            // ############### TURN CHANGE DET
            var turnChangedDetector = new TurnChangedDetector() { Description = "turnChangedDetector" };
            // ############### TURN CHANGED & HUMAN TURN DET
            var turnChangedToHumanPlayerDetector = new AndFeatureDetector() { Detectors = new List<IFeatureDetector>() { humanTurnDetector, turnChangedDetector } };
            turnChangedToHumanPlayerDetector.Description = "turnChangedToHumanPlayerDetector";
            // ############### AI RECOM MOVE DET
            var aiActionDetector = new AIActionPlannedDetector() { Description = "aiActionDetector" };
            // ############### AI RECOM MOVE & AI TURN
            var aiActionDuringAiTurnDetector = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>() { 
                    aiTurnDetector, 
                    aiActionDetector,
                    tutorialDoneDetector,
                }
            };
            aiActionDuringAiTurnDetector.Description = "aiActionDuringAiTurnDetector";
            // ############### GAME ACTION PLAYED
            var gameActionPlayedDetector = new GameActionDetector() { Description = "gameActionPlayedDetector" };
            // ############### AI DID GAME ACTION
            var aiDidGameActionDetector = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>() 
                {  
                    gameActionPlayedDetector,
                    aiTurnDetector,
                    tutorialDoneDetector
                }
            };
            aiDidGameActionDetector.Description = "aiDidGameActionDetector";
            // ############### AI TAKING TOO LONG & AI TURN
            var aiTakingTooLongDuringAITurn = new AndFeatureDetector
            {
                Detectors = new List<IFeatureDetector>() 
                { 
                    aiTurnDetector,
                    new DelayFromTurnStartDetector() { Delay = 9000 },
                    new NotFeatureDetector(){ WatchedDetector = firstTurnDetector},
                    new NotFeatureDetector() { WatchedDetector = aiDidGameActionDetector}
                }
            };
            aiTakingTooLongDuringAITurn.Description = "aiTakingTooLongDuringAITurn";
            // ############### NEW LEVEL
            var newLevelDetector = new NewLevelDetector() { Description = "newLevelDetector" };
            // ############### GAME JUST STARTED
            var earlyGameDetector = new EarlyGameDetector() { Description = "earlyGameDetector" };
            // ############### PLAYER GAME INTERACTION (clicks or actions)
            var playerInteractionInThisTurn = new PlayerInteractionDetector() { Description = "playerInteractionInThisTurn" };
            // ############### PLAYER TURN LASTING TOO LONG
            var playerTurnLastingTooLong = new DelayFromTurnStartDetector() { Delay = 10000, Description = "playerTurnLastingTooLong" };
            // ############### PLAYER NOT INTERACTING FOR TOO LONG
            var noSpeakingForTooLong = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>() 
                { 
                    new NotFeatureDetector() { WatchedDetector = new RobotSpeakingDetector()}, 
                    new DelayFromTurnStartDetector() { Delay = 7000 },
                    humanTurnDetector, 
                },
                Description = "noSpeakingForTooLong"
            };
            var doActionsForNoSpeakingForTooLong = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    noSpeakingForTooLong,
                    new NotFeatureDetector() {
                        WatchedDetector = new WasActiveDetector(){ 
                            WatchedDetector = noSpeakingForTooLong,  
                            Delay = 60000
                        }
                    },
                    new OrFeatureDetector() { 
                        Detectors= new List<IFeatureDetector>(){
                            new TurnDetector(1),
                            new TurnDetector(2),
                            new TurnDetector(3),
                            new TurnDetector(4)
                        }
                    },
                },
                Description = "doActionsForNoSpeakingForTooLong"
            };


            // ############### TALKING ABOUT RULES IN THIS TURN
            var shouldTalkAboutRules = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>() {
                    new NotFeatureDetector() { WatchedDetector = new RobotSpeakingDetector() },
                    new WasActiveDetector() {WatchedDetector = new RobotSpeakingDetector() , Delay=5000},
                },
                Description = "shouldTalkAboutRules"
            };

            var doTalkAboutRulesThisTurn = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    shouldTalkAboutRules,
                },
                Description = "doTalkAboutRulesThisTurn"
            };





            // ############### CLASSIFIER SHOULD ACT
            var classifierShouldAct = new AndFeatureDetector()
            {
                Detectors = new List<IFeatureDetector>()
                {
                    new DelayFromTurnStartDetector(){ Delay = 8000 },
                    new NotFeatureDetector() { WatchedDetector = new StayActiveDetector() { WatchedDetector = new RobotSpeakingDetector(), Delay = 5000 }},
                },
                Description = "classifierShouldAct"
            };

            classifierShouldAct.Description = "classifierShouldAct";
            // ############### CLASSIFIER SHOULD NOT ACT
            var classifierShouldNOTAct = new NotFeatureDetector()
            {
                WatchedDetector = classifierShouldAct,
            };
            classifierShouldNOTAct.Description = "classifierShouldNOTAct";


            // --------------------------------- BEHAVIOURS
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

            //casePool.Add(new Case(tutorialTimeDetector, b_utt_tutorial) { Description = "Tell tutorial" });
            //casePool.Add(new Case(turnChangedToAI, b_utt_turnchanged_self) { Description = "Say when turn changes to self" });
            //casePool.Add(new Case(ecoTurnDetector, b_utt_turnchanged_other) { Description = "Say when turn changes to economist" });
            //casePool.Add(new Case(envTurnDetector, b_utt_turnchanged_other) { Description = "Say when turn changes to environmentalist" });
            casePool.Add(new Case(aiActionDuringAiTurnDetector, b_makeRecomGameMove) { Description = "Play recomended game move" });
            casePool.Add(new Case(aiDidGameActionDetector, b_commentAIGameAction) { Description = "Comment AI game action" });
            //casePool.Add(new Case(aiTakingTooLongDuringAITurn, b_commentAITakingTooLong) { Description = "Comment AI taking too long" });
            //casePool.Add(new Case(newLevelDetector, b_commentNewLevel) { Description = "Say when a new level is reached" });
            //casePool.Add(new Case(doActionsForNoSpeakingForTooLong, b_talkAboutRules) { Description = "Helps the human player in the early game if he doesn't act" });
            //casePool.Add(new Case(doTalkAboutRulesThisTurn, b_talkAboutRules) { Description = "Talk about rules when nothing is being said"});


            //casePool.Add(new Case(classifierShouldAct, b_enableClassifier) { Description = "Classifier Enabled" });
            //casePool.Add(new Case(classifierShouldNOTAct, b_disableClassifier) { Description = "Classifier disabled" });

            return casePool;
        }

        #endregion
    }
}