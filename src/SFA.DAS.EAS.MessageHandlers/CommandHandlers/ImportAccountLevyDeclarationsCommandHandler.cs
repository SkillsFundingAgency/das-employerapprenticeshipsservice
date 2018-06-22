using MediatR;
using Microsoft.Azure;
using NServiceBus;
using NServiceBus.Logging;
using SFA.DAS.EAS.Application.Commands.CreateEnglishFractionCalculationDate;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Application.Commands.UpdateEnglishFractions;
using SFA.DAS.EAS.Application.Queries.GetEnglishFractionUpdateRequired;
using SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.CommandHandlers
{
    public class ImportAccountLevyDeclarationsCommandHandler : IHandleMessages<ImportAccountLevyDeclarationsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IDasAccountService _dasAccountService;

        private static bool HmrcProcessingEnabled => CloudConfigurationManager.GetSetting("DeclarationsEnabled")
            .Equals("both", StringComparison.CurrentCultureIgnoreCase);

        private static bool DeclarationProcessingOnly => CloudConfigurationManager.GetSetting("DeclarationsEnabled")
            .Equals("declarations", StringComparison.CurrentCultureIgnoreCase);

        private static bool FractionProcessingOnly => CloudConfigurationManager.GetSetting("DeclarationsEnabled")
            .Equals("fractions", StringComparison.CurrentCultureIgnoreCase);

        public ImportAccountLevyDeclarationsCommandHandler(IMediator mediator, ILog logger, IDasAccountService dasAccountService)
        {
            _mediator = mediator;
            _logger = logger;
            _dasAccountService = dasAccountService;
        }

        public async Task Handle(ImportAccountLevyDeclarationsCommand message, IMessageHandlerContext context)
        {
            var employerAccountId = message.AccountId;
            var payeRef = message.PayeRef;

            _logger.Debug($"Getting english fraction updates for employer account {employerAccountId}");

            var englishFractionUpdateResponse = await _mediator.SendAsync(new GetEnglishFractionUpdateRequiredRequest());

            _logger.Debug($"Getting levy declarations for PAYE scheme {payeRef} for employer account {employerAccountId}");

            var payeSchemeDeclarations = await ProcessScheme(payeRef, englishFractionUpdateResponse);

            _logger.Debug($"Adding Levy Declarations of PAYE scheme {payeRef} to employer account {employerAccountId}");

            await RefreshEmployerAccountLevyDeclarations(employerAccountId, payeSchemeDeclarations);
        }

        private async Task RefreshEmployerAccountLevyDeclarations(long employerAccountId, ICollection<EmployerLevyData> payeSchemeDeclarations)
        {
            await _mediator.SendAsync(new RefreshEmployerLevyDataCommand
            {
                AccountId = employerAccountId,
                EmployerLevyData = payeSchemeDeclarations
            });
        }

        private async Task<ICollection<EmployerLevyData>> ProcessScheme(string payeRef, GetEnglishFractionUpdateRequiredResponse englishFractionUpdateResponse)
        {
            var payeSchemeDeclarations = new List<EmployerLevyData>();

            await UpdateEnglishFraction(payeRef, englishFractionUpdateResponse);

            _logger.Debug($"Getting levy declarations from HMRC for PAYE scheme {payeRef}");

            var levyDeclarationQueryResult = HmrcProcessingEnabled || DeclarationProcessingOnly ?
                await _mediator.SendAsync(new GetHMRCLevyDeclarationQuery { EmpRef = payeRef }) : null;

            _logger.Debug($"Processing levy declarations retrieved from HMRC for PAYE scheme {payeRef}");

            if (levyDeclarationQueryResult?.LevyDeclarations?.Declarations != null)
            {
                var declarations = CreateDasDeclarations(levyDeclarationQueryResult);

                var employerData = new EmployerLevyData
                {
                    EmpRef = payeRef,
                    Declarations = { Declarations = declarations }
                };

                payeSchemeDeclarations.Add(employerData);
            }

            return payeSchemeDeclarations;
        }

        private List<DasDeclaration> CreateDasDeclarations(GetHMRCLevyDeclarationResponse levyDeclarationQueryResult)
        {
            var declarations = new List<DasDeclaration>();


            foreach (var declaration in levyDeclarationQueryResult.LevyDeclarations.Declarations)
            {
                _logger.Debug($"Creating Levy Declaration with submission Id {declaration.SubmissionId} from HMRC query results");

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
                    InactiveTo = declaration.InactiveTo,
                    SubmissionId = declaration.SubmissionId
                };

                declarations.Add(dasDeclaration);
            }

            return declarations;
        }

        private async Task UpdateEnglishFraction(string payeRef,
            GetEnglishFractionUpdateRequiredResponse englishFractionUpdateResponse)
        {
            if (HmrcProcessingEnabled || FractionProcessingOnly)
            {
                _logger.Debug($"Getting update for english fraction for PAYE scheme {payeRef}");
                await _mediator.SendAsync(new UpdateEnglishFractionsCommand
                {
                    EmployerReference = payeRef,
                    EnglishFractionUpdateResponse = englishFractionUpdateResponse
                });

                _logger.Debug($"Updating english fraction for PAYE scheme {payeRef}");
                await _dasAccountService.UpdatePayeScheme(payeRef);
            }

            if (englishFractionUpdateResponse.UpdateRequired)
            {
                _logger.Debug($"Updating english fraction calculation date to " +
                              $"{englishFractionUpdateResponse.DateCalculated.ToShortDateString()} for PAYE scheme {payeRef}");

                await _mediator.SendAsync(new CreateEnglishFractionCalculationDateCommand
                {
                    DateCalculated = englishFractionUpdateResponse.DateCalculated
                });
            }
        }
    }
}
