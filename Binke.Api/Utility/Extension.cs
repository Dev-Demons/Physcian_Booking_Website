using System;
using System.Collections;
using System.Linq;
using System.Web.Http.ModelBinding;

namespace Binke.Api.Utility
{
    public static class Extension
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        public static string ToDefaultFormate(this DateTime value, string formate = "")
        {
            string defaultFormate = RequestHelpers.GetConfigValue("DefaultDateTimeFormate");
            return value.UtcToUserTime().ToString(String.IsNullOrEmpty(formate) ? defaultFormate : formate);
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
    }

    public static class ModelStateHelper
    {
        public static IEnumerable Errors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return modelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                                    .Where(m => m.Value.Any());
            }
            return null;
        }
    }
}