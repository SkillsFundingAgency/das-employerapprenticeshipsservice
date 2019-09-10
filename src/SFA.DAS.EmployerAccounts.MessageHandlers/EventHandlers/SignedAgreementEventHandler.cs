using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class SignedAgreementEventHandler : IHandleMessages<SignedAgreementEvent>
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMediator _mediator;

        public SignedAgreementEventHandler(IMessagePublisher messagePublisher, IMediator mediator)
        {
            _messagePublisher = messagePublisher;
            _mediator = mediator;
        }

        public async Task Handle(SignedAgreementEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new AgreementSignedMessage(
                message.AccountId,
                message.AgreementId,
                message.OrganisationName,
                message.LegalEntityId,
                message.CohortCreated,
                message.UserName,
                message.UserRef.ToString()));

            await _mediator.Send(new SignAccountAgreementCommand(message.AccountId, message.ag))
        }
    }
}
