using SFA.DAS.EmployerAccounts.Models.Recruit;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class VacancyViewModel
{
    public string Title { get; set; }
    public VacancyStatus Status { get; set; }
    public string ManageVacancyUrl { get; set; }
    public string ClosingDateText { get; set; }
    public string ClosedDateText { get; set; }
    public int? NoOfNewApplications { get; set; }
    public int? NoOfSuccessfulApplications { get; set; }
    public int? NoOfUnsuccessfulApplications { get; set; }
    public int? NoOfApplications => NoOfNewApplications + NoOfSuccessfulApplications + NoOfUnsuccessfulApplications;
}