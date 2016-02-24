package uk.co.emote.transfer;

import javax.persistence.ManyToOne;

import uk.co.emote.Learner;

public class EnercitiesGameInfo {

/*	http://localhost:8080/emote/addEnercitiesGameState
		json: {"learner1":"0","learner2":"1","stepId":"1","activityId":"0","scenarioId":
		"0","CurrentRole":"Economist","EconomyScore":"2","EnvironmentScore":"0.5","Globa
		lScore":"5.5","Level":"1","Money":"90","MoneyEarning":"2","Population":"8","Powe
		rConsumption":"2","PowerProduction":"8","TargetPopulation":"15","WellbeingScore"
		:"3"} */
	
	
	private String learner1;
	private String learner2;
	private Integer stepId; 
	private Integer activityId;
	private Integer scenarioId;
	private Integer emoteScenarioId;
	private Integer sessionNumber;
	 
	private String currentRole;
	private Double economyScore;
	private Double environmentScore;
	private Double globalScore;
	private Integer level;
	private Double money;
	private Double moneyEarning;
	private Double oil;
	private Integer population;
	private Double powerConsumption;
    private Double powerProduction;
    private Integer targetPopulation;
    private Double wellbeingScore;
    
	public String getLearner1() {
		return learner1;
	}
	public void setLearner1(String learner1) {
		this.learner1 = learner1;
	}
	public String getLearner2() {
		return learner2;
	}
	public void setLearner2(String learner2) {
		this.learner2 = learner2;
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
	public Integer getEmoteScenarioId() {
		return emoteScenarioId;
	}
	public void setEmoteScenarioId(Integer emoteScenarioId) {
		this.emoteScenarioId = emoteScenarioId;
	}
	public String getCurrentRole() {
		return currentRole;
	}
	public void setCurrentRole(String currentRole) {
		this.currentRole = currentRole;
	}
	public Double getEconomyScore() {
		return economyScore;
	}
	public void setEconomyScore(Double economyScore) {
		this.economyScore = economyScore;
	}
	public Double getEnvironmentScore() {
		return environmentScore;
	}
	public void setEnvironmentScore(Double environmentScore) {
		this.environmentScore = environmentScore;
	}
	public Double getGlobalScore() {
		return globalScore;
	}
	public void setGlobalScore(Double globalScore) {
		this.globalScore = globalScore;
	}
	public Integer getLevel() {
		return level;
	}
	public void setLevel(Integer level) {
		this.level = level;
	}
	public Double getMoney() {
		return money;
	}
	public void setMoney(Double money) {
		this.money = money;
	}
	public Double getMoneyEarning() {
		return moneyEarning;
	}
	public void setMoneyEarning(Double moneyEarning) {
		this.moneyEarning = moneyEarning;
	}
	public Double getOil() {
		return oil;
	}
	public void setOil(Double oil) {
		this.oil = oil;
	}
	public Integer getPopulation() {
		return population;
	}
	public void setPopulation(Integer population) {
		this.population = population;
	}
	public Double getPowerConsumption() {
		return powerConsumption;
	}
	public void setPowerConsumption(Double powerConsumption) {
		this.powerConsumption = powerConsumption;
	}
	public Double getPowerProduction() {
		return powerProduction;
	}
	public void setPowerProduction(Double powerProduction) {
		this.powerProduction = powerProduction;
	}
	public Integer getTargetPopulation() {
		return targetPopulation;
	}
	public void setTargetPopulation(Integer targetPopulation) {
		this.targetPopulation = targetPopulation;
	}
	public Double getWellbeingScore() {
		return wellbeingScore;
	}
	public void setWellbeingScore(Double wellbeingScore) {
		this.wellbeingScore = wellbeingScore;
	}
	public Integer getSessionNumber() {
		return sessionNumber;
	}
	public void setSessionNumber(Integer sessionNumber) {
		this.sessionNumber = sessionNumber;
	}
    
	
	
    
}
