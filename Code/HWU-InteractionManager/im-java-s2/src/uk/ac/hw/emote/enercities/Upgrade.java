package uk.ac.hw.emote.enercities;

import java.util.ArrayList;

public class Upgrade {
	private String name;
	private int id;
	private float cost;
	private ArrayList<String> supportedStructures;
	private boolean isImplemented = false;
	
	public Upgrade copy()
	{
		Upgrade u = new Upgrade();
		
		u.name = this.name;
		u.id = this.id;
		u.cost = this.cost;
		u.supportedStructures = new ArrayList<String>(this.supportedStructures);
		
		return u;
	}
	
	public Upgrade ()
	{
		supportedStructures = new ArrayList<>();
	}
	
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public int getId() {
		return id;
	}
	public void setId(int id) {
		this.id = id;
	}
	public float getCost() {
		return cost;
	}
	public void setCost(float cost) {
		this.cost = cost;
	}
	public ArrayList<String> getSupportedStructures() {
		return supportedStructures;
	}
	public void setSupportedStructures(ArrayList<String> supportedStructures) {
		this.supportedStructures = supportedStructures;
	}

	public boolean isImplemented() {
		return isImplemented;
	}

	public void setImplemented(boolean isImplemented) {
		this.isImplemented = isImplemented;
	}
	
	
	
}
