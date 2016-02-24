using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.behaviours;
using InOutEmote.behaviours.gameActions;
using InOutEmote.behaviours.utterances;
using InOutEmote.inputs;
using LogicWebLib;
using LogicWebLib.NodeTypes;

namespace InOutEmote
{
    public class InitScriptEmoteEmpathic : InitializationScript
    {
        public override void Run(out LogicFrame logicFrame, out LogicWeb logicWeb)
        {
            logicFrame = new InOutEmote();


            // If i had an editor I would just select the available nodes, already loaded on the screen.
            // Since I'm creating the logic web by code, I have to find the input nodes available in the list of the inputs of the logic frame.
            var nodes = new List<LogicNode>();
            InputNode envTurnIN = null;
            InputNode ecoTurnIN = null;
            InputNode mayorTurnIN = null;
            InputNode newLevelIN = null;
            InputNode isfirstSession = null;
            InputNode isFirstTurn = null;
            InputNode isGameStarted = null;
            InputNode gameEndedNoOil = null;
            InputNode gameEndedTimeUp = null;
            InputNode gameEndedWin = null;
            InputNode firstUpgradeByRobot = null;
            InputNode firstPolicyByRobot = null;
            InputNode firstSkipByRobot = null;
            InputNode structureBuilt = null;
            InputNode structureUpgrade = null;
            InputNode policyImplemented = null;
            InputNode aiPlannedActions = null;
            InputNode learnerModelMemoryEvent = null;
            InputNode usersIdling = null;
            foreach (var inputNode in logicFrame.Inputs)
            {
                if (inputNode.GetType().Name.Equals(typeof(EnvTurnIN).Name))
                {
                    envTurnIN = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(EcoTurnIN).Name))
                {
                    ecoTurnIN = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(MayorTurnIN).Name))
                {
                    mayorTurnIN = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(NewLevelReachedIN).Name))
                {
                    newLevelIN = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(IsFirstSessionIN).Name))
                {
                    isfirstSession = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(IsFirstTurnIN).Name))
                {
                    isFirstTurn = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(IsGameStartedIN).Name))
                {
                    isGameStarted = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(GameEndedNoOilIN).Name))
                {
                    gameEndedNoOil = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(GameEndedTimeUpIN).Name))
                {
                    gameEndedTimeUp = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(GameEndedWinIN).Name))
                {
                    gameEndedWin = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(FirstUpgradeDoneByRobotIN).Name))
                {
                    firstUpgradeByRobot = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(FirstPolicyDoneByRobotIN).Name))
                {
                    firstPolicyByRobot = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(FirstSkipDoneByRobotIN).Name))
                {
                    firstSkipByRobot = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(StructureBuiltIN).Name))
                {
                    structureBuilt = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(StructureUpgradedIN).Name))
                {
                    structureUpgrade = inputNode;
                } 
                if (inputNode.GetType().Name.Equals(typeof(PolicyImplementedIN).Name))
                {
                    policyImplemented = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(ActionPlannedByAiForMayorIN).Name))
                {
                    aiPlannedActions = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(LearnerModelMemoryEventIN).Name))
                {
                    learnerModelMemoryEvent = inputNode;
                }
                if (inputNode.GetType().Name.Equals(typeof(UsersIdlingIN).Name))
                {
                    usersIdling = inputNode;
                }
            }

            logicWeb = new LogicWebLib.LogicWeb(logicFrame, nodes);

           

            var outNode = logicFrame.AddOutput("announceNewLevel");
            outNode.WatchedNode = newLevelIN;
            outNode.BehaviourType = typeof(AnnounceNewLevel);

            outNode = logicFrame.AddOutput("gameendedNoOil");
            outNode.WatchedNode = gameEndedNoOil;
            outNode.BehaviourType = typeof(AnnounceGameEndedNoOil);

            outNode = logicFrame.AddOutput("gameendedTimeup");
            outNode.WatchedNode = gameEndedTimeUp;
            outNode.BehaviourType = typeof(AnnounceGameEndedTimeUp);

            outNode = logicFrame.AddOutput("GameendedWin");
            outNode.WatchedNode = gameEndedWin;
            outNode.BehaviourType = typeof(AnnounceGameEndedWin);

             var gameEndedGeneric = new OrLogicNode("Game ended for any reason", new List<Node>() { gameEndedTimeUp, gameEndedWin, gameEndedNoOil });
            logicWeb.Web.Add(gameEndedGeneric);

            var gameNotYetEnded = new NotLogicNode("Game not yet ended", gameEndedGeneric);
            logicWeb.Web.Add(gameNotYetEnded);

            var envTurnAndGameNotEnded = new AndLogicNode("EnvTurn And GameNotEnded",
            new List<Node>()
            {
                envTurnIN,
                gameNotYetEnded,
            });
            logicWeb.Web.Add(envTurnAndGameNotEnded);
            outNode = logicFrame.AddOutput("announceEnvTurn");
            outNode.WatchedNode = envTurnAndGameNotEnded;
            outNode.BehaviourType = typeof(AnnounceHumanTurn);

            var ecoTurnAndGameNotEnded = new AndLogicNode("EcoTurn And GameNotEnded",
            new List<Node>()
            {
                ecoTurnIN,
                gameNotYetEnded,
            });
            logicWeb.Web.Add(ecoTurnAndGameNotEnded);
            outNode = logicFrame.AddOutput("announceEcoTurn");
            outNode.WatchedNode = ecoTurnAndGameNotEnded;
            outNode.BehaviourType = typeof(AnnounceHumanTurn);

            var mayorTurnAndGameNotEnded = new AndLogicNode("MayorTurn And GameNotEnded",
            new List<Node>()
            {
                mayorTurnIN,
                gameNotYetEnded,
            });
            logicWeb.Web.Add(mayorTurnAndGameNotEnded);
            outNode = logicFrame.AddOutput("announceRobotTurn");
            outNode.WatchedNode = mayorTurnAndGameNotEnded;
            outNode.BehaviourType = typeof(AnnounceRobotTurn);

            var gameStartedInFirstSession = new AndLogicNode("Game started in first session", new List<Node>() { isGameStarted, isfirstSession });
            logicWeb.Web.Add(gameStartedInFirstSession);

            outNode = logicFrame.AddOutput("First Tutorial");
            outNode.WatchedNode = gameStartedInFirstSession;
            outNode.BehaviourType = typeof(PerformBaseTutorial);

            var checkTutorialExecutedInputNode = new OutputBehaviourExecutedInputNode(outNode);                 // ADDED NEW INPUT NODE
            checkTutorialExecutedInputNode.Description = "Checks when the base tutorial has been performed";
            logicFrame.AddInput(checkTutorialExecutedInputNode);

            outNode = logicFrame.AddOutput("Construction tutorial");
            outNode.WatchedNode = checkTutorialExecutedInputNode;
            outNode.BehaviourType = typeof(PerformOwnConstructionTutorial);

            var checkBaseTutorialActionExecuted = new OutputBehaviourExecutedInputNode(outNode);                // ADDED NEW INPUT NODE
            checkBaseTutorialActionExecuted.Description = "Checks when the base tutorial action has been performed";
            logicFrame.AddInput(checkBaseTutorialActionExecuted);

            outNode = logicFrame.AddOutput("Performing tutorial custom action");
            outNode.WatchedNode = checkBaseTutorialActionExecuted;
            outNode.BehaviourType = typeof(CustomBaseTutorialAction);

            outNode = logicFrame.AddOutput("Performing tutorial policy");
            outNode.WatchedNode = firstPolicyByRobot;
            outNode.BehaviourType = typeof(PerformOwnPolicyTutorial);

            outNode = logicFrame.AddOutput("Performing tutorial upgrade");
            outNode.WatchedNode = firstUpgradeByRobot;
            outNode.BehaviourType = typeof(PerformOwnUpgradeTutorial);

            outNode = logicFrame.AddOutput("Performing tutorial Skip");
            outNode.WatchedNode = firstSkipByRobot;
            outNode.BehaviourType = typeof(PerformOwnSkipTutorial);

           

            var robotBuiltStructure = new AndLogicNode("Robot built a structure", new List<Node>() { mayorTurnIN, structureBuilt });
            logicWeb.Web.Add(robotBuiltStructure);

            outNode = logicFrame.AddOutput("Announce what structure the robot built");
            outNode.WatchedNode = robotBuiltStructure;
            outNode.BehaviourType = typeof(CommentRobotBuiltStructure);

            var robotUpgradedStructure = new AndLogicNode("Robot upgraded a structure", new List<Node>() { mayorTurnIN, structureUpgrade });
            logicWeb.Web.Add(robotUpgradedStructure);

            outNode = logicFrame.AddOutput("Announce what structure the robot upgraded a structure");
            outNode.WatchedNode = robotUpgradedStructure;
            outNode.BehaviourType = typeof(CommentRobotUpgrade);

            var robotImplementPolicy = new AndLogicNode("Robot implement a policy", new List<Node>() { mayorTurnIN, policyImplemented });
            logicWeb.Web.Add(robotImplementPolicy);

            outNode = logicFrame.AddOutput("Announce what structure the robot implemented a policy");
            outNode.WatchedNode = robotImplementPolicy;
            outNode.BehaviourType = typeof(CommentRobotPolicy);


            var firstTurnOfFirstSession = new AndLogicNode("", new List<Node>()
            {
                isfirstSession,
                isFirstTurn
            });
            logicWeb.Web.Add(firstTurnOfFirstSession);

            var notFirstTurnOfFirstSession = new NotLogicNode("", firstTurnOfFirstSession);
            logicWeb.Web.Add(notFirstTurnOfFirstSession);

            var aiActionPlannedDuringMayorTurn = new AndLogicNode("AI Actions in Mayour turn", new List<Node>() { 
                aiPlannedActions,
                mayorTurnIN,
                notFirstTurnOfFirstSession,
                gameNotYetEnded,
            });
            logicWeb.Web.Add(aiActionPlannedDuringMayorTurn);

            outNode = logicFrame.AddOutput("Play action chose by AI");
            outNode.WatchedNode = aiActionPlannedDuringMayorTurn;
            outNode.BehaviourType = typeof(PerformBestActionForThisTurn);

            outNode = logicFrame.AddOutput("Perform learner model memory events");
            outNode.WatchedNode = learnerModelMemoryEvent;
            outNode.BehaviourType = typeof (PerformMemoryEvent);

            
            var userIdlingAfterGameStart = new AndLogicNode("Users idling after game started", new List<Node>()
            {
                isGameStarted,
                usersIdling,
                gameNotYetEnded
            });
            logicWeb.Web.Add(userIdlingAfterGameStart);

            outNode = logicFrame.AddOutput("Helps users when they're idling");
            outNode.WatchedNode = userIdlingAfterGameStart;
            outNode.BehaviourType = typeof(HelpIdlingUsers);

            outNode = logicFrame.AddOutput("Says wrapups when the game ends for no oil");
            outNode.WatchedNode = gameEndedNoOil;
            outNode.BehaviourType = typeof(PerformWrapup);

            logicWeb.UpdateWeb();
        }
    }
}
