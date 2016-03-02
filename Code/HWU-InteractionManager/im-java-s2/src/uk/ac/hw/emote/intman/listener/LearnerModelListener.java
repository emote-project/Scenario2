package uk.ac.hw.emote.intman.listener;

import java.util.NoSuchElementException;

import org.eclipse.jetty.util.log.Log;
import org.json.JSONException;
import org.json.JSONObject;

import redstone.xmlrpc.XmlRpcArray;
import uk.ac.hw.emote.intman.dm.InteractionManager;
import uk.ac.hw.emote.intman.dm.TurnTakingManager;
import emotecommonmessages.ILearnerModelToIMEvents;

public class LearnerModelListener implements ILearnerModelToIMEvents {

    @Override
    public void learnerModelValueUpdateAfterAffectPerceptionUpdate (
            String LearnerStateInfo_learnerState, String AffectPerceptionInfo_AffectiveStates) {

        Log.getLog ().info (
                "Learner model message: " + LearnerStateInfo_learnerState + ","
                        + AffectPerceptionInfo_AffectiveStates);
    }
    
    public void newAffect(int learnerId, XmlRpcArray aStates,
            XmlRpcArray aCharges, XmlRpcArray aConfidences)
    {
    	JSONObject systemInput = new JSONObject ();
    	try {
			systemInput.put ("fromModule", "affect");
			for (int i=0; i < aStates.size();i++){
				if (aStates.getString(i).equals("Arousal")){
					systemInput.put("arousal", aCharges.getString(i));
				}
				else if (aStates.getString(i).equals("Valence")){
					systemInput.put("valence", aCharges.getString(i));
					systemInput.put("confidence", aConfidences.getDouble(i));
				}
			}
			Log.getLog ().info ("LearnerModelListener: " + systemInput.toString());
			TurnTakingManager.getInstance ().processInputAndPerform (systemInput);
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    	
    }

	@Override
	public void LearnerStateInfo(String LearnerStateInfo_learnerState) {
		try {
			//Log.getLog ().info ("Learner state info: " + LearnerStateInfo_learnerState);
			//InteractionManager.getInstance().setRecapInfo(LearnerStateInfo_learnerState);
		} 
		catch (NoSuchElementException e){
			e.printStackTrace();
		}catch (Exception e){
			e.printStackTrace();
		}
	}

}
