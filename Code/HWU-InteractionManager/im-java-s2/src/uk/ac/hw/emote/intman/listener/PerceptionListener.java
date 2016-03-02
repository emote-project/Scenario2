package uk.ac.hw.emote.intman.listener;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

import org.apache.commons.collections4.queue.CircularFifoQueue;
import org.eclipse.jetty.util.log.Log;
import org.json.JSONException;
import org.json.JSONObject;

import uk.ac.hw.emote.intman.dm.TurnTakingManager;
import emotecommonmessages.GazeEnum;
import emotecommonmessages.Hand;
import emotecommonmessages.IPerceptionEvents;

public class PerceptionListener implements IPerceptionEvents {

    private static final int SMILE_THRESHOLD = 10;
    private static final int SMILE_BUFFER_SIZE = 2;

    private Map<Integer, Integer> lastSmiles = new HashMap<Integer, Integer> ();

    private Map<Integer, CircularFifoQueue<Integer>> smileBuffers = new HashMap<Integer, CircularFifoQueue<Integer>> ();

    private CircularFifoQueue<Integer> getSmileBuffer (int userID) {
        CircularFifoQueue<Integer> buffer = smileBuffers.get (userID);
        if (buffer == null) {
            buffer = new CircularFifoQueue<Integer> (SMILE_BUFFER_SIZE);
            smileBuffers.put (userID, buffer);
        }
        return buffer;
    }

    private int getSmileRange (int userID) {
        CircularFifoQueue<Integer> smiles = getSmileBuffer (userID);
        if (smiles.isEmpty ())
            return 0;

        int max = Collections.max (smiles);
        int min = Collections.min (smiles);
        return max - min;
    }

    private boolean updateSmile (int userID, int newSmile, int newConf) {
        synchronized (this) {
            CircularFifoQueue<Integer> smileBuffer = getSmileBuffer (userID);
            smileBuffer.add (newSmile);
            Integer lastSmile = lastSmiles.get (userID);
            boolean interesting = (lastSmile == null || (getSmileRange (userID)) < SMILE_THRESHOLD
                    && Math.abs (newSmile - lastSmile) > SMILE_THRESHOLD);
            if (interesting) {
                Log.getLog ().info (
                        "*** Smile buffer now " + smileBuffer + "; interesting is " + interesting);
                lastSmiles.put (userID, newSmile);
            }
            return interesting;
        }
    }

    @Override
    public void Smile (int userID, int smileVal, int confidenceVal) {
        System.out.println ("Smile!!");

        // Ignore any empty information
        if (smileVal == 0 && confidenceVal == 0)
            return;

        if (updateSmile (userID, smileVal, confidenceVal)) {
            JSONObject systemInput = new JSONObject ();
            /* 
            
            try {
                systemInput.put ("fromModule", "affect");
                 
                systemInput.put ("smileVal", (double) smileVal);
                systemInput.put ("smileConfidence", (double) confidenceVal);
                Log.getLog ().info ("Smile message: " + systemInput);
                TurnTakingManager.getInstance ().processInputAndPerform (systemInput);
            } catch (JSONException e) {
                e.printStackTrace ();
            }
            */
            
        }
    }

    @Override
    public void HeadTracking (int userID, double X, double Y, double Z, boolean DetectedSkeleton) {
        // TODO Auto-generated method stub

    }

    @Override
    public void EyebrowsAU (double au2_user1, double au4_user1, double au2_user2, double au4_user2) {
        // TODO Auto-generated method stub

    }

    @Override
    public void PointingPosition (int userID, Hand hand, double X, double Y, double Z) {
        // TODO Auto-generated method stub

    }

    @Override
    public void UserMutualGaze (boolean value) {
        // TODO Auto-generated method stub

    }

    @Override
    public void UserMutualPoint (boolean value, double avegX, double avegY) {
        // TODO Auto-generated method stub

    }

    @Override
    public void EyebrowsAU2 (double au4left_user1, double au4right_user1, double au4left_user2,
            double au4right_user2) {
        // TODO Auto-generated method stub

    }

    @Override
    public void UserTouchChin (int userID, boolean value) {
        // TODO Auto-generated method stub

    }

    @Override
    public void GazeTracking (int userID, GazeEnum direction, int ConfidenceVal) {
        // TODO Auto-generated method stub

    }
    
    

}
