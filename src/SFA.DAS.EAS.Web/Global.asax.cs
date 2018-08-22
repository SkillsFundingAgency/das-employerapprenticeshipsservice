﻿using FluentValidation.Mvc;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using NLog;
using NLog.Targets;
using NServiceBus;
using SFA.DAS.Audit.Client;
using SFA.DAS.Audit.Client.Web;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.Web.Policy;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.NServiceBus;
using SFA.DAS.EAS.Web.App_Start;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.EntityFramework;
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.NServiceBus.Mvc;
using SFA.DAS.NServiceBus.NewtonsoftSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;
using Environment = SFA.DAS.EAS.Infrastructure.DependencyResolution.Environment;

namespace SFA.DAS.EAS.Web
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
#pragma warning disable 169
        private static readonly RedisTarget RedisTarget; // Required to ensure assembly is copied to output.
#pragma warning restore 169

        private IEndpointInstance _endpoint;

        protected void Application_Start()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            AreaRegistration.RegisterAllAreas();
            BinderConfig.RegisterBinders(ModelBinders.Binders);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            FluentValidationModelValidatorProvider.Configure();
            LoggingConfig.ConfigureLogging();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");
            WebMessageBuilders.Register();
            WebMessageBuilders.UserIdClaim = DasClaimTypes.Id;
            WebMessageBuilders.UserEmailClaim = DasClaimTypes.Email;

            AuditMessageFactory.RegisterBuilder(m =>
            {
                m.Source = new Source
                {
                    Component = "EAS-Web",
                    System = "EAS",
                    Version = typeof(MvcApplication).Assembly.GetName().Version.ToString()
                };
            });

            if (ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local, Environment.At, Environment.Test))
            {
                SystemDetailsViewModel.EnvironmentName = ConfigurationHelper.CurrentEnvironmentName;
                SystemDetailsViewModel.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            StartServiceBusEndpoint();
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            new HttpContextPolicyProvider(new List<IHttpContextPolicy> { new ResponseHeaderRestrictionPolicy() })
                .Apply(new HttpContextWrapper(HttpContext.Current), PolicyConcern.HttpResponse);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            if (exception is HttpException httpException && httpException.GetHttpCode() == (int)HttpStatusCode.NotFound)
            {
                return;
            }

            Dictionary<string, object> properties = null;

            try
            {
                properties = new Dictionary<string, object>
                {
                    ["HttpMethod"] = Request.HttpMethod,
                    ["IsAuthenticated"] = Request.IsAuthenticated,
                    ["Url"] = Request.Url.PathAndQuery,
                    ["UrlReferrer"] = Request.UrlReferrer?.PathAndQuery
                };
            }
            catch (Exception)
            {
                // Request not available
            }

            var message = exception.GetMessage();
            var telemetryClient = new TelemetryClient();

            Logger.Error(exception, message, properties);
            telemetryClient.TrackException(exception);
        }

        protected void Application_End()
        {
            StopServiceBusEndpoint();
        }

        private void StartServiceBusEndpoint()
        {
            var container = StructuremapMvc.StructureMapDependencyScope.Container;

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.EAS.Web")
                .SetupAzureServiceBusTransport(container.GetInstance<EmployerApprenticeshipsServiceConfiguration>().ServiceBusConnectionString)
                .SetupEntityFrameworkUnitOfWork<EmployerAccountsDbContext>()
                .SetupErrorQueue()
                .SetupInstallers()
                .SetupMsSqlServerPersistence(() => container.GetInstance<DbConnection>())
                .SetupNewtonsoftSerializer()
                .SetupNLogFactory()
                .SetupOutbox(GlobalFilters.Filters)
                .SetupStructureMapBuilder(container);

            _endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            container.Configure(c =>
            {
                c.For<IMessageSession>().Use(_endpoint);
            });
        }

        private void StopServiceBusEndpoint()
        {
            _endpoint?.Stop().GetAwaiter().GetResult();
        }
    }
}