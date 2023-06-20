using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;

public class GetAccountLegalEntitiesResponse
{
    public List<AccountSpecificLegalEntity> LegalEntities { get; set; }
}