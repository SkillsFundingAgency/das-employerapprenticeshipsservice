namespace SFA.DAS.EmployerAccounts.Queries.GetUserByRef;

public class GetUserByRefQuery : IRequest<GetUserByRefResponse>
{
    public string UserRef { get; set; }
}