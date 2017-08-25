using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class RefreshEmployerLevyService : IRefreshEmployerLevyService
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IMessagePublisher _messagePublisher;

        public RefreshEmployerLevyService(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task QueueRefreshLevyMessage(long accountId, string payeRef)
        {
            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage
            {
                AccountId = accountId,
                PayeRef = payeRef
            });
        }
    }
}
