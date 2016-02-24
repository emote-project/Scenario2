package uk.co.emote;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashSet;
import java.util.List;
import java.util.Locale;

import javax.persistence.Column;
import javax.persistence.EntityManager;
import javax.persistence.ManyToOne;
import javax.persistence.PersistenceContext;

import org.joda.time.DateTime;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.transaction.annotation.Transactional;

import uk.co.emote.transfer.EnercitiesGameInfo;
import uk.co.emote.transfer.EvidenceItemsWrapper;
import uk.co.emote.transfer.LearnerDetails;
import uk.co.emote.transfer.ThalamusEvidenceItem;
import uk.co.emote.transfer.UtteranceWrapper;




@Controller
public class LearnerController {

	@PersistenceContext
    private EntityManager entityManager;

	@Autowired
	private sessionController sessionController;
	
	
	private static final Logger logger = LoggerFactory
			.getLogger(LearnerController.class);

	
	
	@Transactional
    public Learner createLearner(String firstName,String middleName,String lastName, String sex,String birthString,Integer participantId) throws Exception {

		Learner learner = new Learner();
      
		learner.setFirstName(firstName);
		learner.setMiddleName(middleName);
		learner.setLastName(lastName);
		if(sex!=null&&sex.length()>0)
		{
		learner.setSex(sex.charAt(0));
		}
		else
		{
			learner.setSex('?');
		}
		
		if(birthString!=null&&birthString.length()>0 )
		{
				DateFormat df = new SimpleDateFormat("dd/MM/yy", Locale.ENGLISH);
				Date birth =  df.parse(birthString);
				learner.setBirth(birth);
		}
		
		learner.setParticipantId(participantId);
	
		//entityManager.getTransaction().begin();
        entityManager.persist(learner);
      //  entityManager.flush();
        return learner;
    }
/*	@Transactional
    public Learner createLearner(LearnerDetails learnerDetails) throws Exception {

		Learner learner = new Learner();
      
		learner.setFirstName(learnerDetails.getFirstName());
		learner.setMiddleName(learnerDetails.getMiddleName());
		learner.setLastName(learnerDetails.getLastName());
		if(learnerDetails.getSex()!=null&&learnerDetails.getSex().length()>0)
		{
		learner.setSex(learnerDetails.getSex().charAt(0));
		}
		else
		{
			learner.setSex('?');
		}
		
		if(learnerDetails.getBirth()!=null&&learnerDetails.getBirth().length()>0 )
		{
				DateFormat df = new SimpleDateFormat("dd/MM/yy", Locale.ENGLISH);
				Date birth =  df.parse(learnerDetails.getBirth());
				learner.setBirth(birth);
		}
		
		learner.setMapApplicationId(learnerDetails.getMapApplicationId());
		//entityManager.getTransaction().begin();
        entityManager.persist(learner);
      //  entityManager.flush();
        return learner;
    }*/
		
/*	public Learner getLearnerByMapApplicationId(Integer mapApplicationId) {	
		Learner learner=null;
		List<Learner> learners = entityManager.createQuery("select l from Learner l where mapApplicationId = "+mapApplicationId.toString()).getResultList();
		if(learners!=null && !learners.isEmpty())
		{
			learner = learners.get(0);
		}
		return learner;
	}*/

	public Learner getLearnerByThalamusId(Integer participantId) {
		Learner learner=null;
		List<Learner> learners = entityManager.createQuery("select l from Learner l where participantId = "+participantId.toString()).getResultList();
		if(learners!=null && !learners.isEmpty())
		{
			learner = learners.get(0);
		}
		return learner;
	}

	/*@Transactional
	public Learner createLearner(ThalamusStart thalamusStart, Integer mapApplicationId) {
		Learner learner = new Learner();
		
		learner.setFirstName(thalamusStart.getParticipantName());
		//TODO add the details for learner to thalamus. 
		learner.setMiddleName("");
		learner.setLastName("");
		learner.setSex('?');
		learner.setThalamusId(""+thalamusStart.getParticipantId());		
		learner.setMapApplicationId(mapApplicationId);
		//entityManager.getTransaction().begin();
        entityManager.persist(learner);
      //  entityManager.flush();
        return learner;
	}*/
	
	@Transactional
	public void createNewSessionForLearner(Integer id) {
		Learner learner=null;
		List<Learner> learners = entityManager.createQuery("select l from Learner l where id = "+id.toString()).getResultList();
		if(learners!=null && !learners.isEmpty())
		{
			learner = learners.get(0);
		}
		Session session = new Session();
		session.setStart(new Date());
		session.setLearner(learner);
		int sessionCount = 0;
		if(learner.getSessions()!=null)
		{
			sessionCount = learner.getSessions().size();
		}
		else
		{
			//TODO create collection and add? 
			learner.setSessions(new HashSet<Session>());
		}
		
		sessionCount++;
		session.setSessionNumber(sessionCount);
		learner.getSessions().add(session);
		
		entityManager.persist(session);
		//	entityManager.merge(learner);
		//saveSession(session);
	}
	
	
	
