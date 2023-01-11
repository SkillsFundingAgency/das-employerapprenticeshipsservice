namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

public class GetApprenticeshipsRequest : IAsyncRequest<GetApprenticeshipsResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}