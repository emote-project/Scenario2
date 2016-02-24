Running scenario 1 thalamus:
From my point of view you will just need to put the XML in the EmoteMaps\scenario folder. 
Then it will appear in Scenario section of the Menu. Choose the trail you want to from here. 
Then you will need to go to the Mode section of the Menu and choose "Thalamus Study".
Then refresh the page. 
Then you will need to choose the learner or create a new one. Please fill in all values. 
One thing is that you will need the learnermodeljava running, this needs to be set up in eclipse or spring tools, I can do this if you want. 
It is also probably a good idea to drop all of the database tables in the emote and learner model database. I can show you this also.


Scenario 1 Spring Java Set Up.
Install Java 32bit, x64 has an issue with the NAOQI....
http://www.oracle.com/technetwork/java/javase/downloads/jdk7-downloads-1880260.html
Choose the Windows x86 option. 
You can also install Java. x64. jdk1.7.0_25 but this may have issues. 


Install Tomcat. apache-tomcat-7.0.40-windows-x64
http://tomcat.apache.org/download-70.cgi
Core: zip (pgp, md5) > save the zip, extract it where you will remember, i.e. c:/dev....

Install Eclipse, just download and extract. 
https://www.eclipse.org/downloads/
Eclipse IDE for Java EE Developers, 254 MB
Windows 64 Bit

Install GitHub or source tree client.
Clone repo and download code from git hub using git hub client.
This will put some code on you computer somewhere, you have the option to set this or it will go to a default location.		
		
Import project to spring source client:		
Open Eclipse
Tick box for default workspace.
File > Import > General > Existing projects into workspace. 
Choose the folder where you have downloaded the code using git. The folder will be called EmoteMaps 


Project > turn off the build automatically.
Project > Properties > validators .. Enable project specific setting, disable all validators.. 

Project > Properties > builders, remove the builder with red cross.  (this step may not be required)
Project > properties > java build path > select the "source" tab 
Select and click remove on the items.
Add folders:
/src/main/java
/src/main/resources

In the libraries tab
"Add external Jars" Add the following jar to the buildpath.
EmoteMaps\src\main\webapp\WEB-INF\lib\jnaoqi-1.14.3.jar
EmoteMaps\src\main\webapp\WEB-INF\lib\xmlrpc-1.1.1.jar
EmoteMaps\src\main\webapp\WEB-INF\lib\xmlrpc-client-1.1.1.jar
EmoteMaps\src\main\webapp\WEB-INF\lib\ctat.jar
EmoteMaps\src\main\webapp\WEB-INF\lib\xuggle-xuggler-5.2.jar

Right click on project > Maven > Update project
Right click on project > Maven > Download sources


Install and run mySQL database server. I have MySQL Server 5.6.
http://dev.mysql.com/downloads/mysql/
For ease set it to run as a service and start on startup. 
If you do not have it running as a service you can stop and start it by running this command. C:\Program Files\MySQL\MySQL Server 5.6\bin>mysqld 
Or install it running as a service. 
It will ask you to set a root password, I have used root/admin, make a note of the password you choose. 


Create the databases and users, one for scenario 1 and one for the learner model module, use one of the following commands to open the sql interface.. : 
Most likely 
C:\Program Files\MySQL\MySQL Server 5.6\bin>mysql -u root -p
then out in the passowrd from when you installed mysql. 

CREATE DATABASE learnerModel;
USE learnerModel;
CREATE USER 'learnerModel'@'localhost' IDENTIFIED BY 'learnerModel';
GRANT ALL ON learnerModel.* TO 'learnerModel'@'localhost';

CREATE DATABASE emoteMap;
USE emoteMap;
CREATE USER 'emoteMap'@'localhost' IDENTIFIED BY 'emoteMap';
GRANT ALL ON emoteMap.* TO 'emoteMap'@'localhost';

DB Backup
C:\Program Files\MySQL\MySQL Server 5.6\bin>mysqldump -u root -p emotemap > C:\Emote\BurlishBackup\emotedbbackup20150119.sql

DB Clean:
USE learnerModel;
SET FOREIGN_KEY_CHECKS = 0;
SET GROUP_CONCAT_MAX_LEN=32768;
SET @tables = NULL;
SELECT GROUP_CONCAT('`', table_name, '`') INTO @tables
  FROM information_schema.tables
  WHERE table_schema = (SELECT DATABASE());
