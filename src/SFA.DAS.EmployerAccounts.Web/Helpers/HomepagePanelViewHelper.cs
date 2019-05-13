using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public class HomepagePanelViewHelper : IHomepagePanelViewHelper
    {
        public string GetPanel1Action(AccountDashboardViewModel model)
        {
            string viewName = model.PayeSchemeCount == 0 ? "AddPAYE" : model.AgreementsToSign ? "SignAgreement" : "CheckFunding";

            return viewName;
        }
    }
}