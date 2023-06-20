namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;

public class GetAccountLegalEntitiesByHashedAccountIdRequest : IRequest<GetAccountLegalEntitiesByHashedAccountIdResponse>
{
    public string HashedAccountId { get; set; }
}