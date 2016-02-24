using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Thalamus;

namespace Skene
{
    public class IdleManager
    {
        SkeneClient Client;
        Thread idleThread;
        Stopwatch idleTimer;
        bool shutdown = false;
        bool idleState = false;
        public bool IdleState
        {
            get { return idleState; }
            set
            {
                idleState = value;
                if (!idleState) 
                {
                    Client.SkPublisher.StopAnimation(currentAnimationId);
                    currentAnimationId = "";
                    requestedAnimationPlayId = "";
                    queuedAnimationId = "";
                }
            }
        }

        string currentAnimationId = "";
        string requestedAnimationPlayId = "";
        string queuedAnimationId = "";

        int Counter = 0;

        public IdleManager(SkeneClient client)
        {
            this.Client = client;
            idleTimer = new Stopwatch();
            idleThread = new Thread(new ThreadStart(IdleThread));
            idleThread.Start();
            idleState = false;
        }

        public void Dispose()
        {
            idleState = false;
            if (currentAnimationId != "") Client.SkPublisher.StopAnimation(currentAnimationId);
            shutdown = true;
            if (idleThread!=null) idleThread.Abort();
        }

        private string GenerateId()
        {
            return "SkeneAnimation" + Counter++;
        }

        private string GetIdleAnimation()
        {
            return "idle";
        }

        private void IdleThread()
        {
            while (!shutdown)
            {
                if (idleState && Client.IsConnected && Client.RunningAnimations.Count == 0 && Client.RequestedAnimations.Count == 0)
                {
                    try
                    {
                        if (currentAnimationId == "" && requestedAnimationPlayId == "")
                        {
                            requestedAnimationPlayId = GenerateId();
                            Client.SkPublisher.PlayAnimation(requestedAnimationPlayId, GetIdleAnimation());
                        }
                    }
                    catch (Exception e)
                    {
                        Client.DebugException(e);
                    }
                }
                Thread.Sleep(50);
            }
            Client.Debug("Exited IdleThread");
        }

        public void AnimationStarted(string id)
        {
            currentAnimationId = id;
            if (requestedAnimationPlayId == id && queuedAnimationId == "")
            {
                requestedAnimationPlayId = "";
                queuedAnimationId = GenerateId();
                Client.SkPublisher.PlayAnimationQueued(queuedAnimationId, GetIdleAnimation());
            }
        }

        public void AnimationFinished(string id)
        {
            if (id == currentAnimationId && queuedAnimationId != "")
            {
                requestedAnimationPlayId = queuedAnimationId;
            }
            else
            {
                requestedAnimationPlayId = "";
            }
            queuedAnimationId = "";
            currentAnimationId = "";
        }
    }
}
