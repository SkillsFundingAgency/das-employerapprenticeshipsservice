namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class ApprenticeDetailsViewModel
{
    public string ApprenticeName { get; set; }
    public string TrainingProviderName { get; set; }
    public string CourseName { get; set; }
    public string StartDateText { get; set; }
    public string EndDateText { get; set; }
    public string ProposedCostText { get; set; }
    public bool IsApproved { get; set; }
}