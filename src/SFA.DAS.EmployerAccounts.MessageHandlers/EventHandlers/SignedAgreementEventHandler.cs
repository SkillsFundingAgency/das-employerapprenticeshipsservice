using System;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers
{
    public class SignedAgreementEventHandler : IHandleMessages<SignedAgreementEvent>
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IReadStoreMediator _mediator;

        public SignedAgreementEventHandler(IMessagePublisher messagePublisher, IReadStoreMediator mediator)
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

            try
            {
                await _mediator.Send(new SignAccountAgreementCommand(message.AccountId, message.AgreementVersion, message.AgreementType.ToString()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
