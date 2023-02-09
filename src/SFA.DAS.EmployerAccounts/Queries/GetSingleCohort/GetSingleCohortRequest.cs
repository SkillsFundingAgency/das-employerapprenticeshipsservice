namespace SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;

public class GetSingleCohortRequest : IRequest<GetSingleCohortResponse>
{
    public long AccountId { get; set; }        
    public string ExternalUserId { get; set; }
}