SELECT IFNULL(@tables,'dummy') INTO @tables;

SET @tables = CONCAT('DROP TABLE IF EXISTS ', @tables);
PREPARE stmt FROM @tables;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
SET FOREIGN_KEY_CHECKS = 1;

USE emoteMap;
SET FOREIGN_KEY_CHECKS = 0;
SET GROUP_CONCAT_MAX_LEN=32768;
SET @tables = NULL;
SELECT GROUP_CONCAT('`', table_name, '`') INTO @tables
  FROM information_schema.tables
  WHERE table_schema = (SELECT DATABASE());
SELECT IFNULL(@tables,'dummy') INTO @tables;

SET @tables = CONCAT('DROP TABLE IF EXISTS ', @tables);
PREPARE stmt FROM @tables;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
SET FOREIGN_KEY_CHECKS = 1;

Tomcat.
Back inside eclipse. 
File >new > other > server > apache > tomcat 7

In the tomcat launch conf add this -Xms128m -Xmx1024m -XX:PermSize=64m -XX:MaxPermSize=256m

Full list of entries:
-Dcatalina.base="C:\dev\project\.metadata\.plugins\org.eclipse.wst.server.core\tmp0" -Dcatalina.home="C:\dev\apache-tomcat-7.0.14" -Dwtp.deploy="C:\dev\project\.metadata\.plugins\org.eclipse.wst.server.core\tmp0\wtpwebapps" -Djava.endorsed.dirs="C:\dev\apache-tomcat-7.0.14\endorsed" -Xms128m -Xmx1024m -XX:PermSize=64m -XX:MaxPermSize=256m -Djava.library.path="C:\Users\axj100\Documents\spring-workspace\EmoteMaps\src\main\webapp\WEB-INF\lib"


Add the following to the list in catalina.properties... tomcat.util.scan.DefaultJarScanner.jarsToSkip=\
spring-beans-3.1.1.RELEASE.jar,xercesImpl-2.11.0.jar,cglib-2.2.jar,httpcore-4.3.2.jar,jnaoqi-1.14.3.jar,opencsv-2.3.jar,httpclient-4.3.5.jar,spring-orm-3.1.1.RELEASE.jar,jboss-transaction-api_1.2_spec-1.0.0.Final.jar,spring-expression-3.1.1.RELEASE.jar,jandex-1.1.0.Final.jar,javax.inject-1.jar,spring-asm-3.1.1.RELEASE.jar,javassist-3.18.1-GA.jar,castor-1.2.jar,spring-context-support-3.1.1.RELEASE.jar,spring-oxm-3.1.1.RELEASE.jar,slf4j-api-1.6.6.jar,spring-aop-3.1.1.RELEASE.jar,jboss-logging-3.1.3.GA.jar,slf4j-log4j12-1.6.6.jar,commons-lang-2.6.jar,validation-api-1.0.0.GA.jar,jackson-mapper-asl-1.6.4.jar,aopalliance-1.0.jar,spring-context-3.1.1.RELEASE.jar,jboss-logging-annotations-1.2.0.Beta1.jar,antlr-2.7.7.jar,xmlrpc-client-1.1.1.jar,jcl-over-slf4j-1.6.6.jar,jmf-2.1.1e.jar,xmlrpc-1.1.1.jar,xml-apis-1.4.01.jar,ctat.jar,joda-time-1.6.2.jar,spring-core-3.1.1.RELEASE.jar,jackson-core-asl-1.6.4.jar,spring-jdbc-3.1.1.RELEASE.jar,persistence-api-1.0.jar,aspectjrt-1.6.10.jar,commons-codec-1.6.jar,spring-web-3.1.1.RELEASE.jar,spring-tx-3.1.1.RELEASE.jar,mysql-connector-java-5.1.29.jar,dom4j-1.6.1.jar,asm-3.1.jar,sunmscapi.jar

Update the config file in the source code: 
Copy  src\main\resources\META-INF\spring\config.properties.template. Leave it in the same folder. new name should be  config.properties
Update the locations where it mentions EmoteMaps inside the file to point to the folder where files are saved (C:/EmoteJava/EmoteMaps/.....) 
Also create a log folder outside of the repostiry/project folder. 
C:\Emote\logs\OLMExp

