package uk.ac.hw.emote.enercities;

import java.util.ArrayList;
import java.util.Map.Entry;

import org.apache.log4j.Logger;
import org.eclipse.jetty.server.Server;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.sun.corba.se.impl.ior.GenericTaggedComponent;
import com.sun.xml.internal.ws.wsdl.ActionBasedOperationSignature;

import uk.ac.hw.emote.intman.dm.InteractionManager;

public class GamePlayer {
	
	
	Logger logger = Logger.getLogger(GamePlayer.class.getName());
	
	Database db;
	
	//Object that connects with javascript
	//wsHandler.sendMessage("message"); to send message to Javascript
	MyWebSocketHandler wsHandler;
	
	int currLevel = 1;
	
	//Reward value to give to the network
	int reward = 1;
	
	double currWellbeingScore;
	
	//Variables that were used to give a reward to the network based on the population
	double population;
	double difPopulation = 0;
	
	
	private Boolean setToRandom = true;
	
	//Constructor
	public GamePlayer(){
		
		
		//logger.info("Initialiazing Database");
		db = new Database();
		logger.info("Database loaded");
		
		wsHandler = null;
		
		if (!this.setToRandom){
		
			//Server that will connect to javascript
			Server server = new Server(8082);
			
			//Initialize websocket
	        wsHandler = new MyWebSocketHandler();
	        server.setHandler(wsHandler);
			try
			{
				server.start();
			}
			catch(Exception e)
			{
				e.printStackTrace();
			}
	        logger.info("WAITING RESPONSE FROM CONVNETJS");
			waitResponse(200);
			wsHandler.setResponse(false);
			logger.info(wsHandler.getCurrString());
			logger.info("GOT RESPONSE FROM CONVNETJS");
		}
        
	}
		
	
	public void setModeRandom(Boolean r){
		this.setToRandom = r;
	}
	
