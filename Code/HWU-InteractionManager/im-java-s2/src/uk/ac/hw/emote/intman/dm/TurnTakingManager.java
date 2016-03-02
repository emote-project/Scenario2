package uk.ac.hw.emote.intman.dm;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Collections;
import java.util.HashSet;
import java.util.Scanner;
import java.util.Set;
import java.util.Timer;
import java.util.TimerTask;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;
import org.eclipse.jetty.util.log.Log;
import org.json.JSONException;
import org.json.JSONObject;

import uk.ac.hw.emote.intman.Communication;

public class TurnTakingManager {

    private static final TurnTakingManager _inst = new TurnTakingManager ();

    public static TurnTakingManager getInstance () {
        return _inst;
    }

    // Turn taking
    private Timer timer;
    private TimerTask tt;
    private Utterance curUtterance;
    private boolean utterancePlaying;
    private String learnerFirstName;
    private Boolean interactionStatus;
    private Integer timeOut;
    private Boolean mapReady;
    private JSONObject sysInput;
    
    Logger logger = Logger.getLogger(TurnTakingManager.class.getName());
    
    private TurnTakingManager () {
    	PropertyConfigurator.configure("log4j.properties");
    	
    	mapReady = false;
        // Set up the timer for timeout tasks
        timer = new Timer (true);
        curUtterance = null;
        timeOut = 5;
        interactionStatus = false;
        utterancePlaying = false;
        sysInput = new JSONObject();
    }

    /*
     * This is where the IM is initialised.. 
     * We pass the parameters such as learnerId, learnerName, etc about the learner
     * and whether the tutor needs to empathic or not
     * and whether it is the first session or not
     * 
     */
    public void init (String startInfo) {
    	InteractionManager.getInstance ().init (startInfo);
        
        /*
        while (!mapReady){
        	System.out.println("Waiting for map to be ready");
        	try {
				Thread.sleep(1000);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
        }
        */
        
        /*
        //adding a pause
        System.out.println("******************************************");
        System.out.println("******************************************");
        System.out.println("*********INTERACTION MANAGER MODULE*******");
        System.out.println("******************************************");
        System.out.println("********PRESS ENTER KEY TO CONTINUE******");
        System.out.println("******************************************");
        System.out.println("******************************************");
        System.out.println("******************************************");
        */
        
    	/*
        try {
			Thread.sleep(2000);
		} catch (InterruptedException e1) {
			e1.printStackTrace();
		}
        */
        /*
        //following code requires the return key to be pressed.
        int r = 0;
        do {
	        try
	        {
	            r = System.in.read();
	            System.out.println(r);
	        }  
	        catch(IOException e)
	        {
	        	e.printStackTrace();
	        }  
        } while(r != 13);
        
        
        */
        /*
        BufferedReader stdin =new BufferedReader(new InputStreamReader(System.in));
        try {
			String s = stdin.readLine();
			System.out.println("Input:" + s + "!!!");
		} catch (IOException e1) {
			e1.printStackTrace();
		}
        */
        try {
        	/*
        	Utterance sample = new Utterance();
            sample.content = "Test test";
            sample.id = "123456789";
            sample.perform();
            */
        	/*
            logger.info ("Wait a second.....");
            try {
    			Thread.sleep(5000);
    		} catch (InterruptedException e1) {
    			e1.printStackTrace();
    		}
            */
        	interactionStatus = true;
        	
        	processInputAndPerform (sysInput);
            
        } catch (Exception e) {
            e.printStackTrace ();
        }
    }

    public void setLearnerInfo(String firstName){
    	learnerFirstName = firstName;
    }
    
    public boolean isOutputActive () {
        return !activeUtterances.isEmpty ();
    }

    
    
    public void processInput(JSONObject systemInput){
    	InteractionManager.getInstance ().updateInput (systemInput);
    }
    
    
    public void processInputAndPerform (JSONObject systemInput) throws JSONException {
    	logger.info("TTM input: " + systemInput.toString());

    	if (!interactionStatus){
    		sysInput = systemInput;
    	}
    	
    	if (interactionStatus){
	        Utterance nextUtterance = InteractionManager.getInstance ().processInput (systemInput);
	
	        if (nextUtterance != null) {
	        	if (!utterancePlaying){
	        		//cancel the waitForUserResponse timer
	        		cancelTimeout ();
	        	}
	            curUtterance = nextUtterance;
	            //InteractionManager.getInstance().setCurrentUtteranceId(curUtterance.id);
	            curUtterance.perform ();
	            utterancePlaying = true;
	            scheduleTimeout (10 , "utteranceTimer");
	        } else {
	        	if (interactionStatus){
	        		if (!utterancePlaying){
		        		logger.info ("Interaction status: " + interactionStatus);
		        		scheduleTimeout (10, "waitForUserResponse");
	        		}
	        	}
	        }
    	} else {
    		curUtterance = null;
    		logger.info("IM not active..");
    	}
    }

