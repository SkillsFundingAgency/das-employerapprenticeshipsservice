using System;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.UnitOfWork;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    public class UnitOfWorkManagerTestHelper
    {
        private readonly IObjectContainer _objectContainer;

        public UnitOfWorkManagerTestHelper(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        public async Task RunInIsolatedTransactionAsync(Func<Task> operation)
        {
            await operation();
            //var unitOfWorkManager = _objectContainer.Resolve<IUnitOfWorkManager>();

            //try
            //{
            //    await StartAllTransactions(unitOfWorkManager);

            //}
            //catch (Exception e)
            //{
            //    // TODO IT BREAKS HERE SECOND TIME
            //    Console.WriteLine(e);
            //    throw;
            //}
            //try
            //{
            //    await operation();
            //    await EndAllTransactions(unitOfWorkManager);
            //}
            //catch (Exception exception)
            //{
            //    await EndAllTransactions(unitOfWorkManager, exception);
            //    throw;
            //}
        }

        public async Task<TOperationResponseType> RunInIsolatedTransactionAsync<TOperationResponseType>(Func<Task<TOperationResponseType>> operation)
        {
            return await operation();
            //var unitOfWorkManager = _objectContainer.Resolve<IUnitOfWorkManager>();

            //await StartAllTransactions(unitOfWorkManager);
            //try
            //{
            //    var response = await operation();
            //    await EndAllTransactions(unitOfWorkManager);
            //    return response;
            //}
            //catch (Exception exception)
            //{
            //    await EndAllTransactions(unitOfWorkManager, exception);
            //    throw;
            //}
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