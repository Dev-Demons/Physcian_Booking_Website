using Doctyme.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Doctyme.Model.Utility
{
    public static class Extension
    {
        public static string FirstCharToUpper(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().First().ToString().ToUpper() + value.Substring(1);
        }
        //public static string NullToString(this string value)
        //{
        //    return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        //}
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

        public static List<string> ToStringList(this string data, string separator = ",")
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var items = data.Split(new string[] { separator }, StringSplitOptions.None);
                    foreach (var item in items)
                    {
                        if (!string.IsNullOrEmpty(item))
                            list.Add(item);
                    }
                }
                catch { }
            }
            return list;
        }

        public static List<Int64> ToIntList(this string data, string separator = ",")
        {
            var list = new List<Int64>();
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var numbers = data.Split(new string[] { separator }, StringSplitOptions.None);
                    foreach (var number in numbers)
                    {
                        if (!string.IsNullOrEmpty(number))
                            list.Add(Convert.ToInt64(number));
                    }
                }
                catch { }
            }
            return list;
        }

        #region Common Functions

        public static decimal GetReviewStars(string reviewNos)
        {
            var reviewList = reviewNos.ToIntList();
            var reviewModel = new ReviewCalculationModel();

            foreach (var no in reviewList)
            {
                switch (no)
                {
                    case 1: reviewModel.OneStar++; break;
                    case 2: reviewModel.TwoStar++; break;
                    case 3: reviewModel.ThreeStar++; break;
                    case 4: reviewModel.FourStar++; break;
                    case 5: reviewModel.FiveStar++; break;
                    default:
                        break;
                }
            }

            return reviewModel.TotalRatings;
        }

        public struct DateTimeSpan
        {
            public int Years { get; }
            public int Months { get; }
            public int Days { get; }
            public int Hours { get; }
            public int Minutes { get; }
            public int Seconds { get; }
            public int Milliseconds { get; }

            public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
            {
                Years = years;
                Months = months;
                Days = days;
                Hours = hours;
                Minutes = minutes;
                Seconds = seconds;
                Milliseconds = milliseconds;
            }

            enum Phase { Years, Months, Days, Done }

            public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
            {
                if (date2 < date1)
                {
                    var sub = date1;
                    date1 = date2;
                    date2 = sub;
                }

                DateTime current = date1;
                int years = 0;
                int months = 0;
                int days = 0;

                Phase phase = Phase.Years;
                DateTimeSpan span = new DateTimeSpan();
                int officialDay = current.Day;

                while (phase != Phase.Done)
                {
                    switch (phase)
                    {
                        case Phase.Years:
                            if (current.AddYears(years + 1) > date2)
                            {
                                phase = Phase.Months;
                                current = current.AddYears(years);
                            }
                            else
                            {
                                years++;
                            }
                            break;
                        case Phase.Months:
                            if (current.AddMonths(months + 1) > date2)
                            {
                                phase = Phase.Days;
                                current = current.AddMonths(months);
                                if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                    current = current.AddDays(officialDay - current.Day);
                            }
                            else
                            {
                                months++;
                            }
                            break;
                        case Phase.Days:
                            if (current.AddDays(days + 1) > date2)
                            {
                                current = current.AddDays(days);
                                var timespan = date2 - current;
                                span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                                phase = Phase.Done;
                            }
                            else
                            {
                                days++;
                            }
                            break;
                    }
                }

                return span;
            }
        }

        #endregion
    }
}