	//the main
	public JSONObject run(JSONObject gameUpdate) throws JSONException{
		
		//GamePlayer update:{"fromModule":"enercities","gameStructure":"Coal_Plant_Small","cellX":2,"cellY":1,"gameStructureConstructed":"true"}
		//GamePlayer update:{"fromModule":"enercities","upgrade":"improved_insulation","cellX":5,"cellY":1,"structureUpgraded":"true"}
		//GamePlayer update:{"fromModule":"enercities","policyImplemented":"true","policy":"sustainable_technology_fund"}
		//GamePlayer update:{"fromModule":"enercities","turnSkipped":"true"}
		//GamePlayer update:{"fromModule":"enercities","EnvironmentScore":2,"EconomyScore":2,"gameTurnHolder":"Environmentalist","WellbeingScore":2,"turnChanged":"true","MoneyEarning":2,"PowerConsumption":0,"Money":104,"Oil":1800,"Population":1,"TargetPopulation":15,"Level":1,"PowerProduction":4,"GlobalScore":6}
		logger.info("GamePlayer update:" + gameUpdate.toString());
		
		// here u get all the enercities game updates from IM.
		// use it to update ur learning agent
		
		//get population value
		if (gameUpdate.has("Population"))
		{
			double newPopulation = gameUpdate.getDouble("Population");
			difPopulation = newPopulation - population;
			population = newPopulation;
		}
		
		//get wellbeing score
		if (gameUpdate.has("WellbeingScore"))
		{
			currWellbeingScore = gameUpdate.getDouble("WellbeingScore");
		}
		
		
		//check if reached next level
		if (gameUpdate.has("reachedNewLevel") && gameUpdate.getString("reachedNewLevel").equals("true"))
		{
			if (!this.setToRandom){	wsHandler.sendMessage("reward="+1000);}
			//reset our gride
			db.setGrid(new Cell[db.getGridSizeX()][db.getGridSizeY()]);
			db.readMapGrid();
			if (!this.setToRandom){	wsHandler.sendMessage("episode=1"); }
		}
		
		//max episodes reached
		if (gameUpdate.has("maxEpisodeGameTurnsReached") && gameUpdate.getString("maxEpisodeGameTurnsReached").equals("true"))
		{
			if (!this.setToRandom){	wsHandler.sendMessage("reward="+ (-1000));}
			db.setGrid(new Cell[db.getGridSizeX()][db.getGridSizeY()]);
			db.readMapGrid();
			if (!this.setToRandom){	wsHandler.sendMessage("episode=1");}
		}
		
		//
		if (gameUpdate.has("upgrade") && gameUpdate.getString("structureUpgraded").equals("true"))
		{
			//Enter upgrade ");
			String upgrade = gameUpdate.getString("upgrade").toLowerCase();
			
			//Enter upgrade NAME");
			int cellx = gameUpdate.getInt("cellX");
			int celly = gameUpdate.getInt("cellY");
			
			//Enter upgrade CELLS");
			Upgrade u = db.getUpgrade(upgrade).copy();
			u.setImplemented(true);
			
			//Enter upgrade UPGRADE COPY");

			Construction c = db.getCell(cellx, celly).getConstruction();
			
			for (Upgrade u_c : c.getUpgrades())
			{
				if (u_c.getName().equals(u.getName()))
				{
					u_c.setImplemented(true);
				}
			}
			
			c.addUpgrade(u);
			
			
			db.setCell(cellx, celly, c);
			
			db.printGrid();
		}
		
		if (gameUpdate.has("policyImplemented") && gameUpdate.getString("policyImplemented").equals("true"))
		{
			String policy = gameUpdate.getString("policy").toLowerCase();
			Policy p = db.getPolicy(policy);	
			p.setImplemented(true);
		}
		
		
		if (gameUpdate.has("gameStructure") && gameUpdate.has("gameStructureConstructed") && gameUpdate.getString("gameStructureConstructed").equals("true"))
		{
			
			String structure = gameUpdate.getString("gameStructure").toLowerCase();
			//int code = db.getStructureCode(structure);
			
			//SOMETHING CONSTRUCTED FROM OTHER PLAYERS");
			
			int x = gameUpdate.getInt("cellX");
			int y = gameUpdate.getInt("cellY");		
			
			Construction c =db.getConstruction(structure).copy();
			logger.info("Structure added" +  c.getId());
			c.setBuilt(true);
			db.setCell(x, y, c);
			db.getCell(x, y).setBuilt(true);
			logger.info("Printing grid");
			db.printGrid();
		}
		
		if ( (gameUpdate.has("turnChanged") && gameUpdate.getString("turnChanged").equals("true")) || (gameUpdate.has("previousMoveUnsuccessful") && gameUpdate.getString("previousMoveUnsuccessful").equals("true")) )
		{
			logger.info("Entrou Select action");
			/// need a game action decision here..
			JSONObject gpOut = new JSONObject();
			
			int level = gameUpdate.getInt("Level");
			String role = gameUpdate.getString("gameTurnHolder");
			float money = (float)(gameUpdate.getDouble("Money"));

			logger.info("GamePlayer update:" + gameUpdate.toString());
			
			//Our AI
			if (!this.setToRandom && role.equals("Mayor"))
			{
				//sent message to javascript
				wsHandler.sendMessage("brain=0|state="+db.usableGameString(gameUpdate)+"|actions0=0,1");
				
				//wsHandler.sendMessage("Number of Constructions" +db.getConstructions().size());
				//System.out.println("Number of Constructions" +db.getConstructions().size());
				//wsHandler.sendMessage("Number of Upgrades" +db.getUpgrades().size());
				//System.out.println("Number of Upgrades" +db.getUpgrades().size());
				
				//WAITING RESPONSE 1500");
				waitResponse(1500);
				
				//Got Response 1500");
				
				String responseAction = wsHandler.getCurrString();
				wsHandler.sendMessage("reward="+ (-1));
				
				//RESPONSE FROM JAVASCRIPT In GAMEPLAYER Brain 0: "+responseAction);
				
				
				//Skip
				if (responseAction.equals("0"))
				{
					//SKIP FROM JAVASCRIPT");
					
					gpOut.put("bestActionIdentified", "true");
					gpOut.put("bestActionType", 0);
					gpOut.put("bestActionSubtype", 0);
					gpOut.put("bestActionPosX", 0);
					gpOut.put("bestActionPosY", 0);
					
					return gpOut;
					
				}
				//Build Structure
				else if (responseAction.equals("1"))
				{
					//BUILD FROM JAVASCRIPT");
					String possibleLocations = getPossibleLocations(level);
					
					if (possibleLocations.isEmpty())
					{
						logger.info("No possible locations: SKIP");
						
						gpOut.put("bestActionIdentified", "true");
						gpOut.put("bestActionType", 0);
						gpOut.put("bestActionSubtype", 0);
						gpOut.put("bestActionPosX", 0);
						gpOut.put("bestActionPosY", 0);
						
						return gpOut;
					}
					
					wsHandler.sendMessage("brain=1|state="+db.usableGameString(gameUpdate)+"|actions1="+possibleLocations);
					
					logger.info("WAIT RESPONSE1 1500");
					waitResponse(1500);
					logger.info("GOT RESPONSE JAVASCRIPT 1500");
					responseAction = wsHandler.getCurrString();
					
					logger.info("RESPONSE GAMEPLAYER Position Brain1: "+responseAction);
					
					Vector2 pos = db.getCellPositionFromActionID(Integer.parseInt(responseAction));
					
					logger.info("POSITIONS: "+pos.getX()+","+pos.getY());
					
					if (pos.getX() == 0 && pos.getY() == 0)
					{
						logger.info("No possible locations: SKIP");
						
						gpOut.put("bestActionIdentified", "true");
						gpOut.put("bestActionType", 0);
						gpOut.put("bestActionSubtype", 0);
						gpOut.put("bestActionPosX", 0);
						gpOut.put("bestActionPosY", 0);
						
						return gpOut;
					}
					
					String possibleStructures = getPossibleStructures(level, role, money, pos.getX(), pos.getY());
					
					if (possibleStructures.isEmpty())
					{
						logger.info("No possible structures: SKIP");
						
						gpOut.put("bestActionIdentified", "true");
						gpOut.put("bestActionType", 0);
						gpOut.put("bestActionSubtype", 0);
						gpOut.put("bestActionPosX", 0);
						gpOut.put("bestActionPosY", 0);
						
						return gpOut;
					}
					
					wsHandler.sendMessage("brain=2|state="+db.usableGameString(gameUpdate)+"|actions2="+possibleStructures);
					logger.info("WAIT RESPONSE 1500");
					waitResponse(1500);
					logger.info("GOT RESPONSE JAVASCRIPT 1500");
					responseAction = wsHandler.getCurrString();
					logger.info("RESPONSE Gameplayer Structure: "+responseAction);

					int structureID = db.getConstructionIDfromActionID(Integer.parseInt(responseAction));
					logger.info("Structure ID: "+structureID);
					
					logger.info("PLAY");
					gpOut.put("bestActionIdentified", "true");
					gpOut.put("bestActionType", 1);
					gpOut.put("bestActionSubtype", structureID);
					gpOut.put("bestActionPosX", pos.getX());
					gpOut.put("bestActionPosY", pos.getY());
					
					return gpOut;
				}

			} 
			//Other players: Play as Dummy agent
			else
			{
				
				System.out.println("Ready!");
				Action action = selectAction(level,role,money);
				
				//logger.info("[MARCEL] best action: " + action.getBestActionIdentified());
				//logger.info("[MARCEL] Action: " + action.getAction());
				//logger.info("[MARCEL] Subaction: " + action.getSubaction());
				//logger.info("[MARCEL] PosX: " + action.getPosx());
				//logger.info("[MARCEL] posY: " + action.getPosy());
				
				gpOut.put("bestActionIdentified", action.getBestActionIdentified());
				gpOut.put("bestActionType", action.getAction());
				gpOut.put("bestActionSubtype", action.getSubaction());
				gpOut.put("bestActionPosX", action.getPosx());
				gpOut.put("bestActionPosY", action.getPosy());
	
				if (action.getAction() == 2 ) //(i.e. upgrade) then use the following to communicate the other two upgrade actions
				{
					
					gpOut.put("bestActionType2", action.getAction2());
					gpOut.put("bestActionSubtype2", action.getSubaction2());
					gpOut.put("bestActionPosX2", action.getPosx2());
					gpOut.put("bestActionPosY2", action.getPosy2());
									
					gpOut.put("bestActionType3", action.getAction3());
					gpOut.put("bestActionSubtype3", action.getSubaction3());
					gpOut.put("bestActionPosX3", action.getPosx3());
					gpOut.put("bestActionPosY3", action.getPosy3());
					
				}
				return gpOut;
			}	
			
		}
		return null;
	}
	
