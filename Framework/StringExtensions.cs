using System;

namespace Framework.Extensions
{
    public static class StringExtensions
    {
        public static string TruncateAtWord(this string input, int length)
        {
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            var ret = string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
            return ret;
        }

        public static string ToReadableString(this TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} дней, ", span.Days) : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} часов, ", span.Hours) : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} минут, ", span.Minutes) : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} секунд", span.Seconds) : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 секунд";

            return formatted;
        }
    }
}
