namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class AccountTaskListViewModel
    {
        public string HashedAccountId { get; set; }
        public bool HasPayeScheme { get; set; }

        public int CompletedSections
        {
            get
            {
                // by default, will have 1 completed section for user details (step previous)
                return HasPayeScheme ? 2 : 1;
            }
        }
    }
}
