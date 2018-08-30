using System.Data;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions
{
    public static class ContainerExtensions
    {
        public static IContainer FinalizeEmployerAccountsTransactions(this IContainer container)
        {
            container.GetInstance<Data.EmployerAccountsDbContext>().Database.CurrentTransaction
                .Rollback();

            return container;
        }

        public static IContainer InitialiseAndUseEmployerAccountsDbContexts(this IContainer container)
        {
            var employerFinanceEmployerAccountsDbContext = container.GetInstance<Data.EmployerAccountsDbContext>();
            employerFinanceEmployerAccountsDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            container.Configure(c =>
            {
                c.For<Data.EmployerAccountsDbContext>().Use(employerFinanceEmployerAccountsDbContext);
            });

            return container;
        }

        public static IContainer FinalizeTransactions(this IContainer container)
        {
            container.GetInstance<EAS.Infrastructure.Data.EmployerAccountsDbContext>().Database.CurrentTransaction
                .Rollback();
            container.GetInstance<EAS.Infrastructure.Data.EmployerFinanceDbContext>().Database.CurrentTransaction
                .Rollback();

            return container;
        }

        public static IContainer InitialiseAndUseInfrastructureDbContexts(this IContainer container)
        {
            container
                .InitialiseAndUseInfrastructureEmployerAccountsDbContext()
                .InitialiseAndUseInfrastructureEmployerFinanceDbContext();

            return container;
        }

        public static IContainer InitialiseAndUseInfrastructureEmployerAccountsDbContext(this IContainer container)
        {
            var infrasctuctureEmployerAccountsDbContext = container.GetInstance<EAS.Infrastructure.Data.EmployerAccountsDbContext>();
            infrasctuctureEmployerAccountsDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            container.Configure(c =>
            {
                c.For<EAS.Infrastructure.Data.EmployerAccountsDbContext>().Use(infrasctuctureEmployerAccountsDbContext);
            });

            return container;
        }

        public static IContainer InitialiseAndUseInfrastructureEmployerFinanceDbContext(this IContainer container)
        {
            var infrasctuctureEmployerFinanceDbContext = container.GetInstance<EAS.Infrastructure.Data.EmployerFinanceDbContext>();
            infrasctuctureEmployerFinanceDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            container.Configure(c =>
            {
                c.For<EAS.Infrastructure.Data.EmployerFinanceDbContext>().Use(infrasctuctureEmployerFinanceDbContext);
            });

            return container;
        }
    }
}
