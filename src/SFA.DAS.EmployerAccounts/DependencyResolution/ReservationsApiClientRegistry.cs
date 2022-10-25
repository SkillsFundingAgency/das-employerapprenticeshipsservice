using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;
using System;
using System.Net.Http;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class ReservationsApiClientRegistry : Registry
    {
        public ReservationsApiClientRegistry()
        {
            For<ReservationsClientApiConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsConfiguration>().ReservationsApi);
            For<IReservationsClientApiConfiguration>().Use(c => c.GetInstance<ReservationsClientApiConfiguration>());
            For<IReservationsApiClient>().Use<ReservationsApiClient>().Ctor<HttpClient>().Is(c => CreateClient(c));
        }

        private HttpClient CreateClient(IContext context)
        {
            var config = context.GetInstance<ReservationsClientApiConfiguration>();

            HttpClient httpClient;

            if (config.UseStub)
            {
                httpClient = new HttpClient { BaseAddress = new Uri("https://sfa-stub-reservations.herokuapp.com/") };
            }
            else
            {                
                httpClient = new HttpClientBuilder()
               .WithHandler(new RequestIdMessageRequestHandler())
               .WithHandler(new SessionIdMessageRequestHandler())
               .WithDefaultHeaders()
               .Build();
            }

            return httpClient;
        }
    }
}
