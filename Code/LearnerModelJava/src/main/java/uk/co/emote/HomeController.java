package uk.co.emote;

//import it.geosolutions.geoserver.rest.GeoServerRESTReader;

import java.io.DataOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.OutputStream;
import java.lang.annotation.Target;
import java.math.BigDecimal;
import java.net.MalformedURLException;
import java.net.Socket;
import java.net.UnknownHostException;
import java.nio.charset.Charset;
import java.text.DateFormat;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Locale;
import java.util.Map;

import javax.annotation.PostConstruct;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.sound.sampled.AudioFormat;
import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.Clip;
import javax.sound.sampled.DataLine;

import org.codehaus.jackson.JsonParseException;
import org.codehaus.jackson.map.JsonMappingException;
import org.codehaus.jackson.map.ObjectMapper;
import org.hibernate.Hibernate;
import org.joda.time.DateTime;
import org.joda.time.Seconds;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.converter.HttpMessageConverter;
import org.springframework.http.converter.json.MappingJacksonHttpMessageConverter;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.client.RestTemplate;

import uk.co.emote.transfer.EnercitiesGameInfo;
import uk.co.emote.transfer.EvidenceItemsWrapper;
import uk.co.emote.transfer.LearnerDetails;
import uk.co.emote.transfer.MemoryEvent;
import uk.co.emote.transfer.MemoryEventItem;
import uk.co.emote.transfer.ParticipantWrapper;
import uk.co.emote.transfer.StartWrapper;
import uk.co.emote.transfer.ThalamusEvidenceItem;
import uk.co.emote.transfer.UtteranceWrapper;

/**
 * Handles requests for the application home page.
 */
@Controller
public class HomeController {

	private static final Logger logger = LoggerFactory
			.getLogger(HomeController.class);

	@Autowired
	private LearnerController learnerController;
	@Autowired
	private sessionController sessionController;
	@Autowired
	private ThalamusManager thalamusManager;

	private HashMap<Integer, ArrayList<LearnerStateInfo>> previousLsis = new HashMap<Integer, ArrayList<LearnerStateInfo>>();

	@PostConstruct
	public void EnsureDefaultLeaner() {

		Learner learner = learnerController.getLearnerByThalamusId(new Integer(
				0));
		if (learner == null) {
			try {
				learner = learnerController.createLearner("Learner", "",
						"Default", "M", "01/01/2001", new Integer(0));
			} catch (Exception e) {
				logger.error("Could not make default learner in database");
				e.printStackTrace();
			}

		}
	}

	@RequestMapping(value = "/learnerModelUpdate", method = RequestMethod.POST)
	public @ResponseBody String learnerModelUpdate(
			@RequestBody LearnerStateInfo learnerStateInfo) throws Exception {
		logger.info("Got Thalamus Update:Learner model update");
		// This only comes from scenario 1...
		learnerController.persistLearnerModelUpdate(learnerStateInfo);
		// TODO Bring in the update, update the model. Update the update and
		// send it off again.

		LearnerStateInfo lsi = recalculateLSIandEvaluate(
				learnerStateInfo.getLearnerId(),
				learnerStateInfo.getSessionId(), false,true);
		if (learnerStateInfo.getMapEventId() != null) {
			lsi.setMapEventId(learnerStateInfo.getMapEventId());

		}
		lsi.setActivityId(learnerStateInfo.getActivityId());
		lsi.setScenarioId(learnerStateInfo.getScenarioId());
		lsi.setStepId(learnerStateInfo.getStepId());
		lsi.setReasonForUpdate(learnerStateInfo.getReasonForUpdate());
		addLSIToPrevious(lsi);
		thalamusManager
				.learnerModelValueUpdateBeforeAffectPerceptionUpdate(lsi);

		learnerController.updateScenario1Level(lsi);
		
		
		return "Update Success";
	}

	@RequestMapping(value = "/addEvidenceItems", method = RequestMethod.POST)
	public @ResponseBody String addEvidenceItems(
			@RequestBody EvidenceItemsWrapper evidenceItemsWrapper)
			throws Exception {
		logger.info("Got Thalamus addEvidenceItems");
		String returnMessage = "";
		// TODO add to the database...
		// TODO check that the learner is not null.
		// TODO check the session
		returnMessage = learnerController
				.addThalamusEvidenceItems(evidenceItemsWrapper
						.getListFromThalamus());
		int learnerSessionNumber = 1;
		LearnerStateInfo lsi = null;
		HashMap<Integer, String> learnerIds = new HashMap<Integer, String>();

		int participantId = 0;
		
		if (evidenceItemsWrapper.getListFromThalamus() != null
				&& !evidenceItemsWrapper.getListFromThalamus().isEmpty()) {
			for (ThalamusEvidenceItem thalamusEvidenceItem : evidenceItemsWrapper
					.getListFromThalamus()) {
				String present = learnerIds.get(new Integer(
						thalamusEvidenceItem.getLearnerId()));
				learnerSessionNumber = thalamusEvidenceItem.getSessionId();
				if (present == null) {
					learnerIds.put(
							new Integer(thalamusEvidenceItem.getLearnerId()),
							"present");
				}
			}
		}
		Iterator it = learnerIds.entrySet().iterator();
		while (it.hasNext()) {
			Map.Entry pairs = (Map.Entry) it.next();
			// System.out.println(pairs.getKey() + " = " + pairs.getValue());

			int learnerId = (Integer) pairs.getKey();
			participantId = learnerId;
			// Session session =
			// sessionController.getSessionForThalamusLearnerId(learnerId,learnerSessionNumber);
			/*
			 * if(session!=null) { learnerSessionNumber =
			 * session.getSessionNumber(); }
			 */

			lsi = recalculateLSIandEvaluate(learnerId, learnerSessionNumber, true,true);

		}

		evaluateMemoryEvent(evidenceItemsWrapper.getListFromThalamus(),lsi,learnerSessionNumber,participantId);
		
		
		return returnMessage;
	}

