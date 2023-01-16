namespace SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;

public class GetPayeSchemeByRefQuery : IRequest<GetPayeSchemeByRefResponse>
{
    public string HashedAccountId { get; set; }
    public string Ref { get; set; }
}