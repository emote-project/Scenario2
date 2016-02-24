using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Detection;
using CaseBasedController.GameInfo;
using EmoteEvents;

namespace CaseBasedController.Behavior.Enercities
{
    class PerformUtteranceForLatestMemoryEvent : BaseBehavior
    {
        string _utteranceId = "";
        IFeatureDetector _detector;


        public override void Execute(IFeatureDetector detector)
        {
            lock (this.locker)
            {
                _detector = detector;
                System.Threading.Thread.Sleep(300);                                         // Waits to be sure that GameStatus is updated by all the coming events

                try
                {
                    if (GameStatus.CurrentState.MemoryEventData == null)
                    {
                        Logger.Log("No memory event data found",this);
                        this.RaiseFinishedEvent();
                        return;
                    }
                    Logger.Log("Preparing memory event", this);

                    List<MemoryEvent.MemoryEventItem> memoryEventItems =
                        GameInfo.GameStatus.CurrentState.MemoryEventData.memoryEventItems;
                    foreach (var mei in memoryEventItems)
                    {
                        string category = mei.category;
                        string subcategory = mei.subcategory;

                        // Adding slashes to the tags
                        for (int i = 0; i < mei.tagNames.Count(); i++)
                        {
                            mei.tagNames[i] = "/" + mei.tagNames[i] + "/";
                        }

                        var tagToFix = @"/stmAchievedLevel/";
                        if (mei.tagNames.Contains(tagToFix))
                        {
                            for (int i = 0; i < mei.tagNames.Count(); i++)
                            {
                                if (mei.tagNames[i].Equals(tagToFix))
                                {
                                    if (mei.tagValues[i].Contains('.'))
                                        mei.tagValues[i] = mei.tagValues[i].Remove(mei.tagValues[i].IndexOf('.'));
                                }
                            }
                        }


                        var tagsAndValues = GameInfo.GameStatus.GetTagNamesAndValues();
                        _utteranceId = "cbc" + new System.Random().Next();

                        var tagsList = new List<string>(tagsAndValues.Keys);
                        tagsList.AddRange(
                            mei.tagNames.ToList());
                        string[] tags = tagsList.ToArray();

                        var valuesList = new List<string>(tagsAndValues.Values);
                        valuesList.AddRange(
                            mei.tagValues.ToList());
                        string[] values = valuesList.ToArray();

                        _utteranceId = PerformUtterance(category, subcategory, tags, values);
                        Logger.Log("LATEST MEMORY EVENT UTTERANCE: " + category + ":" +
                                          subcategory + " (" + _utteranceId + ")",this);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception executing PerformUtteranceForLatestMemoryEvent: "+ex.Message,this);
                }

            }
        }


        protected override void UtteranceFinishedEvent(string id)
        {
            base.UtteranceFinishedEvent(id);
            Logger.Log("Received utterance finished with id: " + id + ". Stored id = " + _utteranceId,this);
            if (_utteranceId != null && _utteranceId.Equals(id))
                this.RaiseFinishedEvent(_detector);
        }

        public override void Cancel()
        {
            this.actionPublisher.CancelUtterance(_utteranceId);
        }

        public override string ToString()
        {
            return "Perform utterance for Memory Event";
        }

        
    }
}
