using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
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

        public LevyDeclaration(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
        }

        public async Task Handle()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>();
            if (!string.IsNullOrEmpty(message?.Content.Id))
            {

                var employerAccountId = int.Parse(message.Content.Id);

                var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountQuery { Id = employerAccountId });

                if (employerAccountResult.Account == null) return;

                var employerSchemesResult = await _mediator.SendAsync(new GetEmployerSchemesQuery { Id = employerAccountResult.Account.Id });

                if (employerSchemesResult.Schemes.SchemesList == null) return;

                foreach (var scheme in employerSchemesResult.Schemes.SchemesList)
                {
                    var levyDeclarationQueryResult = await _mediator.SendAsync(new GetLevyDeclarationQuery { Id = scheme.Ref });
                }






                /*
                 TODO
                 Add call to do the command    RefreshEmployerLevyDataCommand
             */
            }
        }
    }
}

