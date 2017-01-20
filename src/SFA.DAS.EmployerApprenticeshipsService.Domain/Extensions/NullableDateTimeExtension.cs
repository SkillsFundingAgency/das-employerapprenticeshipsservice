using System;

namespace SFA.DAS.EAS.Domain.Extensions
{
    public static class NullableDateTimeExtension
    {
        public static string GetDateString(this DateTime? dateTime, string dateTimeFormat)
        {
            return dateTime?.ToString(dateTimeFormat) ?? string.Empty;
        }
    }
}
