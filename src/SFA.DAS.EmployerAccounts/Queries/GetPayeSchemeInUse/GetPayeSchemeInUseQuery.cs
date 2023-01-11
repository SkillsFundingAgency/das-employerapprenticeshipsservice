namespace SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;

public class GetPayeSchemeInUseQuery : IAsyncRequest<GetPayeSchemeInUseResponse>
{
    public string Empref { get; set; }
}