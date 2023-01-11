namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class SearchPensionRegulatorByAornViewModel : ViewModelBase
{
    public string Aorn { get; set; }
    public string PayeRef { get; set; }
    public int RemainingAttempts { get; set; }
    public int AllowedAttempts { get; set; }
    public int RemainingLock { get; set; }
    public bool IsLocked { get; set; }

    public string AornError => GetErrorMessage(nameof(Aorn));
    public string PayeRefError => GetErrorMessage(nameof(PayeRef));
}