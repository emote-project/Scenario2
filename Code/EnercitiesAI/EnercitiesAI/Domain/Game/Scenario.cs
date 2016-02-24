using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot("scenario", Namespace = "", IsNullable = false)]
    public class Scenario : IDisposable
    {
        public Scenario()
        {
            this.WinConditions = new Dictionary<int, ScenarioWinCondition>();
        }

        [XmlArray("winconditions", Form = XmlSchemaForm.Unqualified), XmlArrayItem("condition",
            typeof (ScenarioWinCondition), Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public ScenarioWinCondition[] WinConditionsArray
        {
            get { return this.WinConditions.Values.ToArray(); }
            set
            {
                foreach (var winCondition in value)
                    this.WinConditions.Add(winCondition.Level - 1, winCondition);
            }
        }

        [XmlElement("startvalues", typeof (ScenarioStartValues), Form = XmlSchemaForm.Unqualified)]
        public ScenarioStartValues StartValues { get; set; }

        [XmlIgnore]
        public Dictionary<int, ScenarioWinCondition> WinConditions { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            this.WinConditionsArray = null;
        }

        #endregion
    }
}