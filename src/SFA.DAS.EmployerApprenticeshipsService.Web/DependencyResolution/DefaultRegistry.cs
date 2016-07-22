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
using System.IO;
using System.Web;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Web.Pipeline;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.DependencyResolution {
    
    public class DefaultRegistry : Registry {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        
        #region Constructors and Destructors

        public DefaultRegistry() {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                    scan.WithDefaultConventions();
                });
            
            For<IOwinWrapper>().Transient().Use(() => new OwinWrapper(HttpContext.Current.GetOwinContext())).SetLifecycleTo(new HttpContextLifecycle());

            For<IUserRepository>().Use<FileSystemUserRepository>();

            var configurationRepository = GetConfigurationRepository();
            
            RegisterMessageQueues(configurationRepository, environment);

            RegisterMediator();
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository =
                    new AzureTableStorageConfigurationRepository(
                        CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }

        private void RegisterMessageQueues(IConfigurationRepository configurationRepository, string environment)
        {
            var configurationService = new ConfigurationService(
                configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));
            For<IConfigurationService>().Use(configurationService);

            var config = configurationService.Get<EmployerApprenticeshipsServiceConfiguration>();
            if (string.IsNullOrEmpty(config.ServiceBusConnectionString))
            {
                var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                For<IMessagePublisher>().Use(() => new Messaging.FileSystem.FileSystemMessageService(Path.Combine(queueFolder, nameof(QueueNames.das_at_eas_get_employer_levy))));
            }
            else
            {
                For<IMessagePublisher>().Use(() => new AzureServiceBusMessageService(config.ServiceBusConnectionString, nameof(QueueNames.das_at_eas_get_employer_levy)));
            }
        }

        private void RegisterMediator()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }
        #endregion
    }
}