namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;

public class GetAccountLegalEntitiesCountByHashedAccountIdRequest : IAsyncRequest<GetAccountLegalEntitiesCountByHashedAccountIdResponse>
{
    public string HashedAccountId { get; set; }
}