package uk.co.emote;

import java.net.MalformedURLException;
import java.net.URL;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Service;

import com.google.gson.Gson;

import redstone.xmlrpc.XmlRpcArray;
import redstone.xmlrpc.XmlRpcClient;
import redstone.xmlrpc.XmlRpcException;
import redstone.xmlrpc.XmlRpcProxy;
import redstone.xmlrpc.XmlRpcFault;
import uk.co.emote.transfer.LearnerDetails;
import uk.co.emote.transfer.MemoryEvent;



@Service
public class ThalamusManager {

	private XmlRpcClient client;

	private Lock lock = new ReentrantLock();

	private static final Logger logger = LoggerFactory.getLogger(ThalamusManager.class);
	
	
	public void connectClient() {
		try {
			client = new XmlRpcClient("http://localhost:15001", false);
		} catch (MalformedURLException e) {
			e.printStackTrace();
		}
	}

	@Async
	public void nextThalamusId(int i) {
		lock.lock();
		if (client == null) {
			connectClient();
		}
		try {
			Object token = client.invoke("nextThalamusId",
					new Object[] { i });
		} catch (XmlRpcException e) {
			e.printStackTrace();
		} catch (XmlRpcFault e) {
			e.printStackTrace();
		} finally {
			lock.unlock();
		}
	}
	
	
	@Async
	public void sendAllUtterancesForParticipant(int learnerId, ArrayList<Utterance> utterances) {
		lock.lock();
		if (client == null) {
			connectClient();
		}
		try {
			Gson gson = new Gson();
			
			//String json = gson.toJson(learnerInfos);
			//Object token = client.invoke("allLearnerInfo",new Object[] { json});
			List<Object> myList = new ArrayList<Object>();
			String json;
			for(Utterance utterance : utterances)
			{
				//TODO stick in the new object...
				utterance.setLearner(null);
				json = gson.toJson(utterance);
				//af
				myList.add(json);
			}
			
			Object token = client.invoke("allUtterancesForParticipant",new Object[] {learnerId,myList});
		} catch (XmlRpcException e) {
			e.printStackTrace();
		} catch (XmlRpcFault e) {
			e.printStackTrace();
		} finally {
			lock.unlock();
		}
		
		
		
	}
	
	
	@Async
	public void allLearnerInfo(ArrayList<LearnerDetails> learnerInfos) {
		lock.lock();
		if (client == null) {
			connectClient();
		}
		try {
			Gson gson = new Gson();
			
			//String json = gson.toJson(learnerInfos);
			//Object token = client.invoke("allLearnerInfo",new Object[] { json});
			List<Object> myList = new ArrayList<Object>();
			String json;
			for(LearnerDetails learnerDetail : learnerInfos)
			{
				if(learnerDetail.getScenario1Difficulty()==null)
				{
					learnerDetail.setScenario1Difficulty(new Integer(2));
				}
				json = gson.toJson(learnerDetail);
				myList.add(json);
			}
			
			Object token = client.invoke("allLearnerInfo",new Object[] {myList});
		} catch (XmlRpcException e) {
			e.printStackTrace();
		} catch (XmlRpcFault e) {
			e.printStackTrace();
		} finally {
			lock.unlock();
		}
	}
	@Async
	public void sendLSI(LearnerStateInfo lsi) {
		lock.lock();
		logger.info("LSI:"+lsi.toString());
		
		if (client == null) {
			connectClient();
		}
		try {
			Gson gson = new Gson();
			
			
			String json;
			json = gson.toJson(lsi);
			
			Object token = client.invoke("learnerModelValueUpdate",new Object[] {json});
		} catch (XmlRpcException e) {
			e.printStackTrace();
		} catch (XmlRpcFault e) {
			e.printStackTrace();
		} finally {
			lock.unlock();
		}
		
		
		
	}
	
	@Async
	public void learnerModelValueUpdateBeforeAffectPerceptionUpdate(LearnerStateInfo lsi) {
		lock.lock();
		logger.info("LSI:"+lsi.toString());
		
		if (client == null) {
			connectClient();
		}
		try {
			
			Gson gson = new Gson();
			
			String json;
			json = gson.toJson(lsi);
		
			Object token = client.invoke("learnerModelValueUpdateBeforeAffectPerceptionUpdate",new Object[] {json});
			
		} catch (XmlRpcException e) {
			e.printStackTrace();
		} catch (XmlRpcFault e) {
			e.printStackTrace();
		} finally {
			lock.unlock();
		}
			
		
	}
	
	@Async
	public void sendMemoryUpdate(MemoryEvent memoryEvent) {
		lock.lock();
		if (client == null) {
			connectClient();
		}
		try {
			Gson gson = new Gson();
			
			//String json = gson.toJson(learnerInfos);
			//Object token = client.invoke("allLearnerInfo",new Object[] { json});
			//List<Object> myList2 = new ArrayList<Object>();
			String json;
			json = gson.toJson(memoryEvent);
			/*for(ThalamusCompetencyItem thalamusCompetencyItem : lsi.getCompetencyItems())
			{
				json = gson.toJson(thalamusCompetencyItem);
				myList2.add(json);
			}*/
			
			Object token = client.invoke("learnerModelMemoryEvent",new Object[] {json});
		} catch (XmlRpcException e) {
			e.printStackTrace();
		} catch (XmlRpcFault e) {
			e.printStackTrace();
		} finally {
			lock.unlock();
		}
		
	}

	

}
