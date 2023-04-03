using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EAS.Account.Api.Clients;
using SFA.DAS.TokenService.Api.Client;

using HashService = SFA.DAS.HashingService.HashingService;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Web.Services;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EAS.Support.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(ConfigureMvcOptions)
                .AddNewtonsoftJson(options =>
                {
                    options.UseMemberCasing();
                });

            var hashstringChars = Configuration.GetValue<string>("HashingService:AllowedHashstringCharacters");
            var hashstring = Configuration.GetValue<string>("HashingService:Hashstring");
            var employerAccApiClientConfig = Configuration.GetValue<EmployerAccountsApiClientConfiguration>("EmployerAccountsApiClientConfiguration");
            var tokenServiceApiClientConfig = Configuration.GetValue<TokenServiceApiClientConfiguration>("TokenServiceApiClientConfiguration");
            var hmrcApiClientConfig = Configuration.GetValue<HmrcApiClientConfiguration>("LevySubmission:HmrcApi");
            var accApiConfig = Configuration.GetValue<AccountApiConfiguration>("AccountApiConfiguration");

            services.AddSingleton<IHashingService, HashService>(c => new HashService(hashstringChars, hashstring));
            services.AddSingleton<IEmployerAccountsApiClientConfiguration, EmployerAccountsApiClientConfiguration>(c => employerAccApiClientConfig);
            services.AddSingleton<ITokenServiceApiClientConfiguration, TokenServiceApiClientConfiguration>(c => tokenServiceApiClientConfig);
            services.AddSingleton<IHmrcApiClientConfiguration, HmrcApiClientConfiguration>(c => hmrcApiClientConfig);
            services.AddSingleton<IAccountApiConfiguration, AccountApiConfiguration>(c => accApiConfig);

            services.AddSingleton<ISecureHttpClient, EmployerAccountsSecureHttpClient>();
            services.AddSingleton<ITokenServiceApiClient, TokenServiceApiClient>();
            services.AddSingleton<ILevyTokenHttpClientFactory, LevyTokenHttpClientMaker>();
            services.AddSingleton<ILevySubmissionsRepository, LevySubmissionsRepository>();
            services.AddSingleton<IPayeLevyMapper, PayeLevyMapper>();
            services.AddSingleton<IPayeLevySubmissionsHandler, PayeLevySubmissionsHandler>();
            services.AddSingleton<IPayeSchemeObfuscator, PayeSchemeObfuscator>();
            services.AddSingleton<IDatetimeService, DatetimeService>();
            services.AddSingleton<IAccountApiClient, AccountApiClient>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IAccountHandler, AccountHandler>();

            services.AddSingleton<IChallengeService, ChallengeService>();
            services.AddSingleton<IChallengeRepository, ChallengeRepository>();
            services.AddSingleton<IChallengeHandler, ChallengeHandler>();

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureMvcOptions(MvcOptions mvcOptions)
        { 
        }
    }
}
