namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;

public class GetAccountLegalEntitiesCountByHashedAccountIdRequest : IRequest<GetAccountLegalEntitiesCountByHashedAccountIdResponse>
{
    public string HashedAccountId { get; set; }
}