<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c"%>
<%@ page session="false"%>
<html>
<head>
<link rel="stylesheet"
	href='<c:url value="/resources/jqueryui/css/ui-lightness/jquery-ui-1.10.3.custom.css" /> '
	type="text/css" />
<script type="text/javascript"
	src='<c:url value="/resources/OpenLayers/OpenLayers.js" />'>
	
</script>
<%-- <script type="text/javascript"
		src='<c:url value="/resources/OSM/OpenStreetMap.js" />'>
</script>	 --%>
<script type="text/javascript"
	src='<c:url value="/resources/jquery-1.9.1.js" /> '></script>
<script type="text/javascript"
	src='<c:url value="/resources/jqueryui/js/jquery-ui-1.10.3.custom.js" /> '></script>
<script type="text/javascript"
	src='<c:url value="/resources/jquery.blockUI.js" /> '></script>

<script type="text/javascript"
	src='<c:url value="/resources/json.min.js" /> '></script>

<script type="text/javascript"
	src='<c:url value="/resources/speakClient.js" />'></script>

<script type="text/javascript"
	src='<c:url value="/resources/OpenLayers/DynamicMeasure.js" />'>	
</script>


<%-- <link rel="stylesheet"
	href='<c:url value="/resources/css/style.css" /> ' type="text/css" />
 --%>

<title>Scenario 1 Open Layers</title>

<style type="text/css">
html,body,#map {
	margin: 0;
	width: 100%;
	height: 96%;
	overflow-y: hidden;
	overflow-x: hidden;
}

.question-span {
	width: 1000px;
}

#olm {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1px;
	left: 0;
	right: 0;
	margin-left: auto;
	margin-right: auto;
	width: 96%;
	height: 96%;
	z-index: 25000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
	font-size:xx-large;
}

#scrapBookDiv {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1px;
	left: 0;
	right: 0;
	margin-left: auto;
	margin-right: auto;
	width: 50%;
	height: 50%;
	z-index: 25000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
	font-size:xx-large;
}

#infoDiv {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1px;
	left: 0;
	right: 0;
	margin-left: auto;
	margin-right: auto;
	width: 50%;
	height: 50%;
	z-index: 25000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
	font-size:xx-large;
}

#questionnaire {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1px;
	left: 0;
	right: 0;
	margin-left: auto;
	margin-right: auto;
	width: 96%;
	height: 96%;
	z-index: 30000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
	font-size:xx-large;
}

#map_key {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1em;
	right: 1em;
	z-index: 20000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
}

#layerDiv {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1em;
	right: 1em;
	z-index: 20000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
}

#text {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	bottom: 0;
	margin: 0;
	width: 100%;
	height: 10%;
	padding: 5px 0 0 0;
	border-top: 5px solid White;
	text-align: left;
	z-index: 20000;
	background-color: Black;
	color: White;
	font-size:large;
}

#videoDiv {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1em;
	left: 1em;
	z-index: 20000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
}

#scenarioMakerDiv {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1em;
	left: 1em;
	z-index: 20000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
}

#activityMenuDiv {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1em;
	left: 1em;
	z-index: 20000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
}

#newOLM {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1em;
	left: 1em;
	width: 250px;
	z-index: 20000;
	background-color: transparent;
	color: White;
}

#scenarioMakerDivControl {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	top: 1em;
	right: 1em;
	z-index: 20000;
	background-color: Black;
	color: White;
	text-align: center;
	border-style: solid;
	border-width: 5px;
	border-color: White;
}

#controls {
	overflow-y: hidden;
	overflow-x: hidden;
	position: absolute;
	bottom: 2%;
	right: 1em;
	z-index: 20000;
	background-color: transparent;
	color: White;
	text-align: right
}

#meter {
	position: absolute;
	height: 40px;
	border: 1px solid white;
	width: 300px;
	background: black;
}

#compassmeter {
	position: absolute;
	height: 40px;
	background: green;
}

#distancemeter {
	position: absolute;
	height: 40px;
	background: green;
}

#symbolmeter {
	position: absolute;
	height: 40px;
	background: green;
}

#meterText {
    display:inline-block;
    width: 90px;
}
#newOLMMeter {
    display:inline-block;
    position: absolute;
    height: 50%;
    width: 100px;
    text-align:center;
    border-left: Black 2px Solid;
    border-right: Black 2px Solid;
}
#newOLMMeterLine {
    position: absolute;
    top: 45%;
    height: 2px;
    width: 100%;
    background: Black;
}
#newOLMCompassMeter {
    position: absolute;
    height: 90%;
    width: 50%;
    background: green;
    border-width: 1px;
    border-color: Black;
    border-style: solid;
}
#newOLMSymbolMeter {
    position: absolute;
    height: 90%;
    width: 50%;
    background: green;
    border-width: 1px;
    border-color: Black;
    border-style: solid;
}
#newOLMDistanceMeter {
    position: absolute;
    height: 90%;
    width: 50%;
    background: green;
    border-width: 1px;
    border-color: Black;
    border-style: solid;
}

#newOLMCompassOldValue {
    position: absolute;
    left: 50%;
    height: 100%;
    width: 3px;
    background: white;
    border-width: 1px;
    border-color: Black;
    border-style: solid;
}
#newOLMDistanceOldValue {
    position: absolute;
    left: 50%;
    height: 100%;
    width: 3px;
    background: white;
    border-width: 1px;
    border-color: Black;
    border-style: solid;
}
#newOLMSymbolOldValue {
    position: absolute;
    left: 50%;
    height: 100%;
    width: 3px;
    background: white;
    border-width: 1px;
    border-color: Black;
    border-style: solid;
}
</style>

