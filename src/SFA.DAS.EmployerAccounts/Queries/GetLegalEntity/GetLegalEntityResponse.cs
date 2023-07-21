
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;

public class GetLegalEntityResponse
{
    public AccountLegalEntity LegalEntity { get; set; }
    public EmployerAgreement LatestAgreement { get ; set ; }
}