namespace SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

public class GetUserByEmailQuery : IRequest<GetUserByEmailResponse>
{
    public string Email { get; set; }
}