	/*public AnswerAttemptItem addEvidenceItem(Integer mapApplicationId, String type,
			boolean correct, int secondsTakenToAnswer, int stepId,
			int sessionId, int activityId, String scenarioName,
			String evidenceGiven, String evidenceRequired, boolean nextTell, Date stepAnswered) {
*/
	/*
	 * @Transactional
	 * public AnswerAttemptItem addEvidenceItem(Integer mapApplicationId,AnswerAttempt answerAttempt, AnswerAttemptItem answerAttemptItem, boolean nextTell, Date stepAnswered) {
		
		EvidenceItem evidenceItem = persistEvidenceItem(mapApplicationId, answerAttempt, answerAttemptItem, nextTell, stepAnswered);
		
		//TODO add a wrapper where the line below is called outside of transaction context... 
		//Call the competency calculation update.
		updateEvidenceBottomUp(evidenceItem);
		
		//TODO add some stuff to the answerAttemptItem? 
		
		return answerAttemptItem;
	}*/

	/*@Transactional
	private EvidenceItem persistEvidenceItem(Integer mapApplicationId, AnswerAttempt answerAttempt, AnswerAttemptItem answerAttemptItem,  boolean nextTell, Date stepAnswered) {
		EvidenceItem evidenceItem = new EvidenceItem();
		evidenceItem.setStepId(answerAttempt.getStepId());
		evidenceItem.setActivityId(answerAttempt.getActivityId());
		evidenceItem.setScenarioName(answerAttempt.getScenarioName());
		evidenceItem.setType(answerAttemptItem.getCompetencyName());
		evidenceItem.setCorrect(answerAttemptItem.isCompetencyCorrect());
		evidenceItem.setSecondsToAnswer(answerAttempt.getSecondsTakenToAnswer());
		evidenceItem.setEvidenceGiven(answerAttemptItem.getCompetencyActual());
		evidenceItem.setEvidenceRequired(answerAttemptItem.getCompetencyExpected());
		evidenceItem.setTimeEvidenceProvided(stepAnswered);
		
		Learner learner = getLearnerByMapApplicationId(mapApplicationId);
		Session session = sessionController.getCurrentSessionForLearnerId(learner.getId());
		DbCompetency dbCompetency = getCompetencyByNameForLearnerId(competencyName,learner.getId());
		if(dbCompetency==null)
		{
			//Create competency if it doesn't exist for the learner. 
			dbCompetency = new DbCompetency();
			dbCompetency.setLearner(learner);
			dbCompetency.setName(competencyName);
			dbCompetency.setSession(session);
			entityManager.persist(dbCompetency);
	
		}
		evidenceItem.setLearner(learner);
		evidenceItem.setSession(session);
	//	evidenceItem.setDbCompetency(dbCompetency);
		entityManager.persist(evidenceItem);
		return evidenceItem;
	}*/

	private void updateEvidenceBottomUp(EvidenceItem evidenceItem) {
		// TODO Implement
		//TODO get the type of the competency.
		String evidenceItemType = evidenceItem.getType();
		//TODO Look up the weighting and parent comptencies for that type.
		List<CompetencyEvidienceItemLink> competencyEvidienceItemLinks = getCompetencyEvidienceItemLinks(evidenceItemType);
		if(competencyEvidienceItemLinks!=null&&!competencyEvidienceItemLinks.isEmpty())
		{
		for(CompetencyEvidienceItemLink competencyEvidienceItemLink : competencyEvidienceItemLinks)
		{
			//TODO send the weighting and update to that type for the learner and update.
			String parentCompetencyName = competencyEvidienceItemLink.getCompetencyName();
			int weighting = competencyEvidienceItemLink.getWeight();
			
			updateCompetencyWithEvidenceBottomUp(evidenceItem,parentCompetencyName,weighting);
		}
		}
		
	}

	private List<CompetencyEvidienceItemLink> getCompetencyEvidienceItemLinks(
			String evidenceItemType) {

		List<CompetencyEvidienceItemLink> competencyEvidienceItemLinks = entityManager.createQuery("select a from CompetencyEvidienceItemLink a where evidenceItemType like '"+evidenceItemType+"'").getResultList();
		return competencyEvidienceItemLinks;
	}

	private void updateCompetencyWithEvidenceBottomUp(EvidenceItem evidenceItem,
			String competencyName, int weighting) {
		// TODO Auto-generated method stub
		
		
		DbCompetency dbCompetency = updateCompetencyWithEvidence(evidenceItem,competencyName,weighting);
		
		//TODO Look up the weighting and parent comptencies for that type.
		List<CompetencyCompetencyLink> competencyCompetencyLinks = getCompetencyCompetencyLinksByChildName(competencyName);
		if(competencyCompetencyLinks!=null&&!competencyCompetencyLinks.isEmpty())
		{
		for(CompetencyCompetencyLink competencyCompetencyLink : competencyCompetencyLinks)
		{
			//TODO send the weighting and update to that type for the learner and update.
			String parentCompetencyName = competencyCompetencyLink.getParentCompetencyName();
			int newWeighting = competencyCompetencyLink.getWeight();
			
			updateCompetencyWithCompetencyBottomUp(dbCompetency,parentCompetencyName,newWeighting);
		}
		}
	}

