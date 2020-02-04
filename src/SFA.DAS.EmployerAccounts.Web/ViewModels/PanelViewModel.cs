namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class PanelViewModel<T>
    {
        public string ViewName { get; set; }
        public bool FeaturedPanel { get; set; }
        public T Data { get; set; }
    }
}   