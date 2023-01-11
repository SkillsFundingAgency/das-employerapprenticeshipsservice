namespace SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;

public class GetSingleCohortRequest : IAsyncRequest<GetSingleCohortResponse>
{
    public string HashedAccountId { get; set; }        
    public string ExternalUserId { get; set; }
}