	private List<CompetencyCompetencyLink> getCompetencyCompetencyLinksByChildName(
			String childCompetencyName) {
		List<CompetencyCompetencyLink> competencyCompetencyLinks = entityManager.createQuery("select a from CompetencyCompetencyLink a where childCompetencyName like '"+childCompetencyName+"'").getResultList();
		return competencyCompetencyLinks;
	}

	private void updateCompetencyWithCompetencyBottomUp(
			DbCompetency childCompetency, String competencyName, int weighting) {
		// TODO Auto-generated method stub
		DbCompetency updatedCompetency = updateCompetencyWithCompetency(childCompetency,competencyName,weighting);
		
		//TODO Look up the weighting and parent comptencies for that type.
		List<CompetencyCompetencyLink> competencyCompetencyLinks = getCompetencyCompetencyLinksByChildName(updatedCompetency.getName());
		if(competencyCompetencyLinks!=null&&!competencyCompetencyLinks.isEmpty())
		{
		for(CompetencyCompetencyLink competencyCompetencyLink : competencyCompetencyLinks)
		{
			//TODO send the weighting and update to that type for the learner and update.
			String parentCompetencyName = competencyCompetencyLink.getParentCompetencyName();
			int newWeighting = competencyCompetencyLink.getWeight();
			
			updateCompetencyWithCompetencyBottomUp(updatedCompetency,parentCompetencyName,newWeighting);
		}
		}
	}

	@Transactional
	private DbCompetency updateCompetencyWithCompetency(
			DbCompetency childCompetency, String competencyName, int weighting) {
		// TODO Auto-generated method stub
		//TODO do some calculation. Then persist. Do this in a transactional sub method?
		DbCompetency dbCompetency = getCompetencyByNameForLearnerId(competencyName,childCompetency.getLearner().getId());
		if(dbCompetency==null)
		{
			//Learner learner = em.find(Learner.class, learnerId);
			//Create competency if it doesn't exist for the learner. 
			//TODO put this in it's own persistence bit? 
			dbCompetency = new DbCompetency();
			dbCompetency.setLearner(childCompetency.getLearner());
			dbCompetency.setName(competencyName);
			dbCompetency.setSession(childCompetency.getSession());
			
		}
		
		entityManager.persist(dbCompetency);
		
		return dbCompetency;
	}
	
	@Transactional
	private DbCompetency updateCompetencyWithEvidence(EvidenceItem evidenceItem, String competencyName, int weighting) {
		//TODO do some calculation. Then persist. Do this in a transactional sub method?
			DbCompetency dbCompetency = getCompetencyByNameForLearnerId(competencyName,evidenceItem.getLearner().getId());
			if(dbCompetency==null)
			{
				//Learner learner = em.find(Learner.class, learnerId);
				//Create competency if it doesn't exist for the learner. 
				//TODO put this in it's own persistence bit? 
				dbCompetency = new DbCompetency();
				dbCompetency.setLearner(evidenceItem.getLearner());
				dbCompetency.setName(competencyName);
				dbCompetency.setSession(evidenceItem.getSession());
				
			}
			
			entityManager.persist(dbCompetency);
			
			return dbCompetency;
	}

	private DbCompetency getCompetencyByNameForLearnerId(String competencyName,
			int learnerId) {
		DbCompetency competency=null;
		List<DbCompetency> competencies = entityManager.createQuery("select c from DbCompetency c LEFT JOIN FETCH c.learner where c.learner.id = "+learnerId+" AND c.name like '"+competencyName+"'").getResultList();
		if(competencies!=null && !competencies.isEmpty())
		{
			competency = competencies.get(0);
		}
		return competency;
	}

	public ArrayList<LearnerDetails> getAllLearnerDetails() {
		ArrayList<LearnerDetails> learnerInfos = new ArrayList<LearnerDetails>();
		LearnerDetails learnerDetail;
		//Learner learner=null;
		List<Learner> learners = entityManager.createQuery("select l from Learner l").getResultList();
		if(learners!=null && !learners.isEmpty())
		{
			//learner = learners.get(0);
			for(Learner learner : learners)
			{
				learnerDetail = makeLearnerDetail(learner);
				learnerInfos.add(learnerDetail);
			}
			
		}
		return learnerInfos;
	}
	private LearnerDetails makeLearnerDetail(uk.co.emote.Learner learner) {
		LearnerDetails learnerDetail = new LearnerDetails();
		if(learner.getBirth()!=null){
			learnerDetail.setBirth(learner.getBirth().toString());
		}
		if(learner.getFirstName()!=null){
		learnerDetail.setFirstName(learner.getFirstName());
		}
		if(learner.getLastName()!=null){
		learnerDetail.setLastName(learner.getLastName());
		}
		if(learner.getParticipantId()!=null){
		learnerDetail.setMapApplicationId(learner.getParticipantId());
		}
		if(learner.getMiddleName()!=null){
		learnerDetail.setMiddleName(learner.getMiddleName());
		}
	
		learnerDetail.setSex(learner.getSex()+"");
		
		if(learner.getParticipantId()!=null){
		learnerDetail.setThalamusLearnerId(learner.getParticipantId());
		}
		if(learner.getScenario1Difficulty()!=null)
		{
			learnerDetail.setScenario1Difficulty(learner.getScenario1Difficulty());
		}
		return learnerDetail;
	}
	
