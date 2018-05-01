using System;
using MediatR;
using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EAS.Application.Commands.DismissMonthlyTaskReminder
{
    public class DismissMonthlyTaskReminderCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
        public TaskType TaskType { get; set; }
    }
}
