package uk.ac.hw.emote.intman;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.List;

import javax.servlet.ServletConfig;
import javax.servlet.ServletException;

import org.eclipse.jetty.server.Server;
import org.eclipse.jetty.servlet.ServletContextHandler;
import org.eclipse.jetty.servlet.ServletHolder;
import org.eclipse.jetty.util.log.Log;

import redstone.xmlrpc.XmlRpcProxy;
import redstone.xmlrpc.XmlRpcServlet;
import redstone.xmlrpc.handlers.ReflectiveInvocationHandler;
import uk.ac.hw.emote.intman.listener.EmoteListener;
import uk.ac.hw.emote.intman.listener.EnercitiesListener;
import uk.ac.hw.emote.intman.listener.LearnerModelListener;
import uk.ac.hw.emote.intman.listener.MapApplicationListener;
import uk.ac.hw.emote.intman.listener.PerceptionListener;
import uk.ac.hw.emote.intman.listener.SkeneListener;
import emotecommonmessages.IEmoteActions;
import emotecommonmessages.IFMLSpeech;
import emoteenercitiesmessages.IEnercitiesTaskActions;
import emotemapreadingevents.IMapActions;

public class Communication {

    public static final int SERVER_PORT = 11000;
    public static final int CLIENT_PORT = 11100;

    /** An invocation handler that returns true instead of null for void methods */
    private static class CustomInvocationHandler extends ReflectiveInvocationHandler {
        public CustomInvocationHandler (Object target) {
            super (target);
        }

        @SuppressWarnings ("rawtypes")
        @Override
        public Object invoke (String methodName, List arguments) throws Throwable {
            Object result = super.invoke (methodName, arguments);
            return (result == null) ? true : result;
        }

    }

    private static class ServerRunner implements Runnable {
        public void run () {
            Server server = new Server (SERVER_PORT);
            server.setStopAtShutdown (true);

            ServletContextHandler context = new ServletContextHandler ();
            context.setContextPath ("/");
            server.setHandler (context);

            // Create a servlet for the interaction manager
            XmlRpcServlet servlet = new XmlRpcServlet () {
                @Override
                public void init (ServletConfig config) throws ServletException {
                    super.init (config);
                    getXmlRpcServer ().addInvocationHandler ("perception",
                            new CustomInvocationHandler (new PerceptionListener ()));
                    //getXmlRpcServer ().addInvocationHandler ("map",
                      //      new CustomInvocationHandler (new MapApplicationListener ()));
                    getXmlRpcServer ().addInvocationHandler ("skene",
                            new CustomInvocationHandler (new SkeneListener ()));
                    getXmlRpcServer ().addInvocationHandler ("enercities",
                            new CustomInvocationHandler (new EnercitiesListener ()));
                    getXmlRpcServer ().addInvocationHandler ("lm",
                            new CustomInvocationHandler (new LearnerModelListener ()));
                    getXmlRpcServer ().addInvocationHandler ("emote",
                            new CustomInvocationHandler (new EmoteListener ()));
                }
            };

            // Parameters based on the Redstone documentation
            ServletHolder xmlRpcHolder = new ServletHolder ("intman", servlet);
            xmlRpcHolder.setInitParameter ("streamMessages", String.valueOf (1));
            xmlRpcHolder.setInitParameter ("contentType", "text/xml; charset=ISO-8859-1");
            xmlRpcHolder.setInitOrder (2);
            context.addServlet (xmlRpcHolder, "/intman/*");

            // Run the server in its own thread
            try {
                server.start ();
            } catch (Exception e) {
                e.printStackTrace ();
            }
            try {
                server.join ();
            } catch (InterruptedException e) {
                e.printStackTrace ();
            }
        }
    }

    public static Thread getServerThread () {
        return new Thread (new ServerRunner ());
    }

    /** Sending messages to the map application */
    private static IMapActions MAP_PUBLISHER;

    public static IMapActions getMapPublisher () {
        if (MAP_PUBLISHER == null) {
            try {
                MAP_PUBLISHER = (IMapActions) XmlRpcProxy.createProxy (new URL ("http://localhost:"
                        + CLIENT_PORT + "/intman"), "", new Class[] { IMapActions.class }, false);
            } catch (MalformedURLException e) {
                Log.getLog ().warn ("Couldn't connect to server", e);
            }
        }
        return MAP_PUBLISHER;
    } 

    /** Sending messages to Skene */
    private static IFMLSpeech SPEECH_PUBLISHER;

    public static IFMLSpeech getSpeechPublisher () {
        if (SPEECH_PUBLISHER == null) {
            try {
                SPEECH_PUBLISHER = (IFMLSpeech) XmlRpcProxy.createProxy (new URL (
                        "http://localhost:" + CLIENT_PORT + "/intman"), "",
                        new Class[] { IFMLSpeech.class }, false);
            } catch (MalformedURLException e) {
                Log.getLog ().warn ("Couldn't connect to server", e);
            }
        }
        return SPEECH_PUBLISHER;
    }

    /** Sending messages to main Emote thing (start, stop, etc) */
    private static IEmoteActions EMOTE_PUBLISHER;

    public static IEmoteActions getEmotePublisher () {
        if (EMOTE_PUBLISHER == null) {
            try {
                EMOTE_PUBLISHER = (IEmoteActions) XmlRpcProxy.createProxy (new URL (
                        "http://localhost:" + CLIENT_PORT + "/intman"), "",
                        new Class[] { IEmoteActions.class }, false);
            } catch (MalformedURLException e) {
                Log.getLog ().warn ("Couldn't connect to server", e);
            }
        }
        return EMOTE_PUBLISHER;
    }

    /** Sending messages to Enercities game */
    private static IEnercitiesTaskActions ENERCITIES_PUBLISHER;

    public static IEnercitiesTaskActions getEnercitiesPublisher () {
        if (ENERCITIES_PUBLISHER == null) {
            try {
                ENERCITIES_PUBLISHER = (IEnercitiesTaskActions) XmlRpcProxy.createProxy (new URL (
                        "http://localhost:" + CLIENT_PORT + "/intman"), "",
                        new Class[] { IEnercitiesTaskActions.class }, false);
            } catch (MalformedURLException e) {
                Log.getLog ().warn ("Couldn't connect to server", e);
            }
        }
        return ENERCITIES_PUBLISHER;
    }

}
