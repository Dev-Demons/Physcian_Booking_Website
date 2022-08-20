using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Binke.Utility
{
    public static class Extension
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        private static readonly HashSet<char> DefaultNonWordCharacters = new HashSet<char> { ',', '.', ':', ';' };

        /// <summary>
        /// Returns a substring from the start of <paramref name="value"/> no 
        /// longer than <paramref name="length"/>.
        /// Returning only whole words is favored over returning a string that 
        /// is exactly <paramref name="length"/> long. 
        /// </summary>
        /// <param name="value">The original string from which the substring 
        /// will be returned.</param>
        /// <param name="length">The maximum length of the substring.</param>
        /// <param name="nonWordCharacters">Characters that, while not whitespace, 
        /// are not considered part of words and therefor can be removed from a 
        /// word in the end of the returned value. 
        /// Defaults to ",", ".", ":" and ";" if null.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="length"/> is negative
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="value"/> is null
        /// </exception>
        public static string CropWholeWords(this string value, int length, HashSet<char> nonWordCharacters = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (length < 0)
            {
                throw new ArgumentException(@"Negative values not allowed.", nameof(length));
            }

            if (nonWordCharacters == null)
            {
                nonWordCharacters = DefaultNonWordCharacters;
            }

            if (length >= value.Length)
            {
                return value;
            }
            var end = length;

            for (var i = end; i > 0; i--)
            {
                if (value[i].IsWhitespace())
                {
                    break;
                }

                if (nonWordCharacters.Contains(value[i])
                    && (value.Length == i + 1 || value[i + 1] == ' '))
                {
                    //Removing a character that isn't whitespace but not part 
                    //of the word either (ie ".") given that the character is 
                    //followed by whitespace or the end of the string makes it
                    //possible to include the word, so we do that.
                    break;
                }
                end--;
            }

            if (end == 0)
            {
                //If the first word is longer than the length we favor 
                //returning it as cropped over returning nothing at all.
                end = length;
            }

            return value.Substring(0, end);
        }

        private static bool IsWhitespace(this char character)
        {
            return character == ' ' || character == 'n' || character == 't';
        }

        public static string FirstCharToUpper(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().First().ToString().ToUpper() + value.Substring(1);
        }
        public static string NullToString(this string value)
        {
            return String.IsNullOrWhiteSpace(value) ? String.Empty : value.Trim();
        }
        public static string EmptyFiledGroup(this string value)
        {
            return String.IsNullOrWhiteSpace(value) ? "text" : value.Trim();
        }
        public static string GuidToString(this Guid? value)
        {
            var obj = new Guid();
            return value == null ? obj.ToString() : value.ToString();
        }
        public static string GuidToString(this Guid value)
        {
            new Guid();
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

        public static bool IsGreaterDate(this DateTime startDate, DateTime endDate)
        {
            return DateTime.Compare(startDate, endDate) > 0;
        }

        public static DateTime UtcToUserTime(this DateTime value)
        {
            #region For Global TimeZone
            //var currentTimeZone = TimeZone.CurrentTimeZone;
            //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(currentTimeZone.StandardName);
            //var date = TimeZoneInfo.ConvertTimeFromUtc(value, timeZoneInfo); 
            #endregion

            #region Only For Eastern Standard Time

            string standardTimeZoneName = RequestHelpers.GetConfigValue("DefaultTimeZone");
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(standardTimeZoneName);
            var date = TimeZoneInfo.ConvertTimeFromUtc(value, timezone);
            #endregion

            return date;
        }

        public static string ToDefaultFormate(this DateTime value, string formate = "")
        {
            string defaultFormate = RequestHelpers.GetConfigValue("DefaultDateTimeFormate");
            return value.UtcToUserTime().ToString(String.IsNullOrEmpty(formate) ? defaultFormate : formate);
        }

        public static string ToDefaultFormate(this DateTime? value, string formate = "")
        {
            string defaultFormate = RequestHelpers.GetConfigValue("DefaultDateTimeFormate");
            return value != null ? value.Value.UtcToUserTime().ToString(String.IsNullOrEmpty(formate) ? defaultFormate : formate) : "---";
        }

        public static DateTime UserTimeToUtc(this DateTime value)
        {
            #region For Global TimeZone
            //var currentTimeZone = TimeZone.CurrentTimeZone;
            //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(currentTimeZone.StandardName);
            //var date = TimeZoneInfo.ConvertTimeFromUtc(value, timeZoneInfo); 
            #endregion

            #region Only For Eastern Standard Time

            string standardTimeZoneName = RequestHelpers.GetConfigValue("DefaultTimeZone");
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(standardTimeZoneName);
            var date = TimeZoneInfo.ConvertTimeToUtc(value, timezone);
            #endregion

            return date;
        }

        public static TimeSpan ToTimeSpan(this string value)
        {
            if (!Regex.IsMatch(value, "^([0-9]{2}:[0-9]{2})$")) return new TimeSpan();
            if (!TimeSpan.TryParse(value, out TimeSpan time))
            {
                // handle validation error
            }
            return time;
        }
    }
}