<script type="text/javascript">
	$.fn.serializeObject = function() {
		var o = {};
		var a = this.serializeArray();
		$.each(a, function() {
			if (o[this.name]) {
				if (!o[this.name].push) {
					o[this.name] = [ o[this.name] ];
				}
				o[this.name].push(this.value || '');
			} else {
				o[this.name] = this.value || '';
			}
		});
		return o;
	};

	//Misc click handlers
	 OpenLayers.Control.Click = OpenLayers.Class(OpenLayers.Control, {
       defaultHandlerOptions : {
           'single' : true,
           'double' : true,
           'pixelTolerance' : 2,
           'stopSingle' : true,
           'stopDouble' : true
       },

       initialize : function(options) {
           this.handlerOptions = OpenLayers.Util.extend({},
                   this.defaultHandlerOptions);
           OpenLayers.Control.prototype.initialize.apply(this, arguments);
           this.handler = new OpenLayers.Handler.Click(this, {
               'click' : this.trigger,
               'dblclick' : this.dbltrigger
           }, this.handlerOptions);
       },

       trigger : function(e) {
           console.info("Click");
           var toProjection = new OpenLayers.Projection("EPSG:4326");
           var lonlat = map.getLonLatFromPixel(e.xy).transform(
                   map.getProjectionObject(), toProjection);
           
           var screenX = e.screenX;
           var screenY = screen.height - e.screenY;
           checkLonLat(lonlat, '', true, true,
       			false, 0, screenX, screenY);
           logOnServer("Click");
           /*    var lonlat = map.getLonLatFromPixel(e.xy).transform(
                      new OpenLayers.Projection("EPSG:4326"),
                      map.getProjectionObject()
                  ); */

       },
       dbltrigger : function(e) {
           console.info(this);
           console.info("Double Click");
           logOnServer("Double Click");
           // 
           var feat = vectors.getFeatureFromEvent(e);

           // Select a feature

           if (feat) {
               var toProjection = new OpenLayers.Projection("EPSG:4326");
               var lonlat = map.getLonLatFromPixel(e.xy).transform(
                       map.getProjectionObject(), toProjection);

               /*  var lonlat = map.getLonLatFromPixel(e.xy).transform(
                        new OpenLayers.Projection("EPSG:4326"),
                        map.getProjectionObject()
                    ); */
               checkLonLat(lonlat);

           }

       }

   });
	
	
	
	var boxes;
	var map;
	var vectors;
	var symbols;
	var compassLayer;
	var measureControls;
	var navigationControl;
	var mousePositionControl;
	var dragFeature;
	var latRange = .0005;
	var longRange = .001;
	var compassfeature;
	var talkTimeWait;
	var symbolSayNow;
	var symbolBehavior;
	var distanceSayNow;
	var distanceBehavior;
	var compassSayNow;
	var compassBehavior;
	var learnerName;
	var learnerId;
	var questionnaire;
	var multipleChoiceQuestionsToFind;
	var distanceTool;
	var scalebar;
	var timeQuestionStarted
	var zoomLevel;
	var defaultLon;
	var defaultLat;
	var currentLon;
	var currentLat;
	var olmOpened;
	var olmClosed;
	var compassOpened;
	var compassClosed;
	var distanceOpened;
	var distanceClosed;
	var keyOpened;
	var keyClosed;
	var naoWebIp;
	var scenario;
	var scenarioMaker;
	var stepId;
	var activity;
	var activityId;
	var scenarioMakerStep;
	var currentStep;
	//var renderer;
	var scenarioMakerMarker;
	var symbolName;
	var scenarioMakerDiv;
	var length;
	var deleteMode;
	var symbolSize;
	var continueToPoll;
	var layers;
	var mapLayers;
	var mapLayerFeature;
	var mapKeyY;
	var mapKeyX;

	var compassY;
	var compassX;

	var distanceY;
	var distanceX;
	var scrapbookY;
	var scrapbookX;
	
	function init() {
		$("#newOLM").accordion({
		    collapsible: true,
		    active: false,
		     autoHeight: true,
		     autoWidth: true,
		    autoActivate: true
		});
		
		$(window).bind('keypress', function(e) {
			
		    var code = (e.keyCode ? e.keyCode : e.which);
		    console.info(code);
			
		    if(code == 115) {
		    	//show olm
		    	document.getElementById('olm').style.visibility = 'visible';
				olmOpened = new Date();
				logOnServer("OLM Shown");
		  	}
		    if(code == 100) {
		    	//skip
		    	$.get("skipToNextStepTeacher", function(data) {
					console.info("skipToNextStepTeacher");
				});
		  	}

		});
		mapLayers = [];
		scenarioMaker = false;
		continueToPoll = true;
		deleteMode = false;
		//disableScrapBook(false,0,0);
		/* 	document.onmousedown = function(e) {
				//fixPageXY(e);
				console.info("Mouse down x"+e.screenX+" y:"+e.screenY);
			}; */

		//TODO need to update these based on scenario? 
		defaultLon = -2.147318;
		defaultLat = 52.423781;
		currentLon = defaultLon;
		currentLat = defaultLat;

		symbolSize = 30;
		$("#symbolSize").val(symbolSize);

		distanceTool = false;
		zoomLevel = 16.5;

		document.getElementById('scrapBookDiv').style.visibility = 'hidden';
		document.getElementById('questionnaire').style.visibility = 'hidden';
		document.getElementById('videoDiv').style.visibility = 'hidden';
		document.getElementById('map_key').style.visibility = 'hidden';
		document.getElementById('olm').style.visibility = 'hidden';
		document.getElementById('scenarioMakerDiv').style.visibility = 'hidden';
		document.getElementById('scenarioMakerDivControl').style.visibility = 'hidden';
		document.getElementById('result').style.visibility = 'hidden';
		document.getElementById('infoDiv').style.visibility = 'hidden';

		scenarioMakerDiv = false;
		symbolName = 'Garden';
		$("#symbolToAdd").empty();
		$("#symbolToAdd").append(symbolName);
		$.get("getAllSymbols", function(data) {
			$("#symbolsToAdd").empty();
			var i = 0;
			for (x in data) {
				$("#symbolsToAdd").append(
						'<button onClick="setSymbolToAdd(this)" value="'
								+ data[x] + '">' + data[x] + '</button>');
				i++;
				if (i > 5) {
					i = 0;
					$("#symbolsToAdd").append('<br>');

				}
			}

		});

		map = new OpenLayers.Map("map", {
			controls : [],
			projection : new OpenLayers.Projection("EPSG:4326"),
			displayProjection : new OpenLayers.Projection("EPSG:4326")
		});

		scalebar = new OpenLayers.Control.ScaleLine();
		map.addControl(scalebar);
		scalebar.maxWidth = 700;
		scalebar.geodesic = true;

		layer = new OpenLayers.Layer.OSM("Simple OSM Map",
				"/namshub/resources/tiles/\${z}/\${x}/\${y}.png");
		//Code below is for a local layer from geoserver.
		/* layer = new OpenLayers.Layer.WMS("Local OSM",
		 "http://localhost:8090/geoserver/local/wms", 
		{'layers': 'local:planet_osm_roads', transparent: true, format: 'image/gif'},
		{isBaseLayer: true}
		); */
		renderer = OpenLayers.Util.getParameters(window.location.href).renderer;
		renderer = (renderer) ? [ renderer ]
				: OpenLayers.Layer.Vector.prototype.renderers;

		vectors = new OpenLayers.Layer.Vector("Vector Layer");

		symbols = new OpenLayers.Layer.Vector("Symbol Layer");

		compassLayer = new OpenLayers.Layer.Vector("Compass Layer");

		compassLayer.setVisibility(false);

		boxes = new OpenLayers.Layer.Boxes("Boxes");

		map.addLayers([ layer, boxes, symbols, vectors, compassLayer ]);

		map.setCenter(new OpenLayers.LonLat(currentLon, currentLat).transform(
				new OpenLayers.Projection("EPSG:4326"), map
						.getProjectionObject()), zoomLevel);

		var click = new OpenLayers.Control.Click();
		map.addControl(click);
		click.activate();
		
		//Control below shows lat and long in bottom corner. 
		mousePositionControl = new OpenLayers.Control.MousePosition()
		map.addControl(mousePositionControl);

		navigationControl = new OpenLayers.Control.Navigation({
			'zoomWheelEnabled' : false
		});
		map.addControl(navigationControl);
		navigationControl.activate();

		
		
		
		
		//Drag feature for vectors, movable map objects. 
		dragFeature = new OpenLayers.Control.DragFeature(vectors, {
			onComplete : function(e) {
				console.info("Drag Finished");
				logOnServer("Drag Finished");
				var toProjection = new OpenLayers.Projection("EPSG:4326");
				var lonlat = e.geometry.getBounds().getCenterLonLat()
				lonlat = lonlat.transform(map.getProjectionObject(),
						toProjection);
				//var lonlat = map.getLonLatFromPixel(e.xy).transform(map.getProjectionObject(), toProjection);
				var objectName = e.attributes.some;
				var activityOrStep = e.attributes.activityOrStep;
				var movable = e.attributes.movable;
				var arrayPos = e.attributes.arrayPos;
				
				var screenX = dragFeature.handlers.feature.evt.screenX;
				var screenY = screen.height - dragFeature.handlers.feature.evt.screenY;
				
				
				checkLonLat(lonlat, objectName, false, activityOrStep, movable,
						arrayPos, screenX, screenY);
			}
		});

		dragFeature.handlers['drag'].stopDown = false;
		dragFeature.handlers['drag'].stopUp = false;
		dragFeature.handlers['drag'].stopClick = false;
		dragFeature.handlers['feature'].stopDown = false;
		dragFeature.handlers['feature'].stopUp = false;
		dragFeature.handlers['feature'].stopClick = false;

		map.addControl(dragFeature);
		dragFeature.activate();

		vectors.events.on({
			'featureselected' : function(e) {
				console.info("Vector Feature Selected :"
						+ e.feature.attributes.some);
			},
			'featureunselected' : function(e) {
				console
						.info("Vector Un Selected :"
								+ e.feature.attributes.some);
			}
		});
		symbols.events.on({
			'featureselected' : function(e) {
				console.info("Symbol Feature Selected :"
						+ e.feature.attributes.some);
				var toProjection = new OpenLayers.Projection("EPSG:4326");
				var lonlat = e.feature.geometry.getBounds().getCenterLonLat()
				lonlat = lonlat.transform(map.getProjectionObject(),
						toProjection);
				//var lonlat = map.getLonLatFromPixel(e.xy).transform(map.getProjectionObject(), toProjection);
				var objectName = e.feature.attributes.some;
				var activityOrStep = e.feature.attributes.activityOrStep;
				var movable = e.feature.attributes.movable;
				var arrayPos = e.feature.attributes.arrayPos;
			
				var screenX = selectFeature.handlers.feature.evt.screenX;
				var screenY = screen.height - selectFeature.handlers.feature.evt.screenY;
				checkLonLat(lonlat, objectName, true, activityOrStep, movable,
						arrayPos, screenX, screenY);
			},
			'featureunselected' : function(e) {
				console.info("Symbol Feature Un Selected :"
						+ e.feature.attributes.some);
			}
		});

		selectFeature = new OpenLayers.Control.SelectFeature(
				[ vectors, symbols ], {
					//multiple: false, hover: false
					clickout : true
				});

		map.addControl(selectFeature);
		selectFeature.activate();

		measureControls = {
			/* 	line : new OpenLayers.Control.Measure(OpenLayers.Handler.Path, {
					//displaySystem: "english",
			        geodesic: true,
			        persist: true,
			        handlerOptions: {
			            layerOptions: {
			              
			                renderers : renderer
			            }
			        }
				}), */

			line : new OpenLayers.Control.DynamicMeasure(
					OpenLayers.Handler.Path, {
						//displaySystem: "english",
						geodesic : true,
						persist : true,
						handlerOptions : {
							layerOptions : {

								renderers : renderer
							}
						}
					}),

			polygon : new OpenLayers.Control.Measure(
					OpenLayers.Handler.Polygon, {
						persist : true,
						geodesic : true,
						handlerOptions : {
							layerOptions : {
								renderers : renderer
							}
						}
					})
		};

		var control;
		for ( var key in measureControls) {
			control = measureControls[key];
			control.geodesic = true;
			control.setImmediate(true);
			control.events.on({
				"measure" : handleMeasurements,
				"measurepartial" : handleMeasurements
			});
			map.addControl(control);
		}

		//Create compass image.
		var compassImglink = "/namshub/resources/img/symbols/compass.png";
		var compassPosition = map.getCenter();

		compassfeature = new OpenLayers.Feature.Vector(
				new OpenLayers.Geometry.Point(compassPosition.lon,
						compassPosition.lat).transform(
						new OpenLayers.Projection("EPSG:4326"), map
								.getProjectionObject()), {
					some : "compass"
				}, {
					externalGraphic : compassImglink,
					graphicHeight : 200,
					graphicWidth : 200
				});
		compassLayer.addFeatures(compassfeature);

		blockEmoteMapsUI();
		
		
		
		 mapKeyY = screen.height - $("#ToggleMapKey").offset().top + window.screenY;
		 mapKeyX = $("#ToggleMapKey").offset().left + window.screenX;
	
		 compassY = screen.height - $("#ToggleCompass").offset().top + window.screenY;
		 compassX = $("#ToggleCompass").offset().left + window.screenX;
	
		 distanceY = screen.height - $("#ToggleDistance").offset().top + window.screenY;
		 distanceX = $("#ToggleDistance").offset().left + window.screenX;
	
		 //TODO put these back if the element is there.
		 
		// scrapbookY = screen.height - $("#ToggleScrapBook").offset().top + window.screenY;
		// scrapbookX = $("#ToggleScrapBook").offset().left + window.screenX;
		 
		var midY = screen.height / 2;
		var midX = screen.width / 2;
			
		var screenTargets = {
				"mapKeyX" : mapKeyX,
				"mapKeyY" : mapKeyY,
				"compassX" : compassX,
				"compassY" : compassY,
				"distanceX" : distanceX,
				"distanceY" : distanceY,
				"midX" : midX,
				"midY" : midY,
				"x" : screen.width,
				"y" : screen.height,
				"scrapbookY" : scrapbookY,
				"scrapbookX" : scrapbookX
			};
		
			$.postJSON("updateStaticScreenTargets", screenTargets, function(response) {
				console.info("updateStaticScreenTargets result:" + response);
			});
			
			updateScreenTarget(midX,midY);	
			
			start();
			poll();
	}

	function toggleControl(element) {
		/*          for(key in controls) {
		 var control = controls[key];
		 if(element.value == key && element.checked) {
		 control.activate();
		 } else {
		 control.deactivate();
		 }
		 } */
	}
	function getImageLink(imageName) {
		if (imageName == "CaravanSite.png") {
			return "<c:url value="/resources/img/symbols/CaravanSite.png" />";
		}

		return "<c:url value="/resources/img/symbols/CaravanSite.png" />";
	}

	function runBehaviorAndTalk(behavior, textToSay) {
		//$.postJSON("runBehavior", behavior);
		return function() {
			console.info("Starting runBehavior behavior:" + behavior + " text:"
					+ textToSay);
			speak(textToSay);
			console.info("Speaking finished:" + textToSay);
			$.postJSON("runBehavior", behavior, function(response) {
				console.info("runBehavior result:" + response);
			});
		}
	}

	function javaTalk(textToSay) {
		//$.postJSON("runBehavior", behavior);
		return function() {
			console.info("Starting java talk:" + textToSay);
			$.postJSON("say", textToSay, function(response) {
				console.info("java talk:" + response);
			});
		}
	}

	function naoWebRunAndTalk(behavior, textToSay, time, talkViaJS) {
		return function() {

			timeout = time / 1000;
			timeout = timeout + 2;
			if (talkViaJS) {
				speak(textToSay);
			}
			url = "http://" + naoWebIp
					+ "/Nao/do_query.php?call=1&priority=1&behaviour="
					+ behavior + "&speech=" + textToSay + "&timeout=" + timeout
					+ "";
			console.info("behavior:" + behavior);
			console.info("textToSay:" + textToSay);
			console.info("timeout:" + timeout);
			console.info("NAO WEB GET:" + url);
			$.get(url, function(data) {
				console.info("naoWebRunAndTalk ");
			});
		}
	}

	function runVirtualBehaviorAndTalk(behavior, textToSay) {
		return function() {
			document.getElementById('videoDiv').style.visibility = 'visible';

			console.info("Starting virtual runBehavior behavior:" + behavior
					+ " text:" + textToSay);
			speak(textToSay);
			console.info("Speaking finished:" + textToSay);

			var myVideo = document.getElementsByTagName('video')[0];
			myVideo
					.setAttribute(
							"src",
							[ '<c:url value="/resources/animations/'+behavior+'.mp4" /> ' ]);
			myVideo.load();
			myVideo.play();

		}
	}

	function setActivity(button) {
		activityId = button.value;
		activity = scenario.activities[activityId];
		console.info("Setting activity:" + activity.name +" id:" + activity.id);
		
		drawActivity(activity);
		//TODO sned something to the server. 
		
		
	}
	
	
	function runBehavior(behavior) {
		//$.postJSON("runBehavior", behavior);
		console.info("Starting runBehavior behavior:" + behavior);

		$.postJSON("runBehavior", behavior, function(response) {
			console.info("runBehavior result:" + response);
		});

	}

	function doMapUpdate(mapUpdate) {
		learnerId = mapUpdate.learnerId;
		naoWebIp = mapUpdate.naoWebIp;
		console.info("Received map update");

		if (mapUpdate.alert) {
			if (mapUpdate.alertText) {
				//alert(mapUpdate.alertText);
				//continueToPoll = false;
				console.info(mapUpdate.alertText);
				
			}
		}

		if (mapUpdate.scenarioMaker) {
			document.getElementById('scenarioMakerDiv').style.visibility = 'visible';
			document.getElementById('scenarioMakerDivControl').style.visibility = 'visible';
			scenarioMaker = true;
			$.unblockUI();
			navigationControl.activate();
			mousePositionControl.activate();
			getAllScenarios();
		} else {
			navigationControl.deactivate();
			mousePositionControl.deactivate();
			
			talkTimeWait = 0;

		 	if(mapUpdate.resetTools)
			{
				if (distanceTool) {
					disableDistanceTool(false,0,0);
				}
				if (compassLayer.visibility) {
					disableCompass(false,0,0);
				}

				if (document.getElementById('map_key').style.visibility == 'visible') {
					disableMapKey(false,0,0);
				}
				
				disableScrapBook(false,0,0);
				closeInfo();
			} 
			
			
			
			
			if (mapUpdate.update) {
				console.info("Doing Map Update");
				if(mapUpdate.step && mapUpdate.step.name)
				{
					logOnServer("Start of step:" + mapUpdate.step.name);
				}
				currentStep = mapUpdate.step;
				
				if (mapUpdate.newScenario) {
					scenario = mapUpdate.scenario;
					$("#scenario").html("" + mapUpdate.scenarioName);
					for (x in mapUpdate.scenario.activities)
					{
						$("#activityMenuPara").append('<button onClick="setActivity(this)" value="'+mapUpdate.scenario.activities[x].id+'">'+mapUpdate.scenario.activities[x].name+'</button>');
						$("#activityMenuPara").append('<br>');	
					}
				}
				
				
				if (mapUpdate.onScreenExplanation) {
					$("#explanationHeader").html("Explanation");

				} else {
					$("#explanationHeader").html("");

				}

				if (mapUpdate.naoWebMotorOn) {
					$.post("http://" + naoWebIp + "/Nao/do_query.php?init=1&ip=127.0.0.1",function() {});
				}

			
				if (mapUpdate.step) {
					if (mapUpdate.step.target) {
						console.info("Target - Lon:" + mapUpdate.step.target.lon + " Lat:" + mapUpdate.step.target.lat);
						getScreenXYFromLonLat(mapUpdate.step.target.lon, mapUpdate.step.target.lat);
						
					}
					
					if(mapUpdate.step.type && mapUpdate.step.type == "toolUse")
					{
						if(mapUpdate.step.tool == "mapKey")
						{
							updateScreenTarget(mapKeyX,mapKeyY);
						}
						else if(mapUpdate.step.tool == "compass")
						{
							updateScreenTarget(compassX,compassY);
						}
						else if(mapUpdate.step.tool == "distance")
						{
							updateScreenTarget(distanceX,distanceY);
						}
					}
					
					
				}
				boxes.clearMarkers();

				
				/* $("#activity").html("" + mapUpdate.activity.name); */
				$("#step").html("");
				/* $("#learnerModelMessage").html("" + mapUpdate.learnerModelMessage);
				 */
				if (mapUpdate.onScreenResult && mapUpdate.onScreenResult == true) {
					$("#result").html("" + mapUpdate.result);
					document.getElementById('result').style.visibility = 'visible';
				} else {
					document.getElementById('result').style.visibility = 'hidden';
				}
				symbols.destroyFeatures();
				vectors.destroyFeatures();

				
				if(mapUpdate.updateStep)
				{
					drawStep(mapUpdate.step);
				}
				
				if(mapUpdate.updateActivity)
				{
					drawActivity(mapUpdate.activity);
				}
				
				if (mapUpdate.updateLonLat) {
					if (mapUpdate.lonLat.lon && mapUpdate.lonLat.lat) {
						currentLon = mapUpdate.lonLat.lon;
						currentLat = mapUpdate.lonLat.lat;
						map.setCenter(new OpenLayers.LonLat(currentLon,currentLat).transform(new OpenLayers.Projection("EPSG:4326"), map.getProjectionObject()), zoomLevel);
					}
				}

				if (mapUpdate.panToReference) {
					if (mapUpdate.step.reference.lon && mapUpdate.step.reference.lat) {
						currentLon = mapUpdate.step.reference.lon;
						currentLat = mapUpdate.step.reference.lat;
						map.panTo(new OpenLayers.LonLat(currentLon, currentLat).transform(new OpenLayers.Projection("EPSG:4326"), map.getProjectionObject()),zoomLevel);
					}
				}

				
				delayPerChar = 90;

				if (mapUpdate.showInfo) {
					$("#infoPara").empty();
					$("#infoPara").append(mapUpdate.infoText);
					$("#scrapBookPara").append(mapUpdate.infoText);
					$("#scrapBookPara").append('<br>');		
					document.getElementById('infoDiv').style.visibility = 'visible';
				}

				if (mapUpdate.updateOLM ) {
					
					for (x in mapUpdate.learnerModel.competencies) {
						meterval = (mapUpdate.learnerModel.competencies[x].value * 100);
						metercolor = "green";
						// 0 2 4 6 8 10
						// 0 2 5 8 10
						if (meterval < 20) {
							meterval = 10;
							metercolor = "Red";
						} else if (meterval < 40) {
							meterval = 30;
							//orange
							metercolor = "#FFA500";
						} else if (meterval < 60) {
							meterval = 50;
							metercolor = "Yellow";
						} else if (meterval < 80) {
							meterval = 70;
							//greeny yellow
							metercolor = "#9ACD32";
						} else {
							meterval = 90;
							metercolor = "Green";
						}

						if (mapUpdate.learnerModel.competencies[x].name == 'symbol') {
							//	$("#olm1").val(mapUpdate.learnerModel.competencies[x].value);
							$("#symbolmeter").css("width", meterval + "%");
							$("#symbolmeter").css("background", metercolor);
							if (mapUpdate.onScreenExplanation
									&& mapUpdate.learnerModel.competencies[x].explanation != null) {
								$("#symbolexplanation").html(
										"" + mapUpdate.learnerModel.competencies[x].explanation);
							}
							var width = $('#newOLMSymbolMeter').width();
							 var parentWidth = $('#newOLMSymbolMeter').offsetParent().width();
							 var oldVal = 100*width/parentWidth;
							     
							     $("#newSymbolExplanation").html(
											"" + mapUpdate.learnerModel.competencies[x].explanation);
							     $("#newOLMSymbolMeter").animate({
							         width: meterval+'%',
							         backgroundColor: metercolor
							     }, 1000, function () {
							         // Animation complete.
							     });
							     $("#newOLMSymbolOldValue").animate({
							         left: oldVal+'%',
							     }, 500, function () {
							         // Animation complete.
							     }); 
						}
						if (mapUpdate.learnerModel.competencies[x].name == 'distance') {
							//	$("#olm2").val(mapUpdate.learnerModel.competencies[x].value);
							$("#distancemeter").css("width", meterval + "%");
							$("#distancemeter").css("background", metercolor);
							if (mapUpdate.onScreenExplanation
									&& mapUpdate.learnerModel.competencies[x].explanation != null) {
								$("#distanceexplanation").html(
										"" + mapUpdate.learnerModel.competencies[x].explanation);
							}
							var width = $('#newOLMDistanceMeter').width();
							 var parentWidth = $('#newOLMDistanceMeter').offsetParent().width();
							 var oldVal = 100*width/parentWidth;
							     
							     $("#newDistanceExplanation").html(
											"" + mapUpdate.learnerModel.competencies[x].explanation);
							     $("#newOLMDistanceMeter").animate({
							         width: meterval+'%',
							         backgroundColor: metercolor
							     }, 1000, function () {
							         // Animation complete.
							     });
							     $("#newOLMDistanceOldValue").animate({
							         left: oldVal+'%',
							     }, 500, function () {
							         // Animation complete.
							     });
						}
						if (mapUpdate.learnerModel.competencies[x].name == 'direction') {
							//	$("#olm3").val(mapUpdate.learnerModel.competencies[x].value);
							$("#compassmeter").css("width", meterval + "%");
							$("#compassmeter").css("background", metercolor);
							if (mapUpdate.onScreenExplanation
									&& mapUpdate.learnerModel.competencies[x].explanation != null) {
								$("#compassexplanation").html(
										"" + mapUpdate.learnerModel.competencies[x].explanation);
							}
							var width = $('#newOLMCompassMeter').width();
							 var parentWidth = $('#newOLMCompassMeter').offsetParent().width();
							 var oldVal = 100*width/parentWidth;
							     
							     $("#newCompassExplanation").html(
											"" + mapUpdate.learnerModel.competencies[x].explanation);
							     $("#newOLMCompassMeter").animate({
							         width: meterval+'%',
							         backgroundColor: metercolor
							     }, 1000, function () {
							         // Animation complete.
							     });
							     $("#newOLMCompassOldValue").animate({
							         left: oldVal+'%',
							     }, 500, function () {
							         // Animation complete.
							     });
						}

					}

				}
					

				vectors.redraw();
				vectors.refresh({
					force : true
				});

			
			

				if (mapUpdate.runBehaviors) {
					console.info("Got behaviors to Run");
					console.info("NAO Web:" + mapUpdate.naoWeb);
					for (x in mapUpdate.behaviors) {
						console.info("X:" + x);

						if (mapUpdate.virtualRobot == true) {
							setTimeout(runVirtualBehaviorAndTalk(
									mapUpdate.behaviors[x].behavior,
									mapUpdate.behaviors[x].speech), talkTimeWait);
						} else if (mapUpdate.naoWeb == true) {
							setTimeout(naoWebRunAndTalk(
									mapUpdate.behaviors[x].behavior,
									mapUpdate.behaviors[x].speech,
									mapUpdate.behaviors[x].delay,
									mapUpdate.talkViaJS), talkTimeWait);
						} else if (mapUpdate.javaTalk == true) {
							setTimeout(javaTalk(mapUpdate.behaviors[x].speech),
									talkTimeWait);
						} else {
							setTimeout(runBehaviorAndTalk(
									mapUpdate.behaviors[x].behavior,
									mapUpdate.behaviors[x].speech), talkTimeWait);
						}

						talkTimeWait = talkTimeWait + mapUpdate.behaviors[x].delay;
					}
				}

			setTimeout(function() {
				$.unblockUI();
				timeQuestionStarted = new Date();
				if(mapUpdate.step)
				{
					$("#step").html("                " + mapUpdate.step.name);
					logOnServer("TextShownOnScreen:" + mapUpdate.step.name);
				}
			}, talkTimeWait);

			var waitforResult = 3000;
			if (talkTimeWait > waitforResult) {
				waitforResult = talkTimeWait;
			}
			setTimeout(function() {
				document.getElementById('result').style.visibility = 'hidden';
			}, waitforResult);

			}
			
			
			
			if (mapUpdate.highlightLonLat) 
			{
				var latLower = mapUpdate.lonLatToHightlight.lat - latRange;
				var latUpper = mapUpdate.lonLatToHightlight.lat + latRange;
				var longLower = mapUpdate.lonLatToHightlight.lon - longRange;
				var longUpper = mapUpdate.lonLatToHightlight.lon+ longRange;
				bounds = new OpenLayers.Bounds();
				bounds.extend(new OpenLayers.LonLat(longLower, latLower));
				bounds.extend(new OpenLayers.LonLat(longUpper, latUpper));
				bounds = bounds.transform(new OpenLayers.Projection(
						"EPSG:4326"), map.getProjectionObject())
				box = new OpenLayers.Marker.Box(bounds);
				boxes.addMarker(box);
			}
			
			if (mapUpdate.highlightRightAnswer) 
			{
				if (currentStep && currentStep.target) {
					 drawMapFeature('greenCircle.png', currentStep.target.lon,
							 currentStep.target.lat, 'target', 50, 50, symbols, false, -1, 'step');
					 
				 	/* drawMapFeature('orangeCircle.png', step.reference.lon,
							step.reference.lat, 'reference', 50, 50, false, -1, 'step'); */ 
				}
			}
			
			if (mapUpdate.blockUI) {
				blockEmoteMapsUI();
			}
			if (mapUpdate.unBlockUI) {
				$.unblockUI();
			}
			
			if(mapUpdate.showOLM || mapUpdate.onScreenOLM)
			{
				document.getElementById('olm').style.visibility = 'visible';
				olmOpened = new Date();
				logOnServer("OLM Shown");
			}
			
			if (mapUpdate.toolUpdate) {
				if (mapUpdate.compassUpdate) {
					if (mapUpdate.compassShow) {
						enableCompass(false,0,0);
					}
					else
					{
						disableCompass(false,0,0);
					}
				}
				if (mapUpdate.distanceUpdate) {
					if (mapUpdate.distanceShow) {
						enableDistanceTool(false,0,0);
					}
					else
					{
						disableDistanceTool(false,0,0);
					}
				}
				if (mapUpdate.mapKeyUpdate) {
					if (mapUpdate.mapKeyShow) {
						enableMapKey(false,0,0);
					}
					else
					{
						disableMapKey(false,0,0);
					}
				}
				if(mapUpdate.tool && mapUpdate.toolAction)
					{
						if(mapUpdate.tool == 'compass')
						{
							if(mapUpdate.toolAction == 'show')
							{
								enableCompass(false,0,0);
							}
							else if(mapUpdate.toolAction == 'hide')
							{
								disableCompass(false,0,0);
							}	
							else if(mapUpdate.toolAction == 'highlight')
							{
								highlightCompass();
							}
						}
						if(mapUpdate.tool == 'distance')
						{
							//TODO implement start, end, reset
							if(mapUpdate.toolAction == 'show')
							{
								enableDistanceTool(false,0,0);
							}
							else if(mapUpdate.toolAction == 'hide')
							{
								disableDistanceTool(false,0,0);
							}
							else if(mapUpdate.toolAction == 'highlight')
							{
								highlightDistanceTool();
							}
						}
						if(mapUpdate.tool == 'mapKey')
						{
							if(mapUpdate.toolAction == 'show')
							{
								enableMapKey(false,0,0);
							}
							else if(mapUpdate.toolAction == 'hide')
							{
								disableMapKey(false,0,0);
							}
							else if(mapUpdate.toolAction == 'highlight')
							{
								highlightMapKey();
							}
						}
						if(mapUpdate.tool == 'scrapBook')
						{
							if(mapUpdate.toolAction == 'show')
							{
								enableScrapBook(false,0,0);
							}
							else if(mapUpdate.toolAction == 'hide')
							{
								disableScrapBook(false,0,0);
							}
							else if(mapUpdate.toolAction == 'highlight')
							{
								//TODO implement
								//highlightMapKey();
							}
						}
						
					}
			}
			
			if (mapUpdate.askQuestionnaire) {
				//Show questionnaire.
				setTimeout(function() {
					questionnaire = mapUpdate.questionnaire;
					showQuestionnaire();
				}, talkTimeWait);
			}
			
			if (mapUpdate.refreshBrowser && mapUpdate.refreshBrowser == true) {
				console.info("Got reload:"+mapUpdate.refreshBrowser );
				window.location.reload();
			}
			/* if (mapUpdate.refreshBrowser && mapUpdate.refreshBrowser == true) {
				window.location.reload();
			} */
			console.info("End of update");
			
		}
	}

	function hideOLM() {
		document.getElementById('olm').style.visibility = 'hidden';
		olmClosed = new Date();

		logPeriodOnServer("OLM Opened between", olmOpened, olmClosed);

		timeQuestionStarted = new Date();
	}

	function closeInfo() {
		document.getElementById('infoDiv').style.visibility = 'hidden';
	}

	function getScreenXYFromLonLat(lon,lat)
	{
		 var toProjection = new OpenLayers.Projection("EPSG:4326");
	        
		var bounds =  map.getExtent().transform( map.getProjectionObject(), toProjection);
	
		var mapheight = bounds.top - bounds.bottom;
		var mapwidth = bounds.right - bounds.left;
		var distfrombottom = lat - bounds.bottom;
		var distfromleft = lon - bounds.left;
		
		var x = (distfromleft/mapwidth)*screen.width;
		var y = (distfrombottom/mapheight)*screen.height;
		
		updateScreenTarget(x,y);
	}
	
	function updateScreenTarget(x,y)
	{
		var screenTarget = {
				"x" : x,
				"y" : y
			};
		
			$.postJSON("updateRightAnswerTarget", screenTarget, function(response) {
				console.info("updateRightAnswerTarget result:" + response);
			});
	}
	
	function showQuestionnaire() {
		document.getElementById('questionnaire').style.visibility = 'visible';
		$("#questionnaire").empty();
		multipleChoiceQuestionsToFind = new Array();
		$("#questionnaire").append('<h1>Questionnaire</h1>');
		$("#questionnaire").append('Please answer the questions below.');

		for (x in questionnaire.questions) {

			console.info("Creating question id:"
					+ questionnaire.questions[x].id);
			//table.append( '<tr><td>' + questionnaire.questions[x].question + '</td> <td>Poor' + '<div id="q'+questionnaire.questions[x].id+'" class="questionnaireSlider" style="width: 100px; display:  inline-block;"></div>'+ ' Good '+'</td></tr>' );
			if (questionnaire.questions[x].type == 'likert') {
				$("#questionnaire")
						.append(
								'<div class="question-div"><span class="question-span">'
										+ questionnaire.questions[x].question
										+ '</span> Poor'
										+ '<div id="q'+questionnaire.questions[x].id+'" class="questionnaireSlider" style="width: 100px; display:  inline-block;"></div>'
										+ ' Good ' + '</div>');
			} else if (questionnaire.questions[x].type == 'multipleChoice') {
				var mutipleChoiceQuestion = questionnaire.questions[x];
				$("#questionnaire").append('<div class="question-div"></div>');
				var qdiv = $("#questionnaire").last();
				qdiv.append('<span class="question-span">'
						+ mutipleChoiceQuestion.question + '</span>');
				qdiv.append('<div></div>');
				var multiChoiceDiv = qdiv.last();
				if(mutipleChoiceQuestion.start)
					{
					multiChoiceDiv
					.append(''+mutipleChoiceQuestion.start+' - ');
					}
				for (y in mutipleChoiceQuestion.options) {
					multiChoiceDiv
							.append('<input class="questionnaireRadio" type="radio" name="q'+questionnaire.questions[x].id+'" value="'+mutipleChoiceQuestion.options[y]+'">'
									+ mutipleChoiceQuestion.options[y] + '');
				}
				if(mutipleChoiceQuestion.end)
				{
				multiChoiceDiv
				.append(' - '+mutipleChoiceQuestion.end);
				}
				multiChoiceDiv
				.append('<br><br>');
				
				multipleChoiceQuestionsToFind
						.push(questionnaire.questions[x].id);
			}
		}

		$("#questionnaire")
				.append(
						'<div><button onClick="submitQuestionnaire()" name="submitQuestionnaire">submit</button></div>');

		jQuery('.questionnaireSlider').each(function() {
			console.info("Got a slider");

			$(this).slider({
				animate : true,
				range : "min",
				value : 5,
				min : 1,
				max : 10,
				step : 1
			});
		});

	}

	function submitQuestionnaire() {
		document.getElementById('questionnaire').style.visibility = 'hidden';

		var questonnaireAnswers = new Array();

		jQuery('.questionnaireSlider').each(
				function() {
					var qid = "" + $(this).attr("id");
					qid = qid.substring(1);
					console.info("Got a slider value:"
							+ $(this).slider("value") + " id:" + qid);

					var questionnaireAnswer = {
						"questionId" : parseInt(qid),
						"answer" : "" + $(this).slider("value")
					};
					questonnaireAnswers.push(questionnaireAnswer);
				});

		for ( var i in multipleChoiceQuestionsToFind) {
			var id = multipleChoiceQuestionsToFind[i];
			var checked = $('input[name="q' + id + '"]:checked').val();

			//qid = qid.substring(1);
			console.info("Got a multi value:" + checked + " id:" + id);

			var questionnaireAnswer = {
				"questionId" : parseInt(id),
				"answer" : "" + checked
			};
			questonnaireAnswers.push(questionnaireAnswer);
		}

		var questionnaireLog = {
			"learnerId" : learnerId,
			"questonnaireAnswers" : questonnaireAnswers
		};
		$.postJSON("questionnaireAnswer", questionnaireLog, function() {
		});

		timeQuestionStarted = new Date();
	}

	function sendCheckLonLat(lonlat, objectName, clickOrDrag, toolClick, tool,toolAction, block,skip,screenX, screenY) {
		if(block)
		{
			blockEmoteMapsUI();
		}
		var questionAnswered = new Date();
		if(lonlat)
		{
		console.info("Lon:" + lonlat.lon + " Lat:" + lonlat.lat + " Name:"
				+ name);
		}
		console.info("Time Question Started:" + timeQuestionStarted
				+ " Question Answered:" + questionAnswered);
		logPeriodOnServer("Step finished:", timeQuestionStarted,
				questionAnswered);
		var mapClickDTO = {
			"lonlat" : lonlat,
			"name" : "notused",
			"learnerName" : learnerName,
			"learnerId" : learnerId,
			"objectName" : objectName,
			"questionStarted" : timeQuestionStarted,
			"questionAnswered" : questionAnswered,
			"clickOrDrag" : clickOrDrag,
			"toolClick" : toolClick,
			"tool" : tool,
			"toolAction" : toolAction,
			"skip" : skip,
			"screenX" : screenX,
			"screenY" :screenY
		};
		$.postJSON("checkLonLat", mapClickDTO, function(mapUpdate) {
		
		});
	}

	function checkLonLat(lonlat, objectName, clickOrDrag, activityOrStep,
			movable, arrayPos, screenX, screenY) {

		var screenblock = false;
		//Make sure we don't check to often if chld is just thiking.
		if (scenarioMaker) {
			//TODO if object name = target then record those vals. etc.... These canthen be sent to server later. 
			if (deleteMode && deleteMode == true) {
				deleteArrayPosMapObject(objectName, lonlat, activityOrStep,
						movable, arrayPos);
			}

			if (objectName == 'reference') {
				scenarioMakerStep.reference.lon = lonlat.lon;
				scenarioMakerStep.reference.lat = lonlat.lat;
			} else if (objectName == 'target') {
				scenarioMakerStep.target.lon = lonlat.lon;
				scenarioMakerStep.target.lat = lonlat.lat;
			} else if (objectName == 'scenarioMakerMarker') {

				scenarioMakerMarker.lon = lonlat.lon;
				scenarioMakerMarker.lat = lonlat.lat;
			} else {
				updateLonLatMovableMapObject(objectName, lonlat);
			}
		} else {
			console.info("Lon:" + lonlat.lon + " Lat:" + lonlat.lat);
			logOnServer(" Map Click Lon:" + lonlat.lon + " Lat:" + lonlat.lat);
			//If this is a drag and it doesn't leave the area. 
			if(currentStep)
			{
				if (!clickOrDrag && currentStep.type == "haybridge"
						&& lonlat.lon > -2.153079384964059) {
					
					sendCheckLonLat(lonlat, objectName, clickOrDrag,false, '','',true,false,screenX,screenY);
				} else if (currentStep.type == "haybridge") {
					if (!clickOrDrag) {
						logOnServer("Symbol not moved enough:" + objectName);
					} else {
						//clicked not draged...
					}
				} else {
					if (!clickOrDrag && currentStep.type == "drag") {
						sendCheckLonLat(lonlat, objectName, clickOrDrag,false, '','',screenblock,false,screenX,screenY);
					} else if (clickOrDrag && currentStep.type == "click") {
						sendCheckLonLat(lonlat, objectName, clickOrDrag,false, '','',screenblock,false,screenX,screenY);
					} else {
						//not done expected action.
					}
				}
			}
			else
			{
				console.info("Current Step is null");
			}
		}
	}

	function logOnServer(message) {
		var mapMessage = {
			"learnerId" : learnerId,
			"message" : message
		};
		$.postJSON("mapMessage", mapMessage, function(taskLogItem) {
			console.info("Following Logged on server:" + taskLogItem.logMessage
					+ " Time:" + taskLogItem.dateTime);
		});
	}
	

	function logPeriodOnServer(message, from, to) {
		var mapMessage = {
			"learnerId" : learnerId,
			"message" : message,
			"from" : from,
			"to" : to
		};
		$.postJSON("mapMessage", mapMessage, function(taskLogItem) {
			console.info("Following Logged on server:" + taskLogItem.logMessage
					+ " Time:" + taskLogItem.dateTime);
		});
	}

	function blockEmoteMapsUI() {
		//$.blockUI({ overlayCSS: { backgroundColor: '#00f' } }); 
		$.blockUI({
			message : null
		});
	}

	 function start() {
	 console.info("Starting");
	 blockEmoteMapsUI();
	 $.get("start", function(responceString) {
		 console.info("Called start method to ensure a map update is loaded, response:"+responceString );
	 });
	 } 

	function getOrCreateScenario() {
		var scenName = $("#scenarioName").val();
		$.get("getOrCreateScenario", {
			scenarioName : scenName
		}, function(scenarioData) {
			scenarioMakerStep = null;
			scenario = scenarioData;
			stepId = 0;
			activityId = 0;
			$("#stepId").text(stepId);
			$("#activityId").text(activityId);
			displayScenario();

		});
	}

	function addReference() {
		if (scenarioMakerStep.reference) {

		} else {
			scenarioMakerStep.reference = {
				lon : currentLon,
				lat : currentLat
			}
			displayScenario();
		}
	}

	function removeReference() {
		scenarioMakerStep.reference = null;
		displayScenario();
	}

	function removeTarget() {
		scenarioMakerStep.target = null;
		displayScenario();
	}

	function addStarAtReference() {
		if (scenarioMakerStep.reference) {
			if (scenarioMakerStep.mapObjects) {

			} else {
				scenarioMakerStep.mapObjects = new Array();
			}

			var found = false;
			//TODO check it doesn't already exist... 
			for (x in scenarioMakerStep.mapObjects) {
				//Draw objects on screen. 

				if (scenarioMakerStep.mapObject[x].name == 'reference') {
					found = true;
					scenarioMakerStep.mapObject[x].lon = scenarioMakerStep.reference.lon;
					scenarioMakerStep.mapObject[x].lat = scenarioMakerStep.reference.lat;

				}
			}

			if (!found) {
				var lonLat = {
					lon : scenarioMakerStep.reference.lon,
					lat : scenarioMakerStep.reference.lat
				};
				var newMapObject = {
					lonLat : lonLat,
					image : 'Star.png',
					imageHeight : symbolSize,
					imageWidth : symbolSize,
					name : 'reference'
				};
				scenarioMakerStep.mapObjects.push(newMapObject);
			}

			displayScenario();
		} else {

		}
	}

	function addTarget() {
		if (scenarioMakerStep.target) {

		} else {
			scenarioMakerStep.target = {
				lon : currentLon,
				lat : currentLat
			}
			displayScenario();
		}
	}

	function addSymbolAtMarker(button) {
		if (scenarioMakerMarker && scenarioMakerMarker.lon
				&& scenarioMakerMarker.lat) {
			if (button.value == 'movable') {
				if (scenarioMakerStep.movableMapObjects) {

				} else {
					scenarioMakerStep.movableMapObjects = new Array();
				}

				var imageName = symbolName + '.png';
				var lonLat = {
					lon : scenarioMakerMarker.lon,
					lat : scenarioMakerMarker.lat
				};

				var newMapObject = {
					lonLat : lonLat,
					image : imageName,
					imageHeight : symbolSize,
					imageWidth : symbolSize,
					name : symbolName
				};
				scenarioMakerStep.movableMapObjects.push(newMapObject);
			} else {
				if (scenarioMakerStep.mapObjects) {

				} else {
					scenarioMakerStep.mapObjects = new Array();
				}

				var imageName = symbolName + '.png';
				var lonLat = {
					lon : scenarioMakerMarker.lon,
					lat : scenarioMakerMarker.lat
				};
				var newMapObject = {
					lonLat : lonLat,
					image : imageName,
					imageHeight : symbolSize,
					imageWidth : symbolSize,
					name : symbolName
				};
				scenarioMakerStep.mapObjects.push(newMapObject);
			}

			displayScenario();
		}
	}

	function setSymbolToAdd(button) {
		console.info("Sybol to add:" + button.value);
		symbolName = button.value;
		$("#symbolToAdd").empty();
		$("#symbolToAdd").append(symbolName);
	}

	function addSymbolAtMarkerForActivity() {
		if (scenarioMakerMarker && scenarioMakerMarker.lon
				&& scenarioMakerMarker.lat) {

			if (scenarioMakerActivity.mapObjects) {

			} else {
				scenarioMakerActivity.mapObjects = new Array();
			}

			var imageName = symbolName + '.png';
			var lonLat = {
				lon : scenarioMakerMarker.lon,
				lat : scenarioMakerMarker.lat
			};
			var newMapObject = {
				lonLat : lonLat,
				image : imageName,
				imageHeight : symbolSize,
				imageWidth : symbolSize,
				name : symbolName
			};
			scenarioMakerActivity.mapObjects.push(newMapObject);

			scenarioMakerActivity.scenarioName = scenario.name;
			scenarioMakerActivity.id = activityId;

			$.postJSON("submitScenarioActivity", scenarioMakerActivity,
					function(scenarioData) {
						scenarioMakerStep = null;
						scenarioMakerActivity = null;
						scenario = scenarioData;
						displayScenario();
					});

		}
	}

	function addSymbolAtTarget() {
		if (scenarioMakerStep.target) {

			if (scenarioMakerStep.mapObjects) {

			} else {
				scenarioMakerStep.mapObjects = new Array();
			}

			var imageName = symbolName + '.png';
			var lonLat = {
				lon : scenarioMakerStep.target.lon,
				lat : scenarioMakerStep.target.lat
			};
			var newMapObject = {
				lonLat : lonLat,
				image : imageName,
				imageHeight : symbolSize,
				imageWidth : symbolSize,
				name : symbolName
			};
			scenarioMakerStep.mapObjects.push(newMapObject);

			displayScenario();
		} else {

		}
	}

	function updateStepName() {
		var newName = "";
		if (scenarioMakerStep.type) {
			if (scenarioMakerStep.type == "drag") {
				newName = newName + "Please drag the ";
			} else if (scenarioMakerStep.type == "click") {
				newName = newName + "Please click the ";
			}
		}
		if (scenarioMakerStep.symbol) {
			newName = newName + scenarioMakerStep.symbolRequired
					+ " to the point ";
		} else {
			newName = newName + "point ";
		}
		if (scenarioMakerStep.distance) {
			newName = newName + scenarioMakerStep.distanceRequired + " "+ scenarioMakerStep.distanceUnits;
		} else {

		}
		if (scenarioMakerStep.direction) {
			newName = newName + scenarioMakerStep.directionRequired + " ";
		} else {

		}
		newName = newName + "from the...";
		scenarioMakerStep.name = newName;
		displayScenario();
	}

	function setCentreToLocation(button) {
		currentLon = parseFloat(button.name);
		currentLat = parseFloat(button.value);

		map.panTo(new OpenLayers.LonLat(currentLon, currentLat).transform(
				new OpenLayers.Projection("EPSG:4326"), map
						.getProjectionObject()));

		/* map.setCenter(new OpenLayers.LonLat(currentLon,
				currentLat).transform(
				new OpenLayers.Projection("EPSG:4326"), map
						.getProjectionObject()), zoomLevel); */
		displayScenario();
	}

	function setCentre() {
		var lonlat = map.getCenter();
		//.transform(map.getProjectionObject(), toProjection);
		var toProjection = new OpenLayers.Projection("EPSG:4326");
		lonlat = lonlat.transform(map.getProjectionObject(), toProjection);
		currentLon = lonlat.lon;
		currentLat = lonlat.lat;
		displayScenario();
		//var centre = map.getCenter();
		//console.info("Set centre");

		//var lonlat = map.getCentre().transform(map.getProjectionObject(), toProjection);
		//currentLon =  lonlat.lon;
		//currentLat = lonlat.lat;
		//displayScenario();	
		/* 	map.setCenter(new OpenLayers.LonLat(currentLon,
					currentLat).transform(
					new OpenLayers.Projection("EPSG:4326"), map
							.getProjectionObject()), zoomLevel); */

	}

	function setCentreText() {
		currentLon = parseFloat($("#lon").val());
		currentLat = parseFloat($("#lat").val());

		map.setCenter(new OpenLayers.LonLat(currentLon, currentLat).transform(
				new OpenLayers.Projection("EPSG:4326"), map
						.getProjectionObject()), zoomLevel);
		displayScenario();
	}

	function addUpdateTargetFromRequirements() {
		scenarioMakerStep.distanceRequired = $("#distanceRequired").val();
		scenarioMakerStep.distanceUnits = $("#distanceUnits").val();
		scenarioMakerStep.directionRequired = $("#directionRequired").val();
		var good = true;
		if (scenarioMakerStep.reference) {

			if (scenarioMakerStep.distance
					&& scenarioMakerStep.distanceRequired
					&& scenarioMakerStep.distanceRequired != "") {

			} else if (scenarioMakerStep.distance) {
				good = false;
			}
			if (scenarioMakerStep.direction
					&& scenarioMakerStep.directionRequired
					&& scenarioMakerStep.directionRequired != "") {

			} else if (scenarioMakerStep.direction) {
				good = false;
			}
			if (good) {
				$.postJSON("updateTarget", scenarioMakerStep, function(target) {
					if (target.lon) {
						scenarioMakerStep.target = target;
						displayScenario();
					} else {
						//issue with return.
					}
				});

			} else {
				//missing something
			}
		} else {
			//missing reference
		}
	}

	function addSprite() {
		if (scenarioMakerStep.movableMapObjects) {

		} else {
			scenarioMakerStep.movableMapObjects = new Array();
		}
		var lonLat = {
			lon : currentLon,
			lat : currentLat
		};
		var newMapObject = {
			lonLat : lonLat,
			image : 'marker.png',
			imageHeight : symbolSize,
			imageWidth : symbolSize,
			name : 'sprite'
		};

		scenarioMakerStep.movableMapObjects.push(newMapObject);

		displayScenario();
	}

	function removeFromMapObjects(button) {

		if (scenarioMakerStep.mapObjects) {
			removeFromArrayOfMapObjects(scenarioMakerStep.mapObjects,
					button.value);
			displayScenario();
		}

	}

	function updateLonLatMovableMapObject(objectName, lonlat) {
		if (scenarioMakerStep.movableMapObjects) {
			for (x in scenarioMakerStep.movableMapObjects) {
				if (scenarioMakerStep.movableMapObjects[x].name === objectName) {
					scenarioMakerStep.movableMapObjects[x].lonLat.lon = lonlat.lon;
					scenarioMakerStep.movableMapObjects[x].lonLat.lat = lonlat.lat;

				}
			}
			displayScenario();
		}
	}

	function deleteArrayPosMapObject(objectName, lonlat, activityOrStep,
			movable, arrayPos) {
		console.info("Deleting Map Object:" + objectName + " It is for the "
				+ activityOrStep + ". Movable:" + movable + " Array Position:"
				+ arrayPos);

		if (arrayPos && arrayPos >= 0) {
			if (activityOrStep == 'activity') {
				if (movable) {
					removeFromArrayOfMapObjectsArrayPos(
							scenarioMakerActivity.movableMapObjects,
							objectName, arrayPos);
				} else {
					removeFromArrayOfMapObjectsArrayPos(
							scenarioMakerActivity.mapObjects, objectName,
							arrayPos);
				}

				scenarioMakerActivity.scenarioName = scenario.name;
				scenarioMakerActivity.id = activityId;

				$.postJSON("submitScenarioActivity", scenarioMakerActivity,
						function(scenarioData) {
							scenarioMakerStep = null;
							scenarioMakerActivity = null;
							scenario = scenarioData;
							displayScenario();
						});

			} else {
				if (movable) {
					removeFromArrayOfMapObjectsArrayPos(
							scenarioMakerStep.movableMapObjects, objectName,
							arrayPos);
				} else {
					removeFromArrayOfMapObjectsArrayPos(
							scenarioMakerStep.mapObjects, objectName, arrayPos);
				}
				submitScenarioStep();
			}
		}
		//TODO need to check the activity, movable, then array poss. if all valid then remove from relevant array and update. 

		//TODO also need to add in the deleteMode button. 

	}

	function removeFromArrayOfMapObjectsArrayPos(arr, mapObjectName, arrayPos) {
		arr.splice(arrayPos, 1);
	}

	function removeFromMovableMapObjects(button) {
		if (scenarioMakerStep.movableMapObjects) {
			removeFromArrayOfMapObjects(scenarioMakerStep.movableMapObjects,
					button.value);
			displayScenario();
		}
	}

	function removeFromArrayOfMapObjects(arr, mapObjectName) {
		for ( var i = arr.length; i--;) {
			if (arr[i].name === mapObjectName) {
				arr.splice(i, 1);
			}
		}
	}

	function nextScenarioStep() {
		scenarioMakerStep = null;
		stepId = stepId + 1;
		$("#stepId").text(stepId);
		displayScenario();
	}

	function prevScenarioStep() {
		scenarioMakerStep = null;
		stepId = stepId - 1;
		if (stepId < 0) {
			stepId = 0;
		}
		$("#stepId").text(stepId);
		displayScenario();
	}
	
	
	function insertStepAtThisPosition()
	{
		
		
		newStep = {};
		newStep.mustAnswerToProgress = true;
		newStep.type = "click";
		newStep.id = stepId;
		
		scenarioMakerActivity.steps.splice(stepId, 0, newStep);
		for(var i=0;i<scenarioMakerActivity.steps.length;i++){
		    	scenarioMakerActivity.steps[i].id=i; 
		}
		scenarioMakerActivity.scenarioName = scenario.name;
		scenarioMakerActivity.id = activityId;
		$.postJSON("submitScenarioActivity", scenarioMakerActivity,
				function(scenarioData) {
					scenarioMakerStep = null;
					scenarioMakerActivity = null;
					scenario = scenarioData;
					displayScenario();
				});
	}
	
	function removeStepAtThisPosition()
	{
		
		
		scenarioMakerActivity.steps.splice(stepId, 1);
		
		for(var i=0;i<scenarioMakerActivity.steps.length;i++){
		    	scenarioMakerActivity.steps[i].id=i; 
		}
		scenarioMakerActivity.scenarioName = scenario.name;
		scenarioMakerActivity.id = activityId;
		$.postJSON("submitScenarioActivity", scenarioMakerActivity,
				function(scenarioData) {
					scenarioMakerStep = null;
					scenarioMakerActivity = null;
					scenario = scenarioData;
					displayScenario();
				});
	}

	function displayScenario() {
		//TODO check scenario is not null
		symbols.destroyFeatures();
		vectors.destroyFeatures();

		if (scenario) {
			$("#scenarioName").val(scenario.name);

			var activity = null;
			activity = scenario.activities[activityId];
			scenarioMakerActivity = activity;
			if (activity) {
				drawActivity(activity);
			}
			console.info("Drawing Activity Complete");

			var stepFound = false;
			var numberOfSteps = 0;
			if (scenarioMakerStep) {
				scenarioMakerMarker = {
					lon : currentLon,
					lat : currentLat
				};
				drawStep(scenarioMakerStep);
				drawStepOnScenarioMaker(scenarioMakerStep);
				stepFound = true;
			} else if (activity.steps) {
				console.info("Number of steps in activity:"
						+ activity.steps.length);
				numberOfSteps = activity.steps.length;
				if (activity.steps[stepId]) {
					stepFound = true;
					//set the stepIdVariable
					scenarioMakerStep = activity.steps[stepId];

					if (scenarioMakerStep.reference
							&& scenarioMakerStep.reference.lon
							&& scenarioMakerStep.reference.lat) {
						currentLon = scenarioMakerStep.reference.lon;
						currentLat = scenarioMakerStep.reference.lat;

						map.setCenter(new OpenLayers.LonLat(currentLon,
								currentLat).transform(
								new OpenLayers.Projection("EPSG:4326"), map
										.getProjectionObject()), zoomLevel);

						scenarioMakerMarker = {
							lon : currentLon,
							lat : currentLat
						};
					}

					else if (scenarioMakerStep.centre
							&& scenarioMakerStep.centre.lon
							&& scenarioMakerStep.centre.lat) {
						currentLon = scenarioMakerStep.centre.lon;
						currentLat = scenarioMakerStep.centre.lat;

						map.setCenter(new OpenLayers.LonLat(currentLon,
								currentLat).transform(
								new OpenLayers.Projection("EPSG:4326"), map
										.getProjectionObject()), zoomLevel);

						scenarioMakerMarker = {
							lon : currentLon,
							lat : currentLat
						};
					}

					drawStep(scenarioMakerStep);
					drawStepOnScenarioMaker(scenarioMakerStep);

				}
			}
			if (!stepFound) {
				if (stepId > numberOfSteps) {
					stepId = numberOfSteps;
					$("#stepId").text(stepId);
				}
				scenarioMakerStep = {};
				scenarioMakerMarker = {
					lon : currentLon,
					lat : currentLat
				};
				scenarioMakerStep.mustAnswerToProgress = true;
				scenarioMakerStep.type = "click";
				drawStep(scenarioMakerStep);
				drawStepOnScenarioMaker(scenarioMakerStep);

				//TODO else set id to steps lenght +1 or stepIdToDisplay
				//set the stepIdVariable
				//Set refrence to be last target? or centre of the map if that is not available  
				//Srt target to be a little bit offset from reference. 		
			}

		}
	}

	function setSymbolSize() {
		symbolSize = parseInt($("#symbolSize").val());
		$("#symbolSize").val(symbolSize);
	}

	function submitScenarioStep() {

		scenarioMakerStep.scenarioName = scenario.name;
		scenarioMakerStep.id = stepId;
		scenarioMakerStep.activityId = activityId;
		scenarioMakerStep.name = $("#stepName").val();
		scenarioMakerStep.stepSpeech = $("#stepSpeech").val();
		scenarioMakerStep.symbolNameRequired = $("#symbolNameRequired").val();
		scenarioMakerStep.distanceRequired = $("#distanceRequired").val();
		scenarioMakerStep.distanceUnits = $("#distanceUnits").val();
		scenarioMakerStep.directionRequired = $("#directionRequired").val();
		scenarioMakerStep.directionUnits = $("#directionUnits").val();
		scenarioMakerStep.trailId = $("#trailId").val();
		scenarioMakerStep.trailName = $("#trailName").val();
		
		
		scenarioMakerStep.infoText = $("#infoOnCompletion").val();
		scenarioMakerStep.centre = {
			lon : currentLon,
			lat : currentLat
		}

		$.postJSON("submitScenarioStep", scenarioMakerStep, function(
				scenarioData) {
			scenarioMakerStep = null;
			scenario = scenarioData;
			displayScenario();
		});
	}

	function drawStepOnScenarioMaker(step) {
		if (step.name) {
			$("#stepName").val(step.name);
		} else {
			$("#stepName").val("");
		}
		if (step.stepSpeech) {
			$("#stepSpeech").val(step.stepSpeech);
		} else {
			$("#stepSpeech").val("");
		}
		
		
		if (step.symbolNameRequired) {
			$("#symbolNameRequired").val(step.symbolNameRequired);
		} else {
			$("#symbolNameRequired").val("");
		}
		if (step.distanceRequired) {
			$("#distanceRequired").val(step.distanceRequired);
		} else {
			$("#distanceRequired").val("");
		}
		if (step.distanceUnits) {
			$("#distanceUnits").val(step.distanceUnits);
		} else {
			$("#distanceUnits").val("");
		}
		if (step.directionRequired) {
			$("#directionRequired").val(step.directionRequired);
		} else {
			$("#directionRequired").val("");
		}
		if (step.directionUnits) {
			$("#directionUnits").val(step.directionUnits);
		} else {
			$("#directionUnits").val("");
		}
		if (step.trailId) {
			$("#trailId").val(step.trailId);
		} else {
			$("#trailId").val("");
		}
		if (step.trailName) {
			$("#trailName").val(step.trailName);
		} else {
			$("#trailName").val("");
		}

		
		if (step.infoText) {
			$("#infoOnCompletion").val(step.infoText);
		} else {
			$("#infoOnCompletion").val("");
		}

		$("#mapObjects").empty();
		if (step.mapObjects) {
			for (x in step.mapObjects) {
				var buttonText = "" + step.mapObjects[x].name;
				$("#mapObjects").append(
						'<button onClick="removeFromMapObjects(this)" value="'
								+ buttonText + '">' + buttonText + '</button>');
			}
		}
		$("#movableMapObjects").empty();
		if (step.movableMapObjects) {
			for (x in step.movableMapObjects) {
				var buttonText = "" + step.movableMapObjects[x].name;
				$("#movableMapObjects").append(
						'<button onClick="removeFromMovableMapObjects(this)" value="'
								+ buttonText + '">' + buttonText + '</button>');
			}
		}
		//$('input[name="stepType"][value="' + step.type + '"]').prop('checked', true);
		//$('input[name="stepType"]').prop('checked',true);

		if (step.type) {
			//$("#stepType").val(step.type);
			$("#stepType").html("" + step.type);

		} else {
			$("#stepType").html("");
		}
		if (step.tool) {
			//$("#stepType").val(step.type);
			$("#toolToUse").html("" + step.tool);

		} else {
			$("#toolToUse").html("");
		}
		if (step.toolAction) {
			//$("#stepType").val(step.type);
			$("#toolAction").html("" + step.toolAction);

		} else {
			$("#toolAction").html("");
		}
		
		/* if(step.type)
		{
			//$('[id^=stepType][value='+ step.type +']').attr("checked","checked");
			//$radios.prop('checked', true);
			var $radios = $('input:radio[name=stepType]');
			$radios.filter('[value=' + step.type +']').attr('checked', true);
		}
		else
		{
			var $radios = $('input:radio[name=stepType]');
			var none = "none";
			$radios.filter('[value=' + none +']').attr('checked', true);
			//$radios.prop('checked', false);
		} */
		//reference
		if (step.reference) {
			drawMapFeature('StarRed.png', step.reference.lon,
					step.reference.lat, 'reference', 30, 30, vectors,true, -1, 'step');
		}
		//target
		if (step.target) {
			drawMapFeature('StarGreen.png', step.target.lon, step.target.lat,
					'target', 30, 30, vectors,true, -1, 'step');
		}
		if (step.symbol && step.symbol == true) {
			document.getElementById("symbolCheck").checked = true;
		} else {
			document.getElementById("symbolCheck").checked = false;
		}
		if (step.distance && step.distance == true) {
			document.getElementById("distanceCheck").checked = true;
		} else {
			document.getElementById("distanceCheck").checked = false;
		}
		if (step.direction && step.direction == true) {
			document.getElementById("directionCheck").checked = true;
		} else {
			document.getElementById("directionCheck").checked = false;
		}

		if (step.infoOnCompletion && step.infoOnCompletion == true) {
			document.getElementById("infoCheck").checked = true;
		} else {
			document.getElementById("infoCheck").checked = false;
		}

		if (step.mustAnswerToProgress && step.mustAnswerToProgress == true) {
			document.getElementById("mustAnswerToProgressCheck").checked = true;
		} else {
			document.getElementById("mustAnswerToProgressCheck").checked = false;
		}

		drawMapFeature('greenMarker.png', scenarioMakerMarker.lon,
				scenarioMakerMarker.lat, 'scenarioMakerMarker', 30, 30, vectors,true,
				-1, 'step');

	}
	/* 	private String symbolNameRequired;
	private Boolean symbol;
	private Boolean distance;
	private Boolean direction;
	<input type="checkbox" name="symbolCheck" value="symbolCheck" onclick="handleClickCheckBox(this);">Symbol<br>
	<input type="checkbox" name="distanceCheck" value="distanceCheck" onclick="handleClickCheckBox(this);">Distance<br>
	<input type="checkbox" name="directionCheck" value="directionCheck" onclick="handleClickCheckBox(this);">Direction<br>
	 */
	/* function handleClickStepType(radio)
	{
		//alert('Radio changed: ' + radio.value);
		//touch or drag.
		scenarioMakerStep.type = radio.value;
	} */

	function testButton() {
		console.info("Button Reg");
	}

	function setStepType(button) {
		scenarioMakerStep.type = button.value;
		displayScenario();
	}
	
	function setToolToUse(button) {
		scenarioMakerStep.tool = button.value;
		displayScenario();
	}
	
	function setToolAction(button) {
		scenarioMakerStep.toolAction = button.value;
		displayScenario();
	}

	function handleClickCheckBox(check) {
		//alert('Radio changed: ' + radio.value);
		//touch or drag.
		if (check.value == 'symbolCheck') {
			scenarioMakerStep.symbol = check.checked;
		} else if (check.value == 'distanceCheck') {
			scenarioMakerStep.distance = check.checked;
		} else if (check.value == 'directionCheck') {
			scenarioMakerStep.direction = check.checked;
		} else if (check.value == 'infoCheck') {
			scenarioMakerStep.infoOnCompletion = check.checked;
		} else if (check.value == 'mustAnswerToProgressCheck') {
			scenarioMakerStep.mustAnswerToProgress = check.checked;
		}

		console.info("Check box:" + check.value + " updated to:"
				+ check.checked);
		//scenarioMakerStep.type = radio.value;
	}

	function drawStep(step) {
		if(step)
		{
			if (step.reference) {
				drawMapFeature('orangeCircle.png', step.reference.lon,
						step.reference.lat, 'reference', 50, 50, symbols,false, -1, 'step');
			}
	
			if (step.mapObjects) {
				drawMapObjects(step.mapObjects, symbols,false, 'step');
			}
			if (step.movableMapObjects) {
				drawMapObjects(step.movableMapObjects, vectors,true, 'step');
			}
		}
	}

	function drawActivity(activity) {
		if(activity)
		{
			if (activity.mapObjects) {
				drawMapObjects(activity.mapObjects, symbols,false, 'activity');
			}
			if (activity.movableMapObjects) {
				drawMapObjects(activity.movableMapObjects, vectors,true, 'activity');
			}		
			if (activity.layers) {
				drawLayers(activity.layers);
			}
		}
	}

	function drawLayers(layers) {
		$("#layersPara").empty();
		layers = layers;
		 
		//TODO clear old layers.
		// removeLayer or call destroy() on the layer...
		if(mapLayerFeature)
		{
			mapLayerFeature.destroy();
		}
		for (x in mapLayers)
		{
			
			mapLayers[x].destroy();
		}
		
		mapLayers.length = 0;
		//TODO create layers.
		for (x in layers)
		{
			$("#layersPara").append('<button onClick="toggleLayer(this)" value="'+layers[x].id+'">'+layers[x].name+'</button>');
			$("#layersPara").append('<br>');
			
			
			//create new layer
			mapLayer = new OpenLayers.Layer.Vector(layers[x].name);
		//	map.addLayers([ mapLayer ]);
			//add to layersOpenLayer
			drawMapObjects(layers[x].mapObjects, mapLayer, false, 'layer');
			mapLayers[layers[x].id] = mapLayer;
				
			//TODO draw everything on to a layer and hide or not hide. 
			// can use drawMapFeature method below.
			map.addLayer(mapLayer);
			if(layers[x].visible && layers[x].visible == true)
			{
				mapLayer.setVisibility(true);
			}
			
			mapLayer.events.on({
				'featureselected' : function(e) {
					console.info("Symbol Feature Selected :"
							+ e.feature.attributes.some);
					var toProjection = new OpenLayers.Projection("EPSG:4326");
					var lonlat = e.feature.geometry.getBounds().getCenterLonLat()
					lonlat = lonlat.transform(map.getProjectionObject(),
							toProjection);
					//var lonlat = map.getLonLatFromPixel(e.xy).transform(map.getProjectionObject(), toProjection);
					var objectName = e.feature.attributes.some;
					var activityOrStep = e.feature.attributes.activityOrStep;
					//var movable = e.feature.attributes.movable;
					var arrayPos = e.feature.attributes.arrayPos;
				
					var screenX = selectFeature.handlers.feature.evt.screenX;
					var screenY = screen.height - selectFeature.handlers.feature.evt.screenY;
					layerObjectSelected(lonlat, objectName,  
							arrayPos, screenX, screenY);
				},
				'featureunselected' : function(e) {
					console.info("Symbol Feature Un Selected :"
							+ e.feature.attributes.some);
				}
			});

			
			
		}
		mapLayerFeature = new OpenLayers.Control.SelectFeature(
				[ mapLayers ], {
					clickout : true
				});

		map.addControl(mapLayerFeature);
		mapLayerFeature.activate();
	
	}
	
	function layerObjectSelected(lonlat, objectName, arrayPos, screenX, screenY)
	{
		console.info("Layer Object Selected:" + objectName + " Array Position:"	+ arrayPos);
	}
	
	function drawMapFeature(image, lon, lat, name, imageHeight, imageWidth,
			layer, movable, arrayPos, activityOrStep) {
		var imglink = "/namshub/resources/img/symbols/" + image;

		var feature = new OpenLayers.Feature.Vector(
				new OpenLayers.Geometry.Point(lon, lat).transform(
						new OpenLayers.Projection("EPSG:4326"), map
								.getProjectionObject()), {
					some : name,
					activityOrStep : activityOrStep,
					movable : movable,
					arrayPos : arrayPos
				}, {
					externalGraphic : imglink,
					graphicHeight : imageHeight,
					graphicWidth : imageWidth
				});
		layer.addFeatures(feature);
		/* if (movable == true) {
			vectors.addFeatures(feature);
		} else {
			symbols.addFeatures(feature);
		} */
	}

	function drawMapObject(mapObject, layer, movable, arrayPos, activityOrStep) {
		console.info("Drawing Map Object:" + mapObject.name + " It is for: "
				+ activityOrStep + ". Layer:" + layer.name + " Array Position:"
				+ arrayPos);
	
			drawMapFeature(mapObject.image, mapObject.lonLat.lon,
					mapObject.lonLat.lat, mapObject.name, mapObject.imageHeight,
					mapObject.imageWidth, layer, movable, arrayPos, activityOrStep);
		
		
	}

	function drawMapObjects(mapObjects, layer, movable, activityOrStep) {
		for (x in mapObjects) {
			//Draw objects on screen. 
			drawMapObject(mapObjects[x], layer, movable, x, activityOrStep);

		}
	}

	function poll() {
		setTimeout(function() {
			$.ajax({
				url : "http://localhost:8080/namshub/mapPoll",
				success : function(mapUpdate) {
					//console.info("Poll Response:" + mapUpdate.clue);
					doMapUpdate(mapUpdate);
					if (continueToPoll) {
						poll();
					}
				},
				dataType : "json"
			});
		}, 500);
	}

	function toggleMapKey(e) {
	
		var screenX = e.screenX;
		var screenY = screen.height - e.screenY;
		
		if (document.getElementById('map_key').style.visibility == 'hidden') {
			enableMapKey(true,screenX,screenY);
		} else {
			disableMapKey(true,screenX,screenY);
		}
	}
	
	function toggleScrapBook(e) {
		
		var screenX = e.screenX;
		var screenY = screen.height - e.screenY;
		
		if (document.getElementById('scrapBookDiv').style.visibility == 'hidden') {
			enableScrapBook(true,screenX,screenY);
		} else {
			disableScrapBook(true,screenX,screenY);
		}
	}
	function enableScrapBook(userSelected,screenX,screenY) {
		document.getElementById('scrapBookDiv').style.visibility = 'visible';
		if(userSelected)
		{
			logOnServer("Map Key Shown");
			sendCheckLonLat(null, '', false, true, 'scrapBook','show',false,false,screenX,screenY);
		}
	}
	function disableScrapBook(userSelected,screenX,screenY) {
		document.getElementById('scrapBookDiv').style.visibility = 'hidden';
		if(userSelected)
		{
			logOnServer("Map Key Shown");
			sendCheckLonLat(null, '', false, true, 'scrapBook','hide',false,false,screenX,screenY);
		}
	}
	
	function highlighMapKey() {
		$("#keyImage").attr('src',
				'/namshub/resources/img/tool_key_highlight.png');
	}
	function enableMapKey(userSelected,screenX,screenY) {
		$("#keyImage").attr('src',
				'/namshub/resources/img/tool_key_pressed.png');
		document.getElementById('map_key').style.visibility = 'visible';
		keyOpened = new Date();
		if(userSelected)
		{
			logOnServer("Map Key Shown");
			sendCheckLonLat(null, '', false, true, 'mapKey','show',false,false,screenX,screenY);
		}
	}
	function disableMapKey(userSelected,screenX,screenY) {
		$("#keyImage").attr('src', '/namshub/resources/img/tool_key.png');
		document.getElementById('map_key').style.visibility = 'hidden';
		keyClosed = new Date();
		logPeriodOnServer("Map Key Hidden", keyOpened, keyClosed);
		if(userSelected)
		{
			logOnServer("Map Key Hidden");
			sendCheckLonLat(null, '', false, true, 'mapKey','hide',false,false,screenX,screenY);
		}
	}

	function highlightCompass(userSelected,screenX,screenY) {
		$("#compassImage").attr('src',
				'/namshub/resources/img/tool_compass_highlight.png');	
	}
	
	function enableCompass(userSelected,screenX,screenY) {
		$("#compassImage").attr('src',
				'/namshub/resources/img/tool_compass_pressed.png');
		var compassPosition = map.getCenter();
		compassfeature.move(compassPosition);
		/* new OpenLayers.Geometry.Point(
				compassPosition.lon,
				compassPosition.lat)
				.transform(new OpenLayers.Projection(
						"EPSG:4326"), map
						.getProjectionObject())
		compassFeature.geometry. */
		compassLayer.setVisibility(true);
		compassLayer.redraw();
		compassLayer.refresh({
			force : true
		});
		compassOpened = new Date();

		if(userSelected)
		{
			logOnServer("Compass Shown");
			sendCheckLonLat(null, '', false, true, 'compass','show',false,false,screenX,screenY);
		}
	}

	function disableCompass(userSelected,screenX,screenY) {
		compassClosed = new Date();
		$("#compassImage").attr('src',
				'/namshub/resources/img/tool_compass.png');
		compassLayer.setVisibility(false);
		compassClosed = new Date();
		if(userSelected)
		{
			logOnServer("Compass Hidden");
			sendCheckLonLat(null, '', false, true, 'compass','hide',false,false,screenX,screenY);
		}
		logPeriodOnServer("Compass Hidden", compassOpened, compassClosed);
	}

	function toggleCompass(e) {
		var screenX = e.screenX;
		var screenY = screen.height - e.screenY;
		if (compassLayer.visibility) {
			disableCompass(true,screenX,screenY);
		} else {
			enableCompass(true,screenX,screenY);
		}
	}
	
	function toggleLayer(e) {
		var screenX = e.screenX;
		var screenY = screen.height - e.screenY;
		
		
		
		if (mapLayers[e.value].visibility) {
			disableLayer(mapLayers[e.value],true,screenX,screenY);
		} else {
			enableLayer(mapLayers[e.value],true,screenX,screenY);
		}
	}
	
	function enableLayer(layer,userSelected,screenX,screenY) {
	
		layer.setVisibility(true);
		layer.redraw();
		layer.refresh({
			force : true
		});
	
	}

	function disableLayer(layer,userSelected,screenX,screenY) {
		layer.setVisibility(false);
	}

	function disableDistanceTool(userSelected,screenX,screenY) {
		$("#distanceImage").attr('src',
				'/namshub/resources/img/tool_distance.png');
		distanceClosed = new Date();
		if(userSelected)
		{
			logOnServer("Distance Closed");
			sendCheckLonLat(null, '', false, true, 'distance','hide',false,false,screenX,screenY);
		}
		logPeriodOnServer("Distance Closed", distanceOpened, distanceClosed);

		for (key in measureControls) {
			var control = measureControls[key];
			if ('none' == key) {
				control.activate(true);
			} else {
				control.deactivate(true);
			}
		}
		distanceTool = false;
		
	}
	function highlightDistanceTool(userSelected,screenX,screenY) {
		$("#distanceImage").attr('src',
				'/namshub/resources/img/tool_distance_highlight.png');
	}
	function enableDistanceTool(userSelected,screenX,screenY) {
		$("#distanceImage").attr('src',
				'/namshub/resources/img/tool_distance_pressed.png');
		distanceOpened = new Date();

		if(userSelected)
			{
			logOnServer("Distance Shown");
			sendCheckLonLat(null, '', false, true, 'distance','show',false,false,screenX,screenY);
			}
	
		for (key in measureControls) {
			var control = measureControls[key];
			if ('line' == key) {
				control.activate();
			} else {
				control.deactivate();
			}
		}
		distanceTool = true;
	}

	function toggleDistance(e) {
		var screenX = e.screenX;
		var screenY = screen.height - e.screenY;
		if (distanceTool) {
			disableDistanceTool(true,screenX,screenY);
		} else {
			enableDistanceTool(true,screenX,screenY);
		}
	}
	function toggleMeasureControl(element) {
		for (key in measureControls) {
			var control = measureControls[key];
			if (element.value == key && element.checked) {
				control.activate();
			} else {
				control.deactivate();
			}
		}
	}

	function playVideo() {
		var myVideo = document.getElementsByTagName('video')[0];
		myVideo.play();
	}

	function toggleScenarioMakerDiv() {
		if (scenarioMakerDiv) {
			disableScenarioMakerDiv();
		} else {
			enableScenarioMakerDiv();
		}
	}

	function disableScenarioMakerDiv() {

		scenarioMakerDiv = false;
		document.getElementById('scenarioMakerDiv').style.visibility = 'hidden';
	}
	function enableScenarioMakerDiv() {

		scenarioMakerDiv = true;
		document.getElementById('scenarioMakerDiv').style.visibility = 'visible';
	}

	function getAllScenarios() {
		$.get("getAllScenarios", function(data) {
			document.getElementById('scenarios').style.visibility = 'visible';
			$("#scenarios").empty();
			$("#scenarios").append('Please hit buttons to set the scenario.');
			for (x in data) {
				$("#scenarios").append(
						'<button onClick="setScenario(this)" value="' + data[x]
								+ '">' + data[x] + '</button> ');
			}

		});
	}
	

	function setScenario(button) {
		console.info("Scenario Setting to:" + button.value);
		var scenName = button.value;
		$.get("getOrCreateScenario", {
			scenarioName : scenName
		}, function(scenarioData) {
			scenarioMakerStep = null;
			scenario = scenarioData;
			stepId = 0;
			activityId = 0;
			$("#stepId").text(stepId);
			$("#activityId").text(activityId);
			displayScenario();

		});

	}

	function handleMeasurements(event) {
		var geometry = event.geometry;
		var units = event.units;
		var order = event.order;
		var measure = event.measure;
		
		//var mouseEvt = event.object.handler.evt;
	//	if(mouseEvt.mouseDown)
		//if(typeof mouseEvt.mouseDown != 'undefined')	
		/* if(event.object.handler.evt.type == 'mouseDown')
		{
			sendCheckLonLat(null, '', false, true, 'distance','start',false);
		} */
		/* var element = document.getElementById('output');
		var out = "";
		if (order == 1) {
			out += "Distance: " + measure.toFixed(3) + " " + units;
		} else {
			out += "Distance: " + measure.toFixed(3) + " " + units
					+ "<sup>2</" + "sup>";
		}
		element.innerHTML = out; */
	}

	function toggleDeleteMode() {
		if (deleteMode) {
			deleteMode = false;
			document.getElementById("deleteModeButton").innerHTML = "Delete Mode On (Currently Off)";
			//$("#deleteModeButton").text("Delete Mode On (Currently Off)");
		} else {
			deleteMode = true;
			document.getElementById("deleteModeButton").innerHTML = "Delete Mode Off (Currently On)";
			//$("#deleteModeButton").text("Delete Mode Off (Currently On)");
		}
	}
	
	function skip(){
		screenX = 10;
		screenY = 10;
		sendCheckLonLat(null, '', false, false, '','', true,true,screenX,screenY);
	}
	
