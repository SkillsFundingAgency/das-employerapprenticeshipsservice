using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi
{
    [ExcludeFromCodeCoverage]
    public abstract class GivenEmployerAccountsApi
    {
        private readonly WebApplicationFactory<Program> _factory;
        protected HttpResponseMessage Response;

        protected GivenEmployerAccountsApi()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((hostingContext, config) =>
                  {
                      config.Sources.Clear();
                      config.SetBasePath(Directory.GetCurrentDirectory());
                      config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);

                      hostingContext.HostingEnvironment.EnvironmentName = "TEST";
                  });

                    builder.ConfigureTestServices(services =>
                    {
                        services.AddMvc(options =>
                        {
                            options.Filters.Add(new AllowAnonymousFilter());
                        });

                        services.AddEntityFrameworkUnitOfWork<EmployerAccountsDbContext>();
                        services.AddNServiceBusClientUnitOfWork();
                    });
                });
        }

        protected HttpClient BuildClient()
        {
            return _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost:44330"),
            });
        }

        //private HttpServer _server;
        //private ServiceBusEndPointConfigureAndRun _serviceBusEndpointManager;

        //private IContainer _container;

        //[SetUp]
        //public void Startup()
        //{
        //    //var config = new HttpConfiguration();

        //    //config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

        //    //WebApiConfig.Register(config);

        //    //_container = config.DependencyResolver.GetService(typeof(IContainer)) as IContainer;

        //    //_serviceBusEndpointManager = new ServiceBusEndPointConfigureAndRun(_container);

        //    //_serviceBusEndpointManager.ConfigureAndStartServiceBusEndpoint();

        //    //_server = new HttpServer(config);
        //}

        protected void WhenControllerActionIsCalled(string uri)
        {
            using var client = BuildClient();

            Response = client.GetAsync(uri).Result;
        }

        protected async Task InitialiseEmployerAccountData(Func<EmployerAccountsDbBuilder, Task> initialiseAction)
        {
            var builder = new EmployerAccountsDbBuilder(_factory.Services);

            builder.BeginTransaction();
            try
            {
                await initialiseAction(builder);
                builder.CommitTransaction();
            }
            catch (Exception)
            {
                builder.RollbackTransaction();
                throw;
            }
        }
    }
}