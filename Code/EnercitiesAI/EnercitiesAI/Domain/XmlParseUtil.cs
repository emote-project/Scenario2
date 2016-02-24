using System;
using System.Globalization;

namespace EnercitiesAI.Domain
{
    public sealed class XmlParseUtil
    {
        public static readonly CultureInfo CultureInfo = CultureInfo.GetCultures(CultureTypes.NeutralCultures)[1];

        public static double ParseDouble(string value)
        {
            return Double.Parse(value, NumberStyles.Any, CultureInfo);
        }

        public static float ParseFloat(string value)
        {
            return float.Parse(value, NumberStyles.Any, CultureInfo);
        }

        public static decimal ParseDecimal(string value)
        {
            return Decimal.Parse(value, NumberStyles.Any, CultureInfo);
        }
    }
}