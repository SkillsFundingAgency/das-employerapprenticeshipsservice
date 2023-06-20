namespace SFA.DAS.EmployerAccounts.Models.UserProfile;

public class UserAornPayeStatus
{
    public bool IsLocked { get; set; }
    public int RemainingLock { get; set; }
    public int RemainingAttempts { get; set; }
    public int AllowedAttempts { get; set; }
}