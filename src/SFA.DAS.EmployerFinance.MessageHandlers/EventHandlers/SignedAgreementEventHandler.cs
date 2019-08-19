using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Commands.CreateAccountLegalEntity;
using SFA.DAS.EmployerFinance.Commands.LegalEntitySignAgreement;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class SignedAgreementEventHandler : IHandleMessages<SignedAgreementEvent>
    {
        private readonly IMediator _mediator;

        public SignedAgreementEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(SignedAgreementEvent message, IMessageHandlerContext context)
        {
            return _mediator.SendAsync(new LegalEntitySignAgreementCommand(message.AgreementId,
                message.SignedAgreementVersion, message.AccountId, message.LegalEntityId));
        }
    }
}
