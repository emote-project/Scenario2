<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c"%>
<%@ page session="false"%>
<html>
<head>
<link rel="stylesheet"
	href='<c:url value="/resources/jqueryui/css/ui-lightness/jquery-ui-1.10.3.custom.css" /> '
	type="text/css" />
<script type="text/javascript"
	src='<c:url value="/resources/jquery-1.9.1.js" /> '></script>
<script type="text/javascript"
	src='<c:url value="/resources/jquery.blockUI.js" /> '></script>
<script type="text/javascript"
	src='<c:url value="/resources/json.min.js" /> '></script>
<script type="text/javascript"
	src='<c:url value="/resources/speakClient.js" />'></script>
<script type="text/javascript"
	src='<c:url value="/resources/jqueryui/js/jquery-ui-1.10.3.custom.js" /> '></script>


<title>Admin Emote Maps</title>

<script type="text/javascript">
	

	function createScenario() {
		//var id = obj.id;
		$.get("createScenario", function(data) {
			console.info("Scenario created");
		});

	}
	
	function createSampleScenario() {
		//var id = obj.id;
		$.get("createSampleScenario", function(data) {
			console.info("Scenario created");
		});

	}
	function createObjectPlacementScenario() {
		//var id = obj.id;
		$.get("createObjectPlacementScenario", function(data) {
			console.info("Scenario created");
		});

	}
	
	
	
	function setAllValuesInAllSteps() {
		//var id = obj.id;
		$.get("setAllValuesInAllSteps", function(data) {
			console.info("setAllValuesInAllSteps");
		});

	}
	
	function createQuestionnaires() {
		//var id = obj.id;
		$.get("createQuestionnaires", function(data) {
			console.info("Questionnaires created");
		});

	}
	
	function startTCP() {
		var learnerName = $("#learnerName").val();
		var middleName = $("#middleName").val();
		var lastName = $("#lastName").val();
		//var sexobj = $("#sex");
		//var sex = $("#sex").val();
		var sex = $( "#sex option:selected" ).text();
		
		var birth = $("#birth").val();
		var learnerId = parseInt( $("#learnerNumber").val());
		
		if($("#learnerNumber").val()!="")
			{
		
		
		var learnerDetails = {
				"firstName" : learnerName,
				"middleName" : middleName,
				"lastName" : lastName,
				"mapApplicationId" : learnerId,
				"sex" : sex,
				"birth" : birth
		};
		$.postJSON("setLearner", learnerDetails, function(response) {			
			console.info("setLearner result:"+response);
		});
		
		$.get("startTCP", function(data) {
			console.info("startTCP");
		});
			}
		else
			{
			alert("Make sure values are not null")
			}
	
	}
	
	function getTaskLog() {
		var learnerNum = parseInt( $("#learnerNumber").val());
		
		$.get("getTaskLog",{ learnerId: learnerNum}, function(data) {
			console.info(data);
		});

	}
	
	function stopTCP() {
		var learnerName = $("#learnerName").val();
		var learnerId = parseInt( $("#learnerNumber").val());
		var learnerDetails = {
				"learnerName" : learnerName,
				"learnerId" : learnerId};
		$.postJSON("setLearner", learnerDetails, function(response) {			
			console.info("setLearner result:"+response);
		});
		
		
		var learnerNum = parseInt( $("#learnerNumber").val());
		$.get("stopTCP",{ learnerId: learnerNum}, function(data) {
			console.info("stopTCP");
		});

	}
	
	function closeTCP() {
		$.get("closeTCP", function(data) {
			console.info("closeTCP");
		});

	}
	
	function testLoadBehavior() {
		//var id = obj.id;
		$.get("testLoadBehavior", function(data) {
			console.info("testLoadBehavior");
		});

	}
	
	function runAllBehavior() {
		//var id = obj.id;
		$.get("runAllBehavior", function(data) {
			console.info("runAllBehavior");
		});

	}
	
 	function playVideo() {
        var myVideo = document.getElementsByTagName('video')[0];
        myVideo.play();
  }
	
 	function changeSourcePlayVideo() {
        var myVideo = document.getElementsByTagName('video')[0];
        myVideo.setAttribute("src", ['<c:url value="/resources/animations/olmwelcome.mp4"/>']); 
        myVideo.load();
        myVideo.play();
        
  }
	
 	function naoLisbonIP()
	{
 		
		 $.get("http://localhost/Nao/do_query.php?init=1&ip=172.20.41.35", function(data) {console.info("ip set to 169.254.199.42");				
		}); 
		 
		 $.postJSON("setNaoIp", '172.20.41.35', function(response) {			
				console.info("ip set to 172.20.41.35 on java");
			});
		 
	
	}
 	
 	function naoTextIP()
	{
 		
 		var textIP = $("#naoIP").val();
		 $.get("http://localhost/Nao/do_query.php?init=1&ip="+textIP, function(data) {console.info("ip set to 169.254.199.42");				
		}); 
		 
		 $.postJSON("setNaoIp", textIP, function(response) {			
				console.info("ip set to "+textIP);
			});
		 
	
	}
 	
 	
 	
 	function naoWebIP()
	{
		 $.get("http://localhost/Nao/do_query.php?init=1&ip=169.254.199.42", function(data) {console.info("ip set to 169.254.199.42");				
		}); 
		 
		 $.postJSON("setNaoIp", '169.254.199.42', function(response) {			
				console.info("ip set to 169.254.199.42 on java");
			});
		 
	
	}
 	
 	function naoWebIPLocal()
	{
	
 		$.get("http://localhost/Nao/do_query.php?init=1&ip=127.0.0.1", function(data) {console.info("ip set to local");				
		});
 		
 		 $.postJSON("setNaoIp", '127.0.0.1', function(response) {			
				console.info("ip set to 127.0.0.1 on java");
			});
 		
	}
	
	function naoWebInit()
	{
		$.get("http://localhost/Nao/do_query.php?init=2", function(data) {				
			console.info("init");});
	}
	
	function naoRunAndTalk()
	{
	 	$.get("http://localhost/Nao/do_query.php?call=1&priority=1&behaviour=olmwelcome&speech=hello&timeout=10", function(data) {	console.info("naoRunAndTalk");			
		}); 
		
	}
 	
	function motorOn() {
		//var id = obj.id;
		$.get("motorOn", function(data) {
			console.info("motorOn");
		});

	}
	
	function motorOff() {
		//var id = obj.id;
		$.get("motorOff", function(data) {
			console.info("motorOff");
		});

	}
	
	function getGameState() {
		//var id = obj.id;
		$.get("getGameState", function(gameState) {
			$("#condition").html("condition:" + gameState.condition);
			$("#talkViaJS").html("talkViaJS:" + gameState.talkViaJS);
			$("#runBehaviours").html("runBehaviours:" + gameState.runBehaviours);
			$("#robotExplanation").html("robotExplanation:" + gameState.robotExplanation);
			$("#robotOLM").html("robotOLM:" + gameState.robotOLM);
			$("#naovirtual").html("naovirtual:" + gameState.naovirtual);
			$("#naoweb").html("naoweb:" + gameState.naoweb);
			$("#naojava").html("naojava:" + gameState.naojava);
			$("#onScreenExplanation").html("onScreenExplanation:" + gameState.onScreenExplanation);
			$("#onScreenOLM").html("onScreenOLM:" + gameState.onScreenOLM);
			$("#javaTalk").html("javaTalk:" + gameState.javaTalk);
			$("#noRobot").html("noRobot:" + gameState.noRobot);
			$("#learnerIdText").html("learnerIdText:" + gameState.learnerId);
			$("#learnerNameText").html("learnerNameText:" + gameState.learnerName);

			
		});

	}
	
	
	
	function runIdleLoop() {
		//var id = obj.id;
		$.get("runIdleLoop", function(data) {
			console.info("runIdleLoop");
		});
	}
	
	function stopIdleLoop() {
		//var id = obj.id;
		$.get("stopIdleLoop", function(data) {
			console.info("stopIdleLoop");
		});
	}
	
	
	
	function testQueue() {
		//var id = obj.id;
		$.get("motorOn", function(data) {
		});

		var behaviors = new Array();
		var speech = new Array();
		behaviors[0] = 'smallStart';
		behaviors[1] = 'mediumStart';
		behaviors[2] = 'largeStart';
		speech[0] = 'hello 1';
		speech[1] = 'hello 2';
		speech[2] = 'hello 3';
		
		
		var talkAndBehaviorQueue = $({});

		//TODO have a method that gets the length of behavior in seconds and the the text lenght and takes the largest for wait.
		//talkTimeWait =  textToSay.length * 100;
		var talkTimeWait = 5000;
		talkAndBehaviorQueue.queue("anim",function(){ runBehaviorAndTalk(behaviors[0],speech[0],talkTimeWait);});
		talkAndBehaviorQueue.delay(0+talkTimeWait,"anim");
		
		talkTimeWait =  5000;
		talkAndBehaviorQueue.queue("anim",function(){ runBehaviorAndTalk(behaviors[1],speech[1],talkTimeWait);});
		talkAndBehaviorQueue.delay(0+talkTimeWait,"anim");
	 
		talkTimeWait =  5000;
		talkAndBehaviorQueue.queue("anim",function(){ runBehaviorAndTalk(behaviors[2],speech[2],talkTimeWait);});
		talkAndBehaviorQueue.delay(0+talkTimeWait,"anim");
		
		
		
		//Start
		talkAndBehaviorQueue.dequeue("anim");
	
	}
	
	function testWait() {
		
		var textToSay = 'hello 1';
		var talkTimeWait = 2000;
		var behaviors = new Array();
		var speech = new Array();
		behaviors[0] = 'smallStart';
		behaviors[1] = 'mediumStart';
		behaviors[2] = 'largeStart';
		speech[0] = 'hello 1';
		speech[1] = 'hello 2';
		speech[2] = 'hello 3';
		
		for (x in behaviors)
		{
			console.info("X:"+x);

			setTimeout( runBehaviorAndTalk(behaviors[x],speech[x],talkTimeWait), talkTimeWait);
			talkTimeWait = talkTimeWait + 5000;
		}
		
		
		/* textToSay = 'hello 2';
		talkTimeWait = talkTimeWait +  5000;
		setTimeout(function(){ runBehaviorAndTalk(behaviors[1],speech[1],talkTimeWait);},0+ talkTimeWait);
		
		textToSay = 'hello 2';
		talkTimeWait = talkTimeWait +  5000;
		setTimeout(function(){ runBehaviorAndTalk(behaviors[2],speech[2],talkTimeWait);}, 0+talkTimeWait);
		 */
		
	}
	
	
	function runBehaviorAndTalk(behavior,textToSay,delay)
	{
		   return function() {
				//$.postJSON("runBehavior", behavior);
				console.info("Starting runBehavior behavior:"+behavior+" text:"+textToSay+" delay:"+delay);
				speak(textToSay);
				console.info("Speaking finished:"+textToSay);
				$.postJSON("runBehavior", behavior, function(response) {			
					console.info("runBehavior result:"+response);
				});
		   }
	}
	
	function setNoRobot()
	{
		$.postJSON("setNoRobot", 'true', function(response) {			
			console.info("setNoRobot result:"+response);
		});
	}
	function setRobot()
	{
		$.postJSON("setNoRobot", 'false', function(response) {			
			console.info("setRobot result:"+response);
		});
	}
	
	function setCondition(button)
	{
		var condition = ""+button.value;
		$.postJSON("setCondition", condition, function(response) {			
			console.info("setCondition result:"+response);
		});
	}
	


	
	
	function setConditionDemo()
	{
		$.postJSON("setCondition", 'demo', function(response) {			
			console.info("setCondition result:"+response);
		});
	}
	
	function setScenario(button)
	{
		console.info("Scenario Setting to:"+button.value);
		$.get("setScenario",{ scenarioName: button.value}, function(data) {
			console.info("Scenario set");
		});
	}
	
	function init() {
		getAllScenarios();
	
		$("#birth").datepicker();
		/* $("#sex").selectable(); */
	}
	
	function javaTalk() {
		//$.postJSON("runBehavior", behavior);
	var textToSay = $("#textToSay").val();
			console.info("Starting java talk:" + textToSay);
			$.postJSON("say", textToSay, function(response) {
				console.info("java talk:" + response);
			});
		
	}
	
	function getAllScenarios()
	{
		$.get("getAllScenarios", function(data) {
			document.getElementById('scenarios').style.visibility = 'visible';
			$("#scenarios").empty();
			$("#scenarios").append('<h1>Scenarios</h1>');
			$("#scenarios").append('Please hit buttons below to set the scenario.');
			for (x in data) 
			{
				$("#scenarios").append('<button onClick="setScenario(this)" value="'+data[x]+'">'+data[x]+'</button> ');			
			}

		});
	}
	
	function parseLogFiles()
	{
		$.get("parseLogFiles", function(data) {
			console.info("parseLogFiles:" + data);

		});
	}
	function parseLogFilesWoz()
	{
		$.get("parseLogFilesWoz", function(data) {
			console.info("parseLogFilesWoz:" + data);

		});
	}
	
	
