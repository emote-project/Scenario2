using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thalamus;
using CookComputing.XmlRpc;
using EmoteEvents;


namespace IntManInterface
{
    public interface IIntManInterface : /*EmoteMapReadingEvents.ITaskEvents,*/ EmoteCommonMessages.IFMLSpeechEvents,
        EmoteMapReadingEvents.IMapEvents, EmoteCommonMessages.ILearnerModelToIMEvents,
        EmoteEnercitiesMessages.IEnercitiesTaskEvents, EmoteEnercitiesMessages.IEnercitiesAIActions, EmoteEnercitiesMessages.IEnercitiesGameStateEvents,
        EmoteCommonMessages.IEmoteActions, EmoteCommonMessages.ILearnerModelToS2IMPerceptionEvents /*, EmoteCommonMessages.IMapEvents*/
    { }

    public class IntManInterfaceClient : ThalamusClient, IIntManInterface
    {
        internal IntManInterfacePublisher publisher;
        //private bool bFirstTimeMapClick = false;
        public IntManInterfaceClient() : base("InteractionManager")
        {
            SetPublisher<IIntManInterfacePublisher>();
            publisher = new IntManInterfacePublisher(Publisher);
        }

        // Just pass all messages received to the Java implementation through XMLRPC
        [XmlRpcUrl("http://localhost:11000/intman/")]
        public interface IIntManJavaProxy : IXmlRpcProxy
        {
            /*** Map interface events (processed) **/

            [XmlRpcMethod("map.interactionEvaluation")]
            void interactionEvaluation(int learnerId, int stepId, int activityId, int scenarioId, int sessionId, string action, string strategy, string evaluation);

            [XmlRpcMethod("map.stepAnswerAttempt")]
            void stepAnswerAttempt(int learnerId, string learnerName, int stepId, int activityId, int scenarioId, int sessionId, bool correct, string[] competencyName, bool[] competencyCorrect, string[] competencyActual, string[] competencyExpected, double[] competencySkillLevels, String mapEventId);

            /*** LM events (processed) **/
            [XmlRpcMethod("lm.newAffect")]
            void newAffect(int learnerId, string[] aStates, string[] aCharges, double[] aConfidences);

            [XmlRpcMethod("lm.LearnerStateInfo")]
            void LearnerStateInfo(string LearnerStateInfo_learnerState);


            /*** Output monitoring events **/

            [XmlRpcMethod("skene.UtteranceStarted")]
            void UtteranceStarted(string id);

            [XmlRpcMethod("skene.UtteranceFinished")]
            void UtteranceFinished(string id);

            
            /*** Perception events **/

            [XmlRpcMethod("perception.EyebrowsAU")]
            void EyebrowsAU(double au2_user1, double au4_user1, double au2_user2, double au4_user2);

            [XmlRpcMethod("perception.EyebrowsAU2")]
            void EyebrowsAU2(double au4left_user1, double au4right_user1, double au4left_user2, double au4right_user2);

            [XmlRpcMethod("perception.GazeOKAO")]
            void GazeOKAO(int userID, bool GazeAtrobot);

            [XmlRpcMethod("perception.GazeTracking")]
            void GazeTracking(int userID, string direction);

            [XmlRpcMethod("perception.HeadTracking")]
            void HeadTracking(int userID, double X, double Y, double Z, bool DetectedSkeleton);

            [XmlRpcMethod("perception.PointingPosition")]
            void PointingPosition(int userID, string hand, double X, double Y, double Z);

            [XmlRpcMethod("perception.UserMutualGaze")]
            void UserMutualGaze(bool value);

            [XmlRpcMethod("perception.UserMutualPoint")]
            void UserMutualPoint(bool value, double avegX, double avegY);

            [XmlRpcMethod("perception.UserTouchChin")]
            void UserTouchChin(int userID, bool value);

            [XmlRpcMethod("perception.Smile")]
            void Smile(int userID, int smileVal, int confidenceVal);
            
            
            /*** Map interface events (raw) **/

            [XmlRpcMethod("map.CompassHide")]
            void CompassHide();

            [XmlRpcMethod("map.CompassShow")]
            void CompassShow();

            [XmlRpcMethod("map.DistanceHide")]
            void DistanceHide();

            [XmlRpcMethod("map.DistanceShow")]
            void DistanceShow();

            [XmlRpcMethod("map.DistanceToolHasEnded")]
            void DistanceToolHasEnded();

            [XmlRpcMethod("map.DistanceToolHasReset")]
            void DistanceToolHasReset();

            [XmlRpcMethod("map.DistanceToolHasStarted")]
            void DistanceToolHasStarted();

            [XmlRpcMethod("map.DragTo")]
            void DragTo();

            [XmlRpcMethod("map.MapKeyHide")]
            void MapKeyHide();

