// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EAS.Api.Models;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;
using WebGrease.Css.Extensions;
using IConfiguration = SFA.DAS.EAS.Domain.Interfaces.IConfiguration;

namespace SFA.DAS.EAS.Api.DependencyResolution {
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
	
    public class DefaultRegistry : Registry {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {

            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();

            //For<ICommitmentsApi>().Use<CommitmentsApi>().Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);
            //For<ITasksApi>().Use<TasksApi>().Ctor<ITasksApiClientConfiguration>().Is(config.TasksApi);
            RegisterMapper();
            RegisterMediator();
        }
        

        private void RegisterMediator()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }

        private void RegisterMapper()
        {
            var profiles = Assembly.Load($"{ServiceNamespace}.EAS.Infrastructure").GetTypes()
                            .Where(t => typeof(Profile).IsAssignableFrom(t))
                            .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(cfg =>
            {
                profiles.ForEach(cfg.AddProfile);
            });

            var mapper = config.CreateMapper();

            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(mapper).Singleton();
        }
        
    }
}