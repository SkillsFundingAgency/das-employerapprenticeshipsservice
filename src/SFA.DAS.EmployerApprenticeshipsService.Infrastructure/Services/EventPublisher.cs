using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class EventPublisher : IEventPublisher
    {
        private IMediator _mediator;

        public EventPublisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishAccountCreatedEvent(string hashedAccountId)
        {
            var resourceUri = CreateAccountUri(hashedAccountId);
            await PublishEvent(resourceUri, "AccountCreated");
        }

        public async Task PublishAccountRenamedEvent(string hashedAccountId)
        {
            var resourceUri = CreateAccountUri(hashedAccountId);
            await PublishEvent(resourceUri, "AccountRenamed");
        }

        public async Task PublishLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId)
        {
            var resourceUri = CreateAccountUri(hashedAccountId) + $"/legalentities/{legalEntityId}";
            await PublishEvent(resourceUri, "LegalEntityCreated");
        }

        private string CreateAccountUri(string hashedAccountId)
        {
            return $"api/accounts/{hashedAccountId}";
        }

        private async Task PublishEvent(string resourceUri, string eventName)
        {
            var command = new CreateAccountEventCommand { Event = eventName, ResourceUri = resourceUri };
            await _mediator.PublishAsync(command);
        }
    }
}
