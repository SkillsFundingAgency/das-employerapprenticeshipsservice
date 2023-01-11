namespace SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

public class GetUserByEmailQuery : IAsyncRequest<GetUserByEmailResponse>
{
    public string Email { get; set; }
}