package uk.co.emote.transfer;

import uk.co.emote.ThalamusCompetencyItem;

public class ThalamusEvidenceItem {
	 private String evidenceType;
	 private String mapEventId;
	 private String evidenceName;
	 private int learnerId; 
	 private int stepId;
	 private int activityId;
	 private int scenarioId;
	 private int sessionId;
	 private int emoteScenarioId;
	 private Boolean correct;
	 private String actual;
	 private String expected;
	 private String action;
	 
	   /*public EvidenceType evidenceType { get; set; }
       public String mapEventId { get; set; }
       public String evidenceName { get; set; }
       public int learnerId { get; set; }    
       public int stepId { get; set; }
       public int activityId { get; set; }
       public int scenarioId { get; set; }
       public int sessionId { get; set; }
       public int emoteScenarioId { get; set; }
       public Boolean correct { get; set; }
       public String actual { get; set; }
       public String expected { get; set; }
       public String action { get; set; }*/
	 
	public String getEvidenceType() {
		return evidenceType;
	}
	public void setEvidenceType(String evidenceType) {
		this.evidenceType = evidenceType;
	}
	public String getMapEventId() {
		return mapEventId;
	}
	public void setMapEventId(String mapEventId) {
		this.mapEventId = mapEventId;
	}
	public String getEvidenceName() {
		return evidenceName;
	}
	public void setEvidenceName(String evidenceName) {
		this.evidenceName = evidenceName;
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
	public int getEmoteScenarioId() {
		return emoteScenarioId;
	}
	public void setEmoteScenarioId(int emoteScenarioId) {
		this.emoteScenarioId = emoteScenarioId;
	}
	public Boolean getCorrect() {
		return correct;
	}
	public void setCorrect(Boolean correct) {
		this.correct = correct;
	}
	public String getActual() {
		return actual;
	}
	public void setActual(String actual) {
		this.actual = actual;
	}
	public String getExpected() {
		return expected;
	}
	public void setExpected(String expected) {
		this.expected = expected;
	}
	public String getAction() {
		return action;
	}
	public void setAction(String action) {
		this.action = action;
	}
	 
	@Override public String toString() {
	    StringBuilder result = new StringBuilder();
	    String NEW_LINE = System.getProperty("line.separator");

	    result.append(this.getClass().getName() + " Object {" + NEW_LINE);
	    result.append(" thalamusLearnerId: " + learnerId + NEW_LINE);
	    result.append(", stepId: " + stepId + NEW_LINE);
	    result.append(", sessionId: " + sessionId + NEW_LINE );
	    if(action!=null)
	    {
	    result.append(", action: " + action + NEW_LINE );
	    }
	    if(actual!=null)
	    {
	    result.append(", actual: " + actual + NEW_LINE );
	    }
	    if(evidenceName!=null)
	    {
	    result.append(", evidenceName: " + evidenceName + NEW_LINE );
	    }
	    if(evidenceType!=null)
	    {
	    result.append(", evidenceType: " + evidenceType + NEW_LINE );
	    }
	    result.append("}");

	    return result.toString();
	  }
}
