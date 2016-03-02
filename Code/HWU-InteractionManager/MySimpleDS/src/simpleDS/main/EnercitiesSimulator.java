package simpleDS.main;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Hashtable;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;
import org.json.JSONException;
import org.json.JSONObject;



public class EnercitiesSimulator {
	
	int episodes;
	BufferedReader br;
	String startState, currentState, previousState, actionString;
	Boolean gameOver;
	Boolean discardFirstLine;
	
	Double reward, totalScore;
	String actionSet;
	String lastFullState;
	Double EnvironmentScore, EconomyScore, WellbeingScore, Population, PowerProduction, Money, Oil, PowerConsumption, MoneyEarning;
	Double[] scoreArray = {0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0};
	
	Hashtable<String,Integer> structureCounter; 
	Logger logger = Logger.getLogger(EnercitiesSimulator.class.getName());
	
	EnercitiesSimulator(){
		PropertyConfigurator.configure("log4j.properties");
		
		
		loadFile();
			
			
		
	}
	
	private void loadFile(){
		try {
			gameOver = false;
			
			episodes = 0;
			
			reward = 0.0;
			totalScore = 0.0;
			startState = null;
			previousState = null;
			
			actionSet = "0,1,2,3,4,5,6,7,8";
			
			structureCounter = new Hashtable<String,Integer>(); 
			
			
			br = new BufferedReader(new FileReader("gamelog.txt"));
			
			
			String temp;
			try {
				temp = br.readLine();
				if (temp != null){
					startState = temp;
				}
			} catch (IOException e) {
				e.printStackTrace();
			}
			currentState = startState;
			discardFirstLine = false;
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}
	}
	
	private ArrayList<String> readGameTurn(){

		ArrayList<String> out = new ArrayList<String>();
		int i = 0;
		int j = 2;
		
		out.add(currentState);
		
		try {
			String sCurrentLine = null;
			while (i < j) {
				sCurrentLine = br.readLine();
				if (sCurrentLine == null){
					gameOver = true;
					return out;
				}
				//logger.info("Reading line:" + sCurrentLine);
				if (sCurrentLine.startsWith("game reset")){
					//logger.info("Reading line:" + sCurrentLine);
					resetStructureCounter();
					currentState = startState;
					discardFirstLine = true;
					return out;
				}
				if (discardFirstLine && sCurrentLine.contains("enercities")){
					discardFirstLine = false;
				} else {
					//logger.info("Adding line:" + sCurrentLine);
					out.add(sCurrentLine);
					i++;
					discardFirstLine = false;
				}
			}
			previousState = currentState;
			currentState = sCurrentLine;
			//logger.info("Setting currentState:" + currentState);
		} catch (IOException e) {
			e.printStackTrace();
		}
		
		return out;
		
	}
	
	public String getActionString(){
		return this.actionString;
	}
	
