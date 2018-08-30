using System.Data;
using NServiceBus;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class ContainerExtensions
    {
        //public static IContainer InitialiseDatabaseData(this IContainer container)
        //{
        //    container.GetInstance<ITestTransactionRepository>().InitialiseDatabaseData();

        //    return container;
        //}

        public static IContainer ConfigureISession(this IContainer container,
            IEndpointInstance initiateJobServiceBusEndpoint)
        {
            container.Configure(c => { c.For<IMessageSession>().Use(initiateJobServiceBusEndpoint); });

            return container;
        }

        public static IContainer FinalizeEmployerFinanceTransactions(this IContainer container)
        {
            container.GetInstance<Data.EmployerAccountsDbContext>().Database.CurrentTransaction
                .Rollback();
            container.GetInstance<Data.EmployerFinanceDbContext>().Database.CurrentTransaction
                .Rollback();

            return container;
        }

        public static IContainer InitialiseAndUseEmployerFinanceDbContexts(this IContainer container)
        {
            container
                .InitialiseAndUseEmployerFinanceEmployerAccountsDbContext()
                .InitialiseAndUseEmployerFinanceEmployerFinanceDbContext();

            return container;
        }

        public static IContainer InitialiseAndUseEmployerFinanceEmployerAccountsDbContext(this IContainer container)
        {
            var employerFinanceEmployerAccountsDbContext = container.GetInstance<Data.EmployerAccountsDbContext>();
            employerFinanceEmployerAccountsDbContext.Database.CurrentTransaction?.Rollback();
            employerFinanceEmployerAccountsDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            container.Configure(c =>
            {
                c.For<Data.EmployerAccountsDbContext>().Use(employerFinanceEmployerAccountsDbContext);
            });

            return container;
        }

        public static IContainer InitialiseAndUseEmployerFinanceEmployerFinanceDbContext(this IContainer container)
        {
            var employerFinanceEmployerFinanceDbContext = container.GetInstance<Data.EmployerFinanceDbContext>();
            employerFinanceEmployerFinanceDbContext.Database.CurrentTransaction?.Rollback();
            employerFinanceEmployerFinanceDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            container.Configure(c =>
            {
                c.For<Data.EmployerFinanceDbContext>().Use(employerFinanceEmployerFinanceDbContext);
            });

            return container;
        }

        public static IContainer FinalizeTransactions(this IContainer container)
        {
            //container.GetInstance<EAS.Infrastructure.Data.EmployerAccountsDbContext>().Database.CurrentTransaction
            //    .Rollback();
            //container.GetInstance<EAS.Infrastructure.Data.EmployerFinanceDbContext>().Database.CurrentTransaction
            //    .Rollback();

            return container;
        }

        //public static IContainer InitialiseAndUseInfrastructureDbContexts(this IContainer container)
        //{
        //    container
        //        .InitialiseAndUseInfrastructureEmployerAccountsDbContext()
        //        .InitialiseAndUseInfrastructureEmployerFinanceDbContext();

        //    return container;
        //}


        //public static IContainer InitialiseAndUseInfrastructureEmployerAccountsDbContext(this IContainer container)
        //{
        //    var infrasctuctureEmployerAccountsDbContext = container.GetInstance<EAS.Infrastructure.Data.EmployerAccountsDbContext>();
        //    infrasctuctureEmployerAccountsDbContext.Database.CurrentTransaction?.Rollback();
        //    infrasctuctureEmployerAccountsDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

        //    container.Configure(c =>
        //    {
        //        c.For<EAS.Infrastructure.Data.EmployerAccountsDbContext>().Use(infrasctuctureEmployerAccountsDbContext);
        //    });

        //    return container;
        //}

        //public static IContainer InitialiseAndUseInfrastructureEmployerFinanceDbContext(this IContainer container)
        //{
        //    var infrasctuctureEmployerFinanceDbContext = container.GetInstance<EAS.Infrastructure.Data.EmployerFinanceDbContext>();
        //    infrasctuctureEmployerFinanceDbContext.Database.CurrentTransaction?.Rollback();
        //    infrasctuctureEmployerFinanceDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

        //    container.Configure(c =>
        //    {
        //        c.For<EAS.Infrastructure.Data.EmployerFinanceDbContext>().Use(infrasctuctureEmployerFinanceDbContext);
        //    });

        //    return container;
        //}

        //public static IContainer RegisterUnitOfWork(this IContainer container)
        //{

        //    return container;
        //}
    }
}