</script>

</head>

<body onload="init()">
	<h2> Activity control </h2>
	<li>
		<button onClick="createScenario()" name="Create Scenario">Create
			Scenario</button>
	</li>
		<li>
		<button onClick="createSampleScenario()" name="Create Sample Scenario">Create
			Sample Scenario</button>
	</li>
	<li>
		<button onClick="createObjectPlacementScenario()" name="Create Object Placement Scenario">Create Object Placement Scenario</button>
	</li>
	<li>
		<button onClick="setAllValuesInAllSteps()" name="setAllValuesInAllSteps">setAllValuesInAllSteps</button>
	</li>
	<li>
		<button onClick="createQuestionnaires()" name="Create Questionnaires">Create
			Questionnaires</button>
	</li>
		<li>
		<button onClick="getAllScenarios()" name="Get all scenarios">Get all scenarios</button>
	</li>
	<li>
		<button onClick="parseLogFiles()" name="parse Log Files">parse Log Files</button>
	</li>
	<li>
		<button onClick="parseLogFilesWoz()" name="parse Log Files Woz">parse Log Files Woz</button>
	</li>
	
	<div id="scenarios"></div>
	
	
	<h2> Nao Java </h2>
	
	<li>
		<button onClick="testLoadBehavior()" name="testLoadBehavior">testLoadBehavior</button>
	</li>
	<li>
		<button onClick="motorOn()" name="motorOn">motorOn</button>
	</li>
	<li>
		<button onClick="motorOff()" name="motorOff">motorOff</button>
	</li>
	<li>
		<button onClick="runAllBehavior()" name="runAllBehavior">runAllBehaviors</button>
	</li>
		<h2> Virtual Test </h2>
	
	<li>
		<button onClick="playVideo()" name="playVideo">playVideo</button>
	</li>
	<li>
		<button onClick="changeSourcePlayVideo()" name="changeSourcePlayVideo">changeSourcePlayVideo</button>
	</li>
		<li>
		<button onClick="testQueue()" name="testQueue">Test The Queue</button>
	</li>
	<li>
		<button onClick="testWait()" name="testWait">Test The Wait</button>
	</li>
	
	<h2> Nao Control </h2>
	
	
	<input type="text" name="textToSay" id="textToSay">
	<li>
		<button onClick="javaTalk()" name="javaTalk">Say text</button>
	</li>
	<li>
	    172.20.41.35
		<input type="text" name="naoIP" id="naoIP">
	</li>
	<li>
		<button onClick="naoTextIP()" name="naoTextIP">Nao text IP</button>
	</li>
	<li>
		<button onClick="naoLisbonIP()" name="naoLisbonIP">Nao Lisbon IP</button>
	</li>
	<li>
		<button onClick="naoWebIPLocal()" name="naoWebIPLocal">Nao IP Local</button>
	</li>
		<li>
		<button onClick="naoWebIP()" name="naoWebIP">Nao IP On Network</button>
	</li>
	<li>
		<button onClick="naoWebInit()" name="naoWebInit">Nao Web Init</button>
	</li>
	<li>
		<button onClick="naoRunAndTalk()" name="naoRunAndTalk">Nao Run and Talk</button>
	</li>
	
	<h2> Monitoring </h2>
	Id
	<li>
		<input type="text" name="learnerNumber" id="learnerNumber">
	</li>
	First Name
	<li>
		<input type="text" name="learnerName" id="learnerName">
	</li>
	Middle Name
	<li>
		<input type="text" name="middleName" id="middleName">
	</li>
	Last Name
	<li>
		<input type="text" name="lastName" id="lastName">
	</li>
	Sex
	<!-- <ul name="sex" id="sex">
  <li>M</li>
  <li>F</li>
