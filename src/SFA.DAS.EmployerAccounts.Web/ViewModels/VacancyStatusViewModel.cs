namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class VacancyStatusViewModel
    {
        public string VacancyTitle { get; set; }
        public string Status { get; set; }
        public int? NumberOfApplications { get; set; }
        public string Reference { get; set; }
        public string ClosingDateText { get; set; }
        public string ManageVacancyLinkUrl { get; set; }
        public string ManageVacancyLinkText { get; set; }
    }
}