	@Transactional
	public String addThalamusEvidenceItems(List<ThalamusEvidenceItem> listFromThalamus) throws Exception {
		String returnMessage = "";
		if( listFromThalamus!=null && !listFromThalamus.isEmpty()){
			for( ThalamusEvidenceItem thalamusEvidenceItem: listFromThalamus)
			{
				returnMessage = persistEvidenceItem(thalamusEvidenceItem);
				//TODO add a wrapper where the line below is called outside of transaction context... 
				//Call the competency calculation update.
				//TODO this will actually call a top down update. It can be focused based on the rest... 
				//updateEvidenceBottomUp(evidenceItem);
			}
		}
		else
		{
			returnMessage = "no evidence to persist";
		}
		
		
		return returnMessage;
	}
	
	
	@Transactional
	private String persistEvidenceItem(ThalamusEvidenceItem thalamusEvidenceItem) throws Exception {
		String returnMessage = "";
		//TODO make sure all values are set here.
		//TODO do null checks
		EvidenceItem evidenceItem = new EvidenceItem();
		//TODO make step an object.
	
		evidenceItem.setStepId(thalamusEvidenceItem.getStepId());
		
		//TODO make activity an object
		
		evidenceItem.setActivityId(thalamusEvidenceItem.getActivityId());
		//TODO make scenario an object
		evidenceItem.setScenarioName(""+thalamusEvidenceItem.getScenarioId());
		evidenceItem.setType(thalamusEvidenceItem.getEvidenceType());
		evidenceItem.setCorrect(thalamusEvidenceItem.getCorrect());
		//TODO calculate this... 
		evidenceItem.setSecondsToAnswer(0);
		evidenceItem.setEvidenceGiven(thalamusEvidenceItem.getActual());
		evidenceItem.setEvidenceRequired(thalamusEvidenceItem.getExpected());
		evidenceItem.setTimeEvidenceProvided(new Date());
		evidenceItem.setType(thalamusEvidenceItem.getEvidenceType());
		evidenceItem.setMapEventId(thalamusEvidenceItem.getMapEventId());
		evidenceItem.setName(thalamusEvidenceItem.getEvidenceName());
		evidenceItem.setEmoteScenarioId(""+thalamusEvidenceItem.getEmoteScenarioId());
		evidenceItem.setAction(thalamusEvidenceItem.getAction());
		
		//evidenceItem.setData(thalamusEvidenceItem.getActual());
		

		
		Learner learner = getLearnerByThalamusId(thalamusEvidenceItem.getLearnerId());
		if(learner ==null)
		{
			learner = createLearner("Learner","","Default", "M","01/01/2001",new Integer(thalamusEvidenceItem.getLearnerId()));
		}
		
		if(learner!=null)
		{
			Session session = sessionController.getSessionForThalamusLearnerId(thalamusEvidenceItem.getLearnerId(),thalamusEvidenceItem.getSessionId());
			if(session==null)
			{
				
				sessionController.createNewSessionForLearner(learner,thalamusEvidenceItem.getSessionId());
				session = sessionController.getSessionForThalamusLearnerId(thalamusEvidenceItem.getLearnerId(),thalamusEvidenceItem.getSessionId());
			}
			
			if(session!=null)
			{
				evidenceItem.setLearner(learner);
				evidenceItem.setSession(session);
				entityManager.persist(evidenceItem);
				returnMessage = "evidence saved";
			}
			else
			{
				returnMessage = "can not load session";
			}
	
		}
		else
		{
			returnMessage = "can not load learner";
		}
		return returnMessage;
	}

	public ArrayList<EvidenceItem> loadAllEvidenceItemsForLearner(
			int thalamusLearnerId, int learnerSessionNumber) {
		
		logger.info("Loading evidence for learner:"+thalamusLearnerId+" and session:"+learnerSessionNumber);
		ArrayList<EvidenceItem> eisa = new ArrayList<EvidenceItem>();
		List<EvidenceItem> eis = entityManager.createQuery("select ei from EvidenceItem ei LEFT JOIN FETCH ei.learner LEFT JOIN FETCH ei.session where ei.learner.participantId = "+thalamusLearnerId+" AND ei.session.sessionNumber ="+learnerSessionNumber+"").getResultList();
		if(eis!=null && !eis.isEmpty())
		{
			for(EvidenceItem ei :eis)
			{
				eisa.add(ei);
			}
		}
		return eisa;
	}
	
/*	@Transactional
	public void incrementSession(int participantId) throws Exception{
		
			
		Learner learner = getLearnerByThalamusId(participantId);	
		if(learner ==null)
		{
			learner = createLearner("Learner","","Default", "M","01/01/2001",new Integer(participantId));
		}
		sessionController.createNewSessionForLearner(learner);
	
		
	}*/

