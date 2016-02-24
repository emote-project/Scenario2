using System.Collections.Generic;
using System.Linq;
using System.Timers;
using CaseBasedController.GameInfo;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;
using System.Threading.Tasks;

namespace CaseBasedController
{
    class AIGameActionPlayer
    {
        private ControllerClient _client;

        private const int bookmarksTimeoutMilliseconds = 7000;

        /// <summary>
        /// after some times not receiving bookmarks we consider the robot as not speaking
        /// </summary>
        private System.Timers.Timer _resetSpeakingStatusTimer;
        private object _locker = new object();


        //Game status
        private bool _isSpeaking = false;
        private bool _isIATurn = false;
        private bool _isGameMoveReady = false;
        private int _sessionNumber;
        private bool _alreadyPlayedThisTurn = false;
        private static AIGameActionPlayer _instance;
        private bool _shouldComment;


        public bool IsSpeaking
        {
            get { return _isSpeaking; }
            set
            {
                _isSpeaking = value;
                CheckStatus();
            }
        }

        public bool IsIaTurn
        {
            get { return _isIATurn; }
            set
            {
                _isIATurn = value;
                CheckStatus();
            }
        }

        public bool IsGameMoveReady
        {
            get { return _isGameMoveReady; }
            set { 
                _isGameMoveReady = value; 
                CheckStatus(); 
            }
        }

        public int SessionNumber
        {
            get { return _sessionNumber; }
            set
            {
                _sessionNumber = value;
                CheckStatus();
            }
        }

        public bool AlreadyPlayedThisTurn
        {
            get { return _alreadyPlayedThisTurn; }
            set
            {
                _alreadyPlayedThisTurn = value;
                CheckStatus();
            }
        }

        public static AIGameActionPlayer GetInstance()
        {
            if (_instance == null) _instance = new AIGameActionPlayer();
            return _instance;
        }

        public void Init(ControllerClient client)
        {
            _client = client;
        }

        private AIGameActionPlayer()
        {
            //_client = ControllerClient.GetInstance();
            //_client.TurnChangedEvent += _client_TurnChangedEvent;
            //_client.SpeakBookmarksEvent += _client_SpeakBookmarksEvent;
            //_client.ActionsPlannedEvent += _client_ActionsPlannedEvent;
        }

        
        private void CheckStatus()
        {
            Logger.Log(string.Format("Checking status: IsGameMoveReady:{0} - IsIaTurn:{1} - IsSpeaking:{2} - sessionNumber:{3} - turnNumber:{4} - alreadyPlayed:{5}", IsGameMoveReady, IsIaTurn, IsSpeaking, SessionNumber,GameStatus.TurnNumber,AlreadyPlayedThisTurn),this);
            if (IsGameMoveReady && IsIaTurn && !IsSpeaking && !(SessionNumber==1 && GameStatus.TurnNumber<=2) && !AlreadyPlayedThisTurn)
            {
                Logger.Log("Playing action <-------------------------------------------------------",this);
                _alreadyPlayedThisTurn = true;
                PlayAction();
            }
        }

        private void PlayAction()
        {
            var action = GameInfo.GameStatus.CurrentState.BestActionsForThisTurn[0];

            // Some shit is going on
            if (action == null)
            {
                _client.ControllerPublisher.SkipTurn();
                Logger.Log("Played SkipTurn ", this);
                return;
            }

            //executes action accordingly
            switch (action.ActionType)
            {
                case ActionType.BuildStructure:
                    var structure = (StructureType)action.SubType;
                    _client.ControllerPublisher.ConfirmConstruction(structure, action.CellX, action.CellY);
                    Logger.Log("Played confirmconstruction " + structure,this);
                    break;
                case ActionType.ImplementPolicy:
                    var policy = (PolicyType)action.SubType;
                    _client.ControllerPublisher.ImplementPolicy(policy);
                    Logger.Log("Played ImplementPolicy " + policy, this);
                    break;
                case ActionType.UpgradeStructure:
                    var upgrade = (UpgradeType)action.SubType;
                    _client.ControllerPublisher.PerformUpgrade(upgrade, action.CellX, action.CellY);
                    Logger.Log("Played PerformUpgrade " + upgrade, this);
                    System.Threading.Thread.Sleep(2000);

                    var actions =
                        GameInfo.GameStatus.CurrentState.BestActionsForThisTurn.Where(x => (x != null)).ToArray();

                    action = actions[1];
                    upgrade = (UpgradeType)action.SubType;
                    _client.ControllerPublisher.PerformUpgrade(upgrade, action.CellX, action.CellY);
                    Logger.Log("Played PerformUpgrade " + upgrade, this);
                    System.Threading.Thread.Sleep(2000);

                    action = actions[2];
                    upgrade = (UpgradeType)action.SubType;
                    Logger.Log("Played PerformUpgrade " + upgrade, this);
                    _client.ControllerPublisher.PerformUpgrade(upgrade, action.CellX, action.CellY);

                    break;
                case ActionType.SkipTurn:
                    _client.ControllerPublisher.SkipTurn();
                    Logger.Log("Played SkipTurn ", this);
                    break;
            }
            _shouldComment = true;
        }



