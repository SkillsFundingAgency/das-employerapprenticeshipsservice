using System;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.UnitOfWork;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    public class UnitOfWorkManagerTestHelper2
    {
        private readonly IContainer _container;

        public UnitOfWorkManagerTestHelper2(IContainer objectContainer)
        {
            _container = objectContainer;
        }

        public async Task RunInIsolatedTransactionAsync(Func<Task> operation)
        {
            var unitOfWorkManager = _container.GetInstance<IUnitOfWorkManager>();

            try
            {
                await StartAllTransactions(unitOfWorkManager);

            }
            catch (Exception e)
            {
                // TODO IT BREAKS HERE SECOND TIME
                Console.WriteLine(e);
                throw;
            }
            try
            {
                await operation();
                await EndAllTransactions(unitOfWorkManager);
            }
            catch (Exception exception)
            {
                await EndAllTransactions(unitOfWorkManager, exception);
                throw;
            }
        }

        public async Task<TOperationResponseType> RunInIsolatedTransactionAsync<TOperationResponseType>(Func<Task<TOperationResponseType>> operation)
        {
            return await operation();
        }

        public Task StartAllTransactions(IUnitOfWorkManager unitOfWorkManager)
        {
            return unitOfWorkManager.BeginAsync();
        }

        public Task EndAllTransactions(IUnitOfWorkManager unitOfWorkManager, Exception exception = null)
        {
            return unitOfWorkManager.EndAsync(exception);
        }
    }
}