            [XmlRpcMethod("map.MapKeyShow")]
            void MapKeyShow();

            [XmlRpcMethod("map.Select")]
            void Select(double lat, double lon, string symbolName);

            [XmlRpcMethod("map.TextShownOnScreen")]
            void TextShownOnScreen();

            /** Enercities event notifications */

            [XmlRpcMethod("enercities.ConfirmConstruction")]
            void ConfirmConstruction(string p, string translation, int cellX, int cellY);

            [XmlRpcMethod("enercities.ImplementPolicy")]
            void ImplementPolicy(string p, string translation);

            [XmlRpcMethod("enercities.PerformUpgrade")]
            void PerformUpgrade(string p, string translation, int cellX, int cellY);

            [XmlRpcMethod("enercities.SkipTurn")]
            void SkipTurn();

            [XmlRpcMethod("enercities.ExamineCell")]
            void ExamineCell(double screenX, double screenY, int cellX, int cellY, String StructureType_structure, String StructureType_translated);

            [XmlRpcMethod("enercities.BestActionsPlanned")]
            void BestActionsPlanned(string[] EnercitiesActionInfo_actionInfos);

            [XmlRpcMethod("enercities.BestActionPlanned")]
            void BestActionPlanned(string[] EnercitiesActionInfo_actionInfos);

            [XmlRpcMethod("enercities.TurnChanged")]
            void TurnChanged(string serializedGameState);

            [XmlRpcMethod("enercities.ReachedNewLevel")]
            void ReachedNewLevel(int level);

           

            [XmlRpcMethod("enercities.GameStarted")]
            void GameStarted(string player1Name, string player1Role, string player2Name, string player2Role);

            /** Emote messages */
            [XmlRpcMethod("emote.Start")]
            void Start(string StartMessageInfo_info);

            [XmlRpcMethod("emote.Stop")]
            void Stop();

            [XmlRpcMethod("emote.Reset")]
            void Reset();

            [XmlRpcMethod("emote.SetLearnerInfo")]
            void SetLearnerInfo(String learnerInfo);

          
            
        }

        internal IIntManJavaProxy javaProxy = XmlRpcProxyGen.Create<IIntManJavaProxy>();

        void EmoteCommonMessages.IFMLSpeechEvents.UtteranceStarted(string id)
        {
            javaProxy.UtteranceStarted(id);
        }

        void EmoteCommonMessages.IFMLSpeechEvents.UtteranceFinished(string id)
        {
            javaProxy.UtteranceFinished(id);
        }

        override public void Dispose()
        {
            publisher.shutDown();
        }

        /****/
        /** Perception messages **/
        /****/

        /*
        void EmoteCommonMessages.IPerceptionEvents.EyebrowsAU(double au2_user1, double au4_user1, double au2_user2, double au4_user2)
        {
            javaProxy.EyebrowsAU(au2_user1, au4_user1, au2_user2, au4_user2);
        }

        void EmoteCommonMessages.IPerceptionEvents.EyebrowsAU2(double au4left_user1, double au4right_user1, double au4left_user2, double au4right_user2)
        {
            javaProxy.EyebrowsAU2(au4left_user1, au4right_user1, au4left_user2, au4right_user2);
        }

        void EmoteCommonMessages.IPerceptionEvents.HeadTracking(int userID, double X, double Y, double Z, bool DetectedSkeleton)
        {
            javaProxy.HeadTracking(userID, X, Y, Z, DetectedSkeleton);
        }

        void EmoteCommonMessages.IPerceptionEvents.PointingPosition(int userID, EmoteCommonMessages.Hand hand, double X, double Y, double Z)
        {
            javaProxy.PointingPosition(userID, hand.ToString(), X, Y, Z);
        }

        void EmoteCommonMessages.IPerceptionEvents.UserMutualGaze(bool value)
        {
            javaProxy.UserMutualGaze(value);
        }

        void EmoteCommonMessages.IPerceptionEvents.UserMutualPoint(bool value, double avegX, double avegY)
        {
            javaProxy.UserMutualPoint(value, avegX, avegY);
        }

        void EmoteCommonMessages.IPerceptionEvents.UserTouchChin(int userID, bool value)
        {
            javaProxy.UserTouchChin(userID, value);
        }


        void EmoteCommonMessages.IPerceptionEvents.Smile(int UserID, int smileVal, int confienceVal)
        {
            javaProxy.Smile(UserID, smileVal, confienceVal);
        }
        */

        void EmoteMapReadingEvents.IMapEvents.CompassHide()
        {
            javaProxy.CompassHide();
        }

