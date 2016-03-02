package uk.ac.hw.emote.intman.listener;

import org.eclipse.jetty.util.log.Log;
import org.json.JSONException;
import org.json.JSONObject;

import redstone.xmlrpc.XmlRpcArray;
import uk.ac.hw.emote.intman.dm.InteractionManager;
import uk.ac.hw.emote.intman.dm.TurnTakingManager;
import emotemapreadingevents.IMapEvents;
import emotemapreadingevents.ITaskEvents;

public class MapApplicationListener implements IMapEvents, ITaskEvents {

    @Override
    public void DragTo () {
        System.out.println ("DragTo");
    }

    @Override
    public void Select (double lat, double lon, String symbolName) {
        System.out.println ("select" + lat + " " + lon );
    }

    @Override
    public void CompassShow () {
        System.out.println ("CompassShow ");
        JSONObject systemInput = new JSONObject ();
        try {
            systemInput.put ("fromModule", "map");
            systemInput.put ("communicativeFunction", "toolUsed");
            systemInput.put ("dirToolUsed", "true");
            Log.getLog ().info ("MapApplicationListener: " + systemInput.toString());
            TurnTakingManager.getInstance ().processInputAndPerform (systemInput);

        } catch (JSONException e) {
            Log.getLog ().warn (e);
        }
    }

    @Override
    public void CompassHide () {
        System.out.println ("CompassHide ");
    }

    @Override
    public void DistanceShow () {
        System.out.println ("DistanceShow");
    }

    @Override
    public void DistanceHide () {
        System.out.println ("DistanceHide");
    }

    @Override
    public void DistanceToolHasStarted () {
        System.out.println ("DistanceToolHasStarted");
        JSONObject systemInput = new JSONObject ();
        try {
            systemInput.put ("fromModule", "map");
            systemInput.put ("communicativeFunction", "toolUsed");
            systemInput.put ("disToolUsed", "true");
            Log.getLog ().info ("MapApplicationListener: " + systemInput.toString());
            TurnTakingManager.getInstance ().processInputAndPerform (systemInput);

        } catch (JSONException e) {
            Log.getLog ().warn (e);
        }
    }

    @Override
    public void DistanceToolHasEnded () {
        System.out.println ("DistanceToolHasEnded");
    }

    @Override
    public void DistanceToolHasReset () {
        System.out.println ("DistanceToolHasReset");
    }

    @Override
    public void MapKeyShow () {
        System.out.println ("MapKeyShow");
        JSONObject systemInput = new JSONObject ();
        try {
            systemInput.put ("fromModule", "map");
            systemInput.put ("communicativeFunction", "toolUsed");
            systemInput.put ("symToolUsed", "true");
            Log.getLog ().info ("MapApplicationListener: " + systemInput.toString());
            TurnTakingManager.getInstance ().processInputAndPerform (systemInput);

        } catch (JSONException e) {
            Log.getLog ().warn (e);
        }
    }

    @Override
    public void MapKeyHide () {
        System.out.println ("MapKeyHide");
    }

    @Override
    public void TextShownOnScreen () {
        System.out.println ("**************TextShownOnScreen*********************");
        TurnTakingManager.getInstance ().setMapReady(true);
    }

    @Override
    public void interactionEvaluation (int learnerId, int stepId, int activityId, int scenarioId,
            int sessionId, String action, String strategy, String evaluation) {
        // TODO Auto-generated method stub

    }

