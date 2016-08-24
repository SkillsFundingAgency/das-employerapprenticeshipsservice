using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;
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
        private readonly IDasAccountService _dasAccountService;

        public LevyDeclaration(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILogger logger, IDasAccountService dasAccountService)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
            _logger = logger;
            _dasAccountService = dasAccountService;
        }

        public async Task Handle()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>();
            if (message?.Content != null)
            {
                var employerAccountId = message.Content.AccountId;

                _logger.Info($"Processing LevyDeclaration for {employerAccountId}");
                
                var employerSchemesResult = await _dasAccountService.GetAccountSchemes(employerAccountId);
                if (employerSchemesResult?.SchemesList == null)
                {
                    await message.CompleteAsync();
                    return;
                }

                var employerDataList = new List<EmployerLevyData>();

                foreach (var scheme in employerSchemesResult.SchemesList)
                {
                    var levyDeclarationQueryResult = await _mediator.SendAsync(new GetHMRCLevyDeclarationQuery { AuthToken = scheme.AccessToken, EmpRef = scheme.Ref });
                    var employerData = new EmployerLevyData {Fractions = new DasEnglishFractions {Fractions = new List<DasEnglishFraction>()}, Declarations = new DasDeclarations {Declarations = new List<DasDeclaration>()} };

                    if (levyDeclarationQueryResult?.Fractions != null && levyDeclarationQueryResult.LevyDeclarations != null)
                    {
                        foreach (var fractionCalculation in levyDeclarationQueryResult.Fractions.FractionCalculations)
                        {
                            employerData.Fractions.Fractions.Add(new DasEnglishFraction
                            {
                                Amount = decimal.Parse(fractionCalculation.Fractions.Find(fr => fr.Region == "England").Value),
                                DateCalculated = DateTime.Parse(fractionCalculation.CalculatedAt)
                            });
                        }

                        foreach (var declaration in levyDeclarationQueryResult.LevyDeclarations.Declarations)
                        {
                            var dasDeclaration = new DasDeclaration
                            {
                                Date = DateTime.Parse(declaration.SubmissionTime),
                                Id = declaration.Id,
                                PayrollMonth = declaration.PayrollPeriod?.Month,
                                PayrollYear = declaration.PayrollPeriod?.Year,
                                LevyAllowanceForFullYear = declaration.LevyAllowanceForFullYear,
                                LevyDueYtd = declaration.LevyDueYearToDate,
                            };
                            
                            employerData.EmpRef = scheme.Ref;
                            employerData.Declarations.Declarations.Add(dasDeclaration);
                        }

                        employerDataList.Add(employerData);
                    }
                }

                await _mediator.SendAsync(new RefreshEmployerLevyDataCommand
                {
                    AccountId = employerAccountId,
                    EmployerLevyData = employerDataList
                });
            }
            if (message != null)
            {
                await message.CompleteAsync();
            }
        }
    }
}

