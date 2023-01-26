using AutoMapper;
using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Logging;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.NLog.Logger;
using StructureMap;
using SFA.DAS.EmployerFinance.AcceptanceTests;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<ILoggingContext>().Use(c => HttpContextHelper.Current == null ? null : new LoggingContext(new HttpContextWrapper(HttpContextHelper.Current)));
            For<ITestTransactionRepository>().Use<TestTransactionRepository>();

            RegisterEmployerAccountTransactionsController();

            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });
        }

        private void RegisterEmployerAccountTransactionsController()
        {
            RegisterEmployerAccountTransactionsOrchestrator();

            For<EmployerAccountTransactionsController>().Use(c => new EmployerAccountTransactionsController(
                c.GetInstance<IAuthenticationService>(),
                c.GetInstance<EmployerAccountTransactionsOrchestrator>(), 
                c.GetInstance<IMapper>(), 
                c.GetInstance<IMediator>(),
                c.GetInstance<ILog>()));
        }

        private void RegisterEmployerAccountTransactionsOrchestrator()
        {
            For<EmployerAccountTransactionsOrchestrator>().Use(c => new EmployerAccountTransactionsOrchestrator(
                c.GetInstance<IAccountApiClient>(),
                c.GetInstance<IMediator>(),
                c.GetInstance<ICurrentDateTime>(), c.GetInstance<ILog>()));
        }
    }
}