	public String getSAR(ArrayList<String> turn){
		//logger.info(turn.size());
		
		if (turn.size() != 3){
			return null;
		}
		
		try {
			JSONObject  s1 = new JSONObject(turn.get(0));
			JSONObject  action = new JSONObject(turn.get(1));
			JSONObject  s2 = new JSONObject(turn.get(2));
			
			String actionState;
			
			if (action.has("gameStructure")){
				actionString = action.getString("gameStructure");
			}
			
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
			
			
			/*
			if (action.has("gameStructure")){
				incrementStructureCount(action.getString("gameStructure"));
			} else {
				incrementStructureCount("skip");
			}
			
			actionState = getStructureCount("skip") + "," + getStructureCount("Park") + "," + getStructureCount("Forest") + "," + getStructureCount("Wildlife_Reserve") + "," + getStructureCount("Suburban") + 
					"," + getStructureCount("Urban") + "," + getStructureCount("Stadium") + "," + getStructureCount("Light_Industry") + "," + getStructureCount("Heavy_Industry") +
					"," + getStructureCount("Commercial") + "," + getStructureCount("Coal_Plant") + "," + getStructureCount("Nuclear_Plant") + "," + getStructureCount("Windmills") +
					"," + getStructureCount("Solar_Plant") + "," + getStructureCount("Hydro_Plant") + "," + getStructureCount("City_Hall") + "," + getStructureCount("Coal_Plant_Small") +
					"," + getStructureCount("Residential_Tower") + "," + getStructureCount("Super_Solar") + "," + getStructureCount("Super_WindTurbine") + "," + getStructureCount("Nuclear_Fusion") +
					"," + getStructureCount("Market") + "," + getStructureCount("Public_Services");
			*/
			
			//logger.info(s1.toString());
			//logger.info(s2.toString());
			/*
			Double EnvironmentScore = ((double) s2.getInt("EnvironmentScore") - (double) s1.getInt("EnvironmentScore")) /  Math.abs((double) s1.getInt("EnvironmentScore"));
			Double EconomyScore = ((double) s2.getInt("EconomyScore") - (double) s1.getInt("EconomyScore")) /  Math.abs((double) s1.getInt("EconomyScore"));
			Double WellbeingScore = ((double) s2.getInt("WellbeingScore") - (double) s1.getInt("WellbeingScore")) /  Math.abs((double) s1.getInt("WellbeingScore"));
			Double Population = ((double) s2.getInt("Population") - (double) s1.getInt("Population")) /  Math.abs((double) s1.getInt("Population"));
			Double PowerProduction = ((double) s2.getInt("PowerProduction") - (double) s1.getInt("PowerProduction")) /  Math.abs((double) s1.getInt("PowerProduction"));
			Double Money = ((double) s2.getInt("Money") - (double) s1.getInt("Money")) /  Math.abs((double) s1.getInt("Money"));
			Double Oil = ((double) s2.getInt("Oil") - (double) s1.getInt("Oil")) /  Math.abs((double) s1.getInt("Oil"));
			*/
			
			
			EnvironmentScore = ((double) s2.getInt("EnvironmentScore") - (double) s1.getInt("EnvironmentScore"));
			EconomyScore = ((double) s2.getInt("EconomyScore") - (double) s1.getInt("EconomyScore"));
			WellbeingScore = ((double) s2.getInt("WellbeingScore") - (double) s1.getInt("WellbeingScore"));
			Population = ((double) s2.getInt("Population") - (double) s1.getInt("Population"));
			PowerProduction = ((double) s2.getInt("PowerProduction") - (double) s1.getInt("PowerProduction"));
			PowerConsumption = ((double) s2.getInt("PowerConsumption") - (double) s1.getInt("PowerConsumption"));
			Money = ((double) s2.getInt("Money") - (double) s1.getInt("Money"));
			MoneyEarning = ((double) s2.getInt("MoneyEarning") - (double) s1.getInt("MoneyEarning"));
			Oil = ((double) s2.getInt("Oil") - (double) s1.getInt("Oil"));
			
			
			String diffState =  EnvironmentScore + "," + EconomyScore + "," + WellbeingScore + "," + Population +
					"," + PowerProduction + "," + PowerConsumption + "," + Money + "," + MoneyEarning + "," + Oil;
			
			//logger.info("Fullstate:" + fullState);
			
			reward = 0.0;
			
			lastFullState = diffState;
			
			//return actionState + "," + diffState + ",0";
			return diffState + ",0";
			
		} catch (JSONException e) {
			e.printStackTrace();
			logger.info("EXCEPTION");
			
		}
		
		
		return null;
	}
	
	
	public void play(int action){
		
			String actionDesc = null;
			Double score = 0.0;
			
			switch(action){
		   case 0 :
				actionDesc = "EconomyScore";
				score = EconomyScore;
				break;
		   case 1 :
		    	actionDesc = "EnvironmentScore";
		   		score = EnvironmentScore;
				break;
			case 2 :
				actionDesc = "WellbeingScore";
				score = WellbeingScore;
				break;
		    case 3 :
		    	actionDesc = "Population";
		    	score = Population;
		    	break;
		    case 4 :
		    	actionDesc = "PowerProduction";
		    	score = PowerProduction;
		    	break;
		    case 5 :
		    	actionDesc = "PowerConsumption";
		    	score = PowerConsumption;
		    	break;
			case 6 :
				actionDesc = "Money";
				score = Money;
				break;
			case 7 :
				actionDesc = "MoneyEarning";
				score = MoneyEarning;
				break;
			case 8 :
				actionDesc = "Oil";
				score = Oil;
			}
			
			
			score = Math.abs(score);
			scoreArray[action] = score;

			logger.info("Action taken: " + actionDesc);
			totalScore += score;
			logger.info("Score: " + score);
			
		
	}
	
	public String getState(){
		
		if (gameOver){
			loadFile();
		}
		
		ArrayList<String> turn = readGameTurn();
		String out = getSAR(turn);
		
		if (out == null){
			Double stdev = stdev(scoreArray); 
			String temp = "";
			for (int i=0; i < scoreArray.length; i++){
				temp += scoreArray[i] + ",";
			}
			logger.info("ScoreArray:" + temp);
			logger.info("STDDEV:" + stdev);
			
			reward = ( totalScore / scoreArray.length) /  stdev;
			totalScore = 0.0;
			//out = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1";
			out = "0,0,0,0,0,0,0,0,0,1";
		}
		
		return out;
	}
	
	public String getPossibleActions(){
		return this.actionSet;
	}
	
	public Double getReward(){
		return this.reward;
	}
	
	
	
	public double stdev(Double[] list){
	    double sum = 0.0;
	    double mean = 0.0;
	    double num=0.0;
	    double numi = 0.0;
	    double deno = 0.0;
	    for (int i=0; i < list.length; i++){
	        sum+=list[i];
	        mean = sum/list.length-1;

	    }

	    for (int i=0; i <list.length; i++){
	         numi = Math.pow((list[i] - mean),2);
	         num+=numi;
	         deno =list.length - 1;  
	    }


	    double stdevResult = Math.sqrt(num/deno);
	    return stdevResult;
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
