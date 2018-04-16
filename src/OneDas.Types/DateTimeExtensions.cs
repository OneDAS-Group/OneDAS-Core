using System;

namespace OneDas
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundUp(this DateTime dateTime, TimeSpan timeSpan)
        {
            return new DateTime(dateTime.Ticks + timeSpan.Ticks - (dateTime.Ticks % timeSpan.Ticks), dateTime.Kind);
        }

        public static DateTime RoundDown(this DateTime dateTime, TimeSpan timeSpan)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % timeSpan.Ticks), dateTime.Kind);
        }

        public static DateTime Round(this DateTime dateTime, TimeSpan timeSpan)
        {
            return new DateTime(Convert.ToInt64(Math.Truncate((double)(dateTime.Ticks + (timeSpan.Ticks / 2) + 1) / timeSpan.Ticks)) * timeSpan.Ticks, dateTime.Kind);
            // Return New DateTime(CLng(Math.Round(DateTime.Ticks / TimeSpan.Ticks, 0, MidpointRounding.AwayFromZero) * TimeSpan.Ticks)) ' maybe better alternative: round(X / N) * N
        }
    }
}
