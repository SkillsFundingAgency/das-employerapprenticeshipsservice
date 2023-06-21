namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class AccountTaskListViewModel
    {
        public int HashedAccountId { get; set; }
        public bool HasPayeScheme { get; set; }

        public int CompletedSections { get { return 0; } }
    }
}
