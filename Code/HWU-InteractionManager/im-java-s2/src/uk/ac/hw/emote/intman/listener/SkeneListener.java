package uk.ac.hw.emote.intman.listener;

import java.util.Timer;
import java.util.TimerTask;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;
import org.json.JSONException;
import org.json.JSONObject;

import redstone.xmlrpc.XmlRpcArray;
import uk.ac.hw.emote.intman.dm.TurnTakingManager;
import emotecommonmessages.IFMLSpeechEvents;

public class SkeneListener implements IFMLSpeechEvents {
	Timer timer;
	TimerTask tt;
	String uid;
	
	Logger logger = Logger.getLogger(SkeneListener.class.getName());
	
	public SkeneListener(){
		PropertyConfigurator.configure("log4j.properties");
		timer = new Timer (true);
	}
	
    @Override
    public void UtteranceStarted (String id) {
    	if (!id.equals("")){
    		TurnTakingManager.getInstance ().utteranceStarted (id);
    	}
    }

    @Override
    public void UtteranceFinished (String id) {
    	
    	if (id != null && !id.equals("")){
    		TurnTakingManager.getInstance ().utteranceFinished (id);
    		synchronized (timer) {
                if (tt != null) {
                    tt.cancel ();
                    tt = null;
                    logger.info("TTS Timer cancelled!");
                }
            }
    	}
    }

	@Override
	public void SpeakBookmarks(String id, XmlRpcArray text, XmlRpcArray bookmarks) {
		uid = id;
		int l = text.size();
		String temp = "";
		for (int i = 0; i < l; i++){
			temp += text.getString(i) + " ";
		}
		String[] words = temp.split(" ");
		
		timer = new Timer (true);
		tt = new TimerTask () {
            @Override
            public void run () {
                tt = null;
                logger.info("TTS Timer timed out!");
                TurnTakingManager.getInstance ().utteranceFinished (uid);
            }
        };
		logger.info("SKENE UTTERANCE: " + temp);
		timer.schedule (tt, (long) (words.length * 2 * 1000));
		logger.info("TTS Timer scheduled for: " + words.length + " secs");
	}

}