	@Transactional
	public void cancelUtterance(UtteranceWrapper utteranceWrapper) throws Exception {
		
		Utterance utterance =  getUtteranceByUtteranceIdAndThalamusId(utteranceWrapper.getUtterenceId(), utteranceWrapper.getParticipantId());
		utterance.setCanceled(true);
		Date canceledTime = new Date();
		utterance.setCanceledTime(canceledTime);
		entityManager.persist(utterance);
	}
	
	@Transactional
	public void checkUtteranceExists(UtteranceWrapper utteranceWrapper) throws Exception {
		Utterance utterance = null;
		//Check the learner if not create
				if(!utteranceWrapper.getParticipantId().equalsIgnoreCase(""))
				{
					Integer thalamusLearnerId = new Integer(utteranceWrapper.getParticipantId());
					
					Learner learner = getLearnerByThalamusId(thalamusLearnerId);	
					if(learner ==null)
					{
						learner = createLearner("Learner","","Default", "M","01/01/2001",new Integer(thalamusLearnerId));
					}
					
					//TODO check utterance exists if not create
					 utterance = getUtteranceByUtteranceIdAndThalamusId(utteranceWrapper.getUtterenceId(), utteranceWrapper.getParticipantId());
					if(utterance ==null)
					{
						createUtterance(utteranceWrapper,learner);
					//	utterance = getUtteranceByUtteranceId(utteranceWrapper.getUtterenceId());
					}
					
				}
				//TODO update utterance
			
	}
	@Transactional
	private void createUtterance(UtteranceWrapper utteranceWrapper, Learner learner) {
			Utterance utterence=new Utterance();
			
			utterence.setCategory(utteranceWrapper.getCategory());
			utterence.setLearner(learner);
			utterence.setSubcategory(utteranceWrapper.getSubcategory());
			utterence.setUtterance(utteranceWrapper.getUtterance());
			utterence.setUtteranceId(utteranceWrapper.getUtterenceId());
			utterence.setSessionNumber(utteranceWrapper.getSessionNumber());
			entityManager.persist(utterence);
			
		      //  entityManager.flush();
			//return utterence;
	}

	private Utterance getUtteranceByUtteranceIdAndThalamusId(String utterenceId, String participantId) {
		Utterance utterence=null;
		List<Utterance> utterences = entityManager.createQuery("select l from Utterance l LEFT JOIN FETCH l.learner where l.utteranceId like '"+utterenceId+"' AND l.learner.participantId = "+participantId).getResultList();
		
	//	List<EvidenceItem> eis = entityManager.createQuery("select ei from EvidenceItem ei LEFT JOIN FETCH ei.learner LEFT JOIN FETCH ei.session where ei.learner.thalamusId = "+thalamusLearnerId+" AND ei.session.sessionNumber ="+learnerSessionNumber+"").getResultList();
		
		if(utterences!=null && !utterences.isEmpty())
		{
			utterence = utterences.get(0);
		}
		return utterence;
	}

	@Transactional
	public void performUtterance(UtteranceWrapper utteranceWrapper) throws Exception {
		
		Utterance utterance =  getUtteranceByUtteranceIdAndThalamusId(utteranceWrapper.getUtterenceId(), utteranceWrapper.getParticipantId());
		
		 //  parametersForJson["utterance"] = "" + utterance;
        //   parametersForJson["category"] = "" + category;
		utterance.setUtterance(utteranceWrapper.getUtterance());
		utterance.setCategory(utteranceWrapper.getCategory());
		entityManager.persist(utterance);
	}

	@Transactional
	public void performUtteranceFromLibrary(UtteranceWrapper utteranceWrapper) throws Exception {
		Utterance utterance =  getUtteranceByUtteranceIdAndThalamusId(utteranceWrapper.getUtterenceId(), utteranceWrapper.getParticipantId());
		
		//parametersForJson["category"] = "" + category;
      //  parametersForJson["subcategory"] = "" + subcategory;
		utterance.setSubcategory(utteranceWrapper.getSubcategory());
		utterance.setCategory(utteranceWrapper.getCategory());
		entityManager.persist(utterance);
	}

	@Transactional
	public void utteranceFinished(UtteranceWrapper utteranceWrapper) throws Exception {
		Utterance utterance =  getUtteranceByUtteranceIdAndThalamusId(utteranceWrapper.getUtterenceId(), utteranceWrapper.getParticipantId());
		
		Date endDate = new Date();
		utterance.setEnd(endDate);
		
		entityManager.persist(utterance);
	}

	@Transactional
	public void utteranceStarted(UtteranceWrapper utteranceWrapper) throws Exception {
		Utterance utterance =  getUtteranceByUtteranceIdAndThalamusId(utteranceWrapper.getUtterenceId(), utteranceWrapper.getParticipantId());
		
		Date startDate = new Date();
		utterance.setStart(startDate);
		entityManager.persist(utterance);
	}

