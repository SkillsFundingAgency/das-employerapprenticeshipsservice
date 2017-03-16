using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IGenericEventFactory
    {
        Task PublishAccountCreatedEvent(string hashedAccountId);
        Task PublishAccountRenamedEvent(string hashedAccountId);
        Task PublishLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId);
        Task PublishPayeSchemeAddedEvent(string hashedAccountId, string payeSchemeRef);
        Task PublishPayeSchemeRemovedEvent(string hashedAccountId, string payeSchemeRef);
    }
}
