using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Tasks.API.Client;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class TasksRegistry : Registry
    {
        public TasksRegistry()
        {
            For<ITaskApiConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<TaskApiConfiguration>(ConfigurationKeys.TaskApi)).Singleton();
        }
    }
}