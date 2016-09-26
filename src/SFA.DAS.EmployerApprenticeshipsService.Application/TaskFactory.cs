using System;
using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Application
{
    public static class TaskFactory
    {
        public static Task Create(long providerId, string taskName, string body)
        {
            var assignee = $"PROVIDER-{providerId}";

            return new Task
            {
                Assignee = assignee,
                Body = body,
                TaskTemplateId = 1,
                Name = taskName,
                CreatedOn = DateTime.UtcNow,
                TaskStatus = 0
            };
        }
    }
}