	private void evaluateMemoryEvent(List<ThalamusEvidenceItem> listFromThalamus, LearnerStateInfo lsi, int learnerSessionNumber, int participantId) {
		if( listFromThalamus!=null && !listFromThalamus.isEmpty()){
			for( ThalamusEvidenceItem thalamusEvidenceItem: listFromThalamus)
			{
				//TODO see if this has anything that we need to check for... 
				logger.info("EvaluateMemoryEvent:"+thalamusEvidenceItem.toString());
				if(thalamusEvidenceItem.getEvidenceName()!=null)
				{
					if(thalamusEvidenceItem.getEvidenceName().compareToIgnoreCase("toolUsed")==0)
					{
						
						
						if(thalamusEvidenceItem.getActual()!=null && thalamusEvidenceItem.getAction()!=null && thalamusEvidenceItem.getAction().compareToIgnoreCase("policyTooltipShowed")==0)
						{
							//TODO Policy investigation
							String policy = thalamusEvidenceItem.getActual();
							//TODO see if one has been implemented
							//Look for "implementPolicy" as action...structure type as actual			
							
							
							boolean alreadyDone = learnerController.checkIfActualAndActionHappensInEarlierSession(policy,"implementPolicy",learnerSessionNumber,participantId );
							
							
						}
						else if(thalamusEvidenceItem.getActual()!=null && thalamusEvidenceItem.getAction()!=null && thalamusEvidenceItem.getAction().compareToIgnoreCase("implementPolicy")==0)
						{
							//TODO Policy investigation
							String policy = thalamusEvidenceItem.getActual();
							//TODO see if one has been implemented
							//Look for "implementPolicy" as action...structure type as actual	
						}
						else if(thalamusEvidenceItem.getActual()!=null && thalamusEvidenceItem.getAction()!=null && thalamusEvidenceItem.getAction().compareToIgnoreCase("upgradeTooltipShowed")==0)
						{
							//TODO Upgrade investigation
							String upgrade = thalamusEvidenceItem.getActual();
							
							//TODO see if one has been performed
							//Look for "performUpgrade" as action...structure type as actual 
							boolean alreadyDone = learnerController.checkIfActualAndActionHappensInEarlierSession(upgrade,"performUpgrade",learnerSessionNumber,participantId );
							
						}
						else if(thalamusEvidenceItem.getActual()!=null && thalamusEvidenceItem.getAction()!=null && thalamusEvidenceItem.getAction().compareToIgnoreCase("performUpgrade")==0)
						{
							//TODO Upgrade investigation
							String upgrade = thalamusEvidenceItem.getActual();
							
							//TODO see if one has been performed
							//Look for "performUpgrade" as action...structure type as actual 
						}
						else if(thalamusEvidenceItem.getActual()!=null && thalamusEvidenceItem.getAction()!=null && thalamusEvidenceItem.getAction().compareToIgnoreCase("buildingMenuSelected")==0 )
						{
							//TODO Structure investigation
							String building = thalamusEvidenceItem.getActual();
							//TODO see if one has been built
							//Look for "confirmConstruction" as action...structure type as actual 
							boolean alreadyDone = learnerController.checkIfActualAndActionHappensInEarlierSession(building,"confirmConstruction",learnerSessionNumber,participantId );
							
						}
						else if(thalamusEvidenceItem.getActual()!=null && thalamusEvidenceItem.getAction()!=null && thalamusEvidenceItem.getAction().compareToIgnoreCase("confirmConstruction")==0 )
						{
							//TODO Structure investigation
							String building = thalamusEvidenceItem.getActual();
							//TODO see if one has been built
							//Look for "confirmConstruction" as action...structure type as actual 
							
						}
					/*	else if(thalamusEvidenceItem.getActual()!=null && thalamusEvidenceItem.getActual().compareToIgnoreCase("buildingMenuSelected")==0)
						{
							
						}
						else if(thalamusEvidenceItem.getActual()!=null &&thalamusEvidenceItem.getActual().compareToIgnoreCase("policyMenuShowed")==0)
						{
							
						}*/
						
					}
					
					if(thalamusEvidenceItem.getEvidenceName().compareToIgnoreCase("levelReached")==0)
					{
						//TODO some comment on level reached...
						
					}
					
					
					
				}
				
				
				
				
			}
		}
	}

	private LearnerStateInfo recalculateLSIandEvaluate(int learnerId,
			int learnerSessionNumber, boolean send, boolean saveToWorking) {

		// TODO generate LSI...
		// TODO look up previous LSI... to generate deltas.
		ArrayList<LearnerStateInfo> lsiPreviousThisSession = getPreviousLSIForThisSessionLearnerAndSession(
				learnerId, learnerSessionNumber);
		// TODO look up previous LSI from other sessions.
		ArrayList<LearnerStateInfo> lsiPreviousSessions = getPreviousSessionsLSIForLearnerAndSession(
				learnerId, learnerSessionNumber);

		LearnerStateInfo lsi = getCurrentLSIForLearnerAndSession(learnerId,
				learnerSessionNumber, lsiPreviousThisSession,
				lsiPreviousSessions,learnerSessionNumber);

		if (lsi != null && send) {
			// TODO send new LSI?
			if (lsi.getCompetencyItems() != null
					&& !lsi.getCompetencyItems().isEmpty()) {

				if(saveToWorking)
				{
					addLSIToPrevious(lsi);
				}
				thalamusManager.sendLSI(lsi);
			}
		}
		return lsi;
	}

	private void addLSIToPrevious(LearnerStateInfo lsi) {
		ArrayList<LearnerStateInfo> lsis = previousLsis.get(new Integer(lsi
				.getLearnerId()));
		if (lsis == null) {
			lsis = new ArrayList<LearnerStateInfo>();
			lsis.add(lsi);
			previousLsis.put(new Integer(lsi.getLearnerId()), lsis);
		} else {
			lsis.add(lsi);
			previousLsis.put(new Integer(lsi.getLearnerId()), lsis);
		}
	}

	private ArrayList<LearnerStateInfo> getPreviousSessionsLSIForLearnerAndSession(
			int learnerId, int learnerSessionNumber) {
		// TODO Auto-generated method stub
		// getCurrentLSIForLearnerAndSession(learnerId,learnerSessionNumber,lsiPreviousThisSession,lsiPreviousSessions);

		ArrayList<LearnerStateInfo> lsis = null;
		if (learnerSessionNumber > 1) {
			lsis = new ArrayList<LearnerStateInfo>();
			for (int i = 1; i < learnerSessionNumber; i++) {
				// TODO put in array and look up for efficiency...
				LearnerStateInfo lsi = getCurrentLSIForLearnerAndSession(learnerId, i, null, null,learnerSessionNumber);
				lsis.add(lsi);
			}
		}

		return lsis;
	}

	private ArrayList<LearnerStateInfo> getPreviousLSIForThisSessionLearnerAndSession(
			int learnerId, int learnerSessionNumber) {
		ArrayList<LearnerStateInfo> lsis = previousLsis.get(new Integer(
				learnerId));

		return lsis;
	}

