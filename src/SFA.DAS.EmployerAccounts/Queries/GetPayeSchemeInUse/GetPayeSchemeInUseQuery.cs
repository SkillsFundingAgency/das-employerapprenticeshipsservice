namespace SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;

public class GetPayeSchemeInUseQuery : IRequest<GetPayeSchemeInUseResponse>
{
    public string Empref { get; set; }
}