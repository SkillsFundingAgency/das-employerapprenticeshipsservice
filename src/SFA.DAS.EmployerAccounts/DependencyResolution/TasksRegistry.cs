using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Tasks.API.Client;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class TasksRegistry : Registry
    {
        public TasksRegistry()
        {
            For<ITaskApiConfiguration>().Use<TaskApiConfiguration>();
            For<TaskApiConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<TaskApiConfiguration>(ConfigurationKeys.TasksApi)).Singleton();

            For<ITaskService>().Use<TaskService>();
        }
    }
}