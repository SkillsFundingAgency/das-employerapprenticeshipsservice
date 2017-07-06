using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Events.ProcessDeclaration;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommandHandler : AsyncRequestHandler<RefreshEmployerLevyDataCommand>
    {
        private readonly IValidator<RefreshEmployerLevyDataCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IMediator _mediator;
        private readonly IHmrcDateService _hmrcDateService;
        private readonly ILevyEventFactory _levyEventFactory;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IHashingService _hashingService;

        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, IDasLevyRepository dasLevyRepository, IMediator mediator, IHmrcDateService hmrcDateService,
            ILevyEventFactory levyEventFactory, IGenericEventFactory genericEventFactory, IHashingService hashingService)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _mediator = mediator;
            _hmrcDateService = hmrcDateService;
            _levyEventFactory = levyEventFactory;
            _genericEventFactory = genericEventFactory;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(RefreshEmployerLevyDataCommand message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var savedDeclarations = new List<DasDeclaration>();

            foreach (var employerLevyData in message.EmployerLevyData)
            {
                var declarations = employerLevyData.Declarations.Declarations.OrderBy(c => c.SubmissionDate).ToList();

                var existingDeclarationIds = await _dasLevyRepository.GetEmployerDeclarationIds(employerLevyData.EmpRef);
                var existingIdsLookup = new HashSet<string>(existingDeclarationIds);

                declarations = declarations.Where(x => !existingIdsLookup.Contains(x.Id)).ToList();

                declarations = declarations.Where(x => !DoesSubmissionPreDateTheLevy(x)).ToList();

                declarations = declarations.Where(x => !IsSubmissionForFuturePeriod(x)).ToList();

                var noPaymentForPeriodDeclarations = declarations.Where(x => x.NoPaymentForPeriod).ToList();

                foreach (var dasDeclaration in noPaymentForPeriodDeclarations)
                {
                    await GetLevyFromPreviousSubmission(employerLevyData, dasDeclaration);
                }

                var endOfYearAdjustmentDeclarations = declarations.Where(IsEndOfYearAdjustment).ToList();

                foreach (var dasDeclaration in endOfYearAdjustmentDeclarations)
                {
                    await UpdateEndOfYearAdjustment(employerLevyData, dasDeclaration);
                }

                if (!declarations.Any()) continue;

                await _dasLevyRepository.CreateEmployerDeclarations(declarations, employerLevyData.EmpRef,
                    message.AccountId);

                savedDeclarations.AddRange(declarations);
            }

            if (savedDeclarations.Any())
            {
                foreach (var levyData in message.EmployerLevyData)
                {
                    await _mediator.PublishAsync(new ProcessDeclarationsEvent
                    {
                        AccountId = message.AccountId,
                        EmpRef = levyData.EmpRef
                    });
                }
               
                await PublishDeclarationUpdatedEvents(message.AccountId, savedDeclarations);
            }
        }

        private bool DoesSubmissionPreDateTheLevy(DasDeclaration dasDeclaration)
        {
            return _hmrcDateService.DoesSubmissionPreDateLevy(dasDeclaration.PayrollYear);
        }

        private bool IsSubmissionForFuturePeriod(DasDeclaration dasDeclaration)
        {
            return dasDeclaration.PayrollMonth.HasValue && _hmrcDateService.IsSubmissionForFuturePeriod(dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value, DateTime.UtcNow);
        }

        private bool IsEndOfYearAdjustment(DasDeclaration dasDeclaration)
        {
            return dasDeclaration.PayrollMonth.HasValue && _hmrcDateService.IsSubmissionEndOfYearAdjustment(dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value, dasDeclaration.SubmissionDate);
        }

        private async Task UpdateEndOfYearAdjustment(EmployerLevyData employerLevyData, DasDeclaration dasDeclaration)
        {
            var adjustmentDeclaration = await _dasLevyRepository.GetSubmissionByEmprefPayrollYearAndMonth(employerLevyData.EmpRef, dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value);
            dasDeclaration.EndOfYearAdjustment = true;
            dasDeclaration.EndOfYearAdjustmentAmount = adjustmentDeclaration?.LevyDueYtd - dasDeclaration.LevyDueYtd ?? 0;
        }

        private async Task GetLevyFromPreviousSubmission(EmployerLevyData employerLevyData, DasDeclaration dasDeclaration)
        {
            var previousSubmission = await _dasLevyRepository.GetLastSubmissionForScheme(employerLevyData.EmpRef);
            dasDeclaration.LevyDueYtd = previousSubmission?.LevyDueYtd ?? 0;
            dasDeclaration.LevyAllowanceForFullYear = previousSubmission?.LevyAllowanceForFullYear ?? 0;
        }

        private async Task PublishDeclarationUpdatedEvents(long accountId, List<DasDeclaration> savedDeclarations)
        {
            var hashedAccountId = _hashingService.HashValue(accountId);

            var periodsChanged = savedDeclarations.Select(x =>
                new
                {
                    x.PayrollYear,
                    x.PayrollMonth
                }).Distinct();

            var tasks = periodsChanged.Select(x => CreateDeclarationUpdatedEvent(hashedAccountId, x.PayrollYear, x.PayrollMonth));
            await Task.WhenAll(tasks);
        }

        private Task CreateDeclarationUpdatedEvent(string hashedAccountId, string payrollYear, short? payrollMonth)
        {
            var declarationUpdatedEvent = _levyEventFactory.CreateDeclarationUpdatedEvent(hashedAccountId, payrollYear, payrollMonth);
            var genericEvent = _genericEventFactory.Create(declarationUpdatedEvent);

            return _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
        }
    }
}
