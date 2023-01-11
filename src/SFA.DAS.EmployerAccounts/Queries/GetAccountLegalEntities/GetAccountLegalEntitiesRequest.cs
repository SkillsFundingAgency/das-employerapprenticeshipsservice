namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;

public class GetAccountLegalEntitiesRequest : IAsyncRequest<GetAccountLegalEntitiesResponse>
{
    public string HashedLegalEntityId { get; set; }
    public string UserId { get; set; }
}