package uk.ac.hw.emote.enercities;

import java.io.File;
import java.text.DecimalFormat;
import java.text.DecimalFormatSymbols;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Map;
import java.util.Map.Entry;
import java.util.HashMap;
import java.util.List;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.apache.log4j.Logger;
import org.json.JSONObject;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

public class Database {

	Logger logger = Logger.getLogger(GamePlayer.class.getName());
	
	private Map<String,Construction> constructions;
	private Map<String,Upgrade> upgrades;
	private Map<String,Policy> policies;
	
	private Map<Integer,Integer> constructionIDtoActionID;
	private Map<Integer,Integer> actionIDtoConstructionID;
	
	private Cell[][] grid;
	private int gridSizeX = 10;
	private int gridSizeY = 7;
	
	
	private String pathGrid = "EnercitiesAI/EnercitiesData/Level/grid.xml";
	private String pathStructures = "EnercitiesAI/EnercitiesData/Level/structures.xml";
	private String pathUpgrades = "EnercitiesAI/EnercitiesData/Level/upgrades.xml";
	private String pathStructureUpgrades = "EnercitiesAI/EnercitiesData/Level/structureupgrades.xml";
	private String pathSurfaces = "EnercitiesAI/EnercitiesData/Level/surfaces.xml";
	private String pathPolicies = "EnercitiesAI/EnercitiesData/Level/policies.xml";
	
	private String mapString;
	private String policyString;
	private String statesString;
	
	private String gameString;
	
	private void initConstructionIDtoActionID()
	{
		int cnt = 0;
		for (Entry<String,Construction> e : constructions.entrySet())
		{
			constructionIDtoActionID.put(e.getValue().getId(), cnt);
			cnt++;
		}
	}
	
	private void initActionIDtoConstructionID()
	{
		int cnt = 0;
		for (Entry<String,Construction> e : constructions.entrySet())
		{
			actionIDtoConstructionID.put(cnt, e.getValue().getId());
			cnt++;
		}
	}
	
	public Integer getActionIDfromConstructionID(Integer constructionID)
	{
		return constructionIDtoActionID.get(constructionID);
	}
	
	public Integer getConstructionIDfromActionID(Integer actionID)
	{
		return actionIDtoConstructionID.get(actionID);
	}
	
	public Vector2 getCellPositionFromActionID(Integer actionID)
	{
		return new Vector2((int)(actionID)/(gridSizeY),actionID%gridSizeY);
	}
	
	public int getActionIDFromCellPosition(int x, int y)
	{
		return x*gridSizeY + y;
	}
	
	public String getMapString() {
		return mapString;
	}

	public void setMapString(String mapString) {
		this.mapString = mapString;
	}

	public String getPolicyString() {
		return policyString;
	}

	public void setPolicyString(String policyString) {
		this.policyString = policyString;
	}

	public String getStatesString() {
		return statesString;
	}

	public void setStatesString(String statesString) {
		this.statesString = statesString;
	}

	public String getGameString() {
		return gameString;
	}

	public void setGameString(String gameString) {
		this.gameString = gameString;
	}
	
	public String genAndGetGameString(JSONObject gameUpdate)
	{
		createMapString();
		createPolicyString();
		createStateString(gameUpdate);
		concatenateStrings();
		
		return getGameString();
	}
	
