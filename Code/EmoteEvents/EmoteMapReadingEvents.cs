using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmoteMapReadingEvents
{
    public interface IGameStateEvents : Thalamus.IPerception
    {
        void GameState(Boolean running);
    }

    //Events are triggered by user action
    public interface IMapEvents : Thalamus.IPerception
    {
        void DragTo();
        void Select(double lat,double lon, string symbolName);
        void CompassShow();
        void CompassHide();
        void DistanceShow();
        void DistanceHide();
        void DistanceToolHasStarted();
        void DistanceToolHasEnded();
        void DistanceToolHasReset();
        void MapKeyShow();
        void MapKeyHide();
        //To enable "When a textbox shows up on the screen, the robot should look at it." 
        void TextShownOnScreen();
    }

   // public interface IDialogueEvents : Thalamus.IPerception
   // {
      //  void RequestNextInstruction();
     //   void RequestRepeatInstruction();
     //   void RequestHelp();
      //  void RequestTutorial();
     //   void RespondWithInfo();
  //  }

    public interface ITaskEvents : Thalamus.IPerception {

     ///This event is triggered when a learner does an action that can be interpreted as an atempt to answer the step. The system will evaluate the answer and broadcast if the attempt was successful and an update of the learner model. 
       //name, correct, actual, expected. 
        void stepAnswerAttempt(int learnerId, int stepId, int activityId, int scenarioId, int sessionId, Boolean correct, String[] competencyName, Boolean[] competencyCorrect, String[] competencyActual, String[] competencyExpected, String mapEventId, String[] comptencyValue, String[] competencyConfidence, String[] oldCompetencyValue, String[] competencyDelta);
        
        //old method.
        // void stepAnswerAttempt(int stepId, Boolean correct, Boolean distanceCorrect, Boolean directionCorrect, Boolean symbolCorrect, Boolean toolCorrect, String actualTool, String actualSymbol, String actualDistance, String actualDirection);

        //If an action on the task is appropriate, is the action appropriate, ontask, off task, help seeking...
        void interactionEvaluation(int learnerId, int stepId,int activityId,int scenarioId,int sessionId,String action, String strategy, String evaluation);
      
    }



    public interface IFMLActions : Thalamus.IAction
    {
        void Greet(string playerName);
        void CompassTool(bool visible);
        void DistanceTool(bool visible);
        void MapKeyTool(bool visible);
    }


    //these are triggered by the AI/WoZ/BehaviourManager and cause actions on the map
    public interface IMapActions : Thalamus.IAction
    {
        void StartNextStep();
        void StartStep(int stepId);
        void CompassShow();
        void CompassHide();
        void CompassHighlight();
        void DistanceShow();
        void DistanceHide();
        void DistanceHighlight();
        void DistanceToolStart();
        void DistanceToolEnd();
        void DistanceToolReset();
        void MapKeyShow();
        void MapKeyHide();
        void MapKeyHighlight();
        void ToolAction(String toolName, String toolAction);
        void CompassHighlightDirection(String direction);
        void BlockUI();
        void UnBlockUI();
        void GiveQuestionnaire(String name);
        void HighlightRightAnswer();
    }
}
