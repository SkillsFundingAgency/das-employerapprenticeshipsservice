using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.LegalEntities;

namespace SFA.DAS.EmployerAccounts.Queries.GetPagedAccountLegalEntities
{
    public class GetPagedAccountLegalEntitiesResponse
    {
        public PagedApiResponseViewModel<AccountLegalEntityViewModel> AccountLegalEntities { get; set; }
    }
}