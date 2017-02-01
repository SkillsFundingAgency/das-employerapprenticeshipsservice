using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAccountCreatedEvent(string hashedAccountId);
        Task PublishAccountRenamedEvent(string hashedAccountId);
        Task PublishLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId);
    }
}
