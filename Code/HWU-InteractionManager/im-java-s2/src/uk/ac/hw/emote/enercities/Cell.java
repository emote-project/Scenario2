package uk.ac.hw.emote.enercities;

public class Cell {
	private String type;
	private int level = 0;
	private boolean playable = true;
	private boolean isBuilt = false;
	private Construction construction = null;
	
	public Cell()
	{
		setPlayable(false);
	}
	
	
	public String getType() {
		return type;
	}
	public void setType(String type) {
		this.type = type;
	}
	public int getLevel() {
		return level;
	}
	public void setLevel(int level) {
		this.level = level;
	}


	public boolean isPlayable() {
		return playable;
	}


	public void setPlayable(boolean playable) {
		this.playable = playable;
	}


	public Construction getConstruction() {
		return construction;
	}


	public void setConstruction(Construction construction) {
		this.construction = construction;
	}


	public boolean isBuilt() {
		return isBuilt;
	}


	public void setBuilt(boolean is) {
		this.isBuilt = is;
	}

}
