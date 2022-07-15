using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Configuration
{
    public class TasksClientApiConfiguration : ITasksClientApiConfiguration
    {
        public string ApiBaseUrl { get; set; }       
        public string IdentifierUri { get; set; }
    }
}
