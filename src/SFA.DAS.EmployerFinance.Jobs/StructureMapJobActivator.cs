﻿using Microsoft.Azure.WebJobs.Host;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Jobs
{
    public class StructureMapJobActivator : IJobActivator
    {
        private readonly IContainer _container;

        public StructureMapJobActivator(IContainer container)
        {
            _container = container;
        }

        public T CreateInstance<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}