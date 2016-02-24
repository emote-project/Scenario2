using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain
{
    [Serializable]
    public class NamedElementList<TElem> : IDisposable where TElem : INamedElement
    {
        private readonly Dictionary<string, TElem> _namedItems = new Dictionary<string, TElem>();

        protected TElem[] Items
        {
            get { return this._namedItems.Values.ToArray(); }
            set
            {
                if (value == null) return;
                this.Init(value);
            }
        }

        [XmlIgnore]
        public TElem this[string name]
        {
            get { return this._namedItems[name]; }
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            this._namedItems.Clear();
        }

        #endregion

        protected virtual void Init(IEnumerable<TElem> value)
        {
            foreach (var namedElement in value)
                this._namedItems.Add(namedElement.Name, namedElement);
        }

        public bool ContainsName(string name)
        {
            return this._namedItems.ContainsKey(name);
        }
    }
}