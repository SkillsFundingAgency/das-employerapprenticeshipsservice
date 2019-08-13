using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api
{
    public class GetAccountLegalEntitiesResponse
    {
        public PagedApiResponseViewModel<AccountLegalEntityViewModel> AccountLegalEntities { get; set; }
    }
}