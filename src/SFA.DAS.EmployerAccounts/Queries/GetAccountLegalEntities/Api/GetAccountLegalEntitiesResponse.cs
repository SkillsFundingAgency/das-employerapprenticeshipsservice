using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;

public class GetAccountLegalEntitiesResponse
{
    public PagedApiResponse<AccountLegalEntity> AccountLegalEntities { get; set; }
}