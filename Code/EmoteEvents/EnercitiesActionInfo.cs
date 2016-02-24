using System;
using System.IO;
using EmoteEnercitiesMessages;
using Newtonsoft.Json;

namespace EmoteEvents
{
    public class EnercitiesActionInfo
    {
        public EnercitiesActionInfo(ActionType actionType)
        {
            this.ActionType = actionType;
        }

        
        /// <summary>
        ///     Type of action: Build, Upgrade, Policy, SkipTurn
        /// </summary>
        public ActionType ActionType { get; set; }

        /// <summary>
        ///     According to action type, refers to eg StructureType, UpgradeType, PolicyType
        /// </summary>
        public int SubType { get; set; }

        /// <summary>
        ///     Only used for Upgrade and Build
        /// </summary>
        public int CellX { get; set; }

        /// <summary>
        ///     Only used for Upgrade and Build
        /// </summary>
        public int CellY { get; set; }

        #region Serialization methods

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static EnercitiesActionInfo DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                return (EnercitiesActionInfo) serializer.Deserialize(textReader, typeof (EnercitiesActionInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize EnercitiesGameInfo from '" + serialized + "': " + e.Message);
            }

            return null;
        }

        #endregion

        #region Equality methods

        public override bool Equals(object obj)
        {
            return (obj is EnercitiesActionInfo) && this.Equals((EnercitiesActionInfo) obj);
        }

        #endregion

        protected bool Equals(EnercitiesActionInfo other)
        {
            return this.ActionType == other.ActionType &&
                   this.SubType == other.SubType &&
                   this.CellX == other.CellX &&
                   this.CellY == other.CellY;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) this.ActionType;
                hashCode = (hashCode*397) ^ this.SubType;
                hashCode = (hashCode*397) ^ this.CellX;
                hashCode = (hashCode*397) ^ this.CellY;
                return hashCode;
            }
        }

        public override string ToString()
        {
            var subtype = "";
            switch (ActionType)
            {
                case ActionType.BuildStructure:
                    subtype = ((StructureType) SubType).ToString();
                    break;
                case ActionType.ImplementPolicy:
                    subtype = ((PolicyType) SubType).ToString();
                    break;
                case ActionType.UpgradeStructure:
                    subtype = ((UpgradeType) SubType).ToString();
                    break;
                case ActionType.SkipTurn:
                    subtype = "";
                    break;
            }
            return string.Format(
                "{0}: {1}; X: {2}, Y: {3}",
                this.ActionType, subtype, this.CellX, this.CellY);
        }
    }
}