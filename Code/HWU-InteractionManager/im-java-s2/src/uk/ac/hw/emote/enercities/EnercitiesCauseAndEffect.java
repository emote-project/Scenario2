package uk.ac.hw.emote.enercities;

import java.util.Hashtable;

import org.json.JSONException;
import org.json.JSONObject;

import simpleDS.learning.SimpleAgent;

public class EnercitiesCauseAndEffect {
	
	JSONObject s1, s2, action;
	String currentLearnerState, actionState;
	String possibleActions;
	private SimpleAgent simpleAgent;
	int dialogue;
	Hashtable<String,Integer> structureCounter; 
	private Boolean randomChoice = false;
	
	public EnercitiesCauseAndEffect(){
		s1 = null;
		s2 = null;
		currentLearnerState = null;
		actionState = null;
		
		possibleActions = "0,1,2,3,4,5,6,7,8";
		dialogue = 0;
		
		structureCounter = new Hashtable<String,Integer>(); 
		
		if (!randomChoice){
			simpleAgent = new SimpleAgent();
			simpleAgent.start();
			System.out.println("Start DRL network now!");
		}
	}
	
	public void setState(JSONObject s){
		s1 = s2;
		s2 = s;
		
		if (s1 != null){
			prepareLearnerState();
		}
	}
	
