using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Commands.PublishGenericEvent;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Events.ProcessDeclaration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Factories;
using SFA.DAS.EmployerFinance.Models.HmrcLevy;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData
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
        private readonly ILog _logger;

        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, IDasLevyRepository dasLevyRepository, IMediator mediator, IHmrcDateService hmrcDateService,
            ILevyEventFactory levyEventFactory, IGenericEventFactory genericEventFactory, IHashingService hashingService, ILog logger)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _mediator = mediator;
            _hmrcDateService = hmrcDateService;
            _levyEventFactory = levyEventFactory;
            _genericEventFactory = genericEventFactory;
            _hashingService = hashingService;
            _logger = logger;
        }

        protected override async Task HandleCore(RefreshEmployerLevyDataCommand message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var savedDeclarations = new List<DasDeclaration>();
            var updatedEmpRefs = new List<string>();

            foreach (var employerLevyData in message.EmployerLevyData)
            {
                var declarations = employerLevyData.Declarations.Declarations.OrderBy(c => c.SubmissionDate).ToArray();

                declarations = FilterDuplicateHmrcDeclarations(employerLevyData.EmpRef, declarations);

                declarations = await FilterActiveDeclarations(employerLevyData, declarations);

                ProcessNoPaymentForPeriodDeclarations(declarations, employerLevyData);

                await ProcessEndOfYearAdjustmentDeclarations(declarations, employerLevyData);

                if (!declarations.Any()) continue;

                await _dasLevyRepository.CreateEmployerDeclarations(declarations, employerLevyData.EmpRef, message.AccountId);

                updatedEmpRefs.Add(employerLevyData.EmpRef);
                savedDeclarations.AddRange(declarations);
            }

            if (savedDeclarations.Any())
            {
                await PublishProcessDeclarationEvents(message, updatedEmpRefs);
                await PublishDeclarationUpdatedEvents(message.AccountId, savedDeclarations);
            }
        }

        /// <summary> 
        /// If there are any Submissions from Hmrc that have the same submission Id, we should discard all 
        /// but the first. 
        /// </summary> 
        private DasDeclaration[] FilterDuplicateHmrcDeclarations(string empRef,
            DasDeclaration[] declarations)
        { 
            var duplicateIds = declarations.GroupBy(d => d.Id).Where(g => g.Count() > 1)    
                .Select(s => s.First().Id).ToList();

            if (duplicateIds.Any())
            {
                _logger.Info($"PayeScheme '{empRef}' has duplicate submission id(s) from Hmrc = '{string.Join(", ", duplicateIds)}'");
            }

            return declarations.DistinctBy(x => x.Id).ToArray(); 
        } 
 
        private async Task ProcessEndOfYearAdjustmentDeclarations(IEnumerable<DasDeclaration> declarations, EmployerLevyData employerLevyData)
        {
            var endOfYearAdjustmentDeclarations = declarations.Where(IsEndOfYearAdjustment).ToList();

            foreach (var dasDeclaration in endOfYearAdjustmentDeclarations)
            {
                await UpdateEndOfYearAdjustment(employerLevyData, dasDeclaration);
            }
        }

        private static void ProcessNoPaymentForPeriodDeclarations(IEnumerable<DasDeclaration> declarations, EmployerLevyData employerLevyData)
        {
            var noPaymentForPeriodDeclarations = declarations.Where(x => x.NoPaymentForPeriod);

            foreach (var dasDeclaration in noPaymentForPeriodDeclarations)
            {
                dasDeclaration.LevyDueYtd = null;
            }
        }

        private async Task<DasDeclaration[]> FilterActiveDeclarations(EmployerLevyData employerLevyData, IEnumerable<DasDeclaration> declarations)
        {
            var existingSubmissionIds = await _dasLevyRepository.GetEmployerDeclarationSubmissionIds(employerLevyData.EmpRef);
            var existingSubmissionIdsLookup = new HashSet<string>(existingSubmissionIds.Select(x => x.ToString()));

            //NOTE: The submissionId in our database is the same as the declaration ID from HMRC (DasDeclaration)
            declarations = declarations.Where(x => !existingSubmissionIdsLookup.Contains(x.Id)).ToArray();

            declarations = declarations.Where(x => !DoesSubmissionPreDateTheLevy(x)).ToArray();

            return declarations.Where(x => !IsSubmissionForFuturePeriod(x)).ToArray();
        }

        private async Task PublishProcessDeclarationEvents(RefreshEmployerLevyDataCommand message, IEnumerable<string> updatedEmpRefs)
        {
            foreach (var empRef in updatedEmpRefs)
            {
                await _mediator.PublishAsync(new ProcessDeclarationsEvent
                {
                    AccountId = message.AccountId,
                    EmpRef = empRef
                });
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
            if (dasDeclaration.LevyDueYtd == null)
            {
                throw new ArgumentNullException(nameof(dasDeclaration));
            }

            DasDeclaration adjustmentDeclaration = null;
            var payrollMonth = dasDeclaration.PayrollMonth ?? 12;

            do
            {
                adjustmentDeclaration = await _dasLevyRepository.GetSubmissionByEmprefPayrollYearAndMonth(employerLevyData.EmpRef, dasDeclaration.PayrollYear, payrollMonth);
                payrollMonth--;
            } while (adjustmentDeclaration == null && payrollMonth > 0);

            dasDeclaration.EndOfYearAdjustment = true;

            if (adjustmentDeclaration?.LevyDueYtd != null)
            {
                dasDeclaration.EndOfYearAdjustmentAmount =
                    adjustmentDeclaration.LevyDueYtd.Value - dasDeclaration.LevyDueYtd.Value;
            }
            else
            {
                dasDeclaration.EndOfYearAdjustmentAmount = dasDeclaration.LevyDueYtd.Value;
            }
        }



        private async Task PublishDeclarationUpdatedEvents(long accountId, IEnumerable<DasDeclaration> savedDeclarations)
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
