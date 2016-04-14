using System;

namespace Util
{
    internal static class DateExtensions
    {
        public static DateTimeOffset StripMilliseconds(this DateTimeOffset value)
        {
            return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Offset);
        }
    }
}