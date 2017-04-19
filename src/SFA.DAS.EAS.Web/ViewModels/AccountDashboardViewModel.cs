using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class AccountDashboardViewModel
    {
        public Account Account { get; set; }
        public bool RequiresAgreementSigning { get; set; }
    }
}