	public void setGameAction(JSONObject s){
		action = s;
		
		
		if (action.has("gameStructure")){
			try {
				incrementStructureCount(action.getString("gameStructure"));
			} catch (JSONException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		} else {
			incrementStructureCount("skip");
		}
		
		actionState = getStructureCount("skip") + "," + getStructureCount("Park") + "," + getStructureCount("Forest") + "," + getStructureCount("Wildlife_Reserve") + "," + getStructureCount("Suburban") + 
				"," + getStructureCount("Urban") + "," + getStructureCount("Stadium") + "," + getStructureCount("Light_Industry") + "," + getStructureCount("Heavy_Industry") +
				"," + getStructureCount("Commercial") + "," + getStructureCount("Coal_Plant") + "," + getStructureCount("Nuclear_Plant") + "," + getStructureCount("Windmills") +
				"," + getStructureCount("Solar_Plant") + "," + getStructureCount("Hydro_Plant") + "," + getStructureCount("City_Hall") + "," + getStructureCount("Coal_Plant_Small") +
				"," + getStructureCount("Residential_Tower") + "," + getStructureCount("Super_Solar") + "," + getStructureCount("Super_WindTurbine") + "," + getStructureCount("Nuclear_Fusion") +
				"," + getStructureCount("Market") + "," + getStructureCount("Public_Services");
		
		/*
		
			if (action.has("gameStructure") && action.getString("gameStructure").equals("Park")){
				actionState = "0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Forest")){
				actionState = "0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Wildlife_Reserve")){
				actionState = "0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Suburban")){
				actionState = "0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Urban")){
				actionState = "0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Stadium")){
				actionState = "0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Light_Industry")){
				actionState = "0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Heavy_Industry")){
				actionState = "0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Commercial")){
				actionState = "0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Coal_Plant")){
				actionState = "0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Nuclear_Plant")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Windmills")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Solar_Plant")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Hydro_Plant")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("City_Hall")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Coal_Plant_Small")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Residential_Tower")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Super_Solar")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Super_WindTurbine")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Nuclear_Fusion")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Market")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0";
			}
			else if (action.has("gameStructure") && action.getString("gameStructure").equals("Public_Services")){
				actionState = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1";
			}
			else {
				actionState = "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
			} 
			
		 */
		
		
	}
	
	public void prepareLearnerState(){
		Double EnvironmentScore, EconomyScore, WellbeingScore, Population, PowerProduction, Money, Oil, PowerConsumption, MoneyEarning;
		
		try {
			
			
			
			
			EnvironmentScore = ((double) s2.getInt("EnvironmentScore") - (double) s1.getInt("EnvironmentScore"));
		
			EconomyScore = ((double) s2.getInt("EconomyScore") - (double) s1.getInt("EconomyScore"));
			WellbeingScore = ((double) s2.getInt("WellbeingScore") - (double) s1.getInt("WellbeingScore"));
			Population = ((double) s2.getInt("Population") - (double) s1.getInt("Population"));
			PowerProduction = ((double) s2.getInt("PowerProduction") - (double) s1.getInt("PowerProduction"));
			PowerConsumption = ((double) s2.getInt("PowerConsumption") - (double) s1.getInt("PowerConsumption"));
			MoneyEarning = ((double) s2.getInt("MoneyEarning") - (double) s1.getInt("MoneyEarning"));
			Money = ((double) s2.getInt("Money") - (double) s1.getInt("Money"));
			Oil = ((double) s2.getInt("Oil") - (double) s1.getInt("Oil"));
			
			/*
			String fullState = s1.getInt("EnvironmentScore") + "," + s2.getInt("EnvironmentScore") + "," +
								s1.getInt("EconomyScore") + "," + s2.getInt("EconomyScore") + "," +
								s1.getInt("WellbeingScore") + "," + s2.getInt("WellbeingScore") + "," +
								s1.getInt("Population") + "," + s2.getInt("Population") + "," +
								s1.getInt("PowerProduction") + "," + s2.getInt("PowerProduction") + "," +
								s1.getInt("Money") + "," + s2.getInt("Money") + "," +
								s1.getInt("Oil") + "," + s2.getInt("Oil");
			
			currentLearnerState =  this.actionState + "," + EnvironmentScore + "," + EconomyScore + "," + WellbeingScore + "," + Population +
					"," + PowerProduction + "," + Money + "," + Oil + ",0";
			*/
			currentLearnerState = EnvironmentScore + "," + EconomyScore + "," + WellbeingScore + "," + Population +
					"," + PowerProduction + "," + PowerConsumption + "," + Money + "," + MoneyEarning + "," + Oil + ",0";
			
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public String getLearnerState(){
		return this.currentLearnerState;
	}
	
	public String getAction(){
		String actionDesc = null;
		if (currentLearnerState == null){ return null;}
		
		if (!randomChoice){
			simpleAgent.sendMessage("state="+this.currentLearnerState+"|actions="+this.possibleActions+"|rewards=0|dialogues="+dialogue);
		}
		String learnedAction = null;
		
		if (randomChoice){
			String[] actionArray = possibleActions.split(",");
			learnedAction = String.valueOf((int) (Math.floor(Math.random() * actionArray.length)));
		}
		else {
			while (learnedAction == null) {
				try {
					Thread.sleep(1);
					learnedAction = simpleAgent.getLastAction();
					if (learnedAction != null){
						System.out.println("Receiving action:" + learnedAction );
					}
				} catch (InterruptedException e) {
					e.printStackTrace();
				}
			}
			simpleAgent.setLastAction(null);
		}
		
		switch(Integer.valueOf(learnedAction)){
		   case 0 :
				actionDesc = "EconomyScore";
				break;
		   case 1 :
		    	actionDesc = "EnvironmentScore";
		   		break;
			case 2 :
				actionDesc = "WellbeingScore";
				break;
		    case 3 :
		    	actionDesc = "Population";
		    	break;
		    case 4 :
		    	actionDesc = "PowerProduction";
		    	break;
		    case 5 :
		    	actionDesc = "PowerConsumption";
		    	break;
			case 6 :
				actionDesc = "Money";
				break;
			case 7 :
				actionDesc = "MoneyEarning";
				break;
			case 8 :
				actionDesc = "Oil";
			case 9:
				actionDesc = "Skip";
				
		}
		dialogue++;
		
		return actionDesc;
	}
	
	private void incrementStructureCount(String k){
		if (this.structureCounter.containsKey(k)){
			this.structureCounter.put(k, (Integer) this.structureCounter.get(k) + 1);
		} else {
			this.structureCounter.put(k, 0);
		}
	}
	
	private Integer getStructureCount(String k){
		if (this.structureCounter.containsKey(k)){
			return (Integer) this.structureCounter.get(k);
		} else {
			this.structureCounter.put(k, 0);
			return 0;
		}
	}
	
	private void resetStructureCounter(){
		this.structureCounter = new Hashtable<String,Integer>();
	}
	
}
