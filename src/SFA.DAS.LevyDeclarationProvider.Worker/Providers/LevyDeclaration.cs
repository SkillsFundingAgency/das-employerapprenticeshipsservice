using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;
using SFA.DAS.Messaging;

namespace SFA.DAS.LevyDeclarationProvider.Worker.Providers
{
    public class LevyDeclaration : ILevyDeclaration
    {
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;

        public LevyDeclaration(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
        }

        public async Task Handle()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>();

            if (!string.IsNullOrEmpty(message.Content.Id))
            {
                var levyDeclarationQueryResult = await _mediator.SendAsync(new GetLevyDeclarationQuery {Id = message.Content.Id});
                
                /*
                 TODO
                 Add call to do the command    RefreshEmployerLevyDataCommand
             */
            }
        }
    }
}
