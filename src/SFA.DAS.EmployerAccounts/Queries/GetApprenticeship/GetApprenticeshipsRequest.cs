namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

public class GetApprenticeshipsRequest : IRequest<GetApprenticeshipsResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}