using System;
using EmoteCommonMessages;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EmoteEvents.ComplexData;

namespace CaseBasedController.Thalamus
{
    public class EndGameEventArgs : EventArgs
    {
        public int TotalScore { get; set; }
    }

    public class GenericGameEventArgs : EventArgs
    {
        public string Player1Name { get; set; }
        public string Player1Role { get; set; }
        public string Player2Name { get; set; }
        public string Player2Role { get; set; }
        public EnercitiesGameInfo GameState { get; set; }

        public string SerializedGameState
        {
            set
            {
                if (value != null && value != "")
                    this.GameState = EnercitiesGameInfo.DeserializeFromJson(value);
            }
            get
            {
                if (this.GameState != null)
                    return this.GameState.SerializeToJson();
                return null;
            }
        }
    }

    public class PlayersGenderEventArgs : EventArgs
    {
        public Gender Player1Gender { get; set; }
        public Gender Player2Gender { get; set; }
    }

    public class ReachedNewLevelEventArgs : EventArgs
    {
        public int Level { get; set; }
    }

    public class StrategyGameMovesEventArgs : EventArgs
    {
        public string EnvironmentalistMove { get; set; }
        public string EconomistMove { get; set; }
        public string MayorMove { get; set; }
        public string GlobalMove { get; set; }
    }

    public class ExamineCellEventArgs : EventArgs
    {
        public double ScreenX { get; set; }
        public double ScreenY { get; set; }
        public int CellX { get; set; }
        public int CellY { get; set; }
        public StructureType Type { get; set; }
        public string StructureType_translated { get; set; }
    }

    public class GameActionEventArgs : EventArgs
    {
        public int ActionTypeEnum { get; set; }
        public string Translation { get; set; }
        public int CellX { get; set; }
        public int CellY { get; set; }
    }

    public class IAEventArgs : EventArgs
    {
        public string[] EnercitiesActionInfo_bestActionInfos { get; set; }
        public string[] EnercitiesActionInfo_worstActionInfos { get; set; }
        public string Strategy_planStrategy { get; set; }
        public double[] Values { get; set; }
        public string StrategiesSet_strategies { get; set; }
        public EnercitiesRole CurrentPlayer { get; set; }
    }

    public class WordDetectedEventArgs : EventArgs
    {
        public string[] Words { get; set; }
    }

    public class TargetsEventArgs : EventArgs
    {
        public string TargetName { get; set; }
        public string LinkedTargetName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class EyebrowsAUEventArgs : EventArgs
    {
        public double Au2User1 { get; set; }
        public double Au4User1 { get; set; }
        public double Au2User2 { get; set; }
        public double Au4User2 { get; set; }
    }

    public class PerceptionEventArgs : EventArgs
    {
        public int UserID { get; set; }
        public Hand Hand { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool DetectedSkeleton { get; set; }
        public bool Value { get; set; }
        public GazeEnum Direction { get; set; }
        public int ConfidenceVal { get; set; }
    }

    public class ActiveSoundUserEventArgs : EventArgs
    {
        public ActiveUser UserAct { get; set; }
        public double LeftValue { get; set; }
        public double RightValue { get; set; }
    }

    public class SpeechEventArgs : EventArgs
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public string[] Texts { get; set; }
        public string[] Bookmarks { get; set; }
    }

    public class AnimationEventArgs : EventArgs
    {
        public string ID { get; set; }
    }

    public class MenuEventArgs : EventArgs
    {
        public int EnumValue { get; set; }
        public string Translation { get; set; }
    }

    public class PerformUtteranceFromLibraryEventArgs : EventArgs
    {
        public string ID { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string[] TagNames { get; set; }
        public string[] TagValues { get; set; }
    }

    public class PerformUtteranceEventArgs : EventArgs
    {
        public string ID { get; set; }
        public string Category { get; set; }
        public string Utterance { get; set; }
    }

    public class LearnerModelMemoryEventArgs : EventArgs
    {
        public MemoryEvent MemoryEvent { get; set; }
    }
    
    public class OKAOPerceptionArgs : EventArgs
    {
        public OKAOScenario2Perception PerceptionLog { get; set; }
    }


    public class IFMLUtteranceEventArgs : EventArgs
    {
        public string Id { get; set; }
    }

    public class StartEventArgs : EventArgs
    {
        public StartMessageInfo StartMessageInfo { get; set; }
    }
}