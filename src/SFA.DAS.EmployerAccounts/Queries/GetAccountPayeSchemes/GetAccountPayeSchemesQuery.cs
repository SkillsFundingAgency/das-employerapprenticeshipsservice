namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesQuery : IRequest<GetAccountPayeSchemesResponse>
{
    public string HashedAccountId { get; set; }
}