namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesForAuthorisedUserQuery : IRequest<GetAccountPayeSchemesResponse>
{
    public long AccountId { get; set; }
    public string ExternalUserId { get; set; }
}