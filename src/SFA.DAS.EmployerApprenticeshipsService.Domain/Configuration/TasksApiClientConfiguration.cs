using SFA.DAS.Tasks.Api.Client.Configuration;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class TasksApiClientConfiguration : ITasksApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}