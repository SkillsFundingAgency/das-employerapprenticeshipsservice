namespace SFA.DAS.EmployerAccounts.Web.Extensions;

public static class TimeSpanExtensions
{
    public static string ToHumanReadableString(this TimeSpan timeSpan)
    {
        if (timeSpan.TotalMinutes <= 1)
        {
            return $"{timeSpan:%s} seconds";
        }

        if (timeSpan.TotalHours <= 1)
        {
            return $"{timeSpan:%m} minutes";
        }

        if (timeSpan.TotalDays <= 1)
        {
            return $"{timeSpan:%h} hours";
        }

        return $"{timeSpan:%d} days";
    }
}