namespace SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;

public class GetPayeSchemeByRefQuery : IAsyncRequest<GetPayeSchemeByRefResponse>
{
    public string HashedAccountId { get; set; }
    public string Ref { get; set; }
}