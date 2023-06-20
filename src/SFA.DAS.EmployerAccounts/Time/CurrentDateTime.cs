namespace SFA.DAS.EmployerAccounts.Time;

public sealed class CurrentDateTime : ICurrentDateTime
{
    public DateTime Now { get; }

    public CurrentDateTime()
    {
        Now = DateTime.UtcNow;
    }

    public CurrentDateTime(DateTime time)
    {
        Now = time;
    }
}