	private LearnerStateInfo getCurrentLSIForLearnerAndSession(int learnerId,
			int learnerSessionNumber,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions, int currentSession) {
		LearnerStateInfo lsi = new LearnerStateInfo();
		int activityId = 0;
		int scenarioId = 0;
		int sessionId = learnerSessionNumber;
		int stepId = 0;
		ArrayList<ThalamusCompetencyItem> competencyItems = new ArrayList<ThalamusCompetencyItem>();
		ArrayList<EvidenceItem> eis = learnerController
				.loadAllEvidenceItemsForLearner(learnerId);

		boolean valuesSet = false;

		if (eis != null && !eis.isEmpty()) {
			// TODO Group all of the items...
			Map<String, Collection<EvidenceItem>> multiValueMap = new HashMap<String, Collection<EvidenceItem>>();
			for (EvidenceItem evidenceItem : eis) {
				Object obj = multiValueMap.get(evidenceItem.getType());
				List list;
				if (obj == null) {
					list = new ArrayList<EvidenceItem>();
				} else {
					list = ((ArrayList) obj);
				}
				if (evidenceItem.getSession() != null
						&& evidenceItem.getSession().getSessionNumber()
								.equals(new Integer(learnerSessionNumber))) {
					list.add(evidenceItem);
				}
				multiValueMap.put(evidenceItem.getType(), list);
			}

			Iterator it = multiValueMap.entrySet().iterator();
			while (it.hasNext()) {
				Map.Entry pairs = (Map.Entry) it.next();
				// System.out.println(pairs.getKey() + " = " +
				// pairs.getValue());
				String evidenceTypeName = (String) pairs.getKey();
				ArrayList<EvidenceItem> list = ((ArrayList<EvidenceItem>) pairs
						.getValue());

				if (!valuesSet && list != null && !list.isEmpty()) {
					for (EvidenceItem ei : list) {
						activityId = ei.getActivityId();
						stepId = ei.getStepId();
					}
					valuesSet = true;
				}
				ThalamusCompetencyItem thalamusCompetencyItem = createThalamusCompetencyItem(evidenceTypeName, list, lsiPreviousThisSession,lsiPreviousSessions, currentSession, learnerSessionNumber);

				if (thalamusCompetencyItem != null) {
					competencyItems.add(thalamusCompetencyItem);
				}
				it.remove(); // avoids a ConcurrentModificationException

			}
		}
		lsi.setActivityId(activityId);
		lsi.setCorrect(false);
		lsi.setLearnerId(learnerId);
		lsi.setMapEventId("NA");
		lsi.setReasonForUpdate("ToolUse");
		lsi.setScenarioId(scenarioId);
		lsi.setSessionId(sessionId);
		lsi.setStepId(stepId);
		lsi.setCompetencyItems(competencyItems);

		return lsi;
	}

	private ThalamusCompetencyItem createThalamusCompetencyItem(
			String evidenceTypeName, ArrayList<EvidenceItem> list,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions, int currentSession, int learnerSessionNumber) {
		ThalamusCompetencyItem thalamusCompetencyItem = new ThalamusCompetencyItem();
		thalamusCompetencyItem.setCompetencyType(evidenceTypeName);
		if (list != null && !list.isEmpty()) {
			/*
			 * TODO use these to build the competency item 0 numerical, 1
			 * impact, 2 strategyWeight, 3 resourceWeight, 4 toolUse, 5 endGame,
			 * 6 roll, 7 levelReached, 8 turnChanged, 9 takeTurn
			 */
			/*
			 * if(evidenceTypeName.equalsIgnoreCase("4")) { //Tool Use
			 * thalamusCompetencyItem =
			 * createThalamusCompetencyItemToolUse(evidenceTypeName,list,
			 * lsiPreviousThisSession, lsiPreviousSessions); }
			 */
			if (evidenceTypeName.equalsIgnoreCase("5")) {
				// End Game Score
				thalamusCompetencyItem = createThalamusCompetencyItemEndGame(
						evidenceTypeName, list, lsiPreviousThisSession,
						lsiPreviousSessions, thalamusCompetencyItem);
			} else if (evidenceTypeName.equalsIgnoreCase("7")) {
				// Level reached
				thalamusCompetencyItem = createThalamusCompetencyItemLevelReached(
						evidenceTypeName, list, lsiPreviousThisSession,
						lsiPreviousSessions, thalamusCompetencyItem);
			} else if (evidenceTypeName.equalsIgnoreCase("10")
					|| evidenceTypeName.equalsIgnoreCase("11")
					|| evidenceTypeName.equalsIgnoreCase("12")) {
				thalamusCompetencyItem = createThalamusCompetencyItemS1(
						evidenceTypeName, list, lsiPreviousThisSession,
						lsiPreviousSessions, thalamusCompetencyItem);
			}
		} else {
			//This sets the competency if evidence is null from the previous session... 
			Double comptencyValue = null;
			if (lsiPreviousSessions != null && !lsiPreviousSessions.isEmpty()) {
				for (LearnerStateInfo lsip : lsiPreviousSessions) {

					if (lsip.getCompetencyItems() != null
							&& !lsip.getCompetencyItems().isEmpty()) {
						for (ThalamusCompetencyItem tcip : lsip
								.getCompetencyItems()) {
							if (tcip.getComptencyValue() != null) {
								comptencyValue = tcip.getComptencyValue();
							}
						}
					}
				}
			}
			if (comptencyValue != null) {
				thalamusCompetencyItem.setComptencyValue(comptencyValue);
			}
			if(thalamusCompetencyItem.getCompetencyName()==null || thalamusCompetencyItem.getCompetencyName().compareToIgnoreCase("")==0 )
			{
				if(evidenceTypeName.compareToIgnoreCase("10")==0)
				{
					thalamusCompetencyItem.setCompetencyName("distance");
				}
				else if(evidenceTypeName.compareToIgnoreCase("11")==0)
				{
					thalamusCompetencyItem.setCompetencyName("direction");
				}
				else if(evidenceTypeName.compareToIgnoreCase("12")==0)
				{
					thalamusCompetencyItem.setCompetencyName("symbol");
				}
			}
		}
		// else if(evidenceTypeName.e)
		/*
		 * for(EvidenceItem ei:list) {
		 * 
		 * }
		 */
		if(currentSession == learnerSessionNumber)
		{
			
			thalamusCompetencyItem = setMemory(thalamusCompetencyItem,evidenceTypeName, lsiPreviousThisSession, lsiPreviousSessions,currentSession);
		}
		return thalamusCompetencyItem;
	}

	private ThalamusCompetencyItem createThalamusCompetencyItemS1(
			String evidenceTypeName, ArrayList<EvidenceItem> list,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions,
			ThalamusCompetencyItem thalamusCompetencyItem) {

		// String comptencyValue = "";
		EvidenceItem item = null;
		for (EvidenceItem evidenceItem : list) {
			// comptencyValue = evidenceItem.getEvidenceGiven();
			item = evidenceItem;
		}

		if (item != null) {
			thalamusCompetencyItem
					.setComptencyValue(new Double(item.getData()));
			thalamusCompetencyItem.setCompetencyName(item.getName());
			thalamusCompetencyItem.setCompetencyCorrect(item.getCorrect());
			thalamusCompetencyItem.setCompetencyActual(item.getEvidenceGiven());
			thalamusCompetencyItem.setCompetencyExpected(item
					.getEvidenceRequired());
		}

		if(thalamusCompetencyItem.getCompetencyName()==null || thalamusCompetencyItem.getCompetencyName().compareToIgnoreCase("")==0 )
		{
			if(evidenceTypeName.compareToIgnoreCase("10")==0)
			{
				thalamusCompetencyItem.setCompetencyName("distance");
			}
			else if(evidenceTypeName.compareToIgnoreCase("11")==0)
			{
				thalamusCompetencyItem.setCompetencyName("direction");
			}
			else if(evidenceTypeName.compareToIgnoreCase("12")==0)
			{
				thalamusCompetencyItem.setCompetencyName("symbol");
			}
		}
		
		return thalamusCompetencyItem;
	}

