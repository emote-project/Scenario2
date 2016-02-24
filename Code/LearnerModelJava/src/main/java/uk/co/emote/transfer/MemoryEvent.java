package uk.co.emote.transfer;

import java.util.ArrayList;

public class MemoryEvent {

	
    private ArrayList<MemoryEventItem> memoryEventItems;
    private int learnerId;
    private int stepId;
    private int activityId;
    private int scenarioId;
    private int sessionId;
    private String reasonForUpdate;
    
    
	public ArrayList<MemoryEventItem> getMemoryEventItems() {
		return memoryEventItems;
	}
	public void setMemoryEventItems(ArrayList<MemoryEventItem> memoryEventItems) {
		this.memoryEventItems = memoryEventItems;
	}
	public int getLearnerId() {
		return learnerId;
	}
	public void setLearnerId(int learnerId) {
		this.learnerId = learnerId;
	}
	public int getStepId() {
		return stepId;
	}
	public void setStepId(int stepId) {
		this.stepId = stepId;
	}
	public int getActivityId() {
		return activityId;
	}
	public void setActivityId(int activityId) {
		this.activityId = activityId;
	}
	public int getScenarioId() {
		return scenarioId;
	}
	public void setScenarioId(int scenarioId) {
		this.scenarioId = scenarioId;
	}
	public int getSessionId() {
		return sessionId;
	}
	public void setSessionId(int sessionId) {
		this.sessionId = sessionId;
	}
	public String getReasonForUpdate() {
		return reasonForUpdate;
	}
	public void setReasonForUpdate(String reasonForUpdate) {
		this.reasonForUpdate = reasonForUpdate;
	}

	
}