	//Function that selects the location for the dummy agent 	
		public Vector2 selectLocation(int level)
		{
			int X = db.getGridSizeX();
			int Y = db.getGridSizeY();
			
			//for all cells
			ArrayList<Vector2> positions = new ArrayList<>();
			for (int x = 0; x < X; x++)
			{
				for (int y = 0; y < Y; y++)
				{
					//add cell that is for the current level and it is playable
					if (db.getCell(x, y).getLevel() == level &&
						db.getCell(x, y).isPlayable())
					{
						Vector2 pos = new Vector2();
						pos.setX(x);
						pos.setY(y);
						positions.add(pos);
						//logger.info("Added possible cell");
					}
				}
			}
			//return a random position
			return positions.get((int)(Math.floor(Math.random()*positions.size())));
		}
		
		//this function gets all the possible locations to the Deep Learning AI
		//It returns a string separated with coma ',' with all possible locations
		//It uses a sequential location (0 => (0,0), 1 => (0,1) )
		public String getPossibleLocations(int level)
		{
			int X = db.getGridSizeX();
			int Y = db.getGridSizeY();
			
			ArrayList<Vector2> positions = new ArrayList<>();
			for (int x = 0; x < X; x++)
			{
				for (int y = 0; y < Y; y++)
				{
					if (db.getCell(x, y).getLevel() == level &&
						db.getCell(x, y).isPlayable())
					{
						Vector2 pos = new Vector2();
						pos.setX(x);
						pos.setY(y);
						positions.add(pos);
						logger.info("Added possible cell");
					}
				}
			}
			
			String possibleLocations = "";
			for (Vector2 v : positions)
			{
				possibleLocations += String.valueOf(db.getActionIDFromCellPosition(v.getX(), v.getY()))+",";
			}
			
			//removing last coma
			possibleLocations = possibleLocations.substring(0,possibleLocations.length()-1);
			
			return possibleLocations;
		}

