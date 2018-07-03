using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.SetAccountLegalEntityAgreementStatus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.MessageHandlers.Worker.EventHandlers
{
    [TopicSubscription("MH_AgreementCreated")]
    public class AgreementCreatedEventHandler : MessageProcessor<AgreementCreatedMessage>
    {
        private readonly IMediator _mediator;

        public AgreementCreatedEventHandler(IMessageSubscriberFactory subscriberFactory, ILog log, IMediator mediator) : base(subscriberFactory, log)
        {
            _mediator = mediator;
        }

        protected override Task ProcessMessage(AgreementCreatedMessage messageContent)
        {
            return _mediator.SendAsync(new SetAccountLegalEntityAgreementStatusCommand
            {
                AccountId = messageContent.AccountId,
                LegalEntityId = messageContent.LegalEntityId
            });
        }
   }
}
