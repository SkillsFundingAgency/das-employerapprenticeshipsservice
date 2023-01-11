using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;

public class DismissMonthlyTaskReminderCommand : IAsyncRequest
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
    public TaskType TaskType { get; set; }
}