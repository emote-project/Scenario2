using CookComputing.XmlRpc;
using EmoteEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnerModelThalamus
{
    internal class JavaThalamusEventHandler : XmlRpcListenerService, IJavaInterface
    {
        LearnerModelClient client;
        public JavaThalamusEventHandler(LearnerModelClient client)
        {
            this.client = client;
        }

        [XmlRpcMethod()]
        public void learnerModelValueUpdateAfterAffectPerceptionUpdate(string LearnerStateInfo_learnerState, string AffectPerceptionInfo_AffectiveStates)
        {
            client.LMPublisher.learnerModelValueUpdateAfterAffectPerceptionUpdate( LearnerStateInfo_learnerState , AffectPerceptionInfo_AffectiveStates);
        }

        [XmlRpcMethod()]
        public void learnerModelValueUpdateBeforeAffectPerceptionUpdate(string LearnerStateInfo_learnerState)
        {
            EmoteEvents.LearnerStateInfo lsi = EmoteEvents.LearnerStateInfo.DeserializeFromJson(LearnerStateInfo_learnerState);
              Console.WriteLine("Sending Learner State S1 Before Affect Update, learnerId:"+lsi.learnerId+", stepId:"+lsi.stepId+" sessionId:"+lsi.sessionId );
            if (lsi.competencyItems != null)
            {
                foreach (EmoteEvents.LearnerStateInfo.CompetencyItem ci in lsi.competencyItems)
                {
                    Console.WriteLine("Item:"+ci.competencyName+", Type:"+ci.competencyType+", Correct:"+ci.competencyCorrect+" , Value:"+ci.comptencyValue+ " , Actual:"+ci.competencyActual+" Expected:"+ci.competencyExpected);
                }
            }
            client.LMPublisher.learnerModelValueUpdateBeforeAffectPerceptionUpdate(LearnerStateInfo_learnerState);
        }



        [XmlRpcMethod()]
        public void allLearnerInfo(string[] LearnerInfo_learnerInfos)
        {
            client.LMPublisher.allLearnerInfo(LearnerInfo_learnerInfos);
            client.updateAllLearnerInfo(LearnerInfo_learnerInfos);
        }

        [XmlRpcMethod()]
        public void nextThalamusId(int participantId)
        {
            client.LMPublisher.nextThalamusId(participantId);
        }

        [XmlRpcMethod()]
        public void learnerModelValueUpdate(string LearnerStateInfo_learnerState)
        {
            EmoteEvents.LearnerStateInfo lsi = EmoteEvents.LearnerStateInfo.DeserializeFromJson(LearnerStateInfo_learnerState);
            Console.WriteLine("Sending Learner State, learnerId:" + lsi.learnerId + ", stepId:" + lsi.stepId + " sessionId:" + lsi.sessionId);
            if (lsi.competencyItems != null)
            {
                foreach (EmoteEvents.LearnerStateInfo.CompetencyItem ci in lsi.competencyItems)
                {
                    Console.WriteLine("Item:" + ci.competencyName + ", Type:" + ci.competencyType + ", Correct:" + ci.competencyCorrect + " , Value:" + ci.comptencyValue + " , Actual:" + ci.competencyActual + " Expected:" + ci.competencyExpected);
                }
            }
            client.LMPublisher.learnerModelValueUpdate(LearnerStateInfo_learnerState);
        }

          [XmlRpcMethod()]
        public void allUtterancesForParticipant(int participantId, string[] Utterance_utterances)
        {
            Console.WriteLine("Got Utterance list for learner:" + participantId);
            foreach (string Utterance_utterance in Utterance_utterances)
            {
                EmoteEvents.UtteranceHistoryItem u = EmoteEvents.UtteranceHistoryItem.DeserializeFromJson(Utterance_utterance);
                Console.WriteLine("Utterance ID"+u.utteranceId+"Details:" + u.utterance);
            }

            client.LMPublisher.allUtterancesForParticipant(participantId, Utterance_utterances);
        }

        [XmlRpcMethod()]
          public void learnerModelMemoryEvent(string MemoryEvent_memoryEvent)
          {

              Console.WriteLine("Got Memory Event From Java");
              EmoteEvents.MemoryEvent me = EmoteEvents.MemoryEvent.DeserializeFromJson(MemoryEvent_memoryEvent);
              Console.WriteLine("Reason:"+me.reasonForUpdate);
              if (me.memoryEventItems != null)
              {
                  foreach (EmoteEvents.MemoryEvent.MemoryEventItem mei in me.memoryEventItems)
                  {
                      Console.WriteLine("Name:" + mei.name + ", Category:" + mei.category + ", SubCategory:" + mei.subcategory + ", TagNames:" + string.Join(", ", mei.tagNames.Select(v => v.ToString())) + ", TagValues" + string.Join(", ", mei.tagValues.Select(v => v.ToString())));
                  }
              }

              client.LMPublisher.learnerModelMemoryEvent(MemoryEvent_memoryEvent);
          }
    }
}
