namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;

public class GetAccountLegalEntitiesByHashedAccountIdRequest : IAsyncRequest<GetAccountLegalEntitiesByHashedAccountIdResponse>
{
    public string HashedAccountId { get; set; }
}