using SFA.DAS.EmployerAccounts.TasksApi;

namespace SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;

public class DismissMonthlyTaskReminderCommand : IRequest
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
    public TaskType TaskType { get; set; }
}