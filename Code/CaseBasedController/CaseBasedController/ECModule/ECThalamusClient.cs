using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteCommonMessages;
using EmoteEnercitiesMessages;
using Thalamus;

namespace ECModule
{
    interface IECPublisher :
        IThalamusPublisher, IEmotionalClimate
    {}

    interface IECThalamusClient : IThalamusClient, 
        IScenario2Perception
    {}

    class ECThalamusClient : ThalamusClient, IECThalamusClient
    {
        private EmotionalClimateForm _form;
        public ECPublisher ECPublisher { get; set; }


        public ECThalamusClient(EmotionalClimateForm form, string character = "", string clientName = "Emotional Climate Module")
            : base(clientName, character)
        {
            SetPublisher<IECPublisher>();
            ECPublisher = new ECPublisher(Publisher);
            _form = form;
        }

        public void PerceptionLog(double time, double faceUpDownDegrees, double faceLeftRightDegrees, double eyesUpdown,
            double eyesLeftRight, double headPositionY, double headPositionX, double closeRatioLeftEye,
            double closeRatioRightEye, double smile, double confidence, double anger, double disgust, double fear, double joy,
            double sadness, double surprise, double neutral, double gazeVectorX, double gazeVectorY, string gazeDirection,
            string subject)
        {
            var perc = new OKAOScenario2Perception(time, faceUpDownDegrees, faceLeftRightDegrees, eyesUpdown,
                eyesLeftRight, headPositionY, headPositionX, closeRatioLeftEye, closeRatioRightEye, smile, confidence,
                anger, disgust, fear, joy, sadness, surprise, neutral, gazeVectorX, gazeVectorY, gazeDirection, subject);
            _form.UpdatePerception(perc);
        }
    }

    internal class ECPublisher : IECPublisher
    {
        private readonly dynamic _publisher;

        public ECPublisher(dynamic publisher)
        {
            this._publisher = publisher;
        }

        public void EmotionalClimateLevel(EmotionalClimateLevel level)
        {
            _publisher.EmotionalClimateLevel(level);
        }
    }
}
