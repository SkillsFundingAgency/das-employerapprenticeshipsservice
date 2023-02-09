namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

public class GetApprenticeshipsRequest : IRequest<GetApprenticeshipsResponse>
{
    public long AccountId { get; set; }
    public string ExternalUserId { get; set; }
}