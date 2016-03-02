package uk.ac.hw.emote.intman.dm;

import java.util.Arrays;
import java.util.Iterator;
import java.util.Map;
import java.util.concurrent.atomic.AtomicReference;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;
import org.eclipse.jetty.util.log.Log;
import org.json.JSONException;
import org.json.JSONObject;

import redstone.xmlrpc.XmlRpcArray;
import uk.ac.hw.emote.intman.Communication;

public class Utterance {

    public enum UtteranceState {
        Queued, Waiting, Started, Finished, Cancelled
    }
    
    Logger logger = Logger.getLogger(Utterance.class.getName());
    
    public String language;
    public String cf;
    public String content;
    public Integer stepId;
    public double timeout;
    public String id, lastUttId;
    public AtomicReference<UtteranceState> uttState;
    public boolean standalone;
	public Map<String, String> tags;

    public Utterance () {
    	PropertyConfigurator.configure("log4j.properties");
        this.uttState = new AtomicReference<UtteranceState> (UtteranceState.Queued);
        this.id = "null";
        this.timeout = 0;
        //this.lastUttId = "null";
    }

    public void perform () {
        UtteranceState oldState;
        if ( (oldState = uttState.getAndSet (UtteranceState.Waiting)) != UtteranceState.Queued) {
            logger.warn ("Unexpected previous utterance state " + oldState);
        }

        Runnable doPerform = new Runnable () {

            @SuppressWarnings ("unchecked")
            @Override
            public void run () {
            	/*
                if (stepId != null && stepId != 999 ) {
                    logger.info ("Sending StartStep(" + stepId + ")");
                    Communication.getMapPublisher ().StartStep (stepId);
                    try {
                        Thread.sleep (2000);
                    } catch (InterruptedException e) {
                    }
                }
				*/
                  
                if (content != null){
                	
                	 // Disable the map after changing the step
                    // Communication.getMapPublisher ().BlockUI ();
                 
                	
	                
	                id = "U" + System.currentTimeMillis();
	                logger.info ("*** Sending to Skene:: utterance: " + content );
	                //logger.info ("*** Tags: " + tags );
	                
	                lastUttId = InteractionManager.getInstance().getLastUtteranceId();
	                if (lastUttId != null){
	                	Communication.getSpeechPublisher().CancelUtterance (lastUttId);
	                	logger.info ("Last utt cancelled : " + lastUttId);
	                }
	                InteractionManager.getInstance().setLastUtteranceId(id);
	                Communication.getSpeechPublisher ().PerformUtterance (id, content, "");
	                
	                
                } else if (cf != null){
                	// Change to PerformUtteranceFromLibrary
                	String category = "";
	                String subcat = "";

	                if (cf.contains(":")){
                		String[] a = cf.split(":");
                		category = a[0].toLowerCase();
                		subcat = a[1].toLowerCase();
                	} else {
                		category = cf.toLowerCase();
                		subcat = "-";
                	}
	                
	                // category = "pump";
	                // subcat = "direction";
	                
	                XmlRpcArray tagsArray = new XmlRpcArray();
	                XmlRpcArray valuesArray = new XmlRpcArray();
	                
	                if (tags != null){
		                for (String key : tags.keySet()) {
		            		String label = tags.get(key);
		            		tagsArray.add ("/" + key + "/");
		                	valuesArray.add (label);
		                }
	                }
	                
	                if (!category.equals("sayNothing")){
		                id = "U" + System.currentTimeMillis();
		                logger.info ("*** Sending to Skene:: utterance info: " + id + ":" + category + ":" + subcat );
		                logger.info ("*** Tags: " + tags );
		                
		                lastUttId = InteractionManager.getInstance().getLastUtteranceId();
		                if (lastUttId != null){
		                	Communication.getSpeechPublisher().CancelUtterance (lastUttId);
		                	 logger.info ("Last utt cancelled : " + lastUttId);
		                }
		                
		                InteractionManager.getInstance().setLastUtteranceId(id);
		                Communication.getSpeechPublisher().PerformUtteranceFromLibrary(id, category, subcat, tagsArray, valuesArray);
	                }
	                
                }
            }
        };

        new Thread (doPerform).start ();
    }

    

	
    /*
    public void cancel () {
        if (lastUttId.equals("null")) {
            logger.warn ("Not sending CancelUtterance for utterance with no ID");
            uttState.getAndSet (UtteranceState.Cancelled);
        } else {
           //Communication.getSpeechPublisher ().CancelUtterance (lastUttId);
        }
    }
	*/
    
    @Override
    public String toString () {
        String truncContent = (content.length () < 10) ? content
                : (content.substring (0, 10) + "...");
        return "Utterance [content=" + truncContent + ", , stepId=" + stepId + ", timeout="
                + timeout + ", id=" + id + ", uttState=" + uttState + "]";
    }

}
