using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Thalamus.Conflicts
{

    public class ConflictRule
    {
        public enum ConflictTypes
        {
            ActionPublishing,
            ActionSubscription,
            PerceptionPublishing,
            PerceptionSubscription,
            Other
        }

        public List<string> UnsolvedClients { get; set; }
        public Dictionary<string, bool> ClientsRules { get; set; }
        public string FullEventName { get; set; }
        public ConflictTypes ConflictType { get; set; }
        public bool Solved { get; set; }

        public Dictionary<string, int> Funnel { get; set; }
        public List<string> OrderedFunnelClients { get; set; }

        public ConflictRule(string fullEventName, ConflictTypes conflictType)
        {
            ClientsRules = new Dictionary<string, bool>();
            FullEventName = fullEventName;
            ConflictType = conflictType;
            UnsolvedClients = new List<string>();
            Funnel = new Dictionary<string, int>();
            OrderedFunnelClients = new List<string>();
            Solved = false;
        }

        public static ConflictRule Null = new ConflictRule("Null", ConflictTypes.Other);
        public bool IsNull { get { return this == ConflictRule.Null; } }


        internal bool IsDefaultRule(ConflictSettings.DefaultRuleType defaultRule)
        {
            if (defaultRule == ConflictSettings.DefaultRuleType.All)
            {
                foreach (bool b in ClientsRules.Values) if (!b) return false;
                return true;
            }
            else if (defaultRule == ConflictSettings.DefaultRuleType.None)
            {
                foreach (bool b in ClientsRules.Values) if (b) return false;
                return true;
            }
            return false;
        }

        public void AddFunnel(string clientName)
        {
            if (Funnel.ContainsKey(clientName)) return;
            lock (Funnel)
            {
                Funnel[clientName] = Funnel.Count + 1;
                GenerateOrderedFunnel();
            }
        }

        public void RemoveFunnel(string clientName)
        {
            if (!Funnel.ContainsKey(clientName)) return;
            lock (Funnel)
            {
                while (Funnel[clientName] < Funnel.Count) FunnelDown(clientName);
                Funnel.Remove(clientName);
                GenerateOrderedFunnel();
            }
        }

        private void GenerateOrderedFunnel()
        {
            lock (OrderedFunnelClients)
            {
                List<KeyValuePair<string, int>> myList = new List<KeyValuePair<string, int>>(Funnel);
                myList.Sort(
                    delegate(KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair)
                    {
                        return firstPair.Value.CompareTo(nextPair.Value);
                    }
                );
                OrderedFunnelClients = new List<string>();
                foreach (KeyValuePair<string, int> item in myList)
                {
                    OrderedFunnelClients.Add(item.Key);
                }
            }
        }

        public void FunnelUp(string clientName)
        {
            if (!Funnel.ContainsKey(clientName)) return;
            int initialValue = Funnel[clientName];
            if (initialValue > 1)
            {
                lock (Funnel)
                {
                    Dictionary<string, int> tmpFunnel = new Dictionary<string, int>(Funnel);
                    foreach (KeyValuePair<string, int> funnelEntry in Funnel)
                    {
                        if (funnelEntry.Value < initialValue) tmpFunnel[funnelEntry.Key] += 1;
                        else tmpFunnel[funnelEntry.Key] = Funnel[funnelEntry.Key];
                    }
                    tmpFunnel[clientName] -= 1;
                    Funnel = tmpFunnel;
                    GenerateOrderedFunnel();
                }
            }
        }

        public void FunnelDown(string clientName)
        {
            if (!Funnel.ContainsKey(clientName)) return;
            int initialValue = Funnel[clientName];
            if (initialValue < Funnel.Count)
            {
                lock (Funnel)
                {
                    Dictionary<string, int> tmpFunnel = new Dictionary<string, int>(Funnel);
                    foreach (KeyValuePair<string, int> funnelEntry in Funnel)
                    {
                        if (funnelEntry.Value > initialValue) tmpFunnel[funnelEntry.Key] -= 1;
                        else tmpFunnel[funnelEntry.Key] = Funnel[funnelEntry.Key];
                    }
                    tmpFunnel[clientName] += 1;
                    Funnel = tmpFunnel;
                    GenerateOrderedFunnel();
                }
            }
        }
    }
}
