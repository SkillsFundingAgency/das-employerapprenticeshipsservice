using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.Tasks.API.Client;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class TasksRegistry : Registry
    {
        public TasksRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<TaskApiConfiguration>("SFA.DAS.Tasks.Api");

            For<ITaskApiConfiguration>().Use(config);
            For<ITaskService>().Use<TaskService>();
        }
    }
}