	@Transactional
	public void utteranceIsAQuestion(UtteranceWrapper utteranceWrapper) throws Exception {
		Utterance utterance =  getUtteranceByUtteranceIdAndThalamusId(utteranceWrapper.getUtterenceId(), utteranceWrapper.getParticipantId());
		
		utterance.setQuestion(true);
		entityManager.persist(utterance);
	}
	
	@Transactional
	public void utteranceUsed(UtteranceWrapper utteranceWrapper) throws Exception {
		Utterance utterance =  getUtteranceByUtteranceIdAndThalamusId(utteranceWrapper.getUtterenceId(), utteranceWrapper.getParticipantId());
		
		//parametersForJson["utterance"] = "" + Utterance_utterance;
		//private String library;
		//private String repetitions;
		 //private String libraryId;
		
		utterance.setUtterance(utteranceWrapper.getUtterance());
		utterance.setRepetitions(utteranceWrapper.getRepetitions());
		utterance.setLibrary(utteranceWrapper.getLibrary());
		utterance.setLibraryId(utteranceWrapper.getLibraryId());
		
		
		
		entityManager.persist(utterance);
	}
	
	@Transactional
	public Learner updateLearner(String firstName, String middleName,
			String lastName, String sex, String birthString,
			Integer participantId) throws ParseException {
		
		Learner learner = getLearnerByThalamusId(participantId);
	      
		if(!firstName.equalsIgnoreCase(""))
		{
			learner.setFirstName(firstName);
		}
		if(!middleName.equalsIgnoreCase(""))
		{
			learner.setMiddleName(middleName);
		}
		if(!lastName.equalsIgnoreCase(""))
		{
			learner.setLastName(lastName);
		}
		
		if(sex!=null&&sex.length()>0)
		{
		learner.setSex(sex.charAt(0));
		}
		else
		{
			learner.setSex('?');
		}
		
		if(birthString!=null&&birthString.length()>0 )
		{
				DateFormat df = new SimpleDateFormat("dd/MM/yy", Locale.ENGLISH);
				Date birth =  df.parse(birthString);
				learner.setBirth(birth);
		}
		learner.setParticipantId(participantId);
	
		//entityManager.getTransaction().begin();
        entityManager.persist(learner);
      //  entityManager.flush();
        return learner;
	}

	public ArrayList<Utterance> getAllUtterancesForThalamusId(int participant) {
		Utterance utterence=null;
		ArrayList<Utterance> utterencesList = new ArrayList<Utterance>();
		List<Utterance> utterences = entityManager.createQuery("select l from Utterance l LEFT JOIN FETCH l.learner where l.learner.participantId = "+participant +" group by l.libraryId, l.sessionNumber, l.library").getResultList();
		
	
		if(utterences!=null && !utterences.isEmpty())
		{	
			for(Utterance utterance:utterences)
			{
				utterencesList.add(utterance);
			}
			
		}
		return utterencesList;
	}

	@Transactional
	public void addEnercitiesGameState(EnercitiesGameInfo enercitiesGameInfo,
			Integer participantId) throws Exception {
		Learner learner = getLearnerByThalamusId(participantId);
		if(learner ==null)
		{
			learner = createLearner("Learner","","Default", "M","01/01/2001",new Integer(participantId));
		}
		
		if(learner!=null)
		{
			Session session = sessionController.getSessionForThalamusLearnerId(participantId,enercitiesGameInfo.getSessionNumber().intValue());
			if(session==null)
			{
				sessionController.createNewSessionForLearner(learner,enercitiesGameInfo.getSessionNumber().intValue());
				session = sessionController.getSessionForThalamusLearnerId(participantId,enercitiesGameInfo.getSessionNumber().intValue());
			}
			
			if(session!=null)
			{
				//evidenceItem.setLearner(learner);
				//evidenceItem.setSession(session);
				EnercitiesGameInfoDB enercitiesGameInfoDB = new EnercitiesGameInfoDB();
				enercitiesGameInfoDB.setSession(session);
				enercitiesGameInfoDB.setLearner(learner);
				enercitiesGameInfoDB.setStepId(enercitiesGameInfo.getStepId());
				enercitiesGameInfoDB.setActivityId(enercitiesGameInfo.getActivityId());
				enercitiesGameInfoDB.setScenarioId(enercitiesGameInfo.getScenarioId());
				enercitiesGameInfoDB.setEmoteScenarioId(enercitiesGameInfo.getEmoteScenarioId());
				enercitiesGameInfoDB.setCurrentRole(enercitiesGameInfo.getCurrentRole());
				enercitiesGameInfoDB.setEconomyScore(enercitiesGameInfo.getEconomyScore());
				enercitiesGameInfoDB.setEnvironmentScore(enercitiesGameInfo.getEnvironmentScore());
				enercitiesGameInfoDB.setGlobalScore(enercitiesGameInfo.getGlobalScore());
				enercitiesGameInfoDB.setLevel(enercitiesGameInfo.getLevel());
				enercitiesGameInfoDB.setMoney(enercitiesGameInfo.getMoney());
				enercitiesGameInfoDB.setMoneyEarning(enercitiesGameInfo.getMoneyEarning());
				enercitiesGameInfoDB.setOil(enercitiesGameInfo.getOil());
				enercitiesGameInfoDB.setPopulation(enercitiesGameInfo.getPopulation());
				enercitiesGameInfoDB.setPowerConsumption(enercitiesGameInfo.getPowerConsumption());
				enercitiesGameInfoDB.setPowerProduction(enercitiesGameInfo.getPowerProduction());
				enercitiesGameInfoDB.setTargetPopulation(enercitiesGameInfo.getTargetPopulation());
				enercitiesGameInfoDB.setWellbeingScore(enercitiesGameInfo.getWellbeingScore());
				 
		
				
				
				entityManager.persist(enercitiesGameInfoDB);
				//returnMessage = "evidence saved";
			}
			else
			{
				//returnMessage = "can not load session";
			}
	
		}
		else
		{
			//returnMessage = "can not load learner";
		}
		//return returnMessage;
	}

