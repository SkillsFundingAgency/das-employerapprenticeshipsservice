﻿using System;
using MediatR;
using Microsoft.Azure;
using Moq;
using SFA.DAS.Audit.Client;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Web.Authentication;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.EAS.Web.AcceptanceTests.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry(Mock<IOwinWrapper> owinWrapperMock, Mock<ICookieService> cookieServiceMock)
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
                
            });
            
            For<IUserRepository>().Use<UserRepository>();

            For<IOwinWrapper>().Use(() => owinWrapperMock.Object);

            For<ICookieService>().Use(() => cookieServiceMock.Object);
            
            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            
            AddMediatrRegistrations();

            RegisterAuditService();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }

        private void RegisterAuditService()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();

            if (environment.Equals("LOCAL"))
            {
                For<IAuditApiClient>().Use<StubAuditApiClient>();
            }
            else
            {
                For<IAuditApiClient>().Use<AuditApiClient>();
            }

        }

    }
}
