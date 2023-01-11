namespace SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;

public class GetUserAornLockRequest : IAsyncRequest<GetUserAornLockResponse>
{
    public string UserRef { get; set; }
}