		//Select the action based on the money, role and level of the Dummy AI
		//IN THIS FUNCTION WE CAN ADD OR REMOVE THE POSSIBLE ACTIONS
		//SEE BELOW
		public Action selectAction(int level, String role, float money)
		{
			ArrayList<Integer> possibleActions = new ArrayList<>();
			possibleActions.add(0); //Skip
			possibleActions.add(1); //build structure
			//possibleActions.add(2); //upgrade
			//possibleActions.add(4); //policy
		
			Action action = new Action();
			
			int mainAction = possibleActions.get((int)(Math.floor(Math.random()*possibleActions.size())));
			
			//shows main action
			//logger.info("Main Action" + mainAction);
			
			action.setAction(mainAction);
			
			switch (mainAction) {
			case 0:  //skip
				return action;
			case 1:  //build structure;
				int a = 0;
				int cnt = 0;
				Vector2 pos = new Vector2();
				
				//Sometimes it cannot build anything in the current location
				//So we try 10 times to find to find a location where we can build
				//if it does not work, it skips
				while (a==0 && cnt < 10)
				{
					pos = selectLocation(level);
					a = selectStructure(level, role, money, pos.getX(), pos.getY());
					cnt++;
				}
				if (cnt >= 10)
				{
					action.setAction(0);
					return action;
				}
				action.setPosx(Integer.toString(pos.getX()));
				action.setPosy(Integer.toString(pos.getY()));
				action.setSubaction(a);	
				logger.info("SKIP!");
				return action;
			case 2:  //upgrade structure;
				Vector2 positionStructure = new Vector2();
				///////// First upgrade
				//logger.info("First upgrade");
				int subA = 0;
				int cont = 0;
				while (subA == 0 && cont < 10)
				{
					positionStructure = selectCellToUpgrade(role);
					subA = selectUpgradeToStructure(positionStructure.getX(),positionStructure.getY(),money);
					cont++;
				}
				if (cont >= 10)
				{
					logger.info("SKIP!");
					action.setAction(0);
					return action;
				}
				action.setPosx(Integer.toString(positionStructure.getX()));
				action.setPosy(Integer.toString(positionStructure.getY()));
				action.setSubaction(subA);			
				logger.info("PositionX: " + positionStructure.getX() );
				logger.info("PositionY: " + positionStructure.getY() );
				logger.info("SubAction: " + subA );
				
				/////////Second upgrade
				if (subA != 0)
				{
					Vector2 positionStructure2 = new Vector2();
					action.setAction2(mainAction);
					subA = 0;
					cont = 0;
					while (subA == 0 && cont < 10)
					{
						positionStructure2 = selectCellToUpgrade(role);
						subA = selectUpgradeToStructure(positionStructure2.getX(),positionStructure2.getY(),money);
						cont++;
					}
					if (cont >= 10)
					{
						action.setAction2(0);
						return action;
					}
					action.setPosx2(Integer.toString(positionStructure2.getX()));
					action.setPosy2(Integer.toString(positionStructure2.getY()));
					action.setSubaction2(subA);
					logger.info("PositionX: " + positionStructure2.getX() );
					logger.info("PositionY: " + positionStructure2.getY() );
					logger.info("SubAction: " + subA );
					
					//////// Third upgrade
					if (subA != 0)
					{
						Vector2 positionStructure3 = new Vector2();
						action.setAction2(mainAction);
						subA = 0;
						cont = 0;
						while (subA == 0 && cont < 10)
						{
							positionStructure3 = selectCellToUpgrade(role);
							subA = selectUpgradeToStructure(positionStructure3.getX(),positionStructure3.getY(),money);
							cont++;
						}
						if (cont >= 10)
						{
							action.setAction3(0);
							return action;
						}
						action.setPosx3(Integer.toString(positionStructure3.getX()));
						action.setPosy3(Integer.toString(positionStructure3.getY()));
						action.setSubaction3(subA);
						logger.info("PositionX: " + positionStructure3.getX() );
						logger.info("PositionY: " + positionStructure3.getY() );
						logger.info("SubAction: " + subA );
					}
					
				}
					
				return action;
			case 4:  //implement policy;
				int aa = selectPolicy(money);
				if (aa != 0)
				{
					action.setSubaction(aa);
					logger.info("policy: "+ aa);
				}
				else
				{	
					action.setAction(0);
					logger.info("SKIP!");
				}
				
				return action;
			}
			
			return action;
		}
		
