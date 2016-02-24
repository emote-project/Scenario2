package uk.co.emote;

import java.util.List;


public class LearnerStateInfo {

	private List<ThalamusCompetencyItem> competencyItems;
	private String mapEventId;
	private Integer learnerId;
	private String learnerName;
	private Integer stepId;
	private Integer activityId;
	private Integer scenarioId;
	private Integer sessionId;
	private String reasonForUpdate;
	private Boolean correct;
	
	public List<ThalamusCompetencyItem> getCompetencyItems() {
		return competencyItems;
	}
	public void setCompetencyItems(List<ThalamusCompetencyItem> competencyItems) {
		this.competencyItems = competencyItems;
	}
	public String getMapEventId() {
		return mapEventId;
	}
	public void setMapEventId(String mapEventId) {
		this.mapEventId = mapEventId;
	}
	public Integer getLearnerId() {
		return learnerId;
	}
	public void setLearnerId(Integer learnerId) {
		this.learnerId = learnerId;
	}
	public Integer getStepId() {
		return stepId;
	}
	public void setStepId(Integer stepId) {
		this.stepId = stepId;
	}
	public Integer getActivityId() {
		return activityId;
	}
	public void setActivityId(Integer activityId) {
		this.activityId = activityId;
	}
	public Integer getScenarioId() {
		return scenarioId;
	}
	public void setScenarioId(Integer scenarioId) {
		this.scenarioId = scenarioId;
	}
	public Integer getSessionId() {
		return sessionId;
	}
	public void setSessionId(Integer sessionId) {
		this.sessionId = sessionId;
	}
	public String getReasonForUpdate() {
		return reasonForUpdate;
	}
	public void setReasonForUpdate(String reasonForUpdate) {
		this.reasonForUpdate = reasonForUpdate;
	}
	public Boolean getCorrect() {
		return correct;
	}
	public void setCorrect(Boolean correct) {
		this.correct = correct;
	}
	public String getLearnerName() {
		return learnerName;
	}
	public void setLearnerName(String learnerName) {
		this.learnerName = learnerName;
	}

	@Override public String toString() {
	    StringBuilder result = new StringBuilder();
	    String NEW_LINE = System.getProperty("line.separator");

	    result.append(this.getClass().getName() + " Object {" + NEW_LINE);
	    result.append(" thalamusLearnerId: " + learnerId + NEW_LINE);
	    result.append(" stepId: " + stepId + NEW_LINE);
	    result.append(" sessionId: " + sessionId + NEW_LINE );

	    if(competencyItems!=null && !competencyItems.isEmpty())
	    {
	    	result.append(" Answer attempt items:" + NEW_LINE );
	    	for(ThalamusCompetencyItem competencyItem : competencyItems)
	    	{
	    		result.append(competencyItem.toString() + NEW_LINE );
	    	}
	    	
	    }
	    result.append("}");

	    return result.toString();
	  }
}
