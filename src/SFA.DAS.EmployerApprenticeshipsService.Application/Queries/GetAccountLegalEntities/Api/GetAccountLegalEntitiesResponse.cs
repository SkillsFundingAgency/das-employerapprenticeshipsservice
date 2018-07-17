using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities.Api
{
    public class GetAccountLegalEntitiesResponse
    {
        public PagedApiResponseViewModel<AccountLegalEntityViewModel> AccountLegalEntities { get; set; }
    }
}