using MediatR;
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
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NServiceBus;

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
        private readonly IEventPublisher _eventPublisher;

        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, IDasLevyRepository dasLevyRepository, IMediator mediator, IHmrcDateService hmrcDateService,
            ILevyEventFactory levyEventFactory, IGenericEventFactory genericEventFactory, IHashingService hashingService, ILog logger, IEventPublisher eventPublisher)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _mediator = mediator;
            _hmrcDateService = hmrcDateService;
            _levyEventFactory = levyEventFactory;
            _genericEventFactory = genericEventFactory;
            _hashingService = hashingService;
            _logger = logger;
            _eventPublisher = eventPublisher;
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

            var hasDecalarations = savedDeclarations.Any();
            if (hasDecalarations)
            {
                await PublishProcessDeclarationEvents(message, updatedEmpRefs);
                await PublishDeclarationUpdatedEvents(message.AccountId, savedDeclarations);
            }

            await PublishRefreshEmployerLevyDataCompletedEvent(hasDecalarations, message.AccountId);
        }

        private async Task PublishRefreshEmployerLevyDataCompletedEvent(bool levyImported, long accountId)
        {
            await _eventPublisher.Publish(new RefreshEmployerLevyDataCompletedEvent
            {
                AccountId = accountId,
                Created = DateTime.UtcNow,
                LevyImported = levyImported
            });
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

        private async Task UpdateEndOfYearAdjustment(EmployerLevyData employerLevyData, DasDeclaration yearEndAdjustment)
        {
            if (yearEndAdjustment.LevyDueYtd == null && !yearEndAdjustment.NoPaymentForPeriod)
            {
                throw new ArgumentNullException(nameof(yearEndAdjustment));
            }

            yearEndAdjustment.EndOfYearAdjustment = true;

            if (yearEndAdjustment.NoPaymentForPeriod)
                return;

            var period12Declaration = await GetDeclarationEffectiveForPeriod12(employerLevyData, yearEndAdjustment.PayrollYear);

            if (period12Declaration?.LevyDueYtd != null)
            {
                // Caution: the calculation here is incorrect (it should be the other way round). This results in negative values in LevyDeclaration EndOfYearAdjustmentAmount.
                // However, do not fix this as somewhere later (probably in a view or sproc) the EndOfYearAdjustmentAmount is being inverted when writing to the transaction line table.
                yearEndAdjustment.EndOfYearAdjustmentAmount =
                    period12Declaration.LevyDueYtd.Value - yearEndAdjustment.LevyDueYtd ?? 0;
            }
            else
            {
                yearEndAdjustment.EndOfYearAdjustmentAmount = yearEndAdjustment.LevyDueYtd ?? 0;
            }
        }

        /// <summary>
        ///     Returns the latest declaration for the year (which is the one that will be effective for period 12).
        /// </summary>
        /// <param name="employerLevyData">The declarations retrieved from HMRC</param>
        /// <returns>The declaration that was effective for period 12 or null if one was not found.</returns>
        /// <remarks>
        ///     The database contains all the declarations retrieved to date from HMRC, <see cref="employerLevyData"/>
        ///     contains the declarations that HMRC has that we have not retrieved previously (which is determined by submission date).
        /// </remarks>
        private async Task<DasDeclaration> GetDeclarationEffectiveForPeriod12(EmployerLevyData employerLevyData, string payrollYear)
        {
            // Look in the declarations that have just been retrieved from HMRC (which will contain everything not yet in the database)
            DasDeclaration period12Declaration = await GetEffectivePeriod12SubmissionFromLatestHmrcFeed(employerLevyData, payrollYear);

            // Look in the database (which will contain everything previously retrieved from HMRC)
            if (period12Declaration == null)
            {
                period12Declaration = await GetEffectivePeriod12SubmissionFromDatabase(employerLevyData.EmpRef, payrollYear);
            }

            return period12Declaration;
        }

        private Task<DasDeclaration> GetEffectivePeriod12SubmissionFromLatestHmrcFeed(EmployerLevyData employerLevyData, string payrollYear)
        {
            // We are only interested in declarations for the current year that are not year-end-adjustments and that were submitted on time
            DasDeclaration period12Declaration = employerLevyData.Declarations.Declarations
                .Where(ld => ld.PayrollYear == payrollYear
                             && ld.EndOfYearAdjustment == false
                             && ld.PayrollMonth.HasValue
                             && _hmrcDateService.IsDateInPayrollPeriod(ld.PayrollYear, ld.PayrollMonth.Value, ld.SubmissionDate))
                .OrderByDescending(ld => ld.PayrollMonth)
                .ThenByDescending(ld => ld.SubmissionDate)
                .FirstOrDefault();

            return Task.FromResult(period12Declaration);
        }

        private async Task<DasDeclaration> GetEffectivePeriod12SubmissionFromDatabase(string empRef, string payrollYear)
        {
            short payrollMonth = 12;

            DasDeclaration period12Declaration = null;

            // Performance could be improved by reading the most recent LD in the database rather than looping through looking for each month in turn
            while (period12Declaration == null && payrollMonth > 0)
            {
                period12Declaration = await _dasLevyRepository.GetSubmissionByEmprefPayrollYearAndMonth(empRef, payrollYear, payrollMonth);
                payrollMonth--;
            }

            return period12Declaration;
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
