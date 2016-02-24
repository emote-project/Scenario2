package uk.co.emote;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.ManyToOne;
import javax.persistence.Table;

@Entity
@Table(name = "evidenceitem")
public class EvidenceItem {

	 @Id // @Id indicates that this it a unique primary key
	    @GeneratedValue // @GeneratedValue indicates that value is automatically generated by the server
	    private int id;
	 
	 @ManyToOne
	    private Learner learner;

		 @Column
		private Double data;
		 
		 @Column
		private int stepId;
		 
		 @Column
		private int activityId;
		 
		 @Column
		private String scenarioName;
		
		 @Column
			private String type;
		 
		@ManyToOne
		private Session session;
		
		 @Column
		private Boolean correct;
		 
		 @Column
		private int secondsToAnswer;
		 
		 @Column
		private String evidenceGiven;
		 
		 @Column
		private String evidenceRequired;

		 @Column
		private Date timeEvidenceProvided;
		 @Column
			private String mapEventId;
		 @Column
			private String name;
		 @Column
			private String emoteScenarioId;
		 @Column
			private String action;

		 
		 
		public int getId() {
			return id;
		}

		public void setId(int id) {
			this.id = id;
		}

		public Double getData() {
			return data;
		}

		public void setData(Double data) {
			this.data = data;
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

		public String getScenarioName() {
			return scenarioName;
		}

		public void setScenarioName(String scenarioName) {
			this.scenarioName = scenarioName;
		}

		public Session getSession() {
			return session;
		}

		public void setSession(Session session) {
			this.session = session;
		}

		public Boolean getCorrect() {
			return correct;
		}

		public void setCorrect(Boolean correct) {
			this.correct = correct;
		}

		public int getSecondsToAnswer() {
			return secondsToAnswer;
		}

		public void setSecondsToAnswer(int secondsToAnswer) {
			this.secondsToAnswer = secondsToAnswer;
		}

		public String getEvidenceGiven() {
			return evidenceGiven;
		}

		public void setEvidenceGiven(String evidenceGiven) {
			this.evidenceGiven = evidenceGiven;
		}

		public String getEvidenceRequired() {
			return evidenceRequired;
		}

		public void setEvidenceRequired(String evidenceRequired) {
			this.evidenceRequired = evidenceRequired;
		}

		public Learner getLearner() {
			return learner;
		}

		public void setLearner(Learner learner) {
			this.learner = learner;
		}

		public Date getTimeEvidenceProvided() {
			return timeEvidenceProvided;
		}

		public void setTimeEvidenceProvided(Date timeEvidenceProvided) {
			this.timeEvidenceProvided = timeEvidenceProvided;
		}

		public String getType() {
			return type;
		}

		public void setType(String type) {
			this.type = type;
		}

		public String getMapEventId() {
			return mapEventId;
		}

		public void setMapEventId(String mapEventId) {
			this.mapEventId = mapEventId;
		}

		public String getName() {
			return name;
		}

		public void setName(String name) {
			this.name = name;
		}

		public String getEmoteScenarioId() {
			return emoteScenarioId;
		}

		public void setEmoteScenarioId(String emoteScenarioId) {
			this.emoteScenarioId = emoteScenarioId;
		}

		public String getAction() {
			return action;
		}

		public void setAction(String action) {
			this.action = action;
		}
		 
		 
}