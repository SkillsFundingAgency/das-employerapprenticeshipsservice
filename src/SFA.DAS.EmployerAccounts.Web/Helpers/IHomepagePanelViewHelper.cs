using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public interface INextActionPanelViewHelper
    {
        PanelViewModel<AccountDashboardViewModel> GetNextAction(AccountDashboardViewModel model);
    }
}