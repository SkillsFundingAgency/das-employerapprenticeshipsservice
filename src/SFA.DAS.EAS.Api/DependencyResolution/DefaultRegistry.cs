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
using System.Linq;
using System.Reflection;
using System.Web;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Api.Logging;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using StructureMap;
using WebGrease.Css.Extensions;

namespace SFA.DAS.EAS.Api.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(ServiceName);

            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<ICache>().Use<InMemoryCache>();
            For<Domain.Interfaces.IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();

            RegisterHashingService(config);
            RegisterMapper();
            RegisterMediator();
            RegisterLogger();
        }

        private void RegisterHashingService(EmployerApprenticeshipsServiceConfiguration config)
        {
            For<IHashingService>().Use(c => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
            For<IPublicHashingService>().Use(x => new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring));
        }

        private void RegisterLogger()
        {
            For<ILog>().Use(c => new NLogLogger(c.ParentType, c.GetInstance<IRequestContext>(), null)).AlwaysUnique();
            For<IRequestContext>().Use(c => new RequestContext(new HttpContextWrapper(HttpContext.Current)));
        }

        private void RegisterMapper()
        {
            var apiProfiles = Assembly
                .Load($"{ServiceNamespace}.EAS.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var infrastructureProfiles = Assembly
                .Load($"{ServiceNamespace}.EAS.Infrastructure")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                apiProfiles.ForEach(c.AddProfile);
                infrastructureProfiles.ForEach(c.AddProfile);
            });

            var mapper = config.CreateMapper();

            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(mapper).Singleton();
        }

        private void RegisterMediator()
        {
            For<IMediator>().Use<Mediator>();
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(c => t => c.GetAllInstances(t));
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(c => t => c.GetInstance(t));
        }
    }
}