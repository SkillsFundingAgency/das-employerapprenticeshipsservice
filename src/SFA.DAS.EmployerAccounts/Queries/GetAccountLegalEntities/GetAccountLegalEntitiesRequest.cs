namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;

public class GetAccountLegalEntitiesRequest : IRequest<GetAccountLegalEntitiesResponse>
{
    public string HashedLegalEntityId { get; set; }
    public string UserId { get; set; }
}