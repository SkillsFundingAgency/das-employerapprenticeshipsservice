using System.Data.Entity;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public interface IOutboxDbContext
    {
        DbSet<OutboxMessage> OutboxMessages { get; }
    }
}