    @Override
    public void stepAnswerAttempt (int learnerId, String learnerName, int stepId, int activityId, int scenarioId,
            int sessionId, boolean correct, XmlRpcArray competencyName,
            XmlRpcArray competencyCorrect, XmlRpcArray competencyActual,
            XmlRpcArray competencyExpected, XmlRpcArray competencySkillLevel, String mapEventId) {

        Log.getLog ().info (
                "stepAnswerAttempt (" + learnerId + "," + stepId + "," + activityId + ","
                        + scenarioId + "," + sessionId + "," + correct + "," + competencyName + ","
                        + competencyCorrect + "," + competencyActual + "," + competencyExpected
                        + "," + mapEventId);
        
        if (stepId == 9 && !correct) {
            Log.getLog ().warn ("Ignoring incorrect stepAnswerAttempt for step 9");
            return;
        }

        
                
        TurnTakingManager.getInstance ().cancelTimeout ();

        /*
        if (correct) {
            try {
                // happy
                JSONObject affect = new JSONObject ("{fromModule:affect,confidence:800.0}");
                affect.put ("valence", "positive");
                affect.put ("arousal", "positive");
                InteractionManager.getInstance ().processInput (affect);
            } catch (JSONException e) {
                Log.getLog ().warn ("Unable to process happy affect", e);
            }
        }
        */

        JSONObject systemInput = new JSONObject ();
        try {
            systemInput.put ("fromModule", "user");
            systemInput.put ("learnerName", learnerName);
            systemInput.put ("communicativeFunction", "answerTask");
            systemInput.put ("currentStepId", String.valueOf (stepId));
            systemInput.put ("responseCorrect", String.valueOf (correct));
            String bestSkillSoFar = "null";
            Double bestSkillScoreSoFar = 0.0;
            for (int i=0; i < competencyName.size();i++){
            	Log.getLog().info("Competency:" + competencyName.getString(i));
            	if (competencyName.getString(i).equals("symbol")){
            		systemInput.put("symbolCorrect",String.valueOf(competencyCorrect.getBoolean(i)));
            		if (competencySkillLevel.getDouble(i) < 0.33){
            			systemInput.put("symbolSkillLevel", "low");
            		} else if (competencySkillLevel.getDouble(i) < 0.66){
            			systemInput.put("symbolSkillLevel", "medium");
            		} else {
            			systemInput.put("symbolSkillLevel", "high");
            		}
            		if (competencySkillLevel.getDouble(i) > bestSkillScoreSoFar){
            			bestSkillSoFar = "symbol";
            			bestSkillScoreSoFar = competencySkillLevel.getDouble(i);
            		}
            	}
            	else if (competencyName.getString(i).equals("direction")){
            		systemInput.put("directionCorrect",String.valueOf(competencyCorrect.getBoolean(i)));
            		if (competencySkillLevel.getDouble(i) < 0.33){
            			systemInput.put("directionSkillLevel", "low");
            		} else if (competencySkillLevel.getDouble(i) < 0.66){
            			systemInput.put("directionSkillLevel", "medium");
            		} else {
            			systemInput.put("directionSkillLevel", "high");
            		}
            		if (competencySkillLevel.getDouble(i) > bestSkillScoreSoFar){
            			bestSkillSoFar = "direction";
            			bestSkillScoreSoFar = competencySkillLevel.getDouble(i);
            		}
            	}
            	else if (competencyName.getString(i).equals("distance")){
            		systemInput.put("distanceCorrect",String.valueOf(competencyCorrect.getBoolean(i)));
            		if (competencySkillLevel.getDouble(i) < 0.33){
            			systemInput.put("distanceSkillLevel", "low");
            		} else if (competencySkillLevel.getDouble(i) < 0.66){
            			systemInput.put("distanceSkillLevel", "medium");
            		} else {
            			systemInput.put("distanceSkillLevel", "high");
            		}
            		if (competencySkillLevel.getDouble(i) > bestSkillScoreSoFar){
            			bestSkillSoFar = "distance";
            			bestSkillScoreSoFar = competencySkillLevel.getDouble(i);
            		}
                }
            }
            systemInput.put("bestSkillSoFar", bestSkillSoFar);
            Log.getLog ().info ("MapApplicationListener: " + systemInput.toString());
            TurnTakingManager.getInstance ().processInputAndPerform (systemInput);

        } catch (JSONException e) {
            Log.getLog ().warn (e);
        }
    }

}
