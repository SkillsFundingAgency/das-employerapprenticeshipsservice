namespace SFA.DAS.EmployerAccounts.Queries.GetUser;

public class GetUserQuery : IRequest<GetUserResponse>
{
    public long UserId { get; set; }
}