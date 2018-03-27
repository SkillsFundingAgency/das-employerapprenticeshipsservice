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
using System.Net.Http;
using System.Reflection;
using System.Web;
using AutoMapper;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Audit.Client;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Factories;
using SFA.DAS.EAS.Infrastructure.Interfaces.REST;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Infrastructure.Services.Features;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Client.Configuration;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Tasks.API.Client;
using StructureMap;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using WebGrease.Css.Extensions;

namespace SFA.DAS.EAS.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            var config = GetConfiguration();
            var notificationsApiConfig = Infrastructure.DependencyResolution.ConfigurationHelper.GetConfiguration<Domain.Configuration.NotificationsApiClientConfiguration>($"{ServiceName}.Notifications");
            var taskApiConfig = Infrastructure.DependencyResolution.ConfigurationHelper.GetConfiguration<TaskApiConfiguration>($"SFA.DAS.Tasks.Api");

            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(c => c.Singleton());
            });

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For<IApprenticeshipApi>().Use<ApprenticeshipApi>().Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);
            For<Domain.Interfaces.IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
            For<IEventsApi>().Use<EventsApi>().Ctor<IEventsApiClientConfiguration>().Is(config.EventsApi).SelectConstructor(() => new EventsApi(null));
            For<IEmployerCommitmentApi>().Use<EmployerCommitmentApi>().Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);
            For<IHashingService>().Use(x => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
            For<IAuthorizationService>().Use<AuthorizationService>();
            For<IPublicHashingService>().Use(x => new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring));
            For<ITaskApiConfiguration>().Use(taskApiConfig);
            For<ITaskService>().Use<TaskService>();
            For<IUserRepository>().Use<UserRepository>();
            For<IValidationApi>().Use<ValidationApi>().Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);

            RegisterMapper();
            RegisterMediator();
            ReisterNotificationsApi(notificationsApiConfig);
            RegisterAuditService();
            RegisterPostCodeAnywhereService();
            RegisterExecutionPolicies();
            RegisterLogger();
            RegisterAuthorisationPipeline();
        }

        private EmployerApprenticeshipsServiceConfiguration GetConfiguration()
        {
            if (ConfigurationHelper.IsAnyOf(DasEnvironment.Local, DasEnvironment.AT, DasEnvironment.Test))
            {
                PopulateSystemDetails(ConfigurationHelper.CurrentEnvironmentName);
            }

            var configuration = ConfigurationHelper.GetConfigForService<EmployerApprenticeshipsServiceConfiguration>(ServiceName);

            return configuration;
        }

        private IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;

            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }

            return configurationRepository;
        }

        private void PopulateSystemDetails(string envName)
        {
            SystemDetailsViewModel.EnvironmentName = envName;
            SystemDetailsViewModel.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void RegisterAuditService()
        {

            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();

            if (ConfigurationHelper.IsAnyOf(DasEnvironment.Local))
            {
                For<IAuditApiClient>().Use<StubAuditApiClient>();
            }
            else
            {
                For<IAuditApiClient>().Use<AuditApiClient>();
            }
        }

        private void RegisterExecutionPolicies()
        {
            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.CompaniesHouseExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.CompaniesHouseExecutionPolicy.Name);

            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.HmrcExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.HmrcExecutionPolicy.Name);

            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.IdamsExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.IdamsExecutionPolicy.Name);
        }

        private void RegisterLogger()
        {
            For<IWebLoggingContext>().Use(c => new WebLoggingContext(new HttpContextWrapper(HttpContext.Current)));
            For<ILog>().Use(c => new NLogLogger(c.ParentType, c.GetInstance<IWebLoggingContext>(), null)).AlwaysUnique();
        }

        private void RegisterMapper()
        {
            var profiles = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName.StartsWith("SFA.DAS.EAS"))
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Profile).IsAssignableFrom(t) && t.IsConcrete() && t.HasConstructors())
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            var mapper = config.CreateMapper();

            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(mapper).Singleton();
        }

        private void RegisterMediator()
        {
            For<IMediator>().Use<Mediator>();
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
        }

        private void ReisterNotificationsApi(Domain.Configuration.NotificationsApiClientConfiguration config)
        {
            HttpClient httpClient;

            if (string.IsNullOrWhiteSpace(config.ClientId))
            {
                httpClient = new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config)).Build();
            }
            else
            {
                httpClient = new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(config)).Build();
            }

            For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(httpClient);
            For<INotificationsApiClientConfiguration>().Use(config);
        }

        private void RegisterPostCodeAnywhereService()
        {
            For<IAddressLookupService>().Use<AddressLookupService>();
            For<IRestClientFactory>().Use<RestClientFactory>();
            For<IRestServiceFactory>().Use<RestServiceFactory>();
        }

        private void RegisterAuthorisationPipeline()
        {
            For<IOperationAuthorisationHandler[]>().Use(ctx => new IOperationAuthorisationHandler[]
            {
                // The order of the types specified here is the order in which the handlers will be executed. 
                ctx.GetInstance<FeatureToggleAuthorisationHandler>(),
                ctx.GetInstance<AgreementFeatureAuthorisationHandler>()
            }).Singleton();

            For<IOperationAuthorisationHandler>().Use<OperationAuthorisation>().Singleton();
            For<IFeatureService>().Use<FeatureService>().Singleton();
            For<IFeatureCache>().Use<FeatureCache>().Singleton();
            For<IControllerMetaDataService>().Use(() => new ControllerMetaDataService(typeof(HomeController).Assembly)).Singleton();
            For<IFeatureCache>().Use<FeatureCache>().Singleton();
            For<IAccountAgreementService>().Use<AccountAgreementService>().Singleton();
        }
    }
}