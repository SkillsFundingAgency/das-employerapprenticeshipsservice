namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesForAuthorisedUserQuery : IRequest<GetAccountPayeSchemesResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}