</script>

</head>
<body onload="init()">

	<div id="map" class="smallmap"></div>

	<div id="text">
		 
		<!--<p id="activity">Activity Name</p> -->
		<h1 id="step"></h1>
		<p id="result"></p>
	</div>

	<div id="questionnaire"></div>


<div id="newOLM">
     <h3> <div id="meterText"> Compass </div>   
         <div id="newOLMMeter">
          	   <div id="newOLMMeterLine"></div>
				<div id="newOLMCompassMeter">
				</div>
             <div id="newOLMCompassOldValue">
				    </div> 
		    </div>
    </h3>
    <div>
        <p id="newCompassExplanation"></p>
    </div>
     <h3><div id="meterText"> Distance </div>   
         <div id="newOLMMeter">
          	   <div id="newOLMMeterLine"></div>
				<div id="newOLMDistanceMeter">
				</div>
             <div id="newOLMDistanceOldValue">
				    </div> 
		    </div></h3>

    <div>
       <p id="newDistanceExplanation"></p>
       </div>
     <h3><div id="meterText"> Symbol </div>   
         <div id="newOLMMeter">
          	   <div id="newOLMMeterLine"></div>
				<div id="newOLMSymbolMeter">
				</div>
             <div id="newOLMSymbolOldValue">
				    </div> 
		    </div></h3>

    <div>
        <p id="newSymbolExplanation"></p>

    </div>
    
