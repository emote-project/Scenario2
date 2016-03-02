package uk.ac.hw.emote.intman.listener;

import org.eclipse.jetty.util.log.Log;

import uk.ac.hw.emote.intman.dm.InteractionManager;
import uk.ac.hw.emote.intman.dm.TurnTakingManager;
import emotecommonmessages.IEmoteActions;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONTokener;

public class EmoteListener implements IEmoteActions {

	boolean firstTimeStart = true;
	
	
	@Override
	public void Start(String startInfo) {
		Log.getLog ().info (
                "START MESSAGE RECEIVED");
		
		if (firstTimeStart){
			try {
				InteractionManager.getInstance().setTraining(true);
				InteractionManager.getInstance().setMaxEpisodeTurns(300);
				InteractionManager.getInstance().setMaxEpisodes(2000);
				
				TurnTakingManager.getInstance ().init (startInfo);
			} catch (Exception e){
				e.printStackTrace();
			}
			firstTimeStart = false;
		}
	}
	
	
	@Override
	public void Stop() {
		Log.getLog ().info (
                "STOP MESSAGE RECEIVED");
		try {
			TurnTakingManager.getInstance ().stop();
			InteractionManager.getInstance ().stop();
		} catch (Exception e){
			e.printStackTrace();
		}
	}

	@Override
	public void Reset() {
		// TODO Auto-generated method stub
        //TurnTakingManager.getInstance ().init ();
	}

	
	@Override
	public void SetLearnerInfo(String learnerInfo) {
		Log.getLog ().info (
                "Learner model message: " + learnerInfo);
		//TurnTakingManager.getInstance ().setLearnerInfo(learnerInfo);
	}



	

}
