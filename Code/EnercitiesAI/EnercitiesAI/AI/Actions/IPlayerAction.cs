using EmoteEnercitiesMessages;
using EmoteEvents;

namespace EnercitiesAI.AI.Actions
{
    public interface IPlayerAction
    {
        EnercitiesActionInfo ToEnercitiesActionInfo();
        ActionType Type { get; }
    }
}