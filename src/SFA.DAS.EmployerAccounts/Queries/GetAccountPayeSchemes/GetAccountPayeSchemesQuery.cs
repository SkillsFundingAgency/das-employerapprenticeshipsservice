namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesQuery : IRequest<GetAccountPayeSchemesResponse>
{
    public long AccountId { get; set; }
}