        private static readonly Dictionary<ActionType, string> Categories =
            new Dictionary<ActionType, string>
            {
                {ActionType.BuildStructure, "ConfirmConstruction"},
                {ActionType.UpgradeStructure, "PerformUpgrade"},
                {ActionType.ImplementPolicy, "ImplementPolicy"}
            };


        private async void CommentAction(ActionType actionType, int actionTypeEnum)
        {
            await Task.Delay(500);
            PerformUtterance(Categories[actionType], "self");

            //console.writeline("Explaining: main strategy = "+GameStatus.GetMainStrategy().ToString()+" - Action: "+actionType);

            // If the action and the main strategy used to compute it are clearly linked together, than we perform an utterance that explains this link
            switch (actionType)
            {
                case ActionType.BuildStructure:
                    StructureType structureType = (StructureType)actionTypeEnum;
                    //console.writeline("Structure: "+structureType.ToString());

                    // ############ EXPLAIN BUILDING FOR POPULATION 
                    if (
                        (structureType == StructureType.Suburban ||
                        structureType == StructureType.Urban ||
                        structureType == StructureType.Residential_Tower) &&
                        (GameStatus.GetMainStrategy() == GameStatus.AiActionStrategy.Population)
                        )
                    {
                        PerformUtterance("Explanation", "Population");
                    }

                    // ############ EXPLAIN BUILDING FOR ENERGY 
                    if (
                        (structureType == StructureType.Coal_Plant_Small ||
                        structureType == StructureType.Coal_Plant ||
                        structureType == StructureType.Hydro_Plant ||
                        structureType == StructureType.Nuclear_Fusion ||
                        structureType == StructureType.Nuclear_Plant ||
                        structureType == StructureType.Solar_Plant ||
                        structureType == StructureType.Super_Solar ||
                        structureType == StructureType.Super_WindTurbine ||
                        structureType == StructureType.Windmills) &&
                        (GameStatus.GetMainStrategy() == GameStatus.AiActionStrategy.Power)
                        )
                    {
                        PerformUtterance("Explanation", "Energy");
                    }

                    // ############ EXPLAIN BUILDING FOR WELLBEING 
                    if (
                        (structureType == StructureType.Market ||
                        structureType == StructureType.Public_Services ||
                        structureType == StructureType.Stadium ||
                        structureType == StructureType.Commercial) &&
                        (GameStatus.GetMainStrategy() == GameStatus.AiActionStrategy.Wellbeing)
                        )
                    {
                        PerformUtterance("Explanation", "Wellbeing");
                    }

                    break;
            }
        }


        private string PerformUtterance(string category, string subcategory)
        {
            var tagsAndValues = GameInfo.GameStatus.GetTagNamesAndValues();
            var uttId = "cbc" + new System.Random().Next();
            string[] tags = new List<string>(tagsAndValues.Keys).ToArray();
            string[] values = new List<string>(tagsAndValues.Values).ToArray();
            _client.ControllerPublisher.PerformUtteranceFromLibrary(uttId, category, subcategory, tags, values);

            //console.writeline(">>>>>>>>>>>>>> PERFORMING: " + category + ", " + subcategory);

            return uttId;

        }









        public void _client_ActionsPlannedEvent(object sender, IAEventArgs e)
        {
            IsGameMoveReady = e.CurrentPlayer == EnercitiesRole.Mayor;
        }


        public void _client_SpeakBookmarksEvent(object sender, SpeechEventArgs e)
        {
            lock (_locker)
            {
                IsSpeaking = true;
                if (_resetSpeakingStatusTimer == null)
                {
                    _resetSpeakingStatusTimer = new Timer(bookmarksTimeoutMilliseconds);
                    _resetSpeakingStatusTimer.AutoReset = false;
                    _resetSpeakingStatusTimer.Elapsed += delegate(object o, ElapsedEventArgs args)
                    {
                        Logger.Log("is speaking = false ##########################################################",
                            this);
                        IsSpeaking = false;
                    };
                }
                _resetSpeakingStatusTimer.Stop();
                _resetSpeakingStatusTimer.Start();
            }
        }

        public void _client_TurnChangedEvent(object sender, GenericGameEventArgs e)
        {
            Logger.Log("TurnChanged",this);
            IsIaTurn = e.GameState.CurrentRole == EnercitiesRole.Mayor;
            SessionNumber = GameStatus.Session;
            AlreadyPlayedThisTurn = false;
        }


        public void client_ConfirmConstructionEvent(ControllerClient controllerClient, GameActionEventArgs eventArgs)
        {
            if (_shouldComment)
            {
                _shouldComment = false;
                CommentAction(ActionType.BuildStructure,eventArgs.ActionTypeEnum);
            }
        }

        public void client_PerformUpgradeEvent(ControllerClient controllerClient, GameActionEventArgs eventArgs)
        {
            if (_shouldComment)
            {
                _shouldComment = false;
                CommentAction(ActionType.UpgradeStructure, eventArgs.ActionTypeEnum);
            }
        }

        public void client_ImplementPolicyEvent(ControllerClient controllerClient, GameActionEventArgs eventArgs)
        {
            if (_shouldComment)
            {
                _shouldComment = false;
                CommentAction(ActionType.ImplementPolicy, eventArgs.ActionTypeEnum);
            }
        }
    }
}
