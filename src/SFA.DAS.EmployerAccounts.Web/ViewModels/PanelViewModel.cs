namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class PanelViewModel<T>
{
    public string ComponentName { get; set; }
    public PanelType PanelType { get; set; } = PanelType.Interruption;
    public T Data { get; set; }
}

public enum PanelType
{
    Interruption,
    Summary,
    Action
}