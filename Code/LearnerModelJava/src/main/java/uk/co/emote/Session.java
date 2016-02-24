package uk.co.emote;

import java.util.Date;
import java.util.HashSet;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.ManyToOne;
import javax.persistence.OneToMany;
import javax.persistence.Table;

@Entity
@Table(name = "session")
public class Session {

	 @Id // @Id indicates that this it a unique primary key
	    @GeneratedValue // @GeneratedValue indicates that value is automatically generated by the server
	    private int id;
	 
	 @Column(length = 32, unique = false)
	    // the optional @Column allows us makes sure that the name is limited to a suitable size and is unique
	    private int sessionNumber;
	
	 @Column
	    // the optional @Column allows us makes sure that the name is limited to a suitable size and is unique
	    private Date start;
	 
	 @Column
	    // the optional @Column allows us makes sure that the name is limited to a suitable size and is unique
	    private Date end;
	 
	   @ManyToOne
	    private Learner learner;
	   
	   @OneToMany
		private Set<DbCompetency> dbCompetencies  = new HashSet<DbCompetency>();
	   
	   @OneToMany
	    private Set<EvidenceItem> EvidenceItems = new HashSet<EvidenceItem>();


	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public Integer getSessionNumber() {
		return sessionNumber;
	}

	public void setSessionNumber(Integer sessionNumber) {
		this.sessionNumber = sessionNumber;
	}

	public Date getStart() {
		return start;
	}

	public void setStart(Date start) {
		this.start = start;
	}

	public Date getEnd() {
		return end;
	}

	public void setEnd(Date end) {
		this.end = end;
	}

	public Learner getLearner() {
		return learner;
	}

	public void setLearner(Learner learner) {
		this.learner = learner;
	}

	public Set<DbCompetency> getDbCompetencies() {
		return dbCompetencies;
	}

	public void setDbCompetencies(Set<DbCompetency> dbCompetencies) {
		this.dbCompetencies = dbCompetencies;
	}

	public Set<EvidenceItem> getEvidenceItems() {
		return EvidenceItems;
	}

	public void setEvidenceItems(Set<EvidenceItem> evidenceItems) {
		EvidenceItems = evidenceItems;
	}

	 
	   
	   
}
