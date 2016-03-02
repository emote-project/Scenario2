package uk.ac.hw.emote.intman.listener;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;
import org.json.JSONException;
import org.json.JSONObject;

import redstone.xmlrpc.XmlRpcArray;
import uk.ac.hw.emote.intman.dm.InteractionManager;
import uk.ac.hw.emote.intman.dm.TurnTakingManager;
import emoteenercitiesmessages.Gender;
import emoteenercitiesmessages.IEnercitiesAIActions;
import emoteenercitiesmessages.IEnercitiesGameStateEvents;
import emoteenercitiesmessages.IEnercitiesTaskEvents;

public class EnercitiesListener implements IEnercitiesAIActions, IEnercitiesGameStateEvents,
        IEnercitiesTaskEvents {

	Logger logger = Logger.getLogger(EnercitiesListener.class.getName());
	
	public EnercitiesListener() {
		PropertyConfigurator.configure("log4j.properties");
	}
	
	@Override
    public void ConfirmConstruction (String structure, String translation, int cellX, int cellY) {
        logger.info ("Confirm construction" + structure + ";" + translation+ ";" + cellX+ ";" + cellY);
        
        try {
            JSONObject dmInput = new JSONObject ();
            dmInput.put("fromModule", "enercities");
			dmInput.put("gameStructureConstructed", "true");
			dmInput.put("gameStructure", structure);
			dmInput.put("cellX", cellX);
			dmInput.put("cellY", cellY);
            TurnTakingManager.getInstance ().processInputAndPerform (dmInput);
        } catch (JSONException e) {
            e.printStackTrace ();
        }
        
    }

    @Override
    public void ImplementPolicy (String policy, String translation) {
    	logger.info ("ImplementPolicy: " + policy +";"+ translation);
    	try {
			JSONObject dmInput = new JSONObject();
			dmInput.put("fromModule", "enercities");
			dmInput.put("policyImplemented", "true");
			dmInput.put("policy", policy);
			TurnTakingManager.getInstance ().processInputAndPerform (dmInput);
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    }

    @Override
    public void PerformUpgrade (String upgrade, String translation, int cellX, int cellY) {
    	logger.info ("Perform upgrade:" + upgrade);
    	try {
			JSONObject dmInput = new JSONObject();
			dmInput.put("fromModule", "enercities");
			dmInput.put("structureUpgraded", "true");
			dmInput.put("upgrade", upgrade);
			dmInput.put("cellX", cellX);
			dmInput.put("cellY", cellY);
			TurnTakingManager.getInstance ().processInputAndPerform (dmInput);
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    }

    @Override
    public void SkipTurn () {
    	logger.info ("Skip turn");
    	try {
			JSONObject dmInput = new JSONObject();
			dmInput.put("fromModule", "enercities");
			dmInput.put("turnSkipped", "true");
			TurnTakingManager.getInstance ().processInputAndPerform (dmInput);
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    }

    @Override
    public void ExamineCell (double screenX, double screenY, int cellX, int cellY,
            String StructureType_structure, String StructureType_translated) {
    	/*
    	
    	logger.info ("ExamineCell: " + screenX + ";" +  screenY + ";" + cellX + ";" + cellY+ ";" + 
            StructureType_structure + ";" + StructureType_translated);
    	try {
			JSONObject dmInput = new JSONObject();
			dmInput.put("fromModule", "enercities");
			dmInput.put("cellExamined", "true");
			dmInput.put("gameStructure", StructureType_structure);
			dmInput.put("cellX", cellX);
			dmInput.put("cellY", cellY);
			TurnTakingManager.getInstance ().processInputAndPerform (dmInput);
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}*/
    }

    @Override
    public void PlayersGender (Gender player1Gender, Gender player2Gender) {
    	logger.info ("PlayersGender: " +  player1Gender + ";" + player2Gender);

    }

    @Override
    public void PlayersGenderString (String player1Gender, String player2Gender) {
    	logger.info ("PlayersGenderString: " + player1Gender + ";" + player2Gender);

    }

    @Override
    public void GameStarted (String player1Name, String player1Role, String player2Name,
            String player2Role) {
    	logger.info ("GameStarted: " + player1Name + ";" + player1Role + ";" + player2Name
    			+ ";" +  player2Role);
    	InteractionManager.getInstance().gameStarted(true);
    	InteractionManager.getInstance().setGameInfo(player1Name, player2Name);
    }

    @Override
    public void ResumeGame (String player1Name, String player1Role, String player2Name,
            String player2Role, String serializedGameState) {
    	logger.info ("ResumeGame: " + player1Name + ";" +  player1Role + ";" +  player2Name);

    }

    @Override
    public void EndGameSuccessfull (int totalScore) {
    	logger.info ("EndGameSuccessfull: " + totalScore);

    }

    @Override
    public void EndGameNoOil (int totalScore) {
    	logger.info ("EndGameNoOil: " + totalScore);

    }

    @Override
    public void EndGameTimeOut (int totalScore) {
    	logger.info ("EndGameTimeOut: " + totalScore);

    }

    @Override
    public void TurnChanged (String serializedGameState) {
        logger.info ("Turn changed: " + serializedGameState);
        
        try {
			JSONObject gameState = new JSONObject(serializedGameState);
			JSONObject dmInput = new JSONObject();
			dmInput.put("fromModule", "enercities");
			dmInput.put("turnChanged", "true");
			if (gameState.has("CurrentRole")){
				int role = gameState.getInt("CurrentRole");
				if (role == 1){
					dmInput.put("gameTurnHolder", "Environmentalist");
				} 
				else if (role == 0){
					dmInput.put("gameTurnHolder", "Economist");
				}
				else {
					dmInput.put("gameTurnHolder", "Mayor");
				}
			}
			//Turn changed: {"CurrentRole":0,"Level":1,"Population":8,"TargetPopulation":15,"Money":102.0,"MoneyEarning":2.0,
			//"Oil":1792.0,"PowerConsumption":2.0,"PowerProduction":4.0,"EnvironmentScore":4.5,"EconomyScore":2.0,"WellbeingScore":3.0,"GlobalScore":9.5}
			
			dmInput.put("Level", gameState.getInt("Level"));
			dmInput.put("Population", gameState.getInt("Population"));
			dmInput.put("TargetPopulation", gameState.getInt("TargetPopulation"));
			dmInput.put("Money", gameState.getDouble("Money"));
			dmInput.put("MoneyEarning", gameState.getDouble("MoneyEarning"));
			dmInput.put("Oil", gameState.getDouble("Oil"));
			dmInput.put("PowerConsumption", gameState.getDouble("PowerConsumption"));
			dmInput.put("PowerProduction", gameState.getDouble("PowerProduction"));
			dmInput.put("EnvironmentScore", gameState.getDouble("EnvironmentScore"));
			dmInput.put("EconomyScore", gameState.getDouble("EconomyScore"));
			dmInput.put("WellbeingScore", gameState.getDouble("WellbeingScore"));
			dmInput.put("GlobalScore", gameState.getDouble("GlobalScore"));
			
			
			TurnTakingManager.getInstance ().processInputAndPerform (dmInput);
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
        
    }

    @Override
    public void ReachedNewLevel (int level) {
    	logger.info ("ReachedNewLevel: " + level);
    	//InteractionManager.getInstance().resetGame("reachedNewLevel");
    	return;
    }

    @Override
    public void StrategyGameMoves (String environmentalistMove, String economistMove,
            String mayorMove, String globalMove) {
    	logger.info ("StrategyGameMoves: " + environmentalistMove + ";" +  economistMove + ";" +  mayorMove + ";" +  globalMove);

    }

    @Override
    public void StrategiesUpdated (String StrategiesSet_strategies) {
    	logger.info ("StrategiesUpdated: " + StrategiesSet_strategies);

    }

    @Override
    public void BestActionPlanned (XmlRpcArray EnercitiesActionInfo_actionInfos) {
    	logger.info ("BestActionPlanned: " + EnercitiesActionInfo_actionInfos);
    	try {
    		
    		if (EnercitiesActionInfo_actionInfos.size() > 0){
				JSONObject bestAction = new JSONObject(EnercitiesActionInfo_actionInfos.getString(0));
				JSONObject dmInput = new JSONObject();
				dmInput.put("fromModule", "enercitiesAI");
				dmInput.put("bestActionIdentified", "true");
				dmInput.put("bestActionType", bestAction.getInt("ActionType"));
				dmInput.put("bestActionSubtype", bestAction.getInt("SubType"));
				dmInput.put("bestActionPosX", "" + bestAction.getInt("CellX"));
				dmInput.put("bestActionPosY", "" + bestAction.getInt("CellY"));
				
				if (EnercitiesActionInfo_actionInfos.size() > 1 && !EnercitiesActionInfo_actionInfos.getString(1).equals("")){
						bestAction = new JSONObject(EnercitiesActionInfo_actionInfos.getString(1));
						dmInput.put("bestActionType2", bestAction.getInt("ActionType"));
						dmInput.put("bestActionSubtype2", bestAction.getInt("SubType"));
						dmInput.put("bestActionPosX2", "" + bestAction.getInt("CellX"));
						dmInput.put("bestActionPosY2", "" + bestAction.getInt("CellY"));
						
						if (EnercitiesActionInfo_actionInfos.size() > 2 && !EnercitiesActionInfo_actionInfos.getString(2).equals("")){
							bestAction = new JSONObject(EnercitiesActionInfo_actionInfos.getString(2));
							dmInput.put("bestActionType3", bestAction.getInt("ActionType"));
							dmInput.put("bestActionSubtype3", bestAction.getInt("SubType"));
							dmInput.put("bestActionPosX3", "" + bestAction.getInt("CellX"));
							dmInput.put("bestActionPosY3", "" + bestAction.getInt("CellY"));
						} else {
							//skip the turn
							dmInput.put("bestActionType3", 0);
							dmInput.put("bestActionSubtype3", 0);
							dmInput.put("bestActionPosX3", "" + 0);
							dmInput.put("bestActionPosY3", "" + 0);
						}
					
				} else {
					//skip the turn
					dmInput.put("bestActionType2", 0);
					dmInput.put("bestActionSubtype2", 0);
					dmInput.put("bestActionPosX2", "" + 0);
					dmInput.put("bestActionPosY2", "" + 0);
				}
				
				
				logger.info("BestActionParsed:" + dmInput.toString() );
				TurnTakingManager.getInstance ().processInput(dmInput);
				
				
    		}
		} catch (JSONException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
		}	

    }

    @Override
    public void BestActionsPlanned (XmlRpcArray EnercitiesActionInfo_actionInfos) {
        logger.info ("Best actions: " + EnercitiesActionInfo_actionInfos);
        
    }

    @Override
    public void PredictedValuesUpdated (double[] values) {
    	 logger.info ("Predicted Values Updated:" + values.toString());

    }

}
