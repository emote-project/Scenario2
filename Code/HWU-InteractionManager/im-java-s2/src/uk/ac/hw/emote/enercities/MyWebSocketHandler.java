package uk.ac.hw.emote.enercities;

import java.io.IOException;
import java.util.Calendar;
import java.util.Date;

import org.apache.log4j.Logger;
import org.eclipse.jetty.websocket.api.Session;
import org.eclipse.jetty.websocket.api.annotations.OnWebSocketClose;
import org.eclipse.jetty.websocket.api.annotations.OnWebSocketConnect;
import org.eclipse.jetty.websocket.api.annotations.OnWebSocketError;
import org.eclipse.jetty.websocket.api.annotations.OnWebSocketMessage;
import org.eclipse.jetty.websocket.api.annotations.WebSocket;
import org.eclipse.jetty.websocket.server.WebSocketHandler;
import org.eclipse.jetty.websocket.servlet.WebSocketServletFactory;

@WebSocket
public class MyWebSocketHandler extends WebSocketHandler {
	
	
	Logger logger = Logger.getLogger(GamePlayer.class.getName());
	
	long msgTime = Calendar.getInstance().getTimeInMillis();
	static long lastMsgTime = 0;
	
	//Logger logger = Logger.getLogger(GamePlayer.class.getName());
	static private Session sessionInstance = null;

	static private boolean response = false;
	
	static private String currString = "";
	
	public void waitSomeTime(int miliseconds)
	{
		try {
			Thread.sleep(miliseconds);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public boolean getResponse()
	{
		return response;
	}
	
	public void setResponse(boolean res)
	{
		this.response = res;
	}
	
    @OnWebSocketClose
    public void onClose(int statusCode, String reason) {
        //logger.info("Close: statusCode=" + statusCode + ", reason=" + reason);
    	System.out.println("Close: statusCode=" + statusCode + ", reason=" + reason);
    }

    @OnWebSocketError
    public void onError(Throwable t) {
        System.out.println("Error: " + t.getMessage());
        //logger.info("Error Communication: " + t.getMessage());
    }

    @OnWebSocketConnect
    public void onConnect(Session session) {
        System.out.println("Connect: " + session.getRemoteAddress().getAddress());
        
        //this.session = session;//session.getRemote().sendString("Flávia dias casagrande AZXCZXCZX");
        
         //try 
        //{
        	 sessionInstance = session;
        	 System.out.println(sessionInstance);
            //session.getRemote().sendString("Flávia dias casagrande AZXCZXCZX");
         //} catch (IOException e) {
           // e.printStackTrace();
        //}
    }
    
    @SuppressWarnings("static-access")
    public void sendMessage(String msg)
    {
    	//System.out.println(this.sessionInstance);
    	System.out.println(sessionInstance);
    	if (sessionInstance != null)
    	{
	    	 try {
	    		 sessionInstance.getRemote().sendString(msg);
	    	 } catch (IOException e) {
	             e.printStackTrace();
	         }
    	}
    	//try {
    		//System.out.println("WAIT");
			//wait();
		//} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
		//}
    }

    @OnWebSocketMessage
    public void onMessage(String message) {
    	//logger.info("msgtime: "+this.msgTime);
    	//logger.info("LastMesgTime: "+MyWebSocketHandler.lastMsgTime);
    	//if (this.msgTime > MyWebSocketHandler.lastMsgTime)
    	//{
//    		MyWebSocketHandler.currString = message;
//    		logger.info("Message inside ONMESSAGE from JAvascript: " + message);
//    		logger.info("Message inside ONMESSAGE from JAvascript currSTRING: " + MyWebSocketHandler.currString);
//
//        
//    	
//    		//MyWebSocketHandler.lastMsgTime = this.msgTime;
//    		this.response = true;
//            System.out.println("Response: " + response);
    	//}
    	//else{
    		//this.response = false;
    		//logger.info("response is false!");
    	//}
        
        MyWebSocketHandler.currString = message;
		logger.info("Message inside ONMESSAGE from JAvascript: " + message);
		logger.info("Message inside ONMESSAGE from JAvascript currSTRING: " + MyWebSocketHandler.currString);
		
		this.response = true;
        System.out.println("Response: " + response);
    
        logger.info("WAITING 1 seconds");
        waitSomeTime(1000);
        logger.info("WAITING 1 seconds DONE");
        
        //logger.info("Message from Javascript: " + message);
        //System.out.println("NOTIFY");
        //notify();
    }

	@Override
	public void configure(WebSocketServletFactory arg0) {
		arg0.register(MyWebSocketHandler.class);
	}

	public static String getCurrString() {
		return currString;
	}

	public static void setCurrString(String currString) {
		MyWebSocketHandler.currString = currString;
	}
}