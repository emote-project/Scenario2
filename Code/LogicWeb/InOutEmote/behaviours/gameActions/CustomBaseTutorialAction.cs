using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEnercitiesMessages;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.behaviours.gameActions
{
    public class CustomBaseTutorialAction : Behaviour
    {
        public override void BehaviourTask()
        {
            var client = InOutThalamusClient.GetInstance();

            client.IOPublisher.ConfirmConstruction(StructureType.Suburban, 5,2);
            EndIn3Secs();
        }


        private async void EndIn3Secs()
        {
            await Task.Delay(3000);
            ExecutionEnded();
        }
    }
}
