using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Authentication;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi
{
    [ExcludeFromCodeCoverage]
    public abstract class GivenEmployerAccountsApi
    {
        private readonly WebApplicationFactory<Program> _factory;
        protected HttpResponseMessage? Response;
        
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
                  });
                });
        }

        protected HttpClient BuildClient(bool authenticated = true)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMvc(options =>
                    {
                        if (!authenticated)
                            options.Filters.Add(new AllowAnonymousFilter());

                        if (authenticated)
                        {
                            services.Configure<TestAuthenticationOptions>(opt =>{});
                            services.AddAuthentication(opt =>
                                {
                                    opt.DefaultScheme = TestAuthHandler.AuthenticationScheme;
                                    opt.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                                    opt.DefaultScheme = TestAuthHandler.AuthenticationScheme;
                                    opt.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                                })
                                .AddTestAuthentication(TestAuthHandler.AuthenticationScheme, "Test Auth", o => { });
                        }
                    });

                    services.AddEntityFrameworkUnitOfWork<EmployerAccountsDbContext>();
                    services.AddNServiceBusClientUnitOfWork();
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost:44330"),
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: TestAuthHandler.AuthenticationScheme);

            return client;
        }

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