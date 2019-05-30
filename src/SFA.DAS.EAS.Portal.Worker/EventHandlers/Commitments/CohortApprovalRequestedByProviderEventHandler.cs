﻿using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Commands.Cohort;
using SFA.DAS.EAS.Portal.Application.Services;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Commitments
{
    public class CohortApprovalRequestedByProviderEventHandler : IHandleMessages<CohortApprovalRequestedByProvider>
    {
        private readonly IMessageContext _messageContext;
        private readonly ICommandHandler<CohortApprovalRequestedCommand> _handler;
        private readonly IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand> _adapter;

        public CohortApprovalRequestedByProviderEventHandler(
            ICommandHandler<CohortApprovalRequestedCommand> handler,
            IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand> adapter,
            IMessageContext messageContext)
        {
            _handler = handler;
            _adapter = adapter;
            _messageContext = messageContext;
        }

        public Task Handle(CohortApprovalRequestedByProvider message, IMessageHandlerContext context)
        {
            _messageContext.Initialise(context);
            return _handler.Handle(_adapter.Convert(message));
        }
    }
}