</div>

	<div id="olm">
		<h1>Skill Meters</h1>
		<div align="center">
			<table border="0" width="80%"
				style="color: White; vertical-align: middle; font-size:xx-large"  >
				<!-- <tr>
					<td height=100 width="20%">Map Symbols</td>
					<td><meter id="olm1" value="0.5" low=".3" high=".8" optimum="1" width="100%">OLM Element 1</meter></td>
				</tr>
				<tr>
					<td height=100 width="20%">Distance</td>
					<td><meter id="olm2" value="0.5" low=".3" high=".8" optimum="1" width="100%">OLM Element 2</meter></td>
				</tr>
				<tr>
					<td height=100 width="20%">Compass</td>
					<td><meter id="olm3" value="0.5" low=".3" high=".8" optimum="1" width="100%">OLM Element 2</meter></td>
				</tr> -->
				<tr style="color: White; vertical-align: middle">
					<th>Skill</th>
					<th>Skill Meter</th>
					<th id='explanationHeader'>Explanation</th>
				</tr>
				<tr style="color: White; vertical-align: middle">
					<td height=100 width="20%">Distance</td>
					<td><div id="meter">
							<div id="distancemeter"></div>
						</div></td>
					<td style="color: White; vertical-align: middle" 
						id='distanceexplanation'></td>
				</tr>
				<tr style="color: White; vertical-align: middle">
					<td height=100 width="20%">Direction</td>
					<td><div id="meter">
							<div id="compassmeter"></div>
						</div></td>
					<td style="color: White; vertical-align: middle" 
						id='compassexplanation'></td>
				</tr>
				<tr style="color: White; vertical-align: middle">
					<td height=100 width="20%">Map Symbols</td>
					<td width="400px"><div id="meter">
							<div id="symbolmeter"></div>
						</div></td>
					<td style="color: White; vertical-align: middle" 
						id='symbolexplanation'></td>
				</tr>

			</table>
		</div>
		<!-- <h1>Explanation</h1>
		<p id="learnerModelMessage"></p>
		 -->
		<div>
			<button style="background-color: transparent; border: none"
				onClick="hideOLM()" name="hideOLM">
				<img src='<c:url value="/resources/img/CLOSE.png" />' alt="Close">
			</button>
		</div>

	</div>

	<div id="map_key">
		<h1>Map Key</h1>
		<table border="0" style="color: White">
			<tr>
				<td>Building Of <br>Historical Interest</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/BuildingOfHistoricalInterest.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			
				<td>Bus Station</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/BusStation.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>Camp Site</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/CampSite.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			
				<td>Caravan Site</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/CaravanSite.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>Castle</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/Castle.png" />'
					alt="Caravan Site" width="35" height="35"></td>
		
				<td>English Heritage</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/EnglishHeritage.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>Garden</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/Garden.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			
				<td>Golf Course</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/GolfCourse.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>Information Centre</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/InformationCentre.png" />'
					alt="Caravan Site" width="35" height="35"></td>
		
				<td>Information Point</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/InformationPoint.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>Museum</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/Museum.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			
				<td>Nature Reserve</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/NatureReserve.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>Parking</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/Parking.png" />'
					alt="Caravan Site" width="35" height="35"></td>
		
				<td>Picnic Site</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/PicnicSite.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>Place Of Worship</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/PlaceOfWorship.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			
				<td>RailwayStation</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/RailwayStation.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>Sports Centre</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/SportsCentre.png" />'
					alt="Caravan Site" width="35" height="35"></td>
		
				<td>Telephone</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/Telephone.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
			<tr>
				<td>WindGenerator</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/WindGenerator.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			
				<td>Youth Hostel</td>
				<td style="background-color:white"><img
					src='<c:url value="/resources/img/symbols/YouthHostel.png" />'
					alt="Caravan Site" width="35" height="35"></td>
			</tr>
		
			
			
			
		</table>

	</div>

	<div id="controls">
		<table border="0"
			style="color: White; vertical-align: middle; text-align: center">
			<tr>
				<td><button style="background-color: transparent; border: none"
						onClick="toggleMapKey(event)" name="ToggleMapKey" id="ToggleMapKey">
						<img id="keyImage"
							src='<c:url value="/resources/img/tool_key.png" />' alt="Map Key">
					</button></td>
				<td><button style="background-color: transparent; border: none"
						onClick="toggleCompass(event)" name="ToggleCompass" id="ToggleCompass">
						<img id="compassImage"
							src='<c:url value="/resources/img/tool_compass.png" />'
							alt="Compass Tool">
					</button></td>
				<td><button style="background-color: transparent; border: none"
						onClick="toggleDistance(event)" name="ToggleDistance" id="ToggleDistance">
						<img id="distanceImage"
							src='<c:url value="/resources/img/tool_distance.png" />'
							alt="Measure Tool">
					</button></td>
				 	<%-- <td><button style="background-color: transparent; border: none"
						onClick="toggleScrapBook(event)" name="ToggleScrapBook" id="ToggleScrapBook">
						<img id="scrapBookImage"
							src='<c:url value="/resources/img/tool_scrap_book.png" />'
							alt="Scrap Book Tool">
					</button></td>  --%>
				<%-- 	<td><button style="background-color: transparent; border: none"
						onClick="help(event)" name="Help" id="Help">
						<img id="helpImage"
							src='<c:url value="/resources/img/tool_help.png" />'
							alt="Measure Tool">
					</button></td> --%>
					<%--  <td><button style="background-color: transparent; border: none"
						onClick="skip(event)" name="Skip" id="Skip">
						<img id="skipImage"
							src='<c:url value="/resources/img/tool_skip.png" />'
							alt="Skip">
					</button></td>  --%>
			</tr>
			<tr>
				<td>Map Key</td>
				<td>Compass</td>
				<td>Distance Tool</td>
				<!-- <td>Scrap Book</td> -->
				<!-- <td>Help</td> -->
				 <!-- <td>Skip</td> --> 
			</tr>
		</table>

		<!-- <div id="output"></div> -->
	</div>
	<div id="audio"></div>

	<div id="videoDiv">
		<video width="320" height="240">
			<source src='<c:url value="/resources/animations/olmwelcome.mp4" /> '
				type="video/mp4">
			Your browser does not support the video tag.
		
		
		</video>
	</div>
	<div id="scenarioMakerDivControl">
		<button onClick="toggleScenarioMakerDiv()">Toggle Scenario
			Maker Div</button>
	</div>
	<div id="scenarioMakerDiv">
		<h1>Scenario Maker</h1>
		Scenario Name:<input type="text" name="scenarioName" id="scenarioName">
		<button onClick="getOrCreateScenario()" name="getOrCreateScenario">Get
			or Create Scenario</button>
		<br>
		<div id="scenarios"></div>
		Navigation<br>
		<button onClick="prevScenarioStep()" name="prevScenarioStep">Prev
			ScenarioStep</button>
		<button onClick="nextScenarioStep()" name="nextScenarioStep">Next
			ScenarioStep</button>
		
		<br>
		
		<button onClick="insertStepAtThisPosition()" name="insertStepAtThisPosition">Instert Step</button>
		<br>
		<button onClick="submitScenarioStep()" name="submitScenarioStep">Submit
			ScenarioStep</button>
		<br>
		<!-- Activity ID:<p id="activityId"></p><br>
		Activity Name:<input type="text" name="activiyName" id="activiyName"><br> -->
		Trail:
		ID:<input type="text" name="trailId" id="trailId"> Name:<input type="text" name="trailName" id="trailName"><br>
		Step ID:
		<div id="stepId"></div>
		<br> Step on screen text:<input type="text" name="stepName" id="stepName"><br>
		Step speech:<input type="text" name="stepSpeech" id="stepSpeech"><br>
		<input type="checkbox" id="symbolCheck" name="symbolCheck"
			value="symbolCheck" onclick="handleClickCheckBox(this);">Symbol.
		Symbol Name Required:<input type="text" name="symbolNameRequired"
			id="symbolNameRequired"><br> <input type="checkbox"
			id="distanceCheck" name="distanceCheck" value="distanceCheck"
			onclick="handleClickCheckBox(this);">Distance. Distance:<input
			type="text" name="distanceRequired" id="distanceRequired"> units:<input type="text" name="distanceUnits" id="distanceUnits"><br>
		<input type="checkbox" id="directionCheck" name="directionCheck"
			value="directionCheck" onclick="handleClickCheckBox(this);">Direction.
		Direction:<input type="text" name="directionRequired"
			id="directionRequired"><br> Units:<input type="text" name="directionUnits"
			id="directionUnits"><br> 
			<input type="checkbox"
			id="infoCheck" name="infoCheck" value="infoCheck"
			onclick="handleClickCheckBox(this);">Info. Info On
		Completion:<input type="text" name="infoOnCompletion"
			id="infoOnCompletion"><br> AnswerToProgress:<input
			type="checkbox" id="mustAnswerToProgressCheck"
			name="mustAnswerToProgressCheck" value="mustAnswerToProgressCheck"
			onclick="handleClickCheckBox(this);">Must Answer To Progress<br>


		<button onClick="addReference()" name="addReference">Add
			Reference (not needed, but only checks agains lat and long)</button>
		<button onClick="addStarAtReference()" name="addStarAtReference">Add
			Star at Reference</button>
		<br>
		<button onClick="addTarget()" name="addTarget">Add Target(can
			place anywhere by dragging)</button>
		<br> Symbols To Add:
		<div id="symbolsToAdd"></div>
		<br> Symbol To Add:
		<div id="symbolToAdd"></div>
		<br> Symbol Size:<input type="text" name="symbolSize"
			id="symbolSize">
		<button onClick="setSymbolSize()" name="setSymbolSize">Set
			Symbol Size</button>
		<br>
		<button onClick="addSymbolAtTarget()" name="addSymbolAtTarget">Add
			Symbol At Target</button>
		<button onClick="addSymbolAtMarker(this)" value="static">Add
			Symbol At Marker For Step</button>
		<button onClick="addSymbolAtMarker(this)" value="movable">Add
			Movable Symbol At Marker For Step</button>
		<button onClick="addSymbolAtMarkerForActivity()"
			name="addSymbolAtMarkerForActivity">Add Symbol At Marker For
			Activity</button>
		<br>

		<button onClick="addUpdateTargetFromRequirements()" name="addTarget">Add
			or Update Target Based on Distance and Direction</button>
		<br> Step type:
		<div id="stepType"></div>
		<button onClick="setStepType(this)" value="click">Click</button>
		<button onClick="setStepType(this)" value="drag">Drag</button>
		<button onClick="setStepType(this)" value="toolUse">Tool Use</button>
		<button onClick="setStepType(this)" value="noWayToAnswer">No Way To Answer</button>
		<br>

		<div id="toolToUse"></div>
		<button onClick="setToolToUse(this)" value="mapKey">mapKey</button>
		<button onClick="setToolToUse(this)" value="compass">compass</button>
		<button onClick="setToolToUse(this)" value="distance">distance</button>
		<br>
		
		<div id="toolAction"></div>
		<button onClick="setToolAction(this)" value="show">show</button>
		<button onClick="setToolAction(this)" value="hide">hide</button>
		<button onClick="setToolAction(this)" value="reset">reset</button>
		<button onClick="setToolAction(this)" value="start">start</button>
		<button onClick="setToolAction(this)" value="end">end</button>
		<br>

		<!-- <button onClick="addSprite()" name="addSprite">Add Sprite(drag to place)</button><br> -->
		<button onClick="showScaffoling()" name="showScaffoling">Show
			Scaffoling</button>
		<br>
		<button onClick="updateStepName()" name="updateStepName">Update
			Step Name</button>
		<br> Map Objects
		<div id="mapObjects"></div>
		Movable Map Objects
		<div id="movableMapObjects"></div>
		Remove Ref/Target:
		<button onClick="removeReference()" name="removeReference">Remove
			Reference</button>
		<button onClick="removeTarget()" name="removeTarget">Remove
			Target</button>
		<br> Submit:
		<button onClick="submitScenarioStep()" name="submitScenarioStep">Submit
			ScenarioStep</button>
		<br> Lat:<input type="text" name="lat" id="lat">Lon:<input
			type="text" name="lon" id="lon"><br> Set Centre:
		<button onClick="setCentre()" name="setCentre">Set Centre To
			Current Centre</button>
		<button onClick="setCentreText()" name="setCentreText">Set
			Centre To Text Values</button>
		<br>
		<button onClick="setCentreToLocation(this)" value="52.19123"
			name="-2.22231">Worcester</button>
		<button onClick="setCentreToLocation(this)" value="53.958333"
			name="-1.080278">York</button>
		<button onClick="setCentreToLocation(this)" value="51.45"
			name="-2.583333">Bristol</button>
		<button onClick="setCentreToLocation(this)" value="52.423781"
			name="-2.147318">Haybridge</button>
		<br>
		<button id="deleteModeButton" onClick="toggleDeleteMode()"
			name="deleteModeButton">Delete Mode On (Currently Off)</button>
			<br>
		<button onClick="removeStepAtThisPosition()" name="removeStepAtThisPosition">Remove Step</button>
		
	</div>

	<div id="infoDiv">
		
		<h1>You have found some information!</h1>
		<p id="infoPara"></p>
		<p>It has been added to the scrap book, you can look at it at any time.<br>Press the cross to close this window.</p>
		<br>
		<div align="center">
			<br>
			<button style="background-color: transparent; border: none"
				onClick="closeInfo()" name="closeInfo">
				<img src='<c:url value="/resources/img/CLOSE.png" />' alt="Close">
			</button>
			&nbsp;
		</div>

	</div>
	
	<div id="scrapBookDiv">

		<h1>Scrap Book</h1>
		<p id="scrapBookPara"></p>
	
		<br>

	</div>
	
	<div id="activityMenuDiv">
		<h1 id="scenario">Scenario Name</h1>
		<h1>Activity Menu</h1>
		<p id="activityMenuPara"></p>

	</div>

	<div id="layerDiv">
		<h1>Layer</h1>
		<p id="layersPara"></p>
	</div>

</body>
</html>