	private ThalamusCompetencyItem setMemory(
			ThalamusCompetencyItem thalamusCompetencyItem,
			String evidenceTypeName,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions, int currentSession) {

		thalamusCompetencyItem = setShortTermMemory(thalamusCompetencyItem,
				evidenceTypeName, lsiPreviousThisSession, lsiPreviousSessions,currentSession);
		thalamusCompetencyItem = setWorkingMemory(thalamusCompetencyItem,
				evidenceTypeName, lsiPreviousThisSession, lsiPreviousSessions,currentSession);
		return thalamusCompetencyItem;
	}

	private ThalamusCompetencyItem createThalamusCompetencyItemEndGame(
			String evidenceTypeName, ArrayList<EvidenceItem> list,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions,
			ThalamusCompetencyItem thalamusCompetencyItem) {

		thalamusCompetencyItem.setCompetencyName("endGame");
		String comptencyValue = "";
		if (list != null && !list.isEmpty()) {
			for (EvidenceItem evidenceItem : list) {
				//comptencyValue = evidenceItem.getEvidenceGiven();
			}

		}

		if (comptencyValue.compareToIgnoreCase("") != 0) {
			thalamusCompetencyItem
					.setComptencyValue(new Double(comptencyValue));
		}

		return thalamusCompetencyItem;
	}

	private ThalamusCompetencyItem createThalamusCompetencyItemToolUse(
			String evidenceTypeName, ArrayList<EvidenceItem> list,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions) {
		// TODO Auto-generated method stub
		return null;
	}

	private ThalamusCompetencyItem createThalamusCompetencyItemLevelReached(
			String evidenceTypeName, ArrayList<EvidenceItem> list,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions,
			ThalamusCompetencyItem thalamusCompetencyItem) {

		String comptencyValue = "";
        thalamusCompetencyItem.setCompetencyName("levelReached");

		for (EvidenceItem evidenceItem : list) {
			comptencyValue = evidenceItem.getEvidenceGiven();
		}

		if (comptencyValue.compareToIgnoreCase("") != 0) {
			thalamusCompetencyItem
					.setComptencyValue(new Double(comptencyValue));
		}

		return thalamusCompetencyItem;
	}

	private ThalamusCompetencyItem setWorkingMemory(
			ThalamusCompetencyItem thalamusCompetencyItem,
			String evidenceTypeName,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions, int currentSession) {
		
		
		StringBuilder valueHistoryBuilder = new StringBuilder();
		
		if(lsiPreviousThisSession!=null&&!lsiPreviousThisSession.isEmpty())
		{	valueHistoryBuilder.append("{");
			for(LearnerStateInfo learnerStateInfo:lsiPreviousThisSession)
			{
				if(learnerStateInfo.getCompetencyItems()!=null&&!learnerStateInfo.getCompetencyItems().isEmpty())
				{
					for(ThalamusCompetencyItem prevThalamusCompetencyItem:learnerStateInfo.getCompetencyItems())
					{
						if(prevThalamusCompetencyItem.getCompetencyType().equalsIgnoreCase(evidenceTypeName))
						{
							valueHistoryBuilder.append(prevThalamusCompetencyItem.getComptencyValue()+",");
						}
					}
				}
			}
			valueHistoryBuilder.append("}");
		}
		
		
		if (thalamusCompetencyItem != null) {
			Double comptencyValue = thalamusCompetencyItem.getComptencyValue();
			if (comptencyValue != null) {
				ThalamusCompetencyItem previousCompetencyItem = getPreviousCompetencyItem(
						evidenceTypeName, lsiPreviousThisSession);
				if (previousCompetencyItem != null && previousCompetencyItem.getComptencyValue()!=null) {
					Double previousValue = previousCompetencyItem
							.getComptencyValue();
					Double delta = new Double(comptencyValue) - previousValue;
					thalamusCompetencyItem.setCompetencyDelta(delta);
					int comp = delta.compareTo(new Double(0));
					if (comp == 0) {
						thalamusCompetencyItem.setWorking("2");
					} else if (comp > 0) {
						thalamusCompetencyItem.setWorking("1");
					} else {
						thalamusCompetencyItem.setWorking("0");
					}

					/*
					 * decrease,0 increase,1 same,2 fluctuation,3 nocomparision4
					 */
				} else {
					thalamusCompetencyItem.setWorking("4");
				}
			} else {
				thalamusCompetencyItem.setWorking("4");
			}
		}
		
		String valueHistory = valueHistoryBuilder.toString();
		logger.info("Setting Working Memory for:"+evidenceTypeName);
		logger.info("Value History This Session:"+valueHistory);
		if(thalamusCompetencyItem.getWorking()!=null)
		{
		logger.info("Delta:"+thalamusCompetencyItem.getWorking());
		}
		else
		{
			logger.info("Delta:null");
		}
		return thalamusCompetencyItem;
	}

