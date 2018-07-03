using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class UnitOfWorkBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            SetUnitOfWorkContext(context);
            await next().ConfigureAwait(false);
            await SaveChanges(context).ConfigureAwait(false);
            await PublishEvents(context).ConfigureAwait(false);
        }

        private Task PublishEvents(IIncomingLogicalMessageContext context)
        {
            var unitOfWorkContext = context.Builder.Build<IUnitOfWorkContext>();
            var events = unitOfWorkContext.GetEvents();
            var tasks = events.Select(context.Publish);

            return Task.WhenAll(tasks);
        }

        private Task SaveChanges(IIncomingLogicalMessageContext context)
        {
            var db = context.Builder.Build<IOutboxDbContext>();
            return db.SaveChangesAsync();
        }

        private void SetUnitOfWorkContext(IIncomingLogicalMessageContext context)
        {
            var unitOfWorkContext = context.Builder.Build<IUnitOfWorkContext>();

            unitOfWorkContext.Set<DbConnection>(null);
            unitOfWorkContext.Set<DbTransaction>(null);
        }
    }
}