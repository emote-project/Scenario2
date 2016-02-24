package uk.co.emote.transfer;

import java.util.Date;

import javax.persistence.Column;

public class LearnerDetails {

	private String firstName; // was learnerName
    private String middleName;
    private String lastName;
	private Integer mapApplicationId; //was learnerId
	private String sex;
	private String birth;
	private Integer thalamusLearnerId;
	private Integer scenario1Difficulty;
	
	public String getFirstName() {
		return firstName;
	}
	public void setFirstName(String firstName) {
		this.firstName = firstName;
	}
	public String getMiddleName() {
		return middleName;
	}
	public void setMiddleName(String middleName) {
		this.middleName = middleName;
	}
	public String getLastName() {
		return lastName;
	}
	public void setLastName(String lastName) {
		this.lastName = lastName;
	}
	public Integer getMapApplicationId() {
		return mapApplicationId;
	}
	public void setMapApplicationId(Integer mapApplicationId) {
		this.mapApplicationId = mapApplicationId;
	}
	public String getSex() {
		return sex;
	}
	public void setSex(String sex) {
		this.sex = sex;
	}
	public String getBirth() {
		return birth;
	}
	public void setBirth(String birth) {
		this.birth = birth;
	}
	public Integer getThalamusLearnerId() {
		return thalamusLearnerId;
	}
	public void setThalamusLearnerId(Integer thalamusLearnerId) {
		this.thalamusLearnerId = thalamusLearnerId;
	}
	public Integer getScenario1Difficulty() {
		return scenario1Difficulty;
	}
	public void setScenario1Difficulty(Integer scenario1Difficulty) {
		this.scenario1Difficulty = scenario1Difficulty;
	}


	

}