		//Select structure to upgrade (Don't upgrade)
		//choose one random Structure to be upgraded
		private Vector2 selectCellToUpgrade(String role)
		{
			ArrayList<Vector2> possibleCells = new ArrayList<>();
			for (int i = 0; i < db.getGridSizeX(); i++)
			{
				for(int j = 0; j < db.getGridSizeY(); j++)
				{
					if (db.getCell(i, j).isBuilt())
					{
						String r = db.getCell(i, j).getConstruction().getRole();
						if (r.equals("All") || r.equals("role"))
						{
							possibleCells.add(new Vector2(i, j));
						}
					}
				}
			}
			
			return possibleCells.get((int)(Math.floor(Math.random()*possibleCells.size())));				
		}

		//create a upgrade for the structure selected
		private int selectUpgradeToStructure(int x, int y, float money) 
		{
			Construction c = db.getCell(x, y).getConstruction();
			ArrayList<Upgrade> possibleUpgrades = new ArrayList<>();
			for (int i = 0; i < c.getUpgrades().size(); i++)
			{
				for (Upgrade u : c.getUpgrades())
				{
					if (!u.isImplemented() && u.getCost()<money)
					{
						possibleUpgrades.add(u);
					}
				}
			}
				
			if (!possibleUpgrades.isEmpty())
			{
				Upgrade up = possibleUpgrades.get((int)(Math.floor(Math.random()*possibleUpgrades.size())));
				return up.getId();	
			}
			else
			{
				return 0;
			}
		}

		//Select policy based on the money
		public int selectPolicy(float money)
		{
			ArrayList<Policy> possiblePolicies = new ArrayList<>();
			for (Entry<String,Policy> e : db.getPolicies().entrySet())
			{
				Policy p = db.getPolicies().get(e.getKey());
				if (!p.isImplemented() && p.getCost()<money)
				{
					possiblePolicies.add(p);
				}
			}

			if (!possiblePolicies.isEmpty())
			{
				Policy p = possiblePolicies.get((int)(Math.floor(Math.random()*possiblePolicies.size())));
				p.setImplemented(true);
				return p.getId();
			}
			else
			{
				return 0;
			}	
			
		}
		
