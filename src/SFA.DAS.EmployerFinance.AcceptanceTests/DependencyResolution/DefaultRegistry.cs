using System.Web;
using AutoMapper;
using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.AcceptanceTests.Helpers;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Time;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Logging;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<ILoggingContext>().Use(c => HttpContext.Current == null ? null : new LoggingContext(new HttpContextWrapper(HttpContext.Current)));
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
                c.GetInstance<IMediator>(),
                c.GetInstance<ICurrentDateTime>(), c.GetInstance<ILog>()));
        }
    }
}