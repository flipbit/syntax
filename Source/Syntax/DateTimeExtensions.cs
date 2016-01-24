using System;

namespace Syntax
{
    /// <summary>
    /// DateTime extension class
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts <see cref="DateTime" /> objects to a string representing the time elapsed or time until the given date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToHumanReadableString(this DateTime value)
        {
            var result = "never";

            var elapsed = ToAbsHumanReadableString(value, DateTime.Now);

            if (elapsed != "never")
            {
                if (value < DateTime.Now)
                {
                    result = string.Format("{0} ago", elapsed);
                }
                else
                {
                    result = string.Format("in {0}", elapsed);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts <see cref="DateTime"/> objects to a string representing the time between it and another <see cref="DateTime"/>.
        /// </summary>
        /// <param name="from">The from date.</param>
        /// <param name="to">The to date.</param>
        /// <returns></returns>
        public static string ToAbsHumanReadableString(this DateTime @from, DateTime to)
        {
            var value = "never";

            var seconds = Math.Abs(@from.Subtract(to).TotalSeconds);
            var days = Math.Abs(@from.Subtract(to).TotalDays);

            if (days > 366 && days <= 3650) value = "over a year";
            if (days <= 366) value = "a year";
            if (days < 365) value = "about " + Convert.ToInt32(days / 30) + " months";
            if (days < 45) value = "about a month";
            if (days < 28) value = Convert.ToInt32(days) + " days";
            if (seconds < 172800) value = "a day";
            if (seconds < 86400) value = Convert.ToInt32(seconds / 3600) + " hours";
            if (seconds < 7200) value = "an hour";
            if (seconds < 3600) value = Convert.ToInt32(seconds / 60) + " minutes";
            if (seconds < 300) value = "a few minutes";
            if (seconds < 60) value = "a few seconds";

            return value;
        }
    }
}