	public String usableGameString(JSONObject gameUpdate)
	{
		String game = "";
		for(int i = 0; i < gridSizeX; i++)
		{
			for(int j = 0; j < gridSizeY; j++)
			{
				if (grid[i][j].getConstruction() == null)
				{
					game += ",0,0,0,0,0,0,0";
				}
				else
				{
					game += ","+grid[i][j].getConstruction().getId();
					int cnt = 0;
					for (Upgrade u : grid[i][j].getConstruction().getUpgrades() )
					{
						if (u.isImplemented())
						{
							game += ",1";//matrix[i][j][cnt] = u.getId();
						}
						else
						{
							game += ",0";
							//matrix[i][j][cnt] = 0;
						}
						cnt++;
					}
					for (int aa = cnt; aa < 6; aa++)
					{
						game += ",0";
					}
				}
			}

		}
		game += ",";
		int cnt = 0;
		for ( Entry<String,Policy> e : policies.entrySet())
		{
			if (e.getValue().isImplemented())
			{
				game += "1";//policyMatrix[cnt] = e.getValue().getId(); 
			}
			else
			{
				game += "0";
			}
			game += ",";
			cnt++;
		}
		game += "000,";
		game += "000,";
		try
		{
			if (gameUpdate.has("Population"))
			{
				game += gameUpdate.getDouble("Population")+",";
			}
			if (gameUpdate.has("Money"))
			{
				game += gameUpdate.getDouble("Money")+",";
			}
			if (gameUpdate.has("Oil"))
			{
				game += gameUpdate.getDouble("Oil")+",";
			}
			if (gameUpdate.has("PowerConsumption"))
			{
				game += gameUpdate.getDouble("PowerConsumption")+",";
			}
			if (gameUpdate.has("EnvironmentScore"))
			{
				game += gameUpdate.getDouble("EnvironmentScore")+",";
			}
			if (gameUpdate.has("EconomyScore"))
			{
				game += gameUpdate.getDouble("EconomyScore")+",";
			}
			if (gameUpdate.has("WellbeingScore"))
			{
				game += gameUpdate.getDouble("WellbeingScore");
			}
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}

		game = game.substring(1, game.length());
		
		//List<String> list = Arrays.asList(game.split(","));
		//game += "..."+list.size();
		//game += "..."+((10*7*7)+7+5);
		
		
		
		return game;
	}

	public int[] createPolicyMatrix()
	{
		int[] policyMatrix = new int[5];
		int cnt = 0;
		for ( Entry<String,Policy> e : policies.entrySet())
		{
			if (e.getValue().isImplemented())
			{
				policyMatrix[cnt] = e.getValue().getId(); 
			}
			cnt++;
		}
		return policyMatrix;
	}
	
	public void createPolicyString()
	{
		policyString = "policy: ";
		int cnt = 0;
		for ( Entry<String,Policy> e : policies.entrySet())
		{
			if (e.getValue().isImplemented())
			{
				policyString += "1";//policyMatrix[cnt] = e.getValue().getId(); 
			}
			else
			{
				policyString += "0";
			}
			policyString += ",";
			cnt++;
		}
		policyString += "|";
	}
	
