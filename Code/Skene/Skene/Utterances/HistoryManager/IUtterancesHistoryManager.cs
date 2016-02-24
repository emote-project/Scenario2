using EmoteEvents.ComplexData;

namespace Skene.Utterances.HistoryManager
{
    // Using this interface to make easy to swap from one history manager to another
    interface IUtterancesHistoryManager
    {
        /// <summary>
        /// Add an utterance to the history of used utterances
        /// </summary>
        /// <param name="utteranceThalamusId">The ID identifing they utterance in the Thalamus Messages. It differs from utterance.ID which identifies the Utterance inside its library</param>
        /// <param name="u">the utterance to be added to history</param>
        void AddToHistory(string utteranceThalamusId, Utterance u);
        bool WasRecentlyUsed(Utterance u);
        bool WasEverUsed(Utterance u);
    }
}