namespace SFA.DAS.EmployerAccounts.Queries.GetUserByRef;

public class GetUserByRefQuery : IAsyncRequest<GetUserByRefResponse>
{
    public string UserRef { get; set; }
}