	@Transactional
	public void persistLearnerModelUpdate(LearnerStateInfo learnerStateInfo) throws Exception {
		if(learnerStateInfo!=null && learnerStateInfo.getCompetencyItems()!=null && !learnerStateInfo.getCompetencyItems().isEmpty())
		{
			
			
			Learner learner = getLearnerByThalamusId(learnerStateInfo.getLearnerId());
			if(learner ==null)
			{
				learner = createLearner("Learner","","Default", "M","01/01/2001",new Integer(learnerStateInfo.getLearnerId()));
			}
			
			
			Session session = sessionController.getSessionForThalamusLearnerId(learnerStateInfo.getLearnerId(),learnerStateInfo.getSessionId());
			if(session==null)
			{
				
				sessionController.createNewSessionForLearner(learner,learnerStateInfo.getSessionId());
				session = sessionController.getSessionForThalamusLearnerId(learnerStateInfo.getLearnerId(),learnerStateInfo.getSessionId());
			}
			
			
			for(ThalamusCompetencyItem competencyItem:learnerStateInfo.getCompetencyItems())
			{
				EvidenceItem evidenceItem = new EvidenceItem();
				//TODO make step an object.
			
				evidenceItem.setStepId(learnerStateInfo.getStepId());
				
				//TODO make activity an object
				
				evidenceItem.setActivityId(learnerStateInfo.getActivityId());
				//TODO make scenario an object
				evidenceItem.setScenarioName(""+learnerStateInfo.getScenarioId());
				if(competencyItem.getCompetencyName()!=null)
				{
					if(competencyItem.getCompetencyName().compareToIgnoreCase("distance")==0)
					{
						evidenceItem.setType("10");
						
					}
					else if(competencyItem.getCompetencyName().compareToIgnoreCase("direction")==0)
					{
						evidenceItem.setType("11");
						
					}
					else if(competencyItem.getCompetencyName().compareToIgnoreCase("symbol")==0)
					{
						evidenceItem.setType("12");					
					}
					
				}
				
				evidenceItem.setCorrect(competencyItem.getCompetencyCorrect());
				//TODO calculate this... 
				evidenceItem.setSecondsToAnswer(0);
				evidenceItem.setEvidenceGiven(competencyItem.getCompetencyActual());
				evidenceItem.setEvidenceRequired(competencyItem.getCompetencyExpected());
				evidenceItem.setTimeEvidenceProvided(new Date());
				
				evidenceItem.setMapEventId(learnerStateInfo.getMapEventId());
				evidenceItem.setName(competencyItem.getCompetencyName());
				evidenceItem.setData(competencyItem.getComptencyValue());
				evidenceItem.setEmoteScenarioId(""+1);
				//evidenceItem.setAction(competencyItem.get);
				evidenceItem.setLearner(learner);
				evidenceItem.setSession(session);
				entityManager.persist(evidenceItem);
				
			}
			
		}
		
	}

	public ArrayList<EvidenceItem> loadAllEvidenceItemsForLearner(int thalamusLearnerId) {
		logger.info("Loading evidence for learner:"+thalamusLearnerId);
		ArrayList<EvidenceItem> eisa = new ArrayList<EvidenceItem>();
		List<EvidenceItem> eis = entityManager.createQuery("select ei from EvidenceItem ei LEFT JOIN FETCH ei.learner LEFT JOIN FETCH ei.session where ei.learner.participantId = "+thalamusLearnerId).getResultList();
		if(eis!=null && !eis.isEmpty())
		{
			for(EvidenceItem ei :eis)
			{
				eisa.add(ei);
			}
		}
		return eisa;
	}

