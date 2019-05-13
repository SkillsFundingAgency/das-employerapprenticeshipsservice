using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public interface IHomepagePanelViewHelper
    {
        string GetPanel1Action(AccountDashboardViewModel model);
    }
}