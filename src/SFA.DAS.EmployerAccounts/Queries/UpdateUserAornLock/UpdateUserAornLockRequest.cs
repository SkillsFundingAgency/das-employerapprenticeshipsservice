namespace SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;

public class UpdateUserAornLockRequest : IAsyncRequest
{
    public string UserRef { get; set; }
    public bool Success { get; set; }
}