	public void createStateString(JSONObject gameUpdate)
	{
		statesString = "state: ";
		try
		{
			if (gameUpdate.has("Population"))
			{
				statesString += gameUpdate.getDouble("Population")+",";
			}
			if (gameUpdate.has("Money"))
			{
				statesString += gameUpdate.getDouble("Money")+",";
			}
			if (gameUpdate.has("Oil"))
			{
				statesString += gameUpdate.getDouble("Oil")+",";
			}
			if (gameUpdate.has("PowerProduction"))
			{
				statesString += gameUpdate.getDouble("PowerProduction")+",";
			}
			if (gameUpdate.has("EnvironmentScore"))
			{
				statesString += gameUpdate.getDouble("EnvironmentScore")+",";
			}
			if (gameUpdate.has("EconomyScore"))
			{
				statesString += gameUpdate.getDouble("EconomyScore")+",";
			}
			if (gameUpdate.has("WellbeingScore"))
			{
				statesString += gameUpdate.getDouble("WellbeingScore");
			}
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
		statesString += "|";
	}
	
	public void concatenateStrings()
	{
		gameString = mapString+policyString+statesString;
	}
	
	public void createMapString()
	{
		mapString = "map: ";
		for(int i = 0; i < gridSizeX; i++)
		{
			for(int j = 0; j < gridSizeY; j++)
			{
				if (grid[i][j].getConstruction() == null)
				{
					mapString += ""+i+","+j+",0,0,0,0,0,0,0;";//matrix[i][j][0] = 0;
				}
				else
				{
					//matrix[i][j][0] = grid[i][j].getConstruction().getId();
					mapString += ""+i+","+j+","+grid[i][j].getConstruction().getId();
					int cnt = 0;
					for (Upgrade u : grid[i][j].getConstruction().getUpgrades() )
					{
						if (u.isImplemented())
						{
							mapString += ",1";//matrix[i][j][cnt] = u.getId();
						}
						else
						{
							mapString += ",0";
							//matrix[i][j][cnt] = 0;
						}
						cnt++;
					}
					for (int aa = cnt; aa < 6; aa++)
					{
						mapString += ",0";
					}
					mapString += ";";
				}
			}

		}
		mapString += "|";
		
		logger.info("MAP STRING BEGIN: ");
		logger.info(mapString);
		logger.info("MAP STRING END");
	}
	
	public int[][][] convertCellsToMatrix()
	{
		int[][][] matrix = new int[gridSizeX][gridSizeY][6];
		
		for(int i = 0; i < gridSizeX; i++)
		{
			for(int j = 0; j < gridSizeY; j++)
			{
				if (grid[i][j].getConstruction() == null)
				{
					matrix[i][j][0] = 0;
				}
				else
				{
					matrix[i][j][0] = grid[i][j].getConstruction().getId();
					
					int cnt = 0;
					for (Upgrade u : grid[i][j].getConstruction().getUpgrades() )
					{
						if (u.isImplemented())
						{
							matrix[i][j][cnt] = u.getId();
						}
						else
						{
							matrix[i][j][cnt] = 0;
						}
						cnt++;
					}
				}
			}

		}
		
		return matrix;
		
	}


	
	public Policy getPolicy(String s)
	{
		return policies.get(s);
	}
	
	public Cell getCell(int x, int y)
	{
		return grid[x][y];
	}
	
	public int getGridSizeX()
	{
		return gridSizeX;
	}
	
	public int getGridSizeY()
	{
		return gridSizeY;
	}

	
	public Database ()
	{
		constructions = new HashMap<>();
		upgrades = new HashMap<>();
		policies = new HashMap<>();
		setGrid(new Cell[gridSizeX][gridSizeY]);
		
		constructionIDtoActionID = new HashMap<>();
		actionIDtoConstructionID = new  HashMap<>();
		
		init();
		
	}
	
	public Map<String, Upgrade> getUpgrades() {
		return upgrades;
	}


	public void setUpgrades(Map<String, Upgrade> upgrades) {
		this.upgrades = upgrades;
	}


	public Map<String, Policy> getPolicies() {
		return policies;
	}


	public void setPolicies(Map<String, Policy> policies) {
		this.policies = policies;
	}

	
	
	private void readConstructions()
	{
		try {
			File fXmlFile = new File(pathStructures);
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder dBuilder = dbf.newDocumentBuilder();
			Document doc = dBuilder.parse(fXmlFile);
			doc.getDocumentElement().normalize();
			
			NodeList nList = doc.getElementsByTagName("structure");
			
			for (int i = 0; i < nList.getLength()-1; i++)
			{
				Node nNode = nList.item(i);
				Element element = (Element) nNode;
				
				Construction c = new Construction();
				c.setName(element.getAttribute("name").toLowerCase());
				String cat = element.getElementsByTagName("category").item(0).getTextContent();

				if (cat.equals("Environment"))
				{
					c.setRole("Environmentalist");
				}
				else if (cat.equals("Residential") || cat.equals("Energy"))
				{
					c.setRole("All");
				}
				else if (cat.equals("Wellbeing"))
				{
					c.setRole("Mayor");
				}
				else if (cat.equals("Economy"))
				{
					c.setRole("Economist");
				}
				int level = Integer.parseInt(element.getElementsByTagName("unlocklevel").item(0).getTextContent());
				c.setLevel(level);
				float price = Float.parseFloat(element.getElementsByTagName("buildingcost").item(0).getTextContent());
				c.setPrice(price);
				
				
				//System.out.println("DATABASE ITEM "+ i);
				//System.out.println(c.getName());
				//System.out.println(c.getPrice());
				//System.out.println(c.getRole());
				//System.out.println(c.getLevel());
				//System.out.println();
				
				constructions.put(c.getName(), c);
			}
			
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}

		Construction city = constructions.get("city_hall");
		city.canBeBuilt(false);
		city.setField(new ArrayList<String>());
		constructions.put("city_hall",city);
	}
	
	private void readStructures()
	{
		logger.info("init read Constructions");
		readConstructions();
		logger.info("END read structures");
		
		logger.info("init read Surfaces");
		readSurfaces();
		logger.info("END read Surfaces");
	}

	
	public void readMapGrid()
	{
		try {
			File fXmlFile = new File(pathGrid);
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder dBuilder = dbf.newDocumentBuilder();
			Document doc = dBuilder.parse(fXmlFile);
			doc.getDocumentElement().normalize();
			
			NodeList nListUnit = doc.getElementsByTagName("unit");
			NodeList nListSurface = doc.getElementsByTagName("surface");
			NodeList nListStructure = doc.getElementsByTagName("structure");
			
			for (int i = 0; i < nListUnit.getLength(); i++)
			{
				Node nNode = nListUnit.item(i);
				Element element = (Element) nNode;
				
				Cell c = new Cell();
				int x = Integer.parseInt(element.getAttribute("x"));
				int y = Integer.parseInt(element.getAttribute("y"));
				int l = Integer.parseInt(element.getAttribute("level"));
			
				
				Node nNodeSurface = nListSurface.item(i);
				Element elementSurface = (Element) nNodeSurface;
				String type = elementSurface.getAttribute("type").toLowerCase();
				
				Node nNodeStructure = nListStructure.item(i);
				Element elementStructure = (Element) nNodeStructure;
				String struct = elementStructure.getAttribute("name").toLowerCase();
				
				//private String type;
				//private int level;
				//private boolean playable;
				//private Construction construction = null;
				
				Cell cell = new Cell();
				cell.setType(type);
				cell.setLevel(l);
				
				if (struct.equals(""))
				{
					cell.setPlayable(true);
				}
				else
				{
					cell.setConstruction(constructions.get(struct));
					cell.setPlayable(false);
				}
				
				
				grid[x][y] = cell;
				
				//System.out.println("NUmber of Elements"+i);
				//System.out.println("DATABASE ITEM "+ 0);
				//System.out.println(x);
				//System.out.println(y);
				//System.out.println(l);
				//System.out.println(type);
				//System.out.println(struct);
				//System.out.println();
				
				
			}
			
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
	
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				if (grid[x][y] == null)
				{
					grid[x][y] = new Cell();
				}
			}
		}
		
		
		
	}
	
