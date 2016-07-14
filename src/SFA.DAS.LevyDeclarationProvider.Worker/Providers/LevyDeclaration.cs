using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerSchemes;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;
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

                List<EmployerLevyData> employerDataList = new List<EmployerLevyData>();

                foreach (var scheme in employerSchemesResult.Schemes.SchemesList)
                {

                    var levyDeclarationQueryResult = await _mediator.SendAsync(new GetLevyDeclarationQuery { Id = scheme.Ref });
                    var employerData = new EmployerLevyData {Fractions = new DasEnglishFractions(), Declarations = new DasDeclarations {Declarations = new List<DasDeclaration>()} };

                    var fraction = levyDeclarationQueryResult.Fractions.FractionCalculations[0]; //TODO Make some sence of this!
                    if (fraction != null)
                    {
                        employerData.Fractions.DateCalculated = DateTime.Parse(fraction.calculatedAt);
                        employerData.Fractions.Amount = decimal.Parse(fraction.fractions.Find(fr => fr.region == "England").value);
                    }
                    foreach (var declaration in levyDeclarationQueryResult.Declarations.declarations)
                    {
                        var dasDeclaration = new DasDeclaration();
                        dasDeclaration.Amount = declaration.amount;
                        dasDeclaration.Date = DateTime.Parse(declaration.submissionDate);
                        dasDeclaration.SubmissionType = declaration.submissionType;
                        dasDeclaration.Id = declaration.submissionId;
                        employerData.EmpRef = scheme.Ref;
                        employerData.Declarations.Declarations.Add(dasDeclaration);
                    }

                    employerDataList.Add(employerData);
                }


                await _mediator.SendAsync(new RefreshEmployerLevyDataCommand() { EmployerId = employerAccountId, EmployerLevyData = employerDataList });



            }
        }
    }
}