Add the following to you path environmental variable (outside of spring) , update to be required....:
C:\.......\EmoteMaps\src\main\webapp\WEB-INF\lib
C:\EmoteJava\EmoteMaps\src\main\webapp\WEB-INF\lib

Clean the project (selct project on left bar, then select project in tool bar, then clean... )
Right click on project > Run As >  Run on server.

In firefox:
you can disable zoom by pinching:
https://support.mozilla.org/en-US/questions/952203
Go to: about:config
set browser.gesture.pinch.out and browser.gesture.pinch.in to blank string

Windows touch:
You need to disable right click by holding down on the touch screen.
The settings for this should be something like this: 
"Pen and touch screen"
"Touch"
Select "Press and hold - Right click" and click "Settings..."
Uncheck the box "Activate press and hold for right click"

Page will be visible at...
http://localhost:8080/namshub/	
Before this works you will need to set some variables and press some buttons on the admin page. 
http://localhost:8080/namshub/admin
Under the monitoring section put in a username and id, and press the button below. If this is not done the task will not work. 


Screen converter:
install JMF? http://www.oracle.com/technetwork/java/javase/download-142937.html
java -jar screen_converter.jar <screen_cap_file.cap>

java -jar C:\Users\axj100\Documents\spring-workspace\EmoteMaps\java-screen-recorder-0.8\screen_converter.jar xxx.cap
java -jar C:\Users\axj100\Documents\spring-workspace\EmoteMaps\java-screen-recorder-0.8\screen_player.jar

Thalamus:
For this to work with thalamus you need to get the EMOTE-Modules-WIP from https://gaips.inesc-id.pt:8443/rhodecode/
for me the clone URL is https://ajones@gaips.inesc-id.pt:8443/rhodecode/EMOTE-Modules-WIP
You also need the main thalamus repo
http://hg.code.sf.net/p/thalamus/code
You need to speak to Tiago about instructions for how to build all of this....

But you need to be running the EMOTE-Modules-WIP\MapInterface\MapInterface\bin\Release\MapInterface.exe


If you are using it the set up the NAO Web Request, this is for robot behaviour:
EmoteMaps\Nao Web Request\READ ME.txt

You can also run the monitoring:
C:\Users\axj100\Documents\spring-workspace\EmoteMaps\pipeServer\PipeServer\PipeServer\bin\Release
Start listening
Start server.
Then start the video capture:
C:\Users\axj100\Documents\spring-workspace\EmoteMaps\pipeServer\video1\WPF Webcam\bin\Release
or
C:\Users\axj100\Documents\spring-workspace\EmoteMaps\pipeServer\video2\WPF Webcam\bin\Release


How to use the wizard:
Get everything running
open the admin page:
http://localhost:8080/namshub/admin
Click the "Scenario maker wizard"
Go to main window to start using the wizard
http://localhost:8080/namshub/
Now you will see the scenario maker tools on the left. 

Scenario
Choose the scenario drop down and select an existing scenario by clicking the button. Make a new one by typing a new name and then press get or create scenario. 

Set location
Jump down to the set location tab.
Choose a location
Set the centre to current location.

Navigation and step text
Submit ScenarioStep = save the step after making any changes. 
Next and previous step = navigate, there will be a new blank step created after you save the last step. 
Instert Step  = insert step before the current position by pressing 
Remove Step = deletes the current step
set Step on screen text: to what you would like to appear in bottom left. 
set Step speech: to the code from the utterance library that you want the robot to say at this step. (Not implemented by Srini yet, we need to chase about this)



Step detail set up
This is where you add all of the objects to the map and set the start,end.
1st thing to do is add the symbols that you want for the activity.
TO do this move the green diamond where you want the symbol to go. 
Choose the symbol by pressing the button.
Press the Add symbol at marker for activity.
Repeat....

Then you want to start making the steps...
Press the add reference button. 
Drag the red star to the start of the step.
Submit ScenarioStep
Then press the add target button.
Drag the green star to the target of the step. 
Submit ScenarioStep
the go to Navigation and step text and ht next step. 

Step type
Always choose click for the moment, drag does not work very well on the touchtables so to avoid technical issues causing problems with the interaction just use click.

Step requirements
Tick the boxes for the required items and enter the details. (Symbol must match the name in the buttons in the step detail setup section)
Info on completeion will cause a popup box with that text in it.
Answer to progress check box means that the question must be answered correctly to move on, if this is not checked then the system will just give the next item even if they answered worng. 



