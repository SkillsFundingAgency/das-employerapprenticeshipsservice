namespace SFA.DAS.EmployerAccounts.Extensions;

public static class NullableDateTimeExtension
{
    public static string GetDateString(this DateTime? dateTime, string dateTimeFormat)
    {
        return dateTime?.ToString(dateTimeFormat) ?? string.Empty;
    }
}