namespace SFA.DAS.EmployerAccounts.Configuration;

public class UserAornPayeLockConfiguration
{
    public int NumberOfPermittedAttempts { get; set; }
    public int PermittedAttemptsTimeSpanMinutes { get; set; }
    public int LockoutTimeSpanMinutes { get; set; }
}