	private ThalamusCompetencyItem setShortTermMemory(
			ThalamusCompetencyItem thalamusCompetencyItem,
			String evidenceTypeName,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession,
			ArrayList<LearnerStateInfo> lsiPreviousSessions, int currentSession) {
	
		StringBuilder valueHistoryBuilder = new StringBuilder();

		Double highestValuePreviousSessions = null;
		Double lowestFinalValuePreviousSessions = null;
		Double previousValue = null;
		Double comptencyValue = thalamusCompetencyItem.getComptencyValue();
		if (thalamusCompetencyItem != null) {
			
			if (comptencyValue != null) {
				
				boolean highestAndLowestSet = false;
				boolean alwaysIncreasing = true;
				boolean alwaysDecreasing = true;
				boolean alwaysSame = true;
				boolean increasing = false;
				boolean decreasing = false;
				boolean same = false;
				boolean fluctuating = true;

				if (lsiPreviousSessions != null
						&& !lsiPreviousSessions.isEmpty()) {
					for (LearnerStateInfo lsi : lsiPreviousSessions) {
						valueHistoryBuilder.append("{");
						List<ThalamusCompetencyItem> competencyItems = lsi
								.getCompetencyItems();
						if (competencyItems != null && !competencyItems.isEmpty()) {
							for (ThalamusCompetencyItem previousCompetencyItem : competencyItems) {
								if (previousCompetencyItem.getCompetencyType().equalsIgnoreCase(evidenceTypeName)) {
									valueHistoryBuilder.append(previousCompetencyItem.getComptencyValue()+",");
									if (!highestAndLowestSet
											&& previousCompetencyItem
													.getComptencyValue() != null) {
										//Sets values to an initial value... 
										highestValuePreviousSessions = previousCompetencyItem
												.getComptencyValue();
										lowestFinalValuePreviousSessions = previousCompetencyItem
												.getComptencyValue();
										previousValue = previousCompetencyItem
												.getComptencyValue();
										highestAndLowestSet = true;
									}
									if (previousCompetencyItem
											.getComptencyValue() != null
											&& previousCompetencyItem
													.getComptencyValue()
													.compareTo(
															highestValuePreviousSessions) > 0) {
										//sets highest val
										highestValuePreviousSessions = previousCompetencyItem
												.getComptencyValue();
										increasing = true;
									}
									else
									{
										increasing = false;
									}
									if (previousCompetencyItem
											.getComptencyValue() != null
											&& previousCompetencyItem
													.getComptencyValue()
													.compareTo(
															lowestFinalValuePreviousSessions) < 0) {
										//set lowest val
										lowestFinalValuePreviousSessions = previousCompetencyItem
												.getComptencyValue();
										decreasing = true;
									}
									else
									{
										decreasing = false;
									}
									if (previousCompetencyItem
											.getComptencyValue() != null) {
										//Updates to last possible value
										if(previousCompetencyItem
											.getComptencyValue().compareTo(previousValue)==0)
										{
											same = true;
										}
										else
										{
											same = false;
										}
										previousValue = previousCompetencyItem
												.getComptencyValue();
										
										
									}

								}
							}
						}
						valueHistoryBuilder.append("}");
					}
				}

				if (lowestFinalValuePreviousSessions != null) {
					thalamusCompetencyItem
							.setLowestFinalValuePreviousSessions(lowestFinalValuePreviousSessions);
				}
				if (highestValuePreviousSessions != null) {
					thalamusCompetencyItem
							.setHighestValuePreviousSessions(highestValuePreviousSessions);
				}

			} 
		}
		
		if(previousValue!=null)
		{
			thalamusCompetencyItem.setFinalvaluePreviousSession(previousValue);
			if(comptencyValue!=null)
			{
				Double delta = comptencyValue - previousValue;
				thalamusCompetencyItem.setCompetencyDelta(delta);
				int comp = delta.compareTo(new Double(0));
				if (comp == 0) {
					thalamusCompetencyItem.setShortTerm("2");
					thalamusCompetencyItem.setLongTerm("2");
				} else if (comp > 0) {
					thalamusCompetencyItem.setShortTerm("1");
					thalamusCompetencyItem.setLongTerm("1");
				} else {
					thalamusCompetencyItem.setShortTerm("0");
					thalamusCompetencyItem.setLongTerm("0");
				}
			}
		}
		else {
			thalamusCompetencyItem.setShortTerm("4");
			thalamusCompetencyItem.setLongTerm("4");
		}
		/*
		 * decrease,0 increase,1 same,2 fluctuation,3 nocomparision4
		 */
		String valueHistory = valueHistoryBuilder.toString();
		logger.info("Setting Short Term and Long Term Memory for:"+evidenceTypeName);
		logger.info("Value History Previous Sessions:"+valueHistory);
		if(lowestFinalValuePreviousSessions!=null)
		{
		logger.info("lowestFinalValuePreviousSessions:"+lowestFinalValuePreviousSessions);
		}
		else
		{
			logger.info("lowestFinalValuePreviousSessions:null");
		}
		if(highestValuePreviousSessions!=null)
		{
		logger.info("highestValuePreviousSessions:"+highestValuePreviousSessions);
		}
		else
		{
			logger.info("highestValuePreviousSessions:null");
		}
		if(previousValue!=null)
		{
		logger.info("Previous session Final Value:"+previousValue);
		}
		else
		{
			logger.info("Previous session Final Value:null");
		}
	
		logger.info("Short Term:"+thalamusCompetencyItem.getShortTerm());
		logger.info("Long Term:"+thalamusCompetencyItem.getLongTerm());
		return thalamusCompetencyItem;
	}

	private ThalamusCompetencyItem getPreviousCompetencyItem(
			String evidenceTypeName,
			ArrayList<LearnerStateInfo> lsiPreviousThisSession) {
		if (lsiPreviousThisSession != null && !lsiPreviousThisSession.isEmpty()) {
			LearnerStateInfo lsi = lsiPreviousThisSession
					.get(lsiPreviousThisSession.size() - 1);
			List<ThalamusCompetencyItem> competencyItems = lsi
					.getCompetencyItems();
			if (competencyItems != null && !competencyItems.isEmpty()) {
				for (ThalamusCompetencyItem previousCompetencyItem : competencyItems) {
					if (previousCompetencyItem.getCompetencyType()
							.equalsIgnoreCase(evidenceTypeName)) {
						return previousCompetencyItem;
					}
				}
			}
		}
		return null;
	}

	@RequestMapping(value = "/setLearnerInfo", method = RequestMethod.POST)
	public @ResponseBody String setLearnerInfo(
			@RequestBody LearnerDetails learnerDetails) throws Exception {
		logger.info("Got Thalamus Update:setLearnerInfo");

		Learner learner = learnerController
				.getLearnerByThalamusId(learnerDetails.getThalamusLearnerId());
		if (learner == null) {
			learner = learnerController.createLearner(
					learnerDetails.getFirstName(),
					learnerDetails.getMiddleName(),
					learnerDetails.getLastName(), learnerDetails.getSex(),
					learnerDetails.getBirth(),
					learnerDetails.getThalamusLearnerId());
		} else {
			learner = learnerController.updateLearner(
					learnerDetails.getFirstName(),
					learnerDetails.getMiddleName(),
					learnerDetails.getLastName(), learnerDetails.getSex(),
					learnerDetails.getBirth(),
					learnerDetails.getThalamusLearnerId());

		}
		return "Update Success";
	}

	@RequestMapping(value = "/getNextThalamusId", method = RequestMethod.POST)
	public @ResponseBody String getNextThalamusId() throws Exception {
		logger.info("Got Thalamus Update:setLearnerInfo");

		ArrayList<LearnerDetails> learnerInfos = new ArrayList<LearnerDetails>();
		learnerInfos = learnerController.getAllLearnerDetails();

		int highestId = 0;
		if (learnerInfos != null && !learnerInfos.isEmpty()) {
			for (LearnerDetails learnerDetails : learnerInfos) {
				int learnerId = learnerDetails.getThalamusLearnerId()
						.intValue();
				if (learnerId > highestId) {
					highestId = learnerId;
				}

			}
		}

		highestId = highestId + 1;
		thalamusManager.nextThalamusId(highestId);

		return "Update Success";
	}

