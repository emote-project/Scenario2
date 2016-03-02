package uk.ac.hw.emote.enercities;

import java.util.ArrayList;

public class Construction {
	private String name;
	private int id;
	private int level;
	private String role;
	private float price = 9999999;
	private int posX = -1;
	private int posY = -1;
	private boolean isBuilt = false;
	private ArrayList<String> field;
	private boolean canBeBuilt = true;
	private ArrayList<Upgrade> upgrades;
	
	public void canBeBuilt(boolean canBeBuilt)
	{
		this.canBeBuilt = canBeBuilt;
	}
	
	public boolean checkCanBeBuilt()
	{
		return canBeBuilt;
	}
	
	public void setIsBuilt(boolean is)
	{
		isBuilt = is;
	}
	
	public Construction copy()
	{
		Construction c = new Construction();
		c.id = this.id;
		c.name = this.name;
		c.level = this.level;
		c.role = this.role;
		c.price = this.price;
		c.posX = this.posX;
		c.posY = this.posY;
		c.isBuilt = this.isBuilt;
		c.field = new ArrayList<String>(this.field);
		c.canBeBuilt = this.canBeBuilt;
		c.upgrades = new ArrayList<Upgrade>(this.upgrades);
		return c;
	}
	
	public Construction ()
	{
		field = new ArrayList<>();
		upgrades = new ArrayList<Upgrade>();
	}
	
	public void addUpgrade(Upgrade u)
	{
		upgrades.add(u);
	}
	
	public boolean hasUpgrade(Upgrade upgrade)
	{
		for (Upgrade u : upgrades)
		{
			if (u.getId() == upgrade.getId())
			{
				return true;
			}
		}
		return false;
	}
	
	public ArrayList<Upgrade> getUpgrades()
	{
		return upgrades;
	}

	
	
	public Construction (String name, int level, String role, float price, ArrayList<String> field)
	{
		this.name = name;
		this.role = role;
		this.level = level;
		this.price = price;
		this.setField(field);
	}
	
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public int getLevel() {
		return level;
	}
	public void setLevel(int level) {
		this.level = level;
	}
	public String getRole() {
		return role;
	}
	public void setRole(String role) {
		this.role = role;
	}
	public float getPrice() {
		return price;
	}
	public void setPrice(float price) {
		this.price = price;
	}
	public int getPosX() {
		return posX;
	}
	public void setPosX(int posX) {
		this.posX = posX;
	}
	public int getPosY() {
		return posY;
	}
	public void setPosY(int posY) {
		this.posY = posY;
	}
	public boolean isBuilt() {
		return isBuilt;
	}
	public void setBuilt(boolean isBuilt) {
		this.isBuilt = isBuilt;
	}

	public ArrayList<String> getField() {
		return field;
	}

	public void setField(ArrayList<String> field) {
		this.field = field;
	}
	
	public void addField(String field)
	{
		this.field.add(field);
	}


	public int getId() {
		return id;
	}


	public void setId(int id) {
		this.id = id;
	}
	
	

}
