namespace SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;

public class GetUserAornLockRequest : IRequest<GetUserAornLockResponse>
{
    public string UserRef { get; set; }
}