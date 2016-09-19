using SFA.DAS.Tasks.Domain.Entities;

namespace SFA.DAS.EmployerApprenticeshipsService.Application
{
    public static class TaskFactory
    {
        public static Task Create(long providerId, string body)
        {
            var assignee = $"PROVIDER-{providerId}";

            return new Task
            {
                Assignee = assignee,
                Body = body,
                TaskTemplateId = 1 //TODO: LWA - What should this be?
            };
        }
    }
}