	@RequestMapping(value = "/getAllLearnerInfo", method = RequestMethod.POST)
	public @ResponseBody String getAllLearnerInfo() throws Exception {
		logger.info("Got Thalamus Update:getAllLearnerInfo");
		ArrayList<LearnerDetails> learnerInfos = new ArrayList<LearnerDetails>();
		learnerInfos = learnerController.getAllLearnerDetails();
		// TODO get all learner details from the DB...
		thalamusManager.allLearnerInfo(learnerInfos);

		return "Update Success";
	}

	@RequestMapping(value = "/start", method = RequestMethod.POST)
	public @ResponseBody String start(@RequestBody StartWrapper startWrapper)
			throws Exception {
		logger.info("Got Thalamus Start, Player 1: "
				+ startWrapper.getParticipantId() + " Player 2: "
				+ startWrapper.getParticipantId2());

		int participant1 = startWrapper.getParticipantId();
		int participant2 = startWrapper.getParticipantId2();
		// learnerController.incrementSession(participant1);
		//sendAllUtterancesForParticipant(participant1);
		int learnerSessionNumber = new Integer(startWrapper.getSessionNumber())
				.intValue();
		Session session = sessionController.getSessionForThalamusLearnerId(
				participant1, learnerSessionNumber);
		Learner learner = learnerController.getLearnerByThalamusId(new Integer(
				participant1));
		if (learner == null) {
			learner = learnerController.createLearner("Learner", "", "Default",
					"M", "01/01/2001", new Integer(participant1));
		}
		if (session == null) {
			// learnerSessionNumber = session.getSessionNumber();
			sessionController.createNewSessionForLearnerId(participant1,
					learnerSessionNumber);
		}
		/*
		 * if(session.getSessionNumber().equals(new
		 * Integer(learnerSessionNumber))) {
		 * sessionController.createNewSessionForLearnerId(participant1,
		 * learnerSessionNumber); }
		 */
		recalculateLSIandEvaluate(participant1, learnerSessionNumber, true,false);
		sendAllUtterancesForParticipant(participant1);
		if (participant2 > 0) {

			// learnerController.incrementSession(participant2);
			//sendAllUtterancesForParticipant(participant2);

			learner = learnerController.getLearnerByThalamusId(new Integer(
					participant2));
			if (learner == null) {
				learner = learnerController
						.createLearner("Learner", "", "Default", "M",
								"01/01/2001", new Integer(participant2));
			}
			session = sessionController.getSessionForThalamusLearnerId(
					participant2, learnerSessionNumber);

			if (session == null) {
				// learnerSessionNumber = session.getSessionNumber();
				sessionController.createNewSessionForLearnerId(participant2,
						learnerSessionNumber);
			}
			/*
			 * if(session.getSessionNumber().equals(new
			 * Integer(learnerSessionNumber))) {
			 * sessionController.createNewSessionForLearnerId(participant2,
			 * learnerSessionNumber); }
			 */

			// learnerSessionNumber = 1;
			/*
			 * if(session!=null) { learnerSessionNumber =
			 * session.getSessionNumber(); }
			 */
			recalculateLSIandEvaluate(participant2, learnerSessionNumber, true,false);
			sendAllUtterancesForParticipant(participant2);

			sendSummaryForLearners(participant1, participant2,
					learnerSessionNumber);

		}

		return "Update Success";
	}

	@RequestMapping(value = "/getAllUtterancesForParticipant", method = RequestMethod.POST)
	public @ResponseBody String getAllUtterancesForParticipant(
			@RequestBody ParticipantWrapper participantWrapper)
			throws Exception {
		Integer participantId = new Integer(
				participantWrapper.getParticipantId());
		logger.info("Got getAllUtterancesForParticipant, Participant: "
				+ participantId.intValue());

		sendAllUtterancesForParticipant(participantId.intValue());
		return "Update Success";
	}

	private void sendAllUtterancesForParticipant(int participant) {

		ArrayList<Utterance> utterances = learnerController
				.getAllUtterancesForThalamusId(participant);
		if (utterances != null && !utterances.isEmpty()) {
			thalamusManager.sendAllUtterancesForParticipant(participant,
					utterances);
		}
	}

	@RequestMapping(value = "/addEnercitiesGameState", method = RequestMethod.POST)
	public @ResponseBody String addEnercitiesGameState(
			@RequestBody EnercitiesGameInfo enercitiesGameInfo)
			throws Exception {
		logger.info("Got enercities game state for:"
				+ enercitiesGameInfo.getLearner1() + " and:"
				+ enercitiesGameInfo.getLearner2());

		Learner learner = learnerController.getLearnerByThalamusId(new Integer(
				enercitiesGameInfo.getLearner1()));
		if (learner == null) {
			learner = learnerController.createLearner("Learner", "", "Default",
					"M", "01/01/2001",
					new Integer(enercitiesGameInfo.getLearner1()));
		}
		learner = learnerController.getLearnerByThalamusId(new Integer(
				enercitiesGameInfo.getLearner2()));
		if (learner == null) {
			learner = learnerController.createLearner("Learner", "", "Default",
					"M", "01/01/2001",
					new Integer(enercitiesGameInfo.getLearner2()));
		}
		learnerController.addEnercitiesGameState(enercitiesGameInfo,
				new Integer(enercitiesGameInfo.getLearner1()));

		learnerController.addEnercitiesGameState(enercitiesGameInfo,
				new Integer(enercitiesGameInfo.getLearner2()));

		// TODO

		return "game state added to db";
	}

	@RequestMapping(value = "/utteranceUpdate", method = RequestMethod.POST)
	public @ResponseBody String utteranceUpdate(
			@RequestBody UtteranceWrapper utteranceWrapper) throws Exception {
		logger.info("Got Utterance for: " + utteranceWrapper.getParticipantId());

		if (!utteranceWrapper.getUtterenceId().equalsIgnoreCase("")) {

			learnerController.checkUtteranceExists(utteranceWrapper);
			if (utteranceWrapper.getMethod()
					.equalsIgnoreCase("cancelUtterance")) {
				learnerController.cancelUtterance(utteranceWrapper);

			} else if (utteranceWrapper.getMethod().equalsIgnoreCase(
					"performUtterance")) {
				learnerController.performUtterance(utteranceWrapper);

			} else if (utteranceWrapper.getMethod().equalsIgnoreCase(
					"performUtteranceFromLibrary")) {
				learnerController.performUtteranceFromLibrary(utteranceWrapper);
			} else if (utteranceWrapper.getMethod().equalsIgnoreCase(
					"utteranceFinished")) {
				learnerController.utteranceFinished(utteranceWrapper);
			} else if (utteranceWrapper.getMethod().equalsIgnoreCase(
					"utteranceStarted")) {
				learnerController.utteranceStarted(utteranceWrapper);
			} else if (utteranceWrapper.getMethod().equalsIgnoreCase(
					"utteranceIsAQuestion")) {
				learnerController.utteranceIsAQuestion(utteranceWrapper);
			} else if (utteranceWrapper.getMethod().equalsIgnoreCase(
					"utteranceUsed")) {
				learnerController.utteranceUsed(utteranceWrapper);
			}
		}
		return "Update Success";
	}

