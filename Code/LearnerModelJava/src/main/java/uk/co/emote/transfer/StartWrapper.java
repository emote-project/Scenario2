package uk.co.emote.transfer;

import java.util.Date;

import javax.persistence.Column;

public class StartWrapper {

	private int participantId;
	private int participantId2;
	private String participant1Name;
	private String participant2Name;
	private String sessionNumber;
	
	public int getParticipantId() {
		return participantId;
	}
	public void setParticipantId(int participantId) {
		this.participantId = participantId;
	}
	public int getParticipantId2() {
		return participantId2;
	}
	public void setParticipantId2(int participantId2) {
		this.participantId2 = participantId2;
	}
	public String getParticipant1Name() {
		return participant1Name;
	}
	public void setParticipant1Name(String participant1Name) {
		this.participant1Name = participant1Name;
	}
	public String getParticipant2Name() {
		return participant2Name;
	}
	public void setParticipant2Name(String participant2Name) {
		this.participant2Name = participant2Name;
	}
	public String getSessionNumber() {
		return sessionNumber;
	}
	public void setSessionNumber(String sessionNumber) {
		this.sessionNumber = sessionNumber;
	}
	
	

}
