package uk.co.emote;

public class ThalamusCompetencyItem {

	private String competencyName;
	private Boolean competencyCorrect;
	private String competencyActual;
	private String competencyExpected;
	private Double comptencyValue;
	private Integer competencyConfidence;
	private Double oldCompetencyValue;
    private Double competencyDelta;
    private String competencyType;
    private String impact;
    private String working;
    private String shortTerm;
    private String longTerm;
    private Double finalvaluePreviousSession;

    private Double highestValuePreviousSessions;
    private Double lowestFinalValuePreviousSessions;
    
    
	public String getCompetencyName() {
		return competencyName;
	}
	public void setCompetencyName(String competencyName) {
		this.competencyName = competencyName;
	}
	public Boolean getCompetencyCorrect() {
		return competencyCorrect;
	}
	public void setCompetencyCorrect(Boolean competencyCorrect) {
		this.competencyCorrect = competencyCorrect;
	}
	public String getCompetencyActual() {
		return competencyActual;
	}
	public void setCompetencyActual(String competencyActual) {
		this.competencyActual = competencyActual;
	}
	public String getCompetencyExpected() {
		return competencyExpected;
	}
	public void setCompetencyExpected(String competencyExpected) {
		this.competencyExpected = competencyExpected;
	}
	public Double getComptencyValue() {
		return comptencyValue;
	}
	public void setComptencyValue(Double comptencyValue) {
		this.comptencyValue = comptencyValue;
	}


	public Double getOldCompetencyValue() {
		return oldCompetencyValue;
	}
	public void setOldCompetencyValue(Double oldCompetencyValue) {
		this.oldCompetencyValue = oldCompetencyValue;
	}
	public Double getCompetencyDelta() {
		return competencyDelta;
	}
	public void setCompetencyDelta(Double competencyDelta) {
		this.competencyDelta = competencyDelta;
	}
	public String getCompetencyType() {
		return competencyType;
	}
	public void setCompetencyType(String competencyType) {
		this.competencyType = competencyType;
	}
	public String getImpact() {
		return impact;
	}
	public void setImpact(String impact) {
		this.impact = impact;
	}
	public String getShortTerm() {
		return shortTerm;
	}
	public void setShortTerm(String shortTerm) {
		this.shortTerm = shortTerm;
	}
	public String getLongTerm() {
		return longTerm;
	}
	public void setLongTerm(String longTerm) {
		this.longTerm = longTerm;
	}
	public String getWorking() {
		return working;
	}
	public void setWorking(String working) {
		this.working = working;
	}
	public Double getFinalvaluePreviousSession() {
		return finalvaluePreviousSession;
	}
	public void setFinalvaluePreviousSession(Double finalvaluePreviousSession) {
		this.finalvaluePreviousSession = finalvaluePreviousSession;
	}

	public Double getHighestValuePreviousSessions() {
		return highestValuePreviousSessions;
	}
	public void setHighestValuePreviousSessions(Double highestValuePreviousSessions) {
		this.highestValuePreviousSessions = highestValuePreviousSessions;
	}
	public Double getLowestFinalValuePreviousSessions() {
		return lowestFinalValuePreviousSessions;
	}
	public void setLowestFinalValuePreviousSessions(
			Double lowestFinalValuePreviousSessions) {
		this.lowestFinalValuePreviousSessions = lowestFinalValuePreviousSessions;
	}
	public Integer getCompetencyConfidence() {
		return competencyConfidence;
	}
	public void setCompetencyConfidence(Integer competencyConfidence) {
		this.competencyConfidence = competencyConfidence;
	}
	
	@Override public String toString() {
	    StringBuilder result = new StringBuilder();
	    result.append("{competencyName: " + competencyName + " comptencyValue:" + comptencyValue);
	    result.append("}");

	    return result.toString();
	  } 
}
