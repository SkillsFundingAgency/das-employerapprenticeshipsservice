﻿using MediatR;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class MediatorRegistry : Registry
    {
        public MediatorRegistry()
        {
            For<IMediator>().Use<Mediator>();
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(c => c.GetAllInstances);
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(c => c.GetInstance);
        }
    }
}