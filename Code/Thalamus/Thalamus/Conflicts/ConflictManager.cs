using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Thalamus.Conflicts;
namespace Thalamus
{
    public class ConflictManager : Manager
    {
        #region events

        public delegate void SettingsSavedHandler();
        public event SettingsSavedHandler SettingsSaved;
        private void NotifySettingsSaved()
        {
            if (SettingsSaved != null) SettingsSaved();
        }

        public delegate void ConflictsChangedHandler(Character character);
        public event ConflictsChangedHandler ConflictsChanged;
        private void NotifyConflictsChanged(Character character)
        {
            if (ConflictsChanged != null) ConflictsChanged(character);
        }

        #endregion

        #region Singleton
        private static ConflictManager instance;
        public static ConflictManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConflictManager();

                }
                return instance;
            }
        }
        #endregion

        #region Properties

        private ConflictSettings.DefaultRuleType defaultRuleOnActionPublishingConflict = ConflictSettings.DefaultRuleType.None;
        public ConflictSettings.DefaultRuleType DefaultRuleOnActionPublishingConflict
        {
            get { return defaultRuleOnActionPublishingConflict; }
            set {
                defaultRuleOnActionPublishingConflict = value;
                SaveSettings();
            }
        }

        private ConflictSettings.DefaultRuleType defaultRuleOnActionSubscriptionConflict = ConflictSettings.DefaultRuleType.All;
        public ConflictSettings.DefaultRuleType DefaultRuleOnActionSubscriptionConflict
        {
            get { return defaultRuleOnActionSubscriptionConflict; }
            set {
                defaultRuleOnActionSubscriptionConflict = value;
                SaveSettings();
            }
        }

        private ConflictSettings.DefaultRuleType defaultRuleOnPerceptionPublishingConflict = ConflictSettings.DefaultRuleType.None;
        public ConflictSettings.DefaultRuleType DefaultRuleOnPerceptionPublishingConflict
        {
            get { return defaultRuleOnPerceptionPublishingConflict; }
            set
            {
                defaultRuleOnPerceptionPublishingConflict = value;
                SaveSettings();
            }
        }

        private ConflictSettings.DefaultRuleType defaultRuleOnPerceptionSubscriptionConflict = ConflictSettings.DefaultRuleType.All;
        public ConflictSettings.DefaultRuleType DefaultRuleOnPerceptionSubscriptionConflict
        {
            get { return defaultRuleOnPerceptionSubscriptionConflict; }
            set
            {
                defaultRuleOnPerceptionSubscriptionConflict = value;
                SaveSettings();
            }
        }

        private Dictionary<string, Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>> conflictRulesBuffer = new Dictionary<string, Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>>();
        private Dictionary<string, Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>> conflictRules = new Dictionary<string, Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>>();
        public Dictionary<string, Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>> ConflictRules
        {
            get { return conflictRules; }
            set
            {
                conflictRules = value;
                SaveSettings();
            }
        }

        Dictionary<ThalamusClientProxy, Dictionary<string, List<ThalamusClientProxy>>> actionPublicationMap = new Dictionary<ThalamusClientProxy, Dictionary<string, List<ThalamusClientProxy>>>();
        Dictionary<ThalamusClientProxy, Dictionary<string, List<ThalamusClientProxy>>> perceptionPublicationMap = new Dictionary<ThalamusClientProxy, Dictionary<string, List<ThalamusClientProxy>>>();
        Dictionary<ThalamusClientProxy, Dictionary<string, List<ThalamusClientProxy>>> actionSubscriptionMap = new Dictionary<ThalamusClientProxy, Dictionary<string, List<ThalamusClientProxy>>>();
        Dictionary<ThalamusClientProxy, Dictionary<string, List<ThalamusClientProxy>>> perceptionSubscriptionMap = new Dictionary<ThalamusClientProxy, Dictionary<string, List<ThalamusClientProxy>>>();


        private string fileName = "";
        public string FileName
        {
            get { return fileName; }
            set { 
                fileName = value;
                Properties.Settings.Default.ConflictRulesFile = fileName;
                Properties.Settings.Default.Save();
                LoadSettings(ConflictSettings.LoadConflictSettings(Properties.Settings.Default.ConflictRulesFile));
                Debug("Loaded settings from " + Properties.Settings.Default.ConflictRulesFile);
            }
        }

        #endregion

        #region Constructors, Initializers and Settings

        public ConflictManager()
            : base("ConflictManager")
        {
            setDebug(true);
            setDebug("error", true);
            setDebug("all", true);
        }

        public override bool Setup()
        {
            base.Setup();
            if (Properties.Settings.Default.ConflictRulesFile != "")
            {
                try
                {
                    if (File.Exists(Properties.Settings.Default.ConflictRulesFile))
                    {
                        LoadSettings(ConflictSettings.LoadConflictSettings(Properties.Settings.Default.ConflictRulesFile));
                        Debug("Loaded settings from " + Properties.Settings.Default.ConflictRulesFile);
                    }
                    else
                    {
                        DebugError("Did not load Conflict Rules file '{0}' because it does not exist!", Properties.Settings.Default.ConflictRulesFile);
                    }
                }
                catch (Exception e)
                {
                    DebugException(e);
                    return false;
                }
            }
            return true;
        }

        private void LoadSettings(ConflictSettings settings)
        {
            this.conflictRules = settings.ConflictRules;
            this.defaultRuleOnActionPublishingConflict = settings.DefaultRuleOnActionPublishingConflict;
            this.defaultRuleOnActionSubscriptionConflict= settings.DefaultRuleOnActionSubscriptionConflict;
            this.defaultRuleOnPerceptionPublishingConflict = settings.DefaultRuleOnPerceptionPublishingConflict;
            this.defaultRuleOnPerceptionSubscriptionConflict = settings.DefaultRuleOnPerceptionSubscriptionConflict;
        }

        private void SaveSettings(Character character = null)
        {
            if (Properties.Settings.Default.ConflictRulesFile != "")
            {
                ConflictSettings settings = new ConflictSettings();
                settings.ConflictRules = this.ConflictRules;
                settings.DefaultRuleOnActionPublishingConflict = this.defaultRuleOnActionPublishingConflict;
                settings.DefaultRuleOnActionSubscriptionConflict = this.defaultRuleOnActionSubscriptionConflict;
                settings.DefaultRuleOnPerceptionPublishingConflict = this.defaultRuleOnPerceptionPublishingConflict;
                settings.DefaultRuleOnPerceptionSubscriptionConflict = this.defaultRuleOnPerceptionSubscriptionConflict;
                ConflictSettings.Save(Properties.Settings.Default.ConflictRulesFile, settings);
                Debug("Saved settings to " + Properties.Settings.Default.ConflictRulesFile);
                NotifySettingsSaved();
                if (character != null) NotifyConflictsChanged(character);
            }
            else
            {
                DebugError("Did not save Conflict Rules file because its path is not defined!");
            }
        }

        public override bool Start()
        {
            base.Start();
            Environment.Instance.EventInformationChanged += Instance_EventInformationChanged;
            return true;
        }

        void Instance_EventInformationChanged(Character character)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DetectConflicts));
            t.Start(character);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion

        #region fields

        Mutex detectionMutex = new Mutex();

        #endregion

        #region Conflict management


        public List<string> FilterAllowedToSubscribe(string characterName, string clientName, List<string> events)
        {
            if (!conflictRules.ContainsKey(characterName)) return events;
            List<string> filteredEvents = new List<string>();

            foreach (string ev in events)
            {
                if (!conflictRules[characterName].ContainsKey(ev)) filteredEvents.Add(ev);
                else
                {
                    if (conflictRules[characterName][ev].ContainsKey(ConflictRule.ConflictTypes.ActionSubscription))
                    {
                        ConflictRule rule = conflictRules[characterName][ev][ConflictRule.ConflictTypes.ActionSubscription];
                        if (rule.UnsolvedClients.Contains(clientName)) filteredEvents.Add(ev);
                        else if (!rule.ClientsRules.ContainsKey(clientName)) filteredEvents.Add(ev);
                        else if (rule.ClientsRules[clientName]) filteredEvents.Add(ev);
                    }
                    if (conflictRules[characterName][ev].ContainsKey(ConflictRule.ConflictTypes.PerceptionSubscription))
                    {
                        ConflictRule rule = conflictRules[characterName][ev][ConflictRule.ConflictTypes.PerceptionSubscription];
                        if (rule.UnsolvedClients.Contains(clientName)) filteredEvents.Add(ev);
                        else if (!rule.ClientsRules.ContainsKey(clientName)) filteredEvents.Add(ev);
                        else if (rule.ClientsRules[clientName]) filteredEvents.Add(ev);
                    }
                }
            }
            return filteredEvents;
        }

        public List<string> FilterAllowedToPublish(string characterName, string clientName, List<string> events)
        {
            if (!conflictRules.ContainsKey(characterName)) return events;
            List<string> filteredEvents = new List<string>();

            foreach (string ev in events)
            {
                if (!conflictRules[characterName].ContainsKey(ev)) filteredEvents.Add(ev);
                else
                {
                    if (conflictRules[characterName][ev].ContainsKey(ConflictRule.ConflictTypes.ActionPublishing))
                    {
                        ConflictRule rule = conflictRules[characterName][ev][ConflictRule.ConflictTypes.ActionPublishing];
                        if (rule.UnsolvedClients.Contains(clientName)) filteredEvents.Add(ev);
                        else if (!rule.ClientsRules.ContainsKey(clientName)) filteredEvents.Add(ev);
                        else if (rule.ClientsRules[clientName]) filteredEvents.Add(ev);
                    }
                    if (conflictRules[characterName][ev].ContainsKey(ConflictRule.ConflictTypes.PerceptionPublishing))
                    {
                        ConflictRule rule = conflictRules[characterName][ev][ConflictRule.ConflictTypes.PerceptionPublishing];
                        if (rule.UnsolvedClients.Contains(clientName)) filteredEvents.Add(ev);
                        else if (!rule.ClientsRules.ContainsKey(clientName)) filteredEvents.Add(ev);
                        else if (rule.ClientsRules[clientName]) filteredEvents.Add(ev);
                    }
                }
            }
            return filteredEvents;
        }

        public bool IsClientAllowedToPublish(string characterName, string fullEventName, ThalamusClientProxy client)
        {
            if (!conflictRules.ContainsKey(characterName)) return true;
            if (!conflictRules[characterName].ContainsKey(fullEventName)) return true;
            if (conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.ActionPublishing))
            {
                ConflictRule rule = conflictRules[characterName][fullEventName][ConflictRule.ConflictTypes.ActionPublishing];
                if (rule.UnsolvedClients.Contains(client.Name)) return true;
                else if (!rule.ClientsRules.ContainsKey(client.Name) || rule.ClientsRules[client.Name]) return true;
                return false;
            }
            else if (conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.PerceptionPublishing))
            {
                ConflictRule rule = conflictRules[characterName][fullEventName][ConflictRule.ConflictTypes.PerceptionPublishing];
                if (rule.UnsolvedClients.Contains(client.Name)) return true;
                else if (!rule.ClientsRules.ContainsKey(client.Name) || rule.ClientsRules[client.Name]) return true;
                return false;
            }
            else return true;
        }

        public List<ThalamusClientProxy> FilterAllowedToPublish(string characterName, string fullEventName, List<ThalamusClientProxy> clients) 
        {
            if (!conflictRules.ContainsKey(characterName)) return clients;
            if (!conflictRules[characterName].ContainsKey(fullEventName)) return clients;
            //if (!conflictRules[characterName][fullEventName].Solved) return clients;
            List<ThalamusClientProxy> filteredClients = new List<ThalamusClientProxy>();
            if (!(conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.ActionPublishing) || conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.PerceptionPublishing))) return clients;
            else
            {
                if (conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.ActionPublishing))
                {
                    ConflictRule rule = conflictRules[characterName][fullEventName][ConflictRule.ConflictTypes.ActionPublishing];
                    foreach (ThalamusClientProxy client in clients)
                    {
                        if (rule.UnsolvedClients.Contains(client.Name)) filteredClients.Add(client);
                        else if (!rule.ClientsRules.ContainsKey(client.Name) || rule.ClientsRules[client.Name]) filteredClients.Add(client);
                    }
                }
                if (conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.PerceptionPublishing))
                {
                    ConflictRule rule = conflictRules[characterName][fullEventName][ConflictRule.ConflictTypes.PerceptionPublishing];
                    foreach (ThalamusClientProxy client in clients)
                    {
                        if (rule.UnsolvedClients.Contains(client.Name)) filteredClients.Add(client);
                        else if (!rule.ClientsRules.ContainsKey(client.Name) || rule.ClientsRules[client.Name]) filteredClients.Add(client);
                    }
                }
            }
            return filteredClients;
        }

        public List<ThalamusClientProxy> FilterAllowedToSubscribe(string characterName, string fullEventName, List<ThalamusClientProxy> clients)
        {
            if (!conflictRules.ContainsKey(characterName)) return clients;
            if (!conflictRules[characterName].ContainsKey(fullEventName)) return clients;
            //if (!conflictRules[characterName][fullEventName].Solved) return clients;
            List<ThalamusClientProxy> filteredClients = new List<ThalamusClientProxy>();
            if (!(conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.ActionSubscription) || conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.PerceptionSubscription))) return clients;
            else
            {
                if (conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.ActionSubscription))
                {
                    ConflictRule rule = conflictRules[characterName][fullEventName][ConflictRule.ConflictTypes.ActionSubscription];
                    foreach (ThalamusClientProxy client in clients)
                    {
                        if (rule.UnsolvedClients.Contains(client.Name)) filteredClients.Add(client);
                        else if (!rule.ClientsRules.ContainsKey(client.Name) || rule.ClientsRules[client.Name]) filteredClients.Add(client);
                    }
                }
                if (conflictRules[characterName][fullEventName].ContainsKey(ConflictRule.ConflictTypes.PerceptionSubscription))
                {
                    ConflictRule rule = conflictRules[characterName][fullEventName][ConflictRule.ConflictTypes.PerceptionSubscription];
                    foreach (ThalamusClientProxy client in clients)
                    {
                        if (rule.UnsolvedClients.Contains(client.Name)) filteredClients.Add(client);
                        else if (!rule.ClientsRules.ContainsKey(client.Name) || rule.ClientsRules[client.Name]) filteredClients.Add(client);
                    }
                }
            }
            return filteredClients;
        }

        public void DetectConflicts(object ocharacter)
        {
            detectionMutex.WaitOne();
            Character character = (Character)ocharacter;
            int count = 0;
            try
            {
                Debug("Running Conflict Detection for character {0}...", character.Name);


                List<ThalamusClientProxy> clients;
                Dictionary<string, PML> eventInfo;
                List<string> clientEvents;

                conflictRulesBuffer = new Dictionary<string, Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>>();

                lock (character.Clients.RemoteClients)
                {
                    clients = new List<ThalamusClientProxy>(character.Clients.RemoteClients.Values);
                }

                lock (character.Clients.EventInfo)
                {
                    eventInfo = new Dictionary<string, PML>(character.Clients.EventInfo);
                }

                Dictionary<string, List<string>> subscribedActions = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> subscribedPerceptions = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> announcedActions = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> announcedPerceptions = new Dictionary<string, List<string>>();

                foreach (ThalamusClientProxy client in clients)
                {
                    if (!client.Connected) continue;
                    lock (client.SubscribedEvents)
                    {
                        clientEvents = new List<string>(client.SubscribedEvents);
                    }
                    foreach (string ev in clientEvents)
                    {
                        if (eventInfo.ContainsKey(ev))
                        {
                            if (eventInfo[ev].EventType == PMLEventType.Perception)
                            {
                                if (!subscribedPerceptions.ContainsKey(ev)) subscribedPerceptions[ev] = new List<string>();
                                subscribedPerceptions[ev].Add(client.Name);
                            }
                            else
                            {
                                if (!subscribedActions.ContainsKey(ev)) subscribedActions[ev] = new List<string>();
                                subscribedActions[ev].Add(client.Name);
                            }
                        }
                    }

                    lock (client.AnnouncedEvents)
                    {
                        clientEvents = new List<string>(client.AnnouncedEvents);
                    }
                    foreach (string ev in clientEvents)
                    {
                        if (eventInfo.ContainsKey(ev))
                        {
                            if (eventInfo[ev].EventType == PMLEventType.Perception)
                            {
                                if (!announcedPerceptions.ContainsKey(ev)) announcedPerceptions[ev] = new List<string>();
                                announcedPerceptions[ev].Add(client.Name);
                            }
                            else
                            {
                                if (!announcedActions.ContainsKey(ev)) announcedActions[ev] = new List<string>();
                                announcedActions[ev].Add(client.Name);
                            }
                        }
                    }
                }
                bool rulesChanged = false;
                foreach (KeyValuePair<string, List<string>> action in subscribedActions)
                {
                    if (action.Value.Count > 1 && !IsRuleSolved(character.Name, action.Key, ConflictRule.ConflictTypes.ActionSubscription, action.Value, defaultRuleOnActionSubscriptionConflict)) //Multi Action-Subscription
                    {
                        CreateRule(character.Name, action.Key, ConflictRule.ConflictTypes.ActionSubscription, action.Value, defaultRuleOnActionSubscriptionConflict);
                        rulesChanged = true;
                    }
                }
                foreach (KeyValuePair<string, List<string>> perception in subscribedPerceptions)
                {
                    if (perception.Value.Count > 1 && !IsRuleSolved(character.Name, perception.Key, ConflictRule.ConflictTypes.PerceptionSubscription, perception.Value, defaultRuleOnPerceptionSubscriptionConflict)) //Multi Perception-Subscription
                    {
                        CreateRule(character.Name, perception.Key, ConflictRule.ConflictTypes.PerceptionSubscription, perception.Value, defaultRuleOnPerceptionSubscriptionConflict);
                        rulesChanged = true;
                    }
                }
                foreach (KeyValuePair<string, List<string>> action in announcedActions)
                {
                    if (action.Value.Count > 1 && !IsRuleSolved(character.Name, action.Key, ConflictRule.ConflictTypes.ActionPublishing, action.Value, defaultRuleOnActionPublishingConflict)) //Multi Action-Publication
                    {
                        CreateRule(character.Name, action.Key, ConflictRule.ConflictTypes.ActionPublishing, action.Value, defaultRuleOnActionPublishingConflict);
                        rulesChanged = true;
                    }
                }
                foreach (KeyValuePair<string, List<string>> perception in announcedPerceptions)
                {
                    if (perception.Value.Count > 1 && !IsRuleSolved(character.Name, perception.Key, ConflictRule.ConflictTypes.PerceptionPublishing, perception.Value, defaultRuleOnPerceptionPublishingConflict)) //Multi Action-Publication
                    {
                        CreateRule(character.Name, perception.Key, ConflictRule.ConflictTypes.PerceptionPublishing, perception.Value, defaultRuleOnPerceptionPublishingConflict);
                        rulesChanged = true;
                    }
                }
                SwapBuffer();
                RebuildEventsMap(character);
                if (rulesChanged) SaveSettings(character);
                if (conflictRules.ContainsKey(character.Name)) count = conflictRules[character.Name].Count;
            }
            catch (Exception e)
            {
                DebugException(e);
            }
            finally
            {
                detectionMutex.ReleaseMutex();
            }
            Debug("{0} Conflicts Detected.", count);
        }

        private void RebuildEventsMap(Character character)
        {
            Dictionary<string, ThalamusClientProxy> clients;
            lock (character.Clients.RemoteClients)
            {
                clients = new Dictionary<string, ThalamusClientProxy>(character.Clients.RemoteClients);
            }
            foreach (ThalamusClientProxy client in clients.Values)
            {
                if (!client.Connected) continue;
                actionPublicationMap[client] = new Dictionary<string, List<ThalamusClientProxy>>();
                perceptionPublicationMap[client] = new Dictionary<string, List<ThalamusClientProxy>>();
                actionSubscriptionMap[client] = new Dictionary<string, List<ThalamusClientProxy>>();
                perceptionSubscriptionMap[client] = new Dictionary<string, List<ThalamusClientProxy>>();
                lock (client.SubscribedEvents)
                {
                    foreach (string ev in client.SubscribedEvents)
                    {
                        if (!character.Clients.SubscribedToEvent.ContainsKey(ev) || !character.Clients.EventInfo.ContainsKey(ev)) continue;
                        PML pml = character.Clients.EventInfo[ev];
                        if (pml.EventType == PMLEventType.Action) actionSubscriptionMap[client][ev] = character.Clients.SubscribedToEvent[ev];
                        else perceptionSubscriptionMap[client][ev] = character.Clients.SubscribedToEvent[ev];
                    }
                }
                lock (client.AnnouncedEvents)
                {
                    foreach (string ev in client.AnnouncedEvents)
                    {
                        if (!character.Clients.PublishesEvent.ContainsKey(ev) || !character.Clients.EventInfo.ContainsKey(ev)) continue;
                        PML pml = character.Clients.EventInfo[ev];
                        if (pml.EventType == PMLEventType.Action) actionPublicationMap[client][ev] = character.Clients.PublishesEvent[ev];
                        else perceptionPublicationMap[client][ev] = character.Clients.PublishesEvent[ev];
                    }
                }
            }
            foreach (ThalamusClientProxy client in clients.Values)
            {
                /*actionPublicationMap[client] = FilterActionPublicationMap(client, actionPublicationMap[client]);
                perceptionPublicationMap[client] = FilterPerceptionPublicationMap(client, perceptionPublicationMap[client]);
                actionSubscriptionMap[client] = FilterActionSubscriptionMap(client, actionSubscriptionMap[client]);*/
                var z = FilterPerceptionSubscriptionMap(client, perceptionSubscriptionMap[client]);
                perceptionSubscriptionMap[client] = z;
            }
        }

        private Dictionary<string, List<ThalamusClientProxy>> FilterPerceptionSubscriptionMap(ThalamusClientProxy client, Dictionary<string, List<ThalamusClientProxy>> map)
        {
            if (!conflictRules.ContainsKey(client.Character.Name)) return map;
            Dictionary<string, List<ThalamusClientProxy>> result = new Dictionary<string, List<ThalamusClientProxy>>();
            Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>> characterRules = conflictRules[client.Character.Name];
            foreach (KeyValuePair<string, List<ThalamusClientProxy>> ps in map)
            {
                if (!characterRules.ContainsKey(ps.Key) || !characterRules[ps.Key].ContainsKey(ConflictRule.ConflictTypes.PerceptionSubscription)) {
                    result[ps.Key] = ps.Value;
                }
                else
                {
                    ConflictRule rule = characterRules[ps.Key][ConflictRule.ConflictTypes.PerceptionSubscription];
                    if (!rule.ClientsRules.ContainsKey(client.Name)) result[ps.Key] = ps.Value;
                }
            }
            return result;
        }

        private Dictionary<string, List<ThalamusClientProxy>> FilterActionSubscriptionMap(ThalamusClientProxy client, Dictionary<string, List<ThalamusClientProxy>> map)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, List<ThalamusClientProxy>> FilterPerceptionPublicationMap(ThalamusClientProxy client, Dictionary<string, List<ThalamusClientProxy>> map)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, List<ThalamusClientProxy>> FilterActionPublicationMap(ThalamusClientProxy client, Dictionary<string, List<ThalamusClientProxy>> map)
        {
            throw new NotImplementedException();
        }

        private bool IsRuleSolved(string character, string fullEventName, ConflictRule.ConflictTypes conflictType, List<string> clientsList, ConflictSettings.DefaultRuleType defaultRule)
        {
            if (!conflictRules.ContainsKey(character)) return false;
            if (!conflictRules[character].ContainsKey(fullEventName)) return false;
            if (!conflictRules[character][fullEventName].ContainsKey(conflictType)) return false;
            if (!conflictRules[character][fullEventName][conflictType].Solved) return false;
            return true;
        }

        private void CreateRule(string character, string fullEventName, ConflictRule.ConflictTypes conflictType, List<string> clientsList, ConflictSettings.DefaultRuleType defaultRule)
        {
            ConflictRule cr = new ConflictRule(fullEventName, conflictType);
            if (defaultRule != ConflictSettings.DefaultRuleType.Ignore)
            {
                foreach (string client in clientsList) cr.ClientsRules[client] = defaultRule == ConflictSettings.DefaultRuleType.All;
            }
            else
            {
                foreach (string client in clientsList) cr.UnsolvedClients.Add(client);
            }
            if (!conflictRulesBuffer.ContainsKey(character)) conflictRulesBuffer[character] = new Dictionary<string, Dictionary<ConflictRule.ConflictTypes, ConflictRule>>();
            if (!conflictRulesBuffer[character].ContainsKey(fullEventName)) conflictRulesBuffer[character][fullEventName] = new Dictionary<ConflictRule.ConflictTypes, ConflictRule>();
            conflictRulesBuffer[character][fullEventName][conflictType] = cr;
        }

        public void SetRule(Character character, string fullEventName, ConflictRule.ConflictTypes conflictType, string clientName, bool ruleState)
        {
            lock (conflictRules)
            {
                if (conflictRules.ContainsKey(character.Name) && conflictRules[character.Name].ContainsKey(fullEventName) && conflictRules[character.Name][fullEventName].ContainsKey(conflictType))
                {
                    bool changed = false;
                    ConflictRule rule = conflictRules[character.Name][fullEventName][conflictType];
                    if (rule.ClientsRules.ContainsKey(clientName) && rule.ClientsRules[clientName] != ruleState) changed = true;
                    rule.ClientsRules[clientName] = ruleState;
                    if (rule.UnsolvedClients.Contains(clientName)) rule.UnsolvedClients.Remove(clientName);
                    if (rule.UnsolvedClients.Count == 0) rule.Solved = true;
                    conflictRules[character.Name][fullEventName][conflictType] = rule;
                    if (changed) SaveSettings(character);
                }
            }
        }

        private void SwapBuffer()
        {
            lock (conflictRules)
            {
                conflictRules = conflictRulesBuffer;
            }
        }

        

        #endregion

        public void SetFunnel(Character character, ConflictRule selectedConflict, string selectedFunnel)
        {
            conflictRules[character.Name][selectedConflict.FullEventName][selectedConflict.ConflictType].AddFunnel(selectedFunnel);
            SaveSettings(character);
        }
    }
}
