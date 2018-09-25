﻿using SFA.DAS.Configuration;
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
            For<ITaskApiConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<TaskApiConfiguration>("SFA.DAS.Tasks.Api")).Singleton();
            For<ITaskService>().Use<TaskService>();
        }
    }
}