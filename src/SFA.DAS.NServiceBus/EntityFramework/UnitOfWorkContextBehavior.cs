using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class UnitOfWorkContextBehavior : Behavior<IInvokeHandlerContext>
    {
        public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            SetUnitOfWorkContext(context);
            await next().ConfigureAwait(false);
        }

        private void SetUnitOfWorkContext(IInvokeHandlerContext context)
        {
            var unitOfWorkContext = context.Builder.Build<IUnitOfWorkContext>();
            var sqlStorageSession = context.SynchronizedStorageSession.SqlPersistenceSession();

            unitOfWorkContext.Set(sqlStorageSession.Connection);
            unitOfWorkContext.Set(sqlStorageSession.Transaction);
        }
    }
}