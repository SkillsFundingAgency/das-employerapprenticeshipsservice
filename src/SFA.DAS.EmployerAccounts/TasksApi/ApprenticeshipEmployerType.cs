namespace SFA.DAS.EmployerAccounts.TasksApi;

[Flags]
public enum ApprenticeshipEmployerType
{
    None = 0,
    Levy = 1,
    NonLevy = 1 << 1,
    All = ~None
}