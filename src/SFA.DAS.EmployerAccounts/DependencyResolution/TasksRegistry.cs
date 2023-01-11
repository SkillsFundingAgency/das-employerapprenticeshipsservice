using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Tasks.API.Client;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class TasksRegistry : Registry
{
    public TasksRegistry()
    {
        For<ITaskApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().TasksApi);
        For<ITaskService>().Use<TaskService>();
    }
}