		//pick a random structure to be built
		public int selectStructure(int level, String role, float money, int posX, int posY)
		{
			ArrayList<Construction> possibleStructures = new ArrayList<>();
			String field = db.getCell(posX, posY).getType();
			for (Entry<String,Construction> e : db.getConstructions().entrySet())
			{
				Construction c = db.getConstructions().get(e.getKey());
				if (c.getLevel() == level)
				{				
					if (c.getRole().equals("All") || c.getRole().equals(role))
					{
						if (c.getPrice()<money && c.checkCanBeBuilt())
						{

							if (isInsideArray(c.getField(),field))
							{
								possibleStructures.add(c);
							}
						}
					}
				}
			}
			
			if (!possibleStructures.isEmpty())
			{
				//remove manually the coal plant small to be removed from positions
				// 4,1 and 5,1 (it cannot be built by the game
				//but not specified in the xml files
				if ( (posX == 4 && posY ==1) || (posX == 5 && posY == 1))
				{
					for (int i = 0; i < possibleStructures.size(); i++)
					{
						if (possibleStructures.get(i).getName().equals("coal_plant_small") ||
						    possibleStructures.get(i).getName().equals("coal_plant") )
						{
							possibleStructures.remove(i);
						}
					}
					
				}
				
				Construction a = possibleStructures.get((int)(Math.floor(Math.random()*possibleStructures.size())));
				db.setCell(posX, posY, a);
				
				return possibleStructures.get((int)(Math.floor(Math.random()*possibleStructures.size()))).getId();
			}
			else
			{
				logger.info("SKIP!");
				return 0;
			}	
		}
		
		//get a string with all possible strucutres to be built. returns a list of ids separeted by coma
		public String getPossibleStructures(int level, String role, float money, int posX, int posY)
		{
			ArrayList<Construction> possibleStructures = new ArrayList<>();
			String field = db.getCell(posX, posY).getType();
			for (Entry<String,Construction> e : db.getConstructions().entrySet())
			{
				Construction c = db.getConstructions().get(e.getKey());
				if (c.getLevel() == level)
				{				
					if (c.getRole().equals("All") || c.getRole().equals(role))
					{
						if (c.getPrice()<money && c.checkCanBeBuilt())
						{

							if (isInsideArray(c.getField(),field))
							{
								possibleStructures.add(c);
							}
						}
					}
				}
			}
			if (!possibleStructures.isEmpty())
			{
				//remove manually coal_plant and coal_plant_small for certain positions
				if ( (posX == 4 && posY ==1) || (posX == 5 && posY == 1))
				{
					for (int i = 0; i < possibleStructures.size(); i++)
					{
						if (possibleStructures.get(i).getName().equals("coal_plant_small") ||
						    possibleStructures.get(i).getName().equals("coal_plant") )
						{
							possibleStructures.remove(i);
						}
					}	
				}
				
				String possibleStructuresString = "";
				
				for (Construction c : possibleStructures)
				{
					possibleStructuresString += db.getActionIDfromConstructionID(c.getId())+",";
				}
				possibleStructuresString = possibleStructuresString.substring(0,possibleStructuresString.length()-1);
				
				return possibleStructuresString;
			}
			else
			{
				//logger.info("Possible structures is empty, send skip!");
				return "";
			}	
		}
		
		//function that check if a string is inside an arraylist 
		public boolean isInsideArray(ArrayList<String> a,String s)
		{
			for (String str : a)
			{
					
				if (str.equals(s))
				{
					return true;
				}
			}
			return false;
		}
		
		//function that waits response from Javascript
		public void waitResponse(int miliseconds)
		{
			do
			{
				System.out.println("Response from GamePlayer: "+wsHandler.getResponse());
				try {
					Thread.sleep(miliseconds);
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}while (!wsHandler.getResponse());
			wsHandler.setResponse(false);
		}
		
		//function to wait some time
		public void waitSomeTime(int miliseconds)
		{
			try {
				Thread.sleep(miliseconds);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		
}
