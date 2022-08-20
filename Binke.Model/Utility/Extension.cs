using System;
using System.Linq;

namespace Binke.Model.Utility
{
    public static class Extension
    {
        public static string FirstCharToUpper(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().First().ToString().ToUpper() + value.Substring(1);
        }
        public static string NullToString(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
        public static string EmptyFiledGroup(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "text" : value.Trim();
        }
        public static string GuidToString(this Guid? value)
        {
            var obj = new Guid();
            return value == null ? obj.ToString() : value.ToString();
        }
        public static string GuidToString(this Guid value)
        {
            return value.ToString();
        }
        public static int NullToInt(this int? value)
        {
            return value ?? 0;
        }
        public static decimal NullToDecimal(this decimal? value)
        {
            return value ?? 0;
        }
        public static double NullToDouble()
        {
            return NullToDouble(null);
        }

        public static double NullToDouble(this double? value)
        {
            return value ?? 0;
        }
        public static string NumberToCurrency(this decimal value)
        {
            return $"{value:###,##0.00}";
        }
    }
}