	@Transactional
	public void updateScenario1Level(LearnerStateInfo learnerStateInfo) throws Exception {
		if(learnerStateInfo!=null )
		{
			
			
			Learner learner = getLearnerByThalamusId(learnerStateInfo.getLearnerId());
			if(learner ==null)
			{
				learner = createLearner("Learner","","Default", "M","01/01/2001",new Integer(learnerStateInfo.getLearnerId()));
			}
			
			if(learnerStateInfo.getCompetencyItems()!=null && !learnerStateInfo.getCompetencyItems().isEmpty())
			{
				Double distanceValue = new Double(0.5);
				Double directionValue = new Double(0.5);
				Double symbolValue = new Double(0.5);
					
				
				for(ThalamusCompetencyItem competencyItem:learnerStateInfo.getCompetencyItems())
				{
					if(competencyItem.getCompetencyName()!=null)
					{
						if(competencyItem.getCompetencyName().compareToIgnoreCase("distance")==0)
						{
							distanceValue = competencyItem.getComptencyValue();
							
						}
						else if(competencyItem.getCompetencyName().compareToIgnoreCase("direction")==0)
						{
							directionValue = competencyItem.getComptencyValue();
							
						}
						else if(competencyItem.getCompetencyName().compareToIgnoreCase("symbol")==0)
						{
							symbolValue = competencyItem.getComptencyValue();				
						}
						
					}
				}
				
				if(distanceValue.compareTo(new Double(0.4))<0||directionValue.compareTo(new Double(0.4))<0||symbolValue.compareTo(new Double(0.4))<0)
				{
					learner.setScenario1Difficulty(new Integer(1));
				}
				else if(distanceValue.compareTo(new Double(0.6))<0||directionValue.compareTo(new Double(0.6))<0||symbolValue.compareTo(new Double(0.6))<0)
				{
					learner.setScenario1Difficulty(new Integer(2));
				}
				else 
				{
					learner.setScenario1Difficulty(new Integer(3));
				}
			}
			else
			{
				learner.setScenario1Difficulty(new Integer(2));
				
			}
			
			entityManager.persist(learner);
			
		}
	}

	public ArrayList<EvidenceItem> loadAllEndGameEvidenceItemsForLearner(int thalamusLearnerId, int learnerSessionNumber) {
		
		logger.info("Loading evidence for learner:"+thalamusLearnerId+" and session:"+learnerSessionNumber);
		ArrayList<EvidenceItem> eisa = new ArrayList<EvidenceItem>();
		List<EvidenceItem> eis = entityManager.createQuery("select ei from EvidenceItem ei LEFT JOIN FETCH ei.learner LEFT JOIN FETCH ei.session where ei.learner.participantId = "+thalamusLearnerId+" AND ei.session.sessionNumber <="+learnerSessionNumber +" AND ei.name ='endGame'").getResultList();
		if(eis!=null && !eis.isEmpty())
		{
			for(EvidenceItem ei :eis)
			{
				eisa.add(ei);
			}
		}
		return eisa;
	}

	public ArrayList<EvidenceItem> loadAllCrisisEvidenceItemsForLearner(int thalamusLearnerId, int learnerSessionNumber) {
		logger.info("Loading evidence for learner:"+thalamusLearnerId+" and session:"+learnerSessionNumber);
		ArrayList<EvidenceItem> eisa = new ArrayList<EvidenceItem>();
		List<EvidenceItem> eis = entityManager.createQuery("select ei from EvidenceItem ei LEFT JOIN FETCH ei.learner LEFT JOIN FETCH ei.session where ei.learner.participantId = "+thalamusLearnerId+" AND ei.session.sessionNumber <="+learnerSessionNumber +" AND ei.name ='crisis'").getResultList();
		if(eis!=null && !eis.isEmpty())
		{
			for(EvidenceItem ei :eis)
			{
				eisa.add(ei);
			}
		}
		return eisa;
	}

	public boolean checkIfActualAndActionHappensInEarlierSession(String actual,
			String action, int learnerSessionNumber, int thalamusLearnerId) {
		logger.info("Loading evidence for learner:"+thalamusLearnerId+" and before session:"+learnerSessionNumber+", actual:"+actual+", action:"+action);
		
		List<EvidenceItem> eis = entityManager.createQuery("select ei from EvidenceItem ei LEFT JOIN FETCH ei.learner LEFT JOIN FETCH ei.session where ei.learner.participantId = "+thalamusLearnerId+" AND ei.session.sessionNumber <"+learnerSessionNumber +" AND ei.action ='"+ action+"'"+" AND ei.evidenceGiven ='"+ actual+"'" ).getResultList();
		boolean found = false;
		if(eis!=null && !eis.isEmpty())
		{
			found = true;
		}
		return found;
	}

	public ArrayList<EvidenceItem> loadAllCrisisEvidenceItemsForLearner(int thalamusLearnerId, int learnerSessionNumber) {
		logger.info("Loading evidence for learner:"+thalamusLearnerId+" and session:"+learnerSessionNumber);
		ArrayList<EvidenceItem> eisa = new ArrayList<EvidenceItem>();
		List<EvidenceItem> eis = entityManager.createQuery("select ei from EvidenceItem ei LEFT JOIN FETCH ei.learner LEFT JOIN FETCH ei.session where ei.learner.participantId = "+thalamusLearnerId+" AND ei.session.sessionNumber <="+learnerSessionNumber +" AND ei.name ='crisis'").getResultList();
		if(eis!=null && !eis.isEmpty())
		{
			for(EvidenceItem ei :eis)
			{
				eisa.add(ei);
			}
		}
		return eisa;
	}



	
	
}
