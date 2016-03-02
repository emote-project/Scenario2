/* @description This is the main class for implementing SimpleDS learning agents.
 *              It currently establishes communication with a JavaScript client ('SimpleAgent').
 *              This class generates training/test dialogues according to a configuration file,
 *              see README.txt for more information.
 * 
 * @history 2.Nov.2015 Beta version
 *              
 * @author <ahref="mailto:h.cuayahuitl@gmail.com">Heriberto Cuayahuitl</a>
 */

package simpleDS.main;

import java.util.HashMap;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

import simpleDS.learning.SimpleAgent;
import simpleDS.util.IOUtil;

public class SimpleDS {
	private HashMap<String,String> configurations;
	private SimpleAgent simpleAgent;
	private boolean verbose = false;
	//private SampleGame game;
	private EnercitiesSimulator game;
	Logger logger = Logger.getLogger(SimpleDS.class.getName());
	
	public SimpleDS(String configFile) {
		PropertyConfigurator.configure("log4j.properties");
		
		parseConfigFile(configFile);
		
		simpleAgent = new SimpleAgent();
		simpleAgent.start();
 
		game = new EnercitiesSimulator();
		
		synchronized(simpleAgent) {
			//try{
				logger.info("Waiting for SimpleAgent to complete...");
				//simpleAgent.wait();

				interactionManagement();

			//} catch(InterruptedException e) {
			//	e.printStackTrace();
			//}
		}
		System.exit(0);
	}

	private void interactionManagement() {
		
		logger.info("Starting IM...");
		
		int numDialogues = Integer.parseInt(configurations.get("Dialogues"));
		logger.info("Setting numDialogues: " + numDialogues);
		
		
		for (int i=1; i<=numDialogues; i++) {
			if (simpleAgent != null) {
				
				logger.info("Run:" + i);
				String gameActionTaken = game.getActionString();
				logger.info("Game action:" + gameActionTaken);
				String state = game.getState();
				String rewards = game.getReward().toString();
				logger.info("Sending message:" + "state="+state+"|actions="+game.getPossibleActions()+"|rewards="+rewards+"|dialogues="+i);
				simpleAgent.sendMessage("state="+state+"|actions="+game.getPossibleActions()+"|rewards="+rewards+"|dialogues="+i);

				String learnedAction = null;
				while (learnedAction == null) {
					try {
						Thread.sleep(1);
						learnedAction = simpleAgent.getLastAction();
						if (learnedAction != null){
							logger.info("Receiving action:" + learnedAction);
							game.play(Integer.valueOf(learnedAction));
						}
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}
				simpleAgent.setLastAction(null);
			} 
			
		}
	}

	
	
	private void parseConfigFile(String configFile) {
		configurations = new HashMap<String,String>();
		IOUtil.readHashMap(configFile, configurations, "=");
		IOUtil.printHashMap(configurations, "CONFIGURATIONS");
		verbose = configurations.get("Verbose").equals("true") ? true : false;
	}


	public static void main(String[] args) {
		new SimpleDS("config.txt");
	}
}
