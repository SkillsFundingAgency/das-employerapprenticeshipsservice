using SFA.DAS.Authorization;
using SFA.DAS.EAS.Portal.Types;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class NewHomepageViewModel
    {
        public Account Account { get; set; }
        public EmulatedFundingViewModel EmulatedFundingViewModel { get; set; }
        public Role UserRole { get; set; }
        public bool AgreementsToSign => Account.Organisations.FirstOrDefault() != null && Account.Organisations.FirstOrDefault().Agreements.Count(a => a.IsPending) > 0;
    }
}