    private Set<String> activeUtterances = Collections.synchronizedSet (new HashSet<String> ());

    public void utteranceStarted (String id) {
        //Log.getLog () ("Utterance started: " + id);
    	logger.info ("Utterance started: " + id);
        if (!activeUtterances.add (id)) {
            logger.warn ("Utterance ID was already in the active set!");
        } else {
            try {
                JSONObject ttsInput = new JSONObject ("{utteranceStatus:playing,fromModule:tts}");
                ttsInput.put("utteranceId", id);
                processInput(ttsInput);
            } catch (Exception e) {
                e.printStackTrace ();
            }
        }
    }

    public void utteranceFinished (String id) {
        try {
            logger.info ("Utterance finished: " + id);
            
            if (!id.equals(curUtterance.id)){
            	logger.info("Ignoring utterance finished message for id:" + id);
            	logger.info("Waiting for finished message for id:" + curUtterance.id);
            	return;
            }
            utterancePlaying = false;
            cancelTimeout();
            //Communication.getMapPublisher ().UnBlockUI ();
            try {
                JSONObject ttsInput = new JSONObject ("{utteranceStatus:delivered,fromModule:tts}");
                ttsInput.put("utteranceId", id);
                processInputAndPerform (ttsInput);
            } catch (Exception e) {
                e.printStackTrace ();
            }
        } catch (Exception e) {
            e.printStackTrace ();
        }
    }


    
    private void scheduleTimeout (double timeout, String reason) {
        synchronized (timer) {
            cancelTimeout ();
            
            
            if (timeout >= 0 && reason.equalsIgnoreCase("waitForUserResponse")) {
                tt = new TimerTask () {
                    @Override
                    public void run () {
                        tt = null;
                        logger.info ("Timeout expired!");
                        sendTimeout ();
                    }
                };
                logger.info ("Scheduling timeout in " + timeout + " seconds ...");
                if (timer!=null){
                	timer.schedule (tt, (long) (timeout * 1000));
                } else {
                	timer = new Timer (true);
                	timer.schedule (tt, (long) (timeout * 1000));
                	logger.info ("Time out rescheduled..");
                }
            } 
            else if (timeout >= 0 && reason.equalsIgnoreCase("utteranceTimer")) {
                tt = new TimerTask () {
                    @Override
                    public void run () {
                        tt = null;
                        logger.info ("Timeout expired!");
                        sendUtteranceTimeout ();
                    }
                };
                logger.info ("Scheduling utterance timeout in " + timeout + " seconds ...");
                if (timer!=null){
                	timer.schedule (tt, (long) (timeout * 1000));
                } else {
                	timer = new Timer (true);
                	timer.schedule (tt, (long) (timeout * 1000));
                	logger.info ("Time out rescheduled..");
                }
            } 
            
            
            else {
                logger.info ("Waiting forever ...");
            }
        }
    }

    public void cancelTimeout () {
        synchronized (timer) {
            if (tt != null) {
                logger.info ("Cancelling previous scheduled timeout");
                tt.cancel ();
                tt = null;
            }
        }
    }

    
    public void sendUtteranceTimeout () {
        
        utteranceFinished(curUtterance.id);
    }
    
    public void sendTimeout () {
       
        JSONObject systemInput = new JSONObject ();
        try {
            systemInput.put ("fromModule", "hub");
            systemInput.put ("time-out", true);
            processInputAndPerform (systemInput);
        } catch (JSONException e) {
            e.printStackTrace ();
        }
    }

	public void stop() {
		
		cancelTimeout();
		interactionStatus = false;
		mapReady = false;
		logger.info ("Interaction status set to: " + interactionStatus);
	}

	public void setMapReady(boolean b) {
		// TODO Auto-generated method stub
		mapReady = b;
	}
	
	public Boolean getInteractionStatus(){
		return interactionStatus;
	}
	
}