Download local tiles:
If you want new maps. 
Go to 
EmoteMaps\jTileDownloader\jTileDownloader
copy an existing xml file, rename, and change lat/long or zoom level, and the paths for your machine.. 
Good place to find the lat long you want is http://www.latlong.net/
java -jar jTileDownloader-0-6-1.jar dl=york.xml
refresh file system in IDE, clean and republish web app. 

Info from here: 
http://wiki.openstreetmap.org/wiki/OpenLayers_Local_Tiles_Example	
Use this downloader:
http://wiki.openstreetmap.org/wiki/JTileDownloader#Download
This is sat in the jTileDownloader folder that you will have downloaded

Example xml used for haybridge is:
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE properties SYSTEM "http://java.sun.com/dtd/properties.dtd">
<properties>
<entry key="Type">UrlSquare</entry>
<entry key="OutputLocation">C:\Users\axj100\Documents\spring-workspace\EmoteMaps\src\main\webapp\resources\tiles</entry>
<entry key="PasteUrl">http://www.openstreetmap.org/?lat=52.423781&amp;lon=-2.147318&amp;zoom=16&amp;layers=B000TTF</entry>
<entry key="TileServer">http://a.tile.openstreetmap.org</entry>
<entry key="OutputZoomLevel">16</entry>
<entry key="Radius">2</entry>
</properties>	
Then just move the tiles in to the tiles folder. 		
		
		
		
		
Not needed for simple version:
		
PostGresQL
Version 9.2.4
User/Pass postgres/postgres
Port 5432
Locale: English, United Kingdom


Download PostGIS
This is an addon for postgres:
http://www.bostongis.com/PrinterFriendly.aspx?content_name=postgis_tut01
*Update the config so that the local access does not need a password. 
Once PostgreSQL is installed, launch Application Stack Builder, just search for it..
Then choose the postgres server
Then choose spatial extensions and the x64 version.
Then next and okay to all the options.
You don't need to do the rest as db will be created automatically by openlayers import. 

Install Osm2pgsql (OSM Data to PG..)
http://wiki.openstreetmap.org/wiki/Osm2pgsql#Binary

Download a partial cut of the OSM Data:
http://downloads.cloudmade.com/europe/northern_europe/united_kingdom/england#downloads_breadcrumbs
http://osmdata.thinkgeo.com/openstreetmap-data/europe/


Load data from file into postgis:
http://www.bostongis.com/PrinterFriendly.aspx?content_name=loading_osm_postgis

C:\Emote\Utils\OSM Data>osm2pgsql -c -s -S C:\Users\axj100\Documents\GitHub\osm2pgsql\default.style -d osm -U postgres -H localhost -P 5432 england.osm.bz2

http://blog.geoserver.org/2009/01/30/geoserver-and-openstreetmap/


http://geoserver.org/display/GEOS/Stable
GeoServer 2.3.3
user/pass admin/geoserver
Port: 8090
Run Manually
http://localhost:8090/geoserver/web/
Create a new Workspace - local. 
Create a new store, this should link to the postgres db. 
 


Openlayers
http://openlayers.org/
This should already be in the code so no need to repeat. 
2.13.1 (Stable)

Add the code in to SpringSource.
Configure the tomcat server within spring. 

Build and run. 

Get SVN Client
 http://tortoisesvn.net/downloads.html


Spring examples:
http://blog.springsource.org/2012/05/10/spring-mvc-3-2-preview-making-a-controller-method-asynchronous/


geosdiera
http://code.google.com/p/geosdiera/wiki/BuildInstructions
Dojo Openlayers:
https://dojotoolkit.org/reference-guide/1.9/dojox/geo/openlayers.html
JQuery Openlayers?

OSM Map Key:
http://wiki.openstreetmap.org/wiki/Map_Features

OpenLayers setting zoom levels:
http://trac.osgeo.org/openlayers/wiki/SettingZoomLevels



Dojo Openlayers (could be good for polling/2way coms):
https://dojotoolkit.org/reference-guide/1.9/dojox/geo/openlayers.html

Geoserver home page:
http://localhost:8090/geoserver/web/

How to configure geoserver and OSM
http://blog.geoserver.org/2009/01/30/geoserver-and-openstreetmap/

