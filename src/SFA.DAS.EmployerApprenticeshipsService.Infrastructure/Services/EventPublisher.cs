using System.Threading.Tasks;
using System.Web;
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

        public async Task PublishPayeSchemeAddedEvent(string hashedAccountId, string payeSchemeRef)
        {
            var resourceUri = CreatePayeSchemeUri(hashedAccountId, payeSchemeRef);
            await PublishEvent(resourceUri, "PayeSchemeAdded");
        }

        public async Task PublishPayeSchemeRemovedEvent(string hashedAccountId, string payeSchemeRef)
        {
            var resourceUri = CreatePayeSchemeUri(hashedAccountId, payeSchemeRef);
            await PublishEvent(resourceUri, "PayeSchemeRemoved");
        }

        private string CreatePayeSchemeUri(string hashedAccountId, string payeSchemeRef)
        {
            // This is deliberately double encoded to minic the response the accounts web api returns when encoding the URI to json as part of the GetAccounts method.
            return CreateAccountUri(hashedAccountId) + $"/payeschemes/{HttpUtility.UrlEncode(HttpUtility.UrlEncode(payeSchemeRef))}";
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
