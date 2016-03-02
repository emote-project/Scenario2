package uk.ac.hw.emote.intman;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;

import org.eclipse.jetty.util.log.Log;
import org.kohsuke.args4j.Argument;
import org.kohsuke.args4j.CmdLineException;
import org.kohsuke.args4j.CmdLineParser;
import org.kohsuke.args4j.Option;

import redstone.xmlrpc.XmlRpcException;
import uk.ac.hw.emote.intman.dm.InteractionManager;
import uk.ac.hw.emote.intman.dm.TurnTakingManager;

public class Main {
    

    public static void main (String[] args) {
        // Start the XMLRPC server to receive messages from the rest of the
        // system
        Thread serverThread = Communication.getServerThread ();
        
        serverThread.start ();

        /*
        // Start the system off
        try {
            Communication.getEmotePublisher ().Start (new Random ().nextInt (), 123);
        } catch (XmlRpcException e1) {
            Log.getLog ().warn ("Couldn't connect to C# client; exiting");
            return;
        }
		*/
        // Initialise the interaction manager
        //TurnTakingManager.getInstance ().init (123, "Srini");
        
        System.out.println("Ready. Press START on Control Panel.");
        // Just keep going until we are terminated
        while (serverThread.isAlive ()) {
            try {
                Thread.sleep (100);
            } catch (InterruptedException e) {
            }
        }
    }

    

}
