import naoqi
from naoqi import ALBroker
from naoqi import ALModule
from naoqi import ALProxy
import random
from collections import deque
import ConfigParser
from threading import Thread
import thread
from threading import Lock
import time
import Queue
from datetime import datetime

instance = None
mutex = Lock()


def log(text):
    instance.nao.log("BehaviorModule: " + text)
    return

class BehaviorModule(object):
    nao = None
    proxy = None
    stopProxy = None
    goingIdle = False

    bShutdown = False
    _autoIdle = False
    #_resetBehavior = "resetPose"
    _resetBehavior = ""
    _shutdownBehavior = "endPose"
    _idleBehavior = "idle"
    _basePoseBehavior = "basePose"

    def __init__(self, naoServer):
        global instance
        instance = self
        self.nao = naoServer
        return

    def start(self):
        log("Connecting BehaviorManagerProxy...")
        try:

            #global myBroker
            #myBroker = ALBroker("behaviorBroker", "0.0.0.0", 0, self.nao.brokerIp(), self.nao.brokerPort())

            self.proxy = ALProxy("ALBehaviorManager")
            self.stopProxy = ALProxy("ALBehaviorManager")
            log(self.proxy.version())

            self._currentBehavior = ""
            self._nextBehavior = ""
            self._currentBehaviorId = None
            self._nextBehaviorId = ""
            self._stopBehavior = False
            self._idle = False

            self.runningBehaviors = []

            
            self._behaviors = self.proxy.getInstalledBehaviors()
            print "DEBUG>> loaded installed behaviors: "+str(len(self._behaviors))

            counter = 0
            for b in self._behaviors:
                counter = counter + 1
                print "DEBUG>> Preloading: "+str(counter)+"/"+str(len(self._behaviors))+" "+str(b)
                self.proxy.preloadBehavior(b)
            print "DEBUG>> Behaviors preloaded"

            self.nao.memory.proxy.subscribeToEvent("BehaviorsRun","BehaviorWatcher","onBehaviorRun")
            print "DEBUG>> Subscrived to event"

            if (self._resetBehavior in self._behaviors):
                self._nextBehavior = self._resetBehavior

            th = Thread(target=self.behaviorCycle)
            th.start()
            print "DEBUG>> Started thread behavior cycle"

            stopth = Thread(target=self.stopCycle)
            stopth.start()
            print "DEBUG>> Started thread stop cycle"
            self.nao.NotifyAnimationList(self._behaviors)

        except Exception, Argument:
            log("Failed: " + str(Argument))
            self.proxy = None
            return False
        return True

    def ExecuteAction(self, action):
        return

    def dispose(self):
        log("Disconnect BehaviorManagerProxy...")
        self.bShutdown = True

        if (self.proxy != None and not self.runningBehaviors == None):
            try:
                log("Stopping behaviors...")
                for behavior in self.runningBehaviors:
                    if (self.stopProxy.isBehaviorRunning(behavior)):
                        self.stopProxy.stopBehavior(behavior)
                        log("Behaviors stopped.")
            except Exception, Argument:
                log("Failed: " + str(Argument))

            try:
                if(self._shutdownBehavior!=""):
                    log("Running shutdown behavior...")
                    self.proxy.runBehavior(self._shutdownBehavior)
                    log("Shutdown behavior finished.")
            except Exception, Argument:
                log("Failed: " + str(Argument))
        try:
            self.nao.memory.proxy.unsubscribeToEvent("BehaviorsRun","BehaviorWatcher")
        except Exception, Argument:
            log("Failed: " + str(Argument))
        
        return

    def switchIdle(self, state):
        log("switchIdle: " + str(state))
        try:
            if (self.stopProxy.isBehaviorRunning("idle")):
                self.stopProxy.stopBehavior("idle")
        except Exception, Argument:
            pass
        self._autoIdle = state

    def setState(self, state):
        log("SetState: " + self._state + " -> " + state + " (" + str(self.runningBehaviors) + ")")
        self._state = state

    def behaviorCycle(self):
        lastIdle = datetime.now()
        self.goingIdle = False
        log("Running behavior cycle")
        while(not self.bShutdown):
            #newRunning = self.proxy.getRunningBehaviors()
            #if (set(newRunning) != set(self.runningBehaviors)):
            #    print str(self.runningBehaviors) + "->" + str(newRunning)
            #    self.runningBehaviors = newRunning

            running = self.runningBehaviors
            try:
                #log("running: " + str(running) + ", next: " + self._nextBehavior)
                if (running==[]):
                    #log("bc 1")
                    if (self._currentBehaviorId != None):
                        #log("bc 2")
                        self.nao.NotifyAnimationFinished(self._currentBehaviorId)
                        self._currentBehaviorId = None
                        if (not self._autoIdle):
                            #log("bc 3")
                            self.proxy.post.runBehavior(self._basePoseBehavior)

                    if (self._nextBehavior != ""):
                        #log("bc 4")
                        if (not self.goingIdle):
                            #log("bc 5")
                            log("go " + self._nextBehavior)
                            self._currentBehavior = self._nextBehavior
                            self._currentBehaviorId = self._nextBehaviorId
                            self._nextBehavior = ""
                        
                            try:
                                #log("bc 6")
                                self.proxy.post.runBehavior(self._currentBehavior)
                                #log("bc 7")
                                #self.proxy.runBehavior(self._currentBehavior)
                            except Exception, Argument:
                                self._currentBehavior = ""
                                self._nextBehavior = ""
                                if (not Argument == None): log ("Behavior Cycle Exception: "+str(Argument))
                                else: log ("Generic Behavior Cycle Exception")
                                #pass
                        
                    else:
                        #log("bc 8")
                        if self._autoIdle:
                            #log("bc 9")
                            if (not self._idle):
                                #log("bc 10")
                                #log("go idle")
                                lastIdle = datetime.now()
                                try:
                                    self.proxy.post.runBehavior(self._idleBehavior)
                                    self.goingIdle = True
                                    self._idle = True
                                except:
                                    pass
                            else:
                                if (not self.goingIdle):
                                    self._idle = False
                                else:
                                    if ((datetime.now()-lastIdle).total_seconds()>5.0):
                                        log("idle timeout!")
                                        lastIdle = datetime.now()
                                        self.PlayBehavior("","idle")
                else:
                    #log("bc 11")
                    self.goingIdle = False
                    if (self._currentBehavior != ""):
                        self.nao.NotifyAnimationStarted(self._currentBehaviorId)
                        self._currentBehavior = ""
                    if (set(running)!=set(['idle'])):
                        self._idle = False
                    else:
                        if (not self.goingIdle and (datetime.now()-lastIdle).total_seconds()>5.0):
                            log("idle timeout!")
                            lastIdle = datetime.now()
                            self.PlayBehavior("","idle")
                            self.goingIdle = True

            except Exception, Argument:
                if (not Argument == None): log("Behavior Cycle Exception: " + str(Argument))
                else: log("Behavior Cycle Exception.")
            time.sleep(0.1)
            #log("behaviorcycle");
        log("Exiting behavior cycle")
        return

    def stopCycle(self):
        log("Running stop cycle")
        while(not self.bShutdown):
            try:
                if (self._stopBehavior):
                    log("Running: " + str(self.runningBehaviors))
                    log("Stopping behaviors...")
                    #self.proxy.stopAllBehaviors()
                    #try:
                    #    self.proxy.stopBehavior("idle")
                    #except Exception, Argument:
                    #    pass
                    if (not self.runningBehaviors==[]):
                        for behavior in self.runningBehaviors:
                            try:
                                if (self.stopProxy.isBehaviorRunning(behavior)):
                                    self.stopProxy.stopBehavior(behavior)
                            except Exception, Argument:
                                pass
                    self.goingIdle = False
                    self._stopBehavior = False
                    log("Behaviors stopped.")
            except Exception, Argument:
                if (not Argument == None): log("Stop Cycle Exception: " + str(Argument))
                else: log("Stop Cycle Exception.")
            time.sleep(0.1)
            #log("stopcycle");
        log("Exiting stop cycle")
        return

    def ResetPose(self):
        if (self.proxy == None): return False
        try:
            self.PlayBehavior("", self._resetBehavior)
        except Exception, Argument:
            log("ResetPose Failed: " + str(Argument))
            return False
        return True

    def PlayBehavior(self, s_id, s_behavior, b_queue = False):
        if (self.proxy == None): return False
        try:
            if (s_behavior in self._behaviors):
                self._nextBehavior = s_behavior
                self._nextBehaviorId = s_id
                self._stopBehavior = not b_queue
                if (b_queue):
                    log("Queued " + self._nextBehavior)
                else:
                    log("Going to play animation " + self._nextBehavior)
            else:
                log("Unknown behavior: " + s_behavior)
            return
        except Exception, Argument:
            log("PlayBehavior Failed: " + str(Argument))
            return False
        return True

class BehaviorWatcherModule(ALModule):
    """ A simple module able to react
    to behaviorrun events

    """
    def onBehaviorRun(self, *_args):
        """ This will be called each time a the running behavior list is changed.
        """
        global instance
        mutex.acquire()
        try:
            if (instance != None and instance.nao != None):
                print str(instance.runningBehaviors) + "->" + str(_args[1])
                instance.runningBehaviors = _args[1]
        finally:
            mutex.release()

