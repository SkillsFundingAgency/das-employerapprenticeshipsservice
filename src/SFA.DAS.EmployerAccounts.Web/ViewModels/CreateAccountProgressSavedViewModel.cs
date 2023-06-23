using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class CreateAccountProgressSavedViewModel
    {
        public string HashedAccountId { get; set; }
        public string ContinueTaskListRouteName => string.IsNullOrEmpty(HashedAccountId) ? RouteNames.NewEmpoyerAccountTaskList : RouteNames.ContinueNewEmployerAccountTaskList;
    }
}