	public void printUpgrades(Construction c)
	{
		if (c.getUpgrades().size() != 0)
		{
			//logger.info("CONSTRUCTION UPGRADES:");
			for (Upgrade u : c.getUpgrades())
			{
				//logger.info("Upgrade id: " + u.getId());
			}
		}
	}
	
	public void printGrid()
	{
		logger.info("[MARCEL] GRID INIT");
		for (int i = 0; i < gridSizeX; i++)
		{
			for(int j = 0; j < gridSizeY; j++)
			{
				if (grid[i][j].getConstruction() !=  null)
				{
					//logger.info(i + " " + j + " " + grid[i][j].getConstruction().getId() + "\t");
					printUpgrades(grid[i][j].getConstruction());
				}
				else
				{
					//logger.info(i + " " + j + " 0\t");
				}
			}
		}
		logger.info("[MARCEL] GRID END");
	}
	
	public Upgrade getUpgrade(String s)
	{
		return upgrades.get(s);
	}
	
	private void readPolicy()
	{
		try {
			File fXmlFile = new File(pathPolicies);
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder dBuilder = dbf.newDocumentBuilder();
			Document doc = dBuilder.parse(fXmlFile);
			doc.getDocumentElement().normalize();
			
			NodeList nList = doc.getElementsByTagName("policy");
			
			for (int i = 1; i < nList.getLength(); i++)
			{
				Node nNode = nList.item(i);
				Element element = (Element) nNode;
				String name = element.getAttribute("type").toLowerCase();
				
				float cost = Float.parseFloat(element.getElementsByTagName("researchcost").item(0).getTextContent());
				//System.out.println();
				//System.out.println(name);
				//System.out.println(cost);
				//System.out.println();
			
				Policy p = new Policy();
				p.setName(name);
				p.setCost(cost);
				
				
				policies.put(name, p);
				
			}
			
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
	}

	private void readUpgrades()
	{
		readUpgradeFile();
		readStructureUpgradeFile();
	}
	
	private void readSurfaces()
	{
		try {
			File fXmlFile = new File(pathSurfaces);
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder dBuilder = dbf.newDocumentBuilder();
			Document doc = dBuilder.parse(fXmlFile);
			doc.getDocumentElement().normalize();
			
			NodeList nList = doc.getElementsByTagName("surface");
			
			for (int i = 1; i < nList.getLength(); i++)
			{
				Node nNode = nList.item(i);
				Element element = (Element) nNode;
				String name = element.getAttribute("name").toLowerCase();
				
				Node nodeBuild = element.getElementsByTagName("build-rules").item(0);
				Element elementBuild = (Element) nodeBuild;
				NodeList nodeStructures = elementBuild.getElementsByTagName("structure");
				
				//System.out.println("DATABASE ITEM "+ i);
				//System.out.println(name);
				ArrayList<String> structures = new ArrayList<>();
				for (int j = 0; j < nodeStructures.getLength(); j++)
				{
					//System.out.println(name);
					String supp = nodeStructures.item(j).getTextContent().toLowerCase();
					structures.add(supp);
					//System.out.println(supp);
					Construction c = constructions.get(supp);
					c.addField(name);
					constructions.put(supp, c);
				}
				//System.out.println();
						
				//System.out.println();
				//System.out.println(c.getRole());
				//System.out.println(c.getLevel());
				//System.out.println();
				
				
			}
			
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
		
	}
	
	private void readStructureUpgradeFile()
	{
		try {
			File fXmlFile = new File(pathStructureUpgrades);
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder dBuilder = dbf.newDocumentBuilder();
			Document doc = dBuilder.parse(fXmlFile);
			doc.getDocumentElement().normalize();
			
			NodeList nList = doc.getElementsByTagName("upgrade");
			
			for (int i = 1; i < nList.getLength(); i++)
			{
				Node nNode = nList.item(i);
				Element element = (Element) nNode;
				String name = element.getAttribute("name").toLowerCase();
				NodeList nodeStructures = element.getElementsByTagName("supportedstructure");
				//System.out.println("DATABASE ITEM "+ i);
				//System.out.println(name);
				ArrayList<String> structures = new ArrayList<>();
				for (int j = 0; j < nodeStructures.getLength(); j++)
				{
					//System.out.println(name);
					String supp = element.getElementsByTagName("supportedstructure").item(j).getTextContent().toLowerCase();
					structures.add(supp);
					//System.out.println(supp);
				}
				
				
				Upgrade u = upgrades.get(name);
				u.setSupportedStructures(structures);
				upgrades.put(name, u);
			
				//System.out.println();
				//System.out.println(c.getRole());
				//System.out.println(c.getLevel());
				//System.out.println();
				
				
			}
			
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
		
	}
	
	
	private void readUpgradeFile()
	{
		try {
			File fXmlFile = new File(pathUpgrades);
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder dBuilder = dbf.newDocumentBuilder();
			Document doc = dBuilder.parse(fXmlFile);
			doc.getDocumentElement().normalize();
			
			NodeList nList = doc.getElementsByTagName("upgrade");
			
			for (int i = 0; i < nList.getLength(); i++)
			{
				Node nNode = nList.item(i);
				Element element = (Element) nNode;
				
				Upgrade u = new Upgrade();
				
				u.setName(element.getAttribute("name").toLowerCase());
				
				DecimalFormatSymbols symbols =  new DecimalFormatSymbols();
				symbols.setDecimalSeparator(',');
				DecimalFormat format = new DecimalFormat("0.#");
				format.setDecimalFormatSymbols(symbols);
				float cost = format.parse(element.getElementsByTagName("researchcost").item(0).getTextContent()).floatValue();
				
				
				u.setCost(cost);
				//System.out.println("DATABASE ITEM "+ i);
				//System.out.println(u.getName());
				//System.out.println(u.getCost());
				//System.out.println(c.getRole());
				//System.out.println(c.getLevel());
				//System.out.println();
				
				upgrades.put(u.getName(), u);
			}
			
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}		

	}
	
	
	public void init()
	{
		//logger.info("init read structures");
		readStructures();
		//logger.info("END read structures");
		
		//logger.info("init read map grid");
		readMapGrid();
		//logger.info("END read structures");
		
		//logger.info("init read Upgrades");
		readUpgrades();
		//logger.info("End read structures");
		
		
		//logger.info("init read policy");
		readPolicy();
		//logger.info("End read structures");
		
		//logger.info("init SET ID Policy");
		setIDPolicy();
		//logger.info("END read structures");
		
		//logger.info("init SET ID Structure");
		setIDStructure();
		//logger.info("END Set id Strcuture");
		
		//logger.info("init read UPgrade");
		setIDUpgrade();
		//logger.info("End read Upgrade");
		
		//logger.info("init asscoaiting UPgrade");
		associateUpgradeToStructure();
		//logger.info("end asscoaiting UPgrade");
		
		initConstructionIDtoActionID();
		initActionIDtoConstructionID();
		
	}


	public Cell[][] getGrid() {
		return grid;
	}

	public void setCell(int posx, int posy, Construction c) {
		Construction cc = c.copy();
		cc.setIsBuilt(true);
		grid[posx][posy].setConstruction(cc);
		grid[posx][posy].setPlayable(false);
	}
	
	public void setGrid(Cell[][] grid) {
		this.grid = grid;
	}


	public Map<String, Construction> getConstructions() {
		return constructions;
	}


	public void setConstructions(Map<String, Construction> hashMap) {
		this.constructions = hashMap;
	}
	
	public void setIDStructure()
	{
		for (Entry<String,Construction> e : constructions.entrySet())
		{
			Construction c = constructions.get(e.getKey());
			c.setId(getStructureCode(e.getKey()));
			constructions.put(e.getKey(), c);
		}
	}
	
	public void setIDUpgrade()
	{
		for (Entry<String,Upgrade> e : upgrades.entrySet())
		{
			Upgrade u = upgrades.get(e.getKey());
			u.setId(getUpgradeCode(e.getKey()));
			upgrades.put(e.getKey(), u);
		}
	}
	
	public void associateUpgradeToStructure()
	{
		for (Entry<String,Upgrade> e : upgrades.entrySet())
		{
			Upgrade u = upgrades.get(e.getKey());
			for (String s : u.getSupportedStructures())
			{
				Construction c = getConstruction(s);
				Upgrade ucopy = u.copy();
				c.addUpgrade(ucopy);
			}
		}
	}
	
	public void setIDPolicy()
	{
		for (Entry<String,Policy> e : policies.entrySet())
		{
			Policy p = policies.get(e.getKey());
			p.setId(getPolicyCode(e.getKey()));
			policies.put(e.getKey(), p);
		}
	}
	
	public int getStructureCode(String structure)
	{
		int code = 0;
		switch (structure) {
		case "park": code = 1; break;
		case "forest": code = 2; break;
		case "wildlife_reserve": code = 3; break;
		case "suburban": code = 4; break;
		case "urban": code = 5; break;
		case "stadium": code = 6; break;
		case "light_industry": code = 7; break;
		case "heavy_industry": code = 8; break;
		case "commercial": code = 9; break;
		case "coal_plant": code = 10; break;	
		case "nuclear_plant": code = 11; break;
		case "windmills": code = 12; break;
		case "solar_plant": code = 13; break;
		case "hydro_plant": code = 14; break;
		case "city_hall": code = 15; break;
		case "coal_plant_small": code = 16; break;
		case "residential_tower": code = 17; break;
		case "super_solar": code = 18; break;
		case "super_windturbine": code = 19; break;
		case "nuclear_fusion": code = 20; break;
		case "market": code = 21; break;
		case "public_services": code = 22; break;
		}
		return code;
	}


	//////////////Function to return value of upgrade
	public int getUpgradeCode(String upgrade)
	{
		int code = 0;
		switch (upgrade) {
		case "solar_roofs": code = 1; break;
		case "improved_insulation": code = 2; break;
		case "rainwater_storage": code = 3; break;
		case "bus_stop": code = 4; break;
		case "rooftop_windmills": code = 5; break;
		case "thermal_storage": code = 6; break;
		case "birdhouse": code = 7; break;
		case "eco_roofs": code = 8; break;
		case "subway_station": code = 9; break;
		case "energy_efficient_lightbulbs": code = 10; break;	
		case "recycling_facilities": code = 11; break;
		case "co2_reduction_plan": code = 12; break;
		case "cradle_2_cradle": code = 13; break;
		case "bio_food": code = 14; break;
		case "veggie_food": code = 15; break;
		case "watch_tower": code = 16; break;
		case "forest_health_plan": code = 17; break;
		case "wildlife_preservation": code = 18; break;
		case "exhaust_scrubbers": code = 19; break;
		case "coal_washing": code = 20; break;
		case "co2_storage": code = 21; break;
		case "bigger_rotor_blades": code = 22; break;
		case "next_gen_solar_cells": code = 23; break;
		case "moving_solar_pads": code = 24; break;
		case "improved_uranium_storage": code = 25; break;
		case "uranium_recycling": code = 26; break;
		}
		return code;
	}


	//////////////Function to return value of upgrade
	public int getPolicyCode(String policy)
	{
		int code = 0;
		switch (policy) {
		case "co2_taxes": code = 1; break;
		case "electric_car_grid": code = 2; break;
		case "energy_education_program": code = 3; break;
		case "eco_tourism_program": code = 4; break;
		case "sustainable_technology_fund": code = 5; break;
		}
		return code;
	}
	
	public Construction getConstruction(String s)
	{
		return constructions.get(s);
	}


}