        void EmoteMapReadingEvents.IMapEvents.CompassShow()
        {
            javaProxy.CompassShow();
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceHide()
        {
            javaProxy.DistanceHide();
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceShow()
        {
            javaProxy.DistanceShow();
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceToolHasEnded()
        {
            javaProxy.DistanceToolHasEnded();
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceToolHasReset()
        {
            javaProxy.DistanceToolHasReset();
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceToolHasStarted()
        {
            javaProxy.DistanceToolHasStarted();
        }

        void EmoteMapReadingEvents.IMapEvents.DragTo()
        {
            javaProxy.DragTo();
        }

        void EmoteMapReadingEvents.IMapEvents.MapKeyHide()
        {
            javaProxy.MapKeyHide();
        }

        void EmoteMapReadingEvents.IMapEvents.MapKeyShow()
        {
            javaProxy.MapKeyShow();
        }

        void EmoteMapReadingEvents.IMapEvents.Select(double lat, double lon, string symbolName)
        {
            javaProxy.Select(lat, lon, symbolName);
        }

        void EmoteMapReadingEvents.IMapEvents.TextShownOnScreen()
        {
            javaProxy.TextShownOnScreen();
        }

      
        void EmoteCommonMessages.ILearnerModelToIMEvents.learnerModelValueUpdateAfterAffectPerceptionUpdate(string LearnerStateInfo_learnerState, string AffectPerceptionInfo_AffectiveStates)
        {

            AffectPerceptionInfo api = AffectPerceptionInfo.DeserializeFromJson(AffectPerceptionInfo_AffectiveStates);
            LearnerStateInfo lsi = LearnerStateInfo.DeserializeFromJson(LearnerStateInfo_learnerState);



            Console.WriteLine("Reason for update:: " + lsi.reasonForUpdate.ToString());
            if (lsi.reasonForUpdate.ToString().Equals("AffectUpdate")){
                String[] aStates = new String[] { "null", "null", "null", "null", "null" };
                String[] aCharges = new String[] { "null", "null", "null", "null", "null" };
                Double[] aConfidences = new Double[] {0.0, 0.0, 0.0, 0.0, 0.0 };
                int j = 0;
                foreach (AffectPerceptionInfo.AffectType a in api.mAffectiveStates)
                {
                   //Console.WriteLine(a.mState + "," + a.mStateCharge + "," + a.mStateConfidence);
                   aStates[j] = a.mState.ToString();
                   aCharges[j] = a.mStateCharge.ToString().ToLower();
                   aConfidences[j] = a.mStateConfidence;
                   j++;
                }
                javaProxy.newAffect(lsi.learnerId, aStates, aCharges, aConfidences);
            }
            else {
                String[] cItems = new String[]{"null","null","null"};
                Boolean[] cItemsCorrect = new Boolean[] { false, false, false };
                String[] cItemsActual = new String[] { "null", "null", "null" };
                String[] cItemsExpected = new String[] { "null", "null", "null" };
                Double[] cItemsSkillLevels = new Double[]{0.0, 0.0, 0.0};
                int i = 0;
                foreach (LearnerStateInfo.CompetencyItem c in lsi.competencyItems){
                    Console.WriteLine(c.competencyName + "," + c.competencyCorrect + "," + c.comptencyValue);
                    
                    cItems[i] = c.competencyName;
                    cItemsCorrect[i] = c.competencyCorrect;
                    cItemsActual[i] = c.competencyActual;
                    cItemsExpected[i] = c.competencyExpected;
                    cItemsSkillLevels[i] = c.comptencyValue;
                    i++;
                }

                
                //Console.WriteLine("stepAnswerAttempt(" + lsi.learnerId + "," + lsi.stepId + "," + lsi.activityId + "," + lsi.scenarioId + "," + lsi.sessionId + "," + lsi.correct + ","
                  //              + cItems + "," + cItemsCorrect + "," + cItemsActual + "," + cItemsExpected + "," + lsi.mapEventId); 
                Console.WriteLine("Learner name:" + lsi.learnerName);
                javaProxy.stepAnswerAttempt(lsi.learnerId, lsi.learnerName, lsi.stepId, lsi.activityId, lsi.scenarioId, lsi.sessionId, lsi.correct, cItems, cItemsCorrect, new String[0], new String[0], cItemsSkillLevels, lsi.mapEventId);
            }
        }


        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.ConfirmConstruction(EmoteEnercitiesMessages.StructureType structure, string translation, int cellX, int cellY)
        {
            Console.WriteLine("Received ConfirmConstruction event method");
            javaProxy.ConfirmConstruction(structure.ToString(), translation, cellX, cellY);
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.ExamineCell(double screenX, double screenY, int cellX, int cellY, EmoteEnercitiesMessages.StructureType StructureType_structure, string StructureType_translated)
        {
            Console.WriteLine("Received ExamineCell event method");
            javaProxy.ExamineCell(screenX, screenY, cellX, cellY, StructureType_structure.ToString().ToLower(), StructureType_translated);
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.ImplementPolicy(EmoteEnercitiesMessages.PolicyType policy, string translation)
        {
            Console.WriteLine("Received ImplementPolicy event method");
            javaProxy.ImplementPolicy(policy.ToString().ToLower(), translation);
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.PerformUpgrade(EmoteEnercitiesMessages.UpgradeType upgrade, string translation, int cellX, int cellY)
        {
            Console.WriteLine("Received PerformUpgrade event method");
            javaProxy.PerformUpgrade(upgrade.ToString().ToLower(), translation, cellX, cellY);
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.SkipTurn()
        {
            Console.WriteLine("Received SkipTurn event method");
            javaProxy.SkipTurn();
        }

        void EmoteEnercitiesMessages.IEnercitiesAIActions.BestActionPlanned(string[] EnercitiesActionInfo_actionInfos)
        {
            Console.WriteLine("Received BestActionPlanned event method");
            javaProxy.BestActionPlanned(EnercitiesActionInfo_actionInfos);
        }

        void EmoteEnercitiesMessages.IEnercitiesAIActions.BestActionsPlanned(EmoteEnercitiesMessages.EnercitiesRole currentPlayer, string[] EnercitiesActionInfo_actionInfos)
        {
            Console.WriteLine("Received BestActionsPlanned event method");
            javaProxy.BestActionsPlanned(EnercitiesActionInfo_actionInfos);
        }

        void EmoteEnercitiesMessages.IEnercitiesAIActions.PredictedValuesUpdated(double[] values)
        {
            Console.WriteLine("Received PredictedValuesUpdated event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesAIActions.StrategiesUpdated(string StrategiesSet_strategies)
        {
            Console.WriteLine("Received EndGameStrategiesUpdated event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.EndGameNoOil(int totalScore)
        {
            Console.WriteLine("Received EndGameNoOil event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.EndGameSuccessfull(int totalScore)
        {
            Console.WriteLine("Received EndGameSuccessful event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.EndGameTimeOut(int totalScore)
        {
            Console.WriteLine("Received EndGameTimeout event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.GameStarted(string player1Name, string player1Role, string player2Name, string player2Role)
        {
            javaProxy.GameStarted(player1Name, player1Role, player2Name, player2Role);
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.PlayersGender(EmoteEnercitiesMessages.Gender player1Gender, EmoteEnercitiesMessages.Gender player2Gender)
        {
            Console.WriteLine("Received PlayersGender event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.PlayersGenderString(string player1Gender, string player2Gender)
        {
            Console.WriteLine("Received PlayersGenderString event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.ReachedNewLevel(int level)
        {
            Console.WriteLine("Received ReachedNewLevel event method");
            javaProxy.ReachedNewLevel(level);
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.ResumeGame(string player1Name, string player1Role, string player2Name, string player2Role, string serializedGameState)
        {
            Console.WriteLine("Received ResumeGame event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.StrategyGameMoves(string environmentalistMove, string economistMove, string mayorMove, string globalMove)
        {
            Console.WriteLine("Received StrategyGameMoves event method");
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.TurnChanged(string serializedGameState)
        {
            Console.WriteLine("Received TurnChanged event method");
            javaProxy.TurnChanged(serializedGameState);
        }





        void EmoteCommonMessages.IEmoteActions.Reset()
        {
            javaProxy.Reset();
        }

       

        void EmoteCommonMessages.IEmoteActions.Stop()
        {
            javaProxy.Stop();
        }


        void EmoteCommonMessages.IEmoteActions.SetLearnerInfo(string LearnerInfo_learnerInfo)
        {
            EmoteEvents.LearnerInfo li = LearnerInfo.DeserializeFromJson(LearnerInfo_learnerInfo);
            
            javaProxy.SetLearnerInfo(li.firstName);
        }

        void EmoteEnercitiesMessages.IEnercitiesAIActions.ActionsPlanned(EmoteEnercitiesMessages.EnercitiesRole currentPlayer, string Strategy_planStrategy, string[] EnercitiesActionInfo_bestActionInfos, string[] EnercitiesActionInfo_worstActionInfos)
        {
            //throw new NotImplementedException();
        }


        void EmoteCommonMessages.IEmoteActions.Start(string StartMessageInfo_info)
        {
            Console.WriteLine("Start message" + StartMessageInfo_info);
                      
            javaProxy.Start(StartMessageInfo_info);

           
        }

        void EmoteCommonMessages.ILearnerModelToS2IMPerceptionEvents.learnerModelValueUpdate(string LearnerStateInfo_learnerState)
        {
            Console.WriteLine("LearnerStateInfo:" + LearnerStateInfo_learnerState);   
            javaProxy.LearnerStateInfo(LearnerStateInfo_learnerState);
        }
        
        
       
    }

}
