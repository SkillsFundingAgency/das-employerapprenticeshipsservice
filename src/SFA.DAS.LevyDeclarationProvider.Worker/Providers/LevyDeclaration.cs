using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure;
using NLog;
using SFA.DAS.EAS.Application.Commands.CreateEnglishFractionCalculationDate;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Application.Commands.UpdateEnglishFractions;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetEnglishFractionUpdateRequired;
using SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.LevyDeclarationProvider.Worker.Providers
{
    public class LevyDeclaration : ILevyDeclaration
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IDasAccountService _dasAccountService;

        public LevyDeclaration(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, 
            ILogger logger, IDasAccountService dasAccountService)
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
                if (CloudConfigurationManager.GetSetting("DeclarationsEnabled").Equals("false",StringComparison.CurrentCultureIgnoreCase))
                {
                    await message.CompleteAsync();
                    return;
                }
                var employerAccountId = message.Content.AccountId;

                _logger.Info($"Processing LevyDeclaration for {employerAccountId}");
                
                var employerSchemesResult = await _dasAccountService.GetAccountSchemes(employerAccountId);
                if (employerSchemesResult?.SchemesList == null)
                {
                    await message.CompleteAsync();
                    return;
                }

                var employerDataList = new List<EmployerLevyData>();

                var englishFractionUpdateResponse = await _mediator.SendAsync(new GetEnglishFractionUpdateRequiredRequest());

                foreach (var scheme in employerSchemesResult.SchemesList)
                {
                    
                    await _mediator.SendAsync(new UpdateEnglishFractionsCommand
                    {
                        EmployerReference = scheme.Ref,
                        EnglishFractionUpdateResponse = englishFractionUpdateResponse
                    });
                    
                    
                    var levyDeclarationQueryResult = await _mediator.SendAsync(new GetHMRCLevyDeclarationQuery { EmpRef = scheme.Ref });

                    var employerData = new EmployerLevyData();

                    if (levyDeclarationQueryResult?.LevyDeclarations?.Declarations != null)
                    {

                        foreach (var declaration in levyDeclarationQueryResult.LevyDeclarations.Declarations)
                        {
                            var dasDeclaration = new DasDeclaration
                            {
                                SubmissionDate = DateTime.Parse(declaration.SubmissionTime),
                                Id = declaration.Id,
                                PayrollMonth = declaration.PayrollPeriod?.Month,
                                PayrollYear = declaration.PayrollPeriod?.Year,
                                LevyAllowanceForFullYear = declaration.LevyAllowanceForFullYear,
                                LevyDueYtd = declaration.LevyDueYearToDate,
                                NoPaymentForPeriod = declaration.NoPaymentForPeriod,
                                DateCeased = declaration.DateCeased,
                                InactiveFrom = declaration.InactiveFrom,
                                InactiveTo = declaration.InactiveTo
                                
                            };
                            
                            employerData.EmpRef = scheme.Ref;
                            employerData.Declarations.Declarations.Add(dasDeclaration);
                        }

                        employerDataList.Add(employerData);
                    }
                }

                if (englishFractionUpdateResponse.UpdateRequired)
                {
                    await _mediator.SendAsync(new CreateEnglishFractionCalculationDateCommand
                    {
                        DateCalculated = englishFractionUpdateResponse.DateCalculated
                    });
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

