using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class EmployerAgreementListViewModel
{
    public long AccountId { get; set; }
    public GetAccountEmployerAgreementsResponse EmployerAgreementsData { get; set; }
    public string HashedAccountId { get; set; }
}