</ul> -->
	
	<form name="sex" id="sex">
   <select>
    <option name="M" id="M">M</option>
    <option name="F" id="F">F</option>
   </select> 
  </form> 
	Birth
	<li>
		<input type="text" name="birth" id="birth">
		
	</li>
	

	
	<li>
		<button onClick="startTCP()" name="startTCP">startTCP and set learner name and id in system</button>
	</li>
	<li>
		<button onClick="stopTCP()" name="stopTCP">stopTCP and set learner id back to default</button>	 
	</li>
	<li>
		<button onClick="closeTCP()" name="closeTCP">closeTCP</button>
	</li>
	
	<li>
		<button onClick="getTaskLog()" name="getTaskLog">Get task log</button>
	</li>
	
	
	<h2> Is there a robot? </h2>
	<li>
		<button onClick="setNoRobot()" name="setNoRobot">set No Robot</button>
	</li>
	<li>
		<button onClick="setRobot()" name="setRobot">set Robot</button>
	</li>
		<li>
		<button onClick="runIdleLoop()" name="runIdleLoop">Run Idle Loop</button>
	</li>
	<li>
		<button onClick="stopIdleLoop()" name="stopIdleLoop">Stop Idle Loop</button>
	</li>
	
<h2> Conditions </h2>
	<li>
		<button onClick="setCondition(this)" value ="olmrobotexplanation">olm robot explanation</button>
	</li>
	
    <li>
		<button onClick="setCondition(this)" value="olmexplanation">olm text explanation</button>
	</li>
	 <li>
		<button onClick="setCondition(this)" value="robotolmandrobotexplanation">Robot olm and robot explanation</button>
	</li>
	<li>
		<button onClick="setCondition(this)" value="nobehaviours">no behaviours</button>
	</li>
	<li>
		<button onClick="setCondition(this)" value="scenariomaker">scenario maker</button>
	</li>
		<li>
		<button onClick="setCondition(this)" value="demo">Demo</button>
	</li>
	<li>
		<button onClick="setCondition(this)" value="demoWithIntro">Demo With Intro</button>
	</li>
	<li>
		<button onClick="setCondition(this)" value="scen1DemoNoRobot">Scen 1 Demo No Robot</button>
	</li>
	
	
<h2> Game State </h2>
<li>
		<button onClick="getGameState()" name="getGameState">get Game State</button>
	</li>
		<h3 id="condition"></h3>
	<h3 id="talkViaJS"></h3>
	<h3 id="runBehaviours"></h3>
	<h3 id="robotExplanation"></h3>
	<h3 id="robotOLM"></h3>
	<h3 id="naovirtual"></h3>
	<h3 id="naoweb"></h3>
	<h3 id="naojava"></h3>
	<h3 id="onScreenExplanation"></h3>
	<h3 id="onScreenOLM"></h3>
	<h3 id="javaTalk"></h3>
	<h3 id="noRobot"></h3>
	<h3 id="learnerIdText"></h3>
	<h3 id="learnerNameText"></h3>

	
	<video width="320" height="240">
 			<source src='<c:url value="/resources/animations/olmwelcome.mp4"/>' type="video/mp4">
			Your browser does not support the video tag.
	</video>	

	<div id="audio"></div>
	
	
	
	
	
</body>
</html>