	@RequestMapping(value = "/enercitiesEndGameTimeout", method = RequestMethod.POST)
	public @ResponseBody String enercitiesEndGameTimeout(
			@RequestBody StartWrapper startWrapper) throws Exception {

		logger.info("Got Thalamus Update:enercitiesEndGameTimeout Player 1:"
				+ startWrapper.getParticipantId() + " Player 2: "
				+ startWrapper.getParticipantId2());
		int participant1 = startWrapper.getParticipantId();
		int participant2 = startWrapper.getParticipantId2();

		int learnerSessionNumber = new Integer(startWrapper.getSessionNumber())
				.intValue();
		/*
		 * Session session =
		 * sessionController.getCurrentSessionForThalamusLearnerId
		 * (participant1); if(session!=null) { learnerSessionNumber =
		 * session.getSessionNumber(); }
		 */
		sendWrapUpForLearners(participant1, participant2, learnerSessionNumber);

		return "Update Success";
	}

	// Wrap up of this session.
	private void sendWrapUpForLearners(int participant1, int participant2,
			int learnerSessionNumber) {

		MemoryEvent memoryEvent = new MemoryEvent();
		memoryEvent.setReasonForUpdate("4");
		memoryEvent.setLearnerId(participant1);
		memoryEvent.setSessionId(learnerSessionNumber);

		ArrayList<MemoryEventItem> memoryEventItems = new ArrayList<MemoryEventItem>();
		MemoryEventItem memoryEventItem = new MemoryEventItem();
		ArrayList<String> tagNames = new ArrayList<String>();
		ArrayList<String> tagValues = new ArrayList<String>();

		LearnerStateInfo lsi = recalculateLSIandEvaluate(participant1,
				learnerSessionNumber, false,false);

		String stmAchievedLevel = "0";
		String wmAchievedLevel = "0";
		String ltmMaxAchievedLevel = "0";
		String ltmMinAchievedLevel = "0";
		
		List<ThalamusCompetencyItem> competencyItems = lsi.getCompetencyItems();
		if (competencyItems != null && !competencyItems.isEmpty()) {
			for (ThalamusCompetencyItem competencyItem : competencyItems) {
				if (competencyItem.getCompetencyName()!=null && competencyItem.getCompetencyName().equalsIgnoreCase(
						"levelReached")) {
					if (competencyItem.getComptencyValue() != null) {
						stmAchievedLevel = competencyItem.getComptencyValue()
								.toString();
					}
					if (competencyItem.getFinalvaluePreviousSession() != null) {
						wmAchievedLevel = competencyItem
								.getFinalvaluePreviousSession().toString();
					}
					if (competencyItem.getHighestValuePreviousSessions() != null) {
						ltmMaxAchievedLevel = competencyItem
								.getHighestValuePreviousSessions().toString();
					}
					if (competencyItem.getLowestFinalValuePreviousSessions() != null) {
						ltmMinAchievedLevel = competencyItem
								.getLowestFinalValuePreviousSessions()
								.toString();
					}
				}

			}
		}

		tagNames.add("stmAchievedLevel");
		tagValues.add(stmAchievedLevel);
		tagNames.add("wmAchievedLevel");
		tagValues.add(wmAchievedLevel);
		tagNames.add("ltmMaxAchievedLevel");
		tagValues.add(ltmMaxAchievedLevel);
		tagNames.add("ltmMinAchievedLevel");
		tagValues.add(ltmMinAchievedLevel);
		
		
		
		
		
		
		memoryEventItem.setCategory("wrapup");
		if (learnerSessionNumber == 1) {
			memoryEventItem.setName("Session1");
			memoryEventItem.setSubcategory("0");// "Today we were able to understand the game dynamics and what is part of building a sustainable city. let's see how our city will look like next week when we play together."
			
		} else if (learnerSessionNumber == 2) {
		
			memoryEventItem.setName("Session2");
			
			//TODO To see if students ALWAYS end the sessions due to No Oil. 
			//TODO check the end game reasons. 
			ArrayList<EvidenceItem> eis =  learnerController.loadAllEndGameEvidenceItemsForLearner(participant1,learnerSessionNumber);
			
			//Default option
			memoryEventItem.setSubcategory("2");//"We have been collaborating a lot but and our city was able to grow keeping it's natural resources. It has been challenging, but we have been doing just great."
			
			if(eis!=null && !eis.isEmpty())
			{
				//Should default to the other......
				//TODO check this... 
				boolean allOil = false;
				for(EvidenceItem evidenceItem : eis)
				{
					
					if(evidenceItem.getEvidenceGiven().equalsIgnoreCase("noOil"))
					{
						allOil = true;
					}
					else
					{
						allOil = false;
						break;
					}
				}
				if(allOil)
				{
					memoryEventItem.setSubcategory("1");//"We have been collaborating a lot but our city was not able to grow because we run out of natural resources. This makes us realize that building a suitable city is a really challenging process."
				}
				
			}
			else
			{
				
			}
			
		} else if (learnerSessionNumber == 3) {
		
			memoryEventItem.setName("Session3");
			
			//This is the fall back option
			memoryEventItem.setSubcategory("6");//"It looks like we have been managing our city resources and oil levels pretty well during all game sessions. Our team work has been contributing to the creation of a very cool sustainable city."
			
			
			// 1) they ended a previous game (Session 1 or Session 2) due to No Oil and 2) if it never happened again until the  end of the current session (Session 3).
			boolean recoveredFromOil = false;
			boolean issueWithOilInEarlierSessions = false;
			boolean issueWithOilThisSession = false;
			ArrayList<EvidenceItem> eis =  learnerController.loadAllEndGameEvidenceItemsForLearner(participant1,learnerSessionNumber);
			if(eis!=null && !eis.isEmpty())
			{
				
				for(EvidenceItem evidenceItem : eis)
				{
					
					if(evidenceItem.getEvidenceGiven().equalsIgnoreCase("noOil"))
					{
						if(evidenceItem.getSession().getSessionNumber().compareTo(new Integer(3))<0)
						{
							issueWithOilInEarlierSessions = true;
						}
						else
						{
							issueWithOilThisSession = true;
						}
						
					}
				}
			}
			if(issueWithOilInEarlierSessions && !issueWithOilThisSession)
			{
				recoveredFromOil = true;
			}
			if(recoveredFromOil)
			{
				memoryEventItem.setSubcategory("3");//"It looks like our oil remained positive. We have learner the lesson from the session where our game ended due to the shortage of this natural resource Well done!."				
			}
			else
			{
				
				// if they faced an energectic OR economic crisis in AT LEAST ONE of the previous sessions (Sessions 1 and/or Session 2); and if 3) they did not faced a crisis in the current session.
				boolean recoveredFromCrisis = false;
				boolean issueWithCrisisInEarlierSessions = false;
				boolean issueWithCrisisThisSession = false;
				String crisis = "";
				eis =  learnerController.loadAllCrisisEvidenceItemsForLearner(participant1,learnerSessionNumber);
				if(eis!=null && !eis.isEmpty())
				{					
					for(EvidenceItem evidenceItem : eis)
					{				
						if(evidenceItem.getSession().getSessionNumber().compareTo(new Integer(3))<0)
						{
							issueWithCrisisInEarlierSessions = true;
							crisis = evidenceItem.getEvidenceGiven();
						}
						else
						{
							issueWithCrisisThisSession = true;
							crisis = evidenceItem.getEvidenceGiven(); 
						}												
					}
				}
				if(issueWithCrisisInEarlierSessions && !issueWithCrisisThisSession)
				{
					recoveredFromCrisis = true;
				}
				if(recoveredFromCrisis)
				{
					memoryEventItem.setSubcategory("4");//"Although we faced an /crisis/ in the past game, we succeded this time. Nicely done."
					tagNames.add("crisis");
					tagValues.add(crisis);
					
				}
				
				if(issueWithCrisisInEarlierSessions && issueWithCrisisThisSession)
				{
					memoryEventItem.setSubcategory("5");//"Today we have faced an /crisis/ and the same happened in the past game. Despite our best efforts, we are having some trouble managing our resources. I hope we can do better next week."		
					tagNames.add("crisis");
					tagValues.add(crisis);
					
				}			
			}
			
			
			
		} else if (learnerSessionNumber == 4) {
		
			memoryEventItem.setName("Session4");
			
			//Default 
			memoryEventItem.setSubcategory("8");//"We have been collaborating a lot and we have stabilise when reaching level /ltmAchievedLevel/. Our stability required a lot of joint efforts, showing how challenging it was to build a sustainable city."
			
			if(ltmMinAchievedLevel.compareToIgnoreCase(ltmMaxAchievedLevel)!=0)
			{
				memoryEventItem.setSubcategory("7");//"We have been working hard together. Have you noticed that we started reaching level /ltmMinAchievedLevel/ and then we were able to reach /ltmMaxAchievedLevel/. Level progression enabled us to understand how challenging it is to build a sustainable city."		
				
			}
			
		}
		memoryEventItem.setTagNames(tagNames);
		memoryEventItem.setTagValues(tagValues);
		memoryEventItems.add(memoryEventItem);
		memoryEvent.setMemoryEventItems(memoryEventItems);
		thalamusManager.sendMemoryUpdate(memoryEvent);
	}

