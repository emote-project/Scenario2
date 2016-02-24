using System;

namespace CaseBasedController.GameInfo
{
    /// <summary>
    ///     Stores an enumerator as integer along with its translation
    ///     Eg.: Value=StructureType.Urban, Translation="Casas", Type=StructureType
    /// </summary>
    public class TranslatebleEnum
    {
        public TranslatebleEnum(int enumValue, string translation, Type type)
        {
            this.Value = enumValue;
            this.Translation = translation;
            this.Type = type;
        }

        public int Value { get; set; }

        public string Translation { get; set; }

        public Type Type { get; set; }
    }
}