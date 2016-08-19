using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerSchemes;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;
using SFA.DAS.LevyDeclarationProvider.Worker.Queries.GetAccount;
using SFA.DAS.Messaging;

namespace SFA.DAS.LevyDeclarationProvider.Worker.Providers
{
    public class LevyDeclaration : ILevyDeclaration
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public LevyDeclaration(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILogger logger)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle()
        {

            var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>();
            if (message?.Content != null)
            {

                var employerAccountId = message.Content.AccountId;

                _logger.Info($"Processing LevyDeclaration for {employerAccountId}");

                var employerAccountResult = await _mediator.SendAsync(new GetAccountRequest
                {
                    AccountId = employerAccountId
                });

                if (employerAccountResult?.Account == null)
                {
                    await message.CompleteAsync();
                    return;
                }

                var employerSchemesResult = await _mediator.SendAsync(new GetEmployerSchemesQuery { Id = employerAccountResult.Account.Id });
                if (employerSchemesResult?.Schemes?.SchemesList == null)
                {
                    await message.CompleteAsync();
                    return;
                }

                List<EmployerLevyData> employerDataList = new List<EmployerLevyData>();

                foreach (var scheme in employerSchemesResult.Schemes.SchemesList)
                {

                    var levyDeclarationQueryResult = await _mediator.SendAsync(new GetHMRCLevyDeclarationQuery { Id = scheme.Ref });
                    var employerData = new EmployerLevyData {Fractions = new DasEnglishFractions(), Declarations = new DasDeclarations {Declarations = new List<DasDeclaration>()} };

                    if (levyDeclarationQueryResult?.Fractions != null && levyDeclarationQueryResult.Declarations != null)
                    {
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
                    
                }


                await _mediator.SendAsync(new RefreshEmployerLevyDataCommand() { EmployerId = employerAccountId, EmployerLevyData = employerDataList });

                

            }
            if (message != null)
            {
                await message.CompleteAsync();
            }
            
        }
    }
}

