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
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.Messaging;
using StructureMap.Web.Pipeline;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.DependencyResolution {
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
	
    public class DefaultRegistry : Registry {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string DevEnv = "LOCAL";

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
                    scan.WithDefaultConventions();
                    scan.AssemblyContainingType<IEmployerVerificationService>();
                    scan.AssemblyContainingType<GetUsersQuery>();
                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                });
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IOwinWrapper>().Transient().Use(() => new OwinWrapper(HttpContext.Current.GetOwinContext())).SetLifecycleTo(new HttpContextLifecycle());

            IConfigurationRepository configurationRepository;
            

            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }

            var configurationService = new ConfigurationService(
                configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));
            For<IConfigurationService>().Use(configurationService);

            var config = configurationService.Get<EmployerApprenticeshipsServiceConfiguration>();

            var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            For<IMessagePublisher>().Use(() => new Messaging.FileSystem.FileSystemMessageService(Path.Combine(queueFolder, "GetEmployerLevyQueue")));

            For<IEmployerVerificationService>().Use<CompaniesHouseEmployerVerificationService>().Ctor<string>().Is(config.CompaniesHouse.ApiKey);
            For<IUserAccountRepository>().Use<UserAccountRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            For<IAccountRepository>().Use<AccountRepository>().Ctor<string>().Is(config.Employer.DatabaseConnectionString);
            var appData = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            For<IUserRepository>().Use<FileSystemUserRepository>().Ctor<string>().Is(appData);
			For<ICookieService>().Use<HttpCookieService>();
            For<IMediator>().Use<Mediator>();
        }

        #endregion
    }
}