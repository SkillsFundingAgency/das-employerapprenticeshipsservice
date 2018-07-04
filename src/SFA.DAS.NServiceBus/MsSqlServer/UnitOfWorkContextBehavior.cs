using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

namespace SFA.DAS.NServiceBus.MsSqlServer
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
            var sqlStorageSession = context.SynchronizedStorageSession.GetSqlStorageSession();

            unitOfWorkContext.Set(sqlStorageSession.Connection);
            unitOfWorkContext.Set(sqlStorageSession.Transaction);
        }
    }
}