	// Summary of previous sessions
	private void sendSummaryForLearners(int participant1, int participant2,
			int learnerSessionNumber) {
		MemoryEvent memoryEvent = new MemoryEvent();
		memoryEvent.setReasonForUpdate("3");
		memoryEvent.setLearnerId(participant1);
		memoryEvent.setSessionId(learnerSessionNumber);

		ArrayList<MemoryEventItem> memoryEventItems = new ArrayList<MemoryEventItem>();
		MemoryEventItem memoryEventItem = new MemoryEventItem();

		ArrayList<String> tagNames = new ArrayList<String>();
		ArrayList<String> tagValues = new ArrayList<String>();
		LearnerStateInfo lsi = recalculateLSIandEvaluate(participant1,
				learnerSessionNumber, false,false);

		String stmAchievedLevel = "0";
		String wmAchievedLevel = "0";
		String ltmMaxAchievedLevel = "0";
		String ltmMinAchievedLevel = "0";

		List<ThalamusCompetencyItem> competencyItems = lsi.getCompetencyItems();
		if (competencyItems != null && !competencyItems.isEmpty()) {
			for (ThalamusCompetencyItem competencyItem : competencyItems) {
				if (competencyItem.getCompetencyName()!=null && competencyItem.getCompetencyName().equalsIgnoreCase(
						"levelReached")) {
					if (competencyItem.getComptencyValue() != null) {
						stmAchievedLevel = competencyItem.getComptencyValue()
								.toString();
					}
					if (competencyItem.getFinalvaluePreviousSession() != null) {
						wmAchievedLevel = competencyItem
								.getFinalvaluePreviousSession().toString();
					}
					if (competencyItem.getHighestValuePreviousSessions() != null) {
						ltmMaxAchievedLevel = competencyItem
								.getHighestValuePreviousSessions().toString();
					}
					if (competencyItem.getLowestFinalValuePreviousSessions() != null) {
						ltmMinAchievedLevel = competencyItem
								.getLowestFinalValuePreviousSessions()
								.toString();
					}
				}

			}
		}

		tagNames.add("stmAchievedLevel");
		tagValues.add(stmAchievedLevel);
		/*tagNames.add("wmAchievedLevel");
		tagValues.add(wmAchievedLevel);
		tagNames.add("ltmMaxAchievedLevel");
		tagValues.add(ltmMaxAchievedLevel);
		tagNames.add("ltmMinAchievedLevel");
		tagValues.add(ltmMinAchievedLevel);*/

		memoryEventItem.setCategory("summary");
		if (learnerSessionNumber == 1) {
			// No summary of previous session.,
			memoryEventItem.setName("Session1");
			memoryEventItem.setSubcategory("0");// Send dummy. i.e give intro
			
		} else if (learnerSessionNumber == 2) {
			memoryEventItem.setName("Session2");
			memoryEventItem.setSubcategory("1");// "In our last game we reached level /stmAchievedLevel/. The levels indicate how big a city becomes, but that does not necessarily mean it is a sustainable one. Let’s see what happens today."
		} else if (learnerSessionNumber == 3) {
			// "Do you still remember which level we reached last session?
			// \pau=300\ Exactly, our population grew and we reached level
			// /stmAchievedLevel/."
			memoryEventItem.setName("Session3");
			memoryEventItem.setSubcategory("2");
		} else if (learnerSessionNumber == 4) {
			memoryEventItem.setName("Session4");
			memoryEventItem.setSubcategory("3");// "We have been doing am amazing work together. Remember that this is our last session, so let's keep in mind all the things we have learned until now to build our last sustainable city."
			

		}
		memoryEventItem.setTagNames(tagNames);
		memoryEventItem.setTagValues(tagValues);
		memoryEventItems.add(memoryEventItem);

		memoryEvent.setMemoryEventItems(memoryEventItems);
		thalamusManager.sendMemoryUpdate(memoryEvent);

	}

}
