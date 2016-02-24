using System;
using EmoteEnercitiesMessages;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.behaviours.utterances
{
    public class AnnounceCurrentPlayer : Behaviour
    {
        private string _id;
        private EnercitiesRole _currentPlayerRole;

        public AnnounceCurrentPlayer()
        {
            var gameState = GameState.GetInstance();
            _currentPlayerRole = gameState.CurrentState.CurrentPlayer.Role;
        }

        public override void BehaviourTask()
        {
            InOutThalamusClient client = InOutThalamusClient.GetInstance();
            client.UtteranceFinishedEvent += client_UtteranceFinishedEvent;


            _id = "acp" + DateTime.Now.Ticks;
            var tags = new[] {""};
            var values = new[] { "" };

            if (_currentPlayerRole == EnercitiesRole.Mayor)
            {
                client.IOPublisher.PerformUtteranceFromLibrary(_id, UtterancesMapping.TURNCHANGED_SELF.Key,
                    UtterancesMapping.TURNCHANGED_SELF.Value, tags, values);
            }
            else
            {
                client.IOPublisher.PerformUtteranceFromLibrary(_id, UtterancesMapping.TURNCHANGED_OTHER.Key,
                    UtterancesMapping.TURNCHANGED_SELF.Value, tags, values);
            }
            Console.WriteLine("Announce current player: id "+_id);
        }

        void client_UtteranceFinishedEvent(object sender, IFMLUtteranceEventArgs e)
        {
            if (_id != null && e.Id.Equals(_id))
            {
                ExecutionEnded();
                Console.WriteLine("Announce current player Execution ended: id " + _id);
            }
        }
    }
}
