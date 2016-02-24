package uk.co.emote;

import java.util.Date;
import java.util.List;
import java.util.TreeSet;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.transaction.annotation.Transactional;

@Controller
public class sessionController {
	@PersistenceContext
    private EntityManager entityManager;
	
	@Autowired
	private LearnerController learnerController;

/*	@Transactional
	public void endSessionForLearnerId(int learnerId) {
		Session currentSession = getCurrentSessionForLearnerId(learnerId);
		if(currentSession!=null)
		{
			currentSession.setEnd(new Date());
			currentSession = saveSession(currentSession);
		}
	}
*/
	
/*	public Session getCurrentSessionForLearnerId(int learnerId) {
		Session session=null;
		List<Session> sessions = entityManager.createQuery("select s from Session s LEFT JOIN FETCH s.learner where s.learner.id = "+learnerId+" order by start DESC").getResultList();
		if(sessions!=null && !sessions.isEmpty())
		{
			session = sessions.get(0);
		}
		return session;
	}*/
	
/*	public Session getCurrentSessionForThalamusLearnerId(int participantId) {
		Session session=null;
		List<Session> sessions = entityManager.createQuery("select s from Session s LEFT JOIN FETCH s.learner where s.learner.participantId = "+participantId+" order by start DESC").getResultList();
		if(sessions!=null && !sessions.isEmpty())
		{
			session = sessions.get(0);
		}
		return session;
	}*/

	
	@Transactional
	public Session saveSession(Session session)
	{
		entityManager.persist(session);
		return session;
	}
	
	/*@Transactional
	public void createNewSessionForLearner(Learner learner) {
		Session session = new Session();
		session.setLearner(learner);
		session.setStart(new Date());
		int sessionCount = 0;
		if(learner.getSessions()==null)
		{
			learner.setSessions(new TreeSet<Session>());
		}
		
		sessionCount = learner.getSessions().size();
		sessionCount++;
		session.setSessionNumber(sessionCount);
		learner.getSessions().add(session);
		entityManager.persist(session);
		entityManager.persist(learner);

		
		//saveSession(session);
	}*/
	@Transactional
	public void createNewSessionForLearner(Learner learner, int sessionId) {
		
		
		Session session = new Session();
		session.setLearner(learner);
		session.setStart(new Date());
		
		if(learner.getSessions()==null)
		{
			learner.setSessions(new TreeSet<Session>());
		}
		
		session.setSessionNumber(sessionId);
		learner.getSessions().add(session);
		entityManager.persist(session);
		entityManager.persist(learner);

		
	}

	@Transactional
	public void createNewSessionForLearnerId(int participantId,
			int learnerSessionNumber) {
		Learner learner = learnerController.getLearnerByThalamusId(participantId);
		createNewSessionForLearner(learner, learnerSessionNumber);
	}

	public Session getSessionForThalamusLearnerId(int participantId,
			int learnerSessionNumber) {
		Session session=null;
		List<Session> sessions = entityManager.createQuery("select s from Session s LEFT JOIN FETCH s.learner where s.learner.participantId = "+participantId+" AND s.sessionNumber = "+learnerSessionNumber+" order by start DESC").getResultList();
		if(sessions!=null && !sessions.isEmpty())
		{
			session = sessions.get(0);
		}
		return session;
	}


}


