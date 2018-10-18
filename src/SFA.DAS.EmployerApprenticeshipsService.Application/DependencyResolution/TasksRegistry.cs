using SFA.DAS.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.Tasks.API.Client;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class TasksRegistry : Registry
    {
        public TasksRegistry()
        {
            For<ITaskApiConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<TaskApiConfiguration>("SFA.DAS.Tasks.Api")).Singleton();
            For<ITaskService>().Use<TaskService>();
        }
    }
}