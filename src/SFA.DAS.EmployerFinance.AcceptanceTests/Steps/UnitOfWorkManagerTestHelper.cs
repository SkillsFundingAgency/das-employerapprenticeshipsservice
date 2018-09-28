using System;
using System.Collections.Generic;
using System.Linq;
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
            var unitOfWorkManagers = _objectContainer.Resolve<IEnumerable<IUnitOfWorkManager>>().ToArray();

            await StartAllTransactions(unitOfWorkManagers);
            try
            {
                await operation();
                await EndAllTransactions(unitOfWorkManagers);
            }
            catch (Exception exception)
            {
                await EndAllTransactions(unitOfWorkManagers, exception);
                throw;
            }
        }

        public async Task<TOperationResponseType> RunInIsolatedTransactionAsync<TOperationResponseType>(Func<Task<TOperationResponseType>> operation)
        {
            var unitOfWorkManagers = _objectContainer.Resolve<IEnumerable<IUnitOfWorkManager>>().ToArray();

            await StartAllTransactions(unitOfWorkManagers);
            try
            {
                var response = await operation();
                await EndAllTransactions(unitOfWorkManagers);
                return response;
            }
            catch (Exception exception)
            {
                await EndAllTransactions(unitOfWorkManagers, exception);
                throw;
            }
        }

        private Task StartAllTransactions(IEnumerable<IUnitOfWorkManager> unitOfWorkManagers)
        {
            List<Task> startTranTasks = new List<Task>();

            foreach (var unitOfWork in unitOfWorkManagers)
            {
                startTranTasks.Add(unitOfWork.BeginAsync());
            }

            return Task.WhenAll(startTranTasks);
        }

        private Task EndAllTransactions(IEnumerable<IUnitOfWorkManager> unitOfWorkManagers, Exception exception = null)
        {
            List<Task> endTranTasks = new List<Task>();
            foreach (var unitOfWork in unitOfWorkManagers)
            {
                endTranTasks.Add(unitOfWork.EndAsync(exception));
            }

            return Task.WhenAll(endTranTasks);
        }
    }
}