App.
http://localhost:8080/namshub/

Spring Ajax Js
http://chimpler.wordpress.com/2013/02/11/pushing-real-time-data-to-the-browser-using-cometd-and-spring/
http://crunchify.com/how-to-use-ajax-jquery-in-spring-web-mvc-jsp-example/
http://www.raistudies.com/spring/spring-mvc/ajax-spring-mvc-3-annonations-jquery/



Overview.
The user interface is via a web page. The web pages are served by java server running spring framework.
Spring allows us to do user tracking, logging, etc...
Spring tutorial:
http://www.tutorialspoint.com/spring/index.htm

The user can navigate around web based client and we will use Spring controllers etc to do this. 

When you start an exercise we will open up a web page with a map. 
In spring we create a MAP.jsp, this contains java script that contains the OpenLayerToolkit Framework which is a javascript library for manipulating maps.
When the user connects to the webpage, the MAP.jsp is loaded and the webpage dowloaded. 

This page uses long polling? to connect to the server and get updates. 
User does something on the map we will send a request to the server. 
By AJAX (http://blog.springsource.org/2010/01/25/ajax-simplifications-in-spring-3-0/)


The response should be able to do a number of things in javascript in the following cases. 
Correct: Next objective
Incorrect: Do nothing, Update hint (recentre map/update hint text)

Definition of longpoling, 
The long-polling Comet technique is a technique that optimizes traditional polling to reduce latency.
Traditional polling sends an XMLHttpRequest to the server in fixed intervals. For example, open a new XMLHttpRequest every 15 seconds, receive an immediate response, and close the connection.
Long-polling sends a request to the server, but a response is not returned to the client until one is available. As soon as the connection is closed, either due to a response being received by the client or if a request times out, a new connection is initiated. The result is a significant reduction in latency because the server usually has a connection established when it is ready to return information to return to the client.


Primer on GIS:
http://www.ibm.com/developerworks/library/os-geoserver/



For reverse geocoding need to use wfs service of geoserver, this will return xml/json etc. Need to parse.
This is how to decode the json... 
http://blog.jdriven.com/2012/11/consume-rest-json-webservices-easily-using-spring-web/

This is an example wfs query that returns JSON...
http://localhost:8090/geoserver/local/ows?service=WFS&version=1.0.0&request=GetFeature&typeName=local:planet_osm_roads&bbox=-305896.89,7095551.46,-205896.89,8095551.46&maxFeatures=50&outputFormat=application/json


JSON parsing...
http://wiki.fasterxml.com/JacksonInFiveMinutes




Dojo integration... Not used at the moment. 
Might want to use Dijit to make interface better
Recommended dojo tutorial.
http://dojotoolkit.org/documentation/tutorials/1.9/hello_dojo/

Map demo http://demos.dojotoolkit.org/demos/mapTileProviders/demo.html
Usefull webpages:
http://www.sitepen.com/blog/2011/08/11/how-do-you-use-the-dojo-store-jsonrest-api-with-spring/

Put dojo file in webapp resources:
Download from http://dojotoolkit.org/download/
C:\Users\axj100\Documents\spring-workspace\EmoteMaps\src\main\webapp\resources

This is needed if we want google maps. 
<script
       src='http://maps.google.com/maps?file=api&amp;v=2&amp;key=ABQIAAAAjpkAC9ePGem0lIq5XcMiuhR_wWLPFku8Ix9i2SXYRVK3e45q1BQUd_beF8dtzKET_EteAjPdGDwqpQ'
       type="text/javascript" ></script>



Good website for lat long calc:
http://www.movable-type.co.uk/scripts/latlong.html


Sometimes might need to add this to git config
[http]
	sslVerify = false
	
Or run this in console:
git config --global http.sslVerify false




JS
http://chamaps.com/watervliet/
http://chamaps.com/watervliet/js/map_js.php



JMF for reading and streaming video...
http://bcook.cs.georgiasouthern.edu/cs523/2vandana/index.html#Transmitting
http://www.oracle.com/technetwork/java/javase/samplecode-138571.html#
http://blog.boehme.me/2010/09/jmf-video-chat.html
https://java.sys-con.com/node/45832
http://www.oracle.com/technetwork/java/javase/tech/index-jsp-140239.html
http://stackoverflow.com/questions/17585917/capture-live-video-from-webcam-using-java-an-jmf 