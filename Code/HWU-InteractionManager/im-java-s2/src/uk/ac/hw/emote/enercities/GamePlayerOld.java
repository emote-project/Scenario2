package uk.ac.hw.emote.enercities;

import org.apache.log4j.Logger;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import uk.ac.hw.emote.intman.dm.InteractionManager;

public class GamePlayerOld {
	
	
	Logger logger = Logger.getLogger(GamePlayer.class.getName());
	
	public GamePlayerOld(){
		
	}
	
	public JSONObject run(JSONObject gameUpdate) throws JSONException{
		
		//GamePlayer update:{"fromModule":"enercities","gameStructure":"Coal_Plant_Small","cellX":2,"cellY":1,"gameStructureConstructed":"true"}
		//GamePlayer update:{"fromModule":"enercities","upgrade":"improved_insulation","cellX":5,"cellY":1,"structureUpgraded":"true"}
		//GamePlayer update:{"fromModule":"enercities","policyImplemented":"true","policy":"sustainable_technology_fund"}
		//GamePlayer update:{"fromModule":"enercities","turnSkipped":"true"}
		//GamePlayer update:{"fromModule":"enercities","EnvironmentScore":2,"EconomyScore":2,"gameTurnHolder":"Environmentalist","WellbeingScore":2,"turnChanged":"true","MoneyEarning":2,"PowerConsumption":0,"Money":104,"Oil":1800,"Population":1,"TargetPopulation":15,"Level":1,"PowerProduction":4,"GlobalScore":6}
		
		logger.info("GamePlayer update:" + gameUpdate.toString());
		
		// here u get all the enercities game updates from IM.
		// use it to update ur learning agent
		
		if (gameUpdate.has("turnChanged") && gameUpdate.getString("turnChanged").equals("true")){
			
			
			
			/// need a game action decision here..
			
			JSONObject gpOut = new JSONObject();
			gpOut.put("bestActionIdentified", "true");
			gpOut.put("bestActionType", 2);
			gpOut.put("bestActionSubtype", 1);
			gpOut.put("bestActionPosX", "2");
			gpOut.put("bestActionPosY", "3");
			
			//if actiontype == 2 (i.e. upgrade) then use the following to communicate the other two upgrade actions
			
			gpOut.put("bestActionType2", 0);
			gpOut.put("bestActionSubtype2", 0);
			gpOut.put("bestActionPosX2", "0");
			gpOut.put("bestActionPosY2", "0");
			
			gpOut.put("bestActionType3", 0);
			gpOut.put("bestActionSubtype3", 0);
			gpOut.put("bestActionPosX3", "0");
			gpOut.put("bestActionPosY3", "0");
			
			return gpOut;
		} 
		
		
		return null;
		
	}
	
}
