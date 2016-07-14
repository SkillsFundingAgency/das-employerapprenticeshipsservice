using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerSchemes;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;
using SFA.DAS.Messaging;

namespace SFA.DAS.LevyDeclarationProvider.Worker.Providers
{
    public class LevyDeclaration : ILevyDeclaration
    {
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;
        private IMessagePublisher _messagePublisher;

        public LevyDeclaration(IPollingMessageReceiver pollingMessageReceiver, IMessagePublisher messagePublisher, IMediator mediator)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
            _messagePublisher = messagePublisher;
        }

        public async Task Handle()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>();
            if (message?.Content != null)
            {

                var employerAccountId = message.Content.Id;

                var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountQuery { Id = employerAccountId });
                if (employerAccountResult?.Account == null) return;
           
                var employerSchemesResult = await _mediator.SendAsync(new GetEmployerSchemesQuery { Id = employerAccountResult.Account.Id });
                if (employerSchemesResult?.Schemes?.SchemesList == null) return;

                var employerLevyDataChanged = false;
                foreach (var scheme in employerSchemesResult.Schemes.SchemesList)
                {
                    var levyDeclarationQueryResult = await _mediator.SendAsync(new GetLevyDeclarationQuery { Id = scheme.Ref });

                    foreach (var declaration in levyDeclarationQueryResult.Declarations.declarations)
                    {
                        
                    }
               
                }

                await _mediator.SendAsync(new RefreshEmployerLevyDataCommand() {employerId = employerAccountId});
              

           
            }
        }
    }
}

