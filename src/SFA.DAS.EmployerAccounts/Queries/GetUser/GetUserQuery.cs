namespace SFA.DAS.EmployerAccounts.Queries.GetUser;

public class GetUserQuery : IAsyncRequest<GetUserResponse>
{
    public long UserId { get; set; }
}