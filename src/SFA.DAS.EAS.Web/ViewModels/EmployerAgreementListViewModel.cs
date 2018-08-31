using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class EmployerAgreementListViewModel
    {
        public long AccountId { get; set; }
        public GetAccountEmployerAgreementsResponse EmployerAgreementsData { get; set; }
        public string HashedAccountId { get; set; }
    }
}