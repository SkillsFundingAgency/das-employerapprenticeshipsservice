using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public class HomepagePanelViewHelper : IHomepagePanelViewHelper
    {
        public PanelViewModel<AccountDashboardViewModel> GetPanel1Action(AccountDashboardViewModel model)
        {
            string viewName = null;
            viewName = model.AgreementsToSign ? "SignAgreement" : "CheckFunding";

            return new PanelViewModel<AccountDashboardViewModel> { ViewName = viewName, Data = model };
        }
    }
}