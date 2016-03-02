package uk.ac.hw.emote.enercities;

public class Policy {
	private String name;
	private float cost;
	private int id;
	private boolean isImplemented = false;
	
	
	public float getCost() {
		return cost;
	}
	public void setCost(float cost) {
		this.cost = cost;
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
	public boolean isImplemented() {
		return isImplemented;
	}
	public void setImplemented(boolean isImplemented) {
		this.isImplemented = isImplemented;
	}
}
