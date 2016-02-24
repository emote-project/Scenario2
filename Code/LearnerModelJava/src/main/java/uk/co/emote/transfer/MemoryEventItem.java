package uk.co.emote.transfer;

import java.util.ArrayList;
import java.util.HashMap;


public class MemoryEventItem {
	
	
	private String name;
	private String category;
	private String subcategory;
	private ArrayList<String> tagNames;
	private ArrayList<String> tagValues;
	/*private String working;
	private String shortTerm;
	private String longTerm;
	private Double finalvaluePreviousSession;
	private Double highestValuePreviousSessions;
	private Double lowestFinalValuePreviousSessions;
	private HashMap<String, String> learnerModels;
    */
	
	
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public ArrayList<String> getTagNames() {
		return tagNames;
	}
	public void setTagNames(ArrayList<String> tagNames) {
		this.tagNames = tagNames;
	}
	public ArrayList<String> getTagValues() {
		return tagValues;
	}
	public void setTagValues(ArrayList<String> tagValues) {
		this.tagValues = tagValues;
	}
	public String getCategory() {
		return category;
	}
	public void setCategory(String category) {
		this.category = category;
	}
	public String getSubcategory() {
		return subcategory;
	}
	public void setSubcategory(String subcategory) {
		this.subcategory = subcategory;
	}
	
}
