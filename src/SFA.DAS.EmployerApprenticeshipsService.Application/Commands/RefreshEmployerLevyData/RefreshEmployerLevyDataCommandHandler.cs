using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Events.ProcessDeclaration;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

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
        private readonly IMapper _mapper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILog _logger;

        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, IDasLevyRepository dasLevyRepository, IMediator mediator, IHmrcDateService hmrcDateService,
            ILevyEventFactory levyEventFactory, IGenericEventFactory genericEventFactory, IHashingService hashingService,
            IMapper mapper, IMessagePublisher messagePublisher, ILog logger )
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _mediator = mediator;
            _hmrcDateService = hmrcDateService;
            _levyEventFactory = levyEventFactory;
            _genericEventFactory = genericEventFactory;
            _hashingService = hashingService;
            _mapper = mapper;
            _messagePublisher = messagePublisher;
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
            var adjustmentDeclaration = await _dasLevyRepository.GetSubmissionByEmprefPayrollYearAndMonth(employerLevyData.EmpRef, dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value);
            dasDeclaration.EndOfYearAdjustment = true;
            dasDeclaration.EndOfYearAdjustmentAmount = adjustmentDeclaration?.LevyDueYtd - dasDeclaration.LevyDueYtd ?? 0;
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


            var tasks = new List<Task>();
            foreach (var x in periodsChanged)
            {
                tasks.Add(CreateDeclarationUpdatedGenericEvent(hashedAccountId, x.PayrollYear, x.PayrollMonth));
                tasks.Add(PublishDeclarationUpdatedEventMessage(accountId, x.PayrollYear, x.PayrollMonth));
            }
            await Task.WhenAll(tasks);
        }

        private async Task CreateDeclarationUpdatedGenericEvent(string hashedAccountId, string payrollYear, short? payrollMonth)
        {
            var declarationUpdatedEvent = _levyEventFactory.CreateDeclarationUpdatedEvent(hashedAccountId, payrollYear, payrollMonth);
            var genericEvent = _genericEventFactory.Create(declarationUpdatedEvent);

            await _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
        }

        private async Task PublishDeclarationUpdatedEventMessage(long accountId, string payrollYear, short? payrollMonth)
        {
            if (!payrollMonth.HasValue)
                return;

            try
            {
                var dasLevyDeclarations = await _dasLevyRepository.GetAccountLevyDeclarations(accountId, payrollYear, payrollMonth.Value);
                foreach (var levyDeclarationView in dasLevyDeclarations)
                {
                    try
                    {
                        var levySchemeDeclarationUpdatedEvent =
                            new LevyDeclarationProcessedEvent(accountId, null, null);
                        _mapper.Map(levyDeclarationView, levySchemeDeclarationUpdatedEvent);
                        await _messagePublisher.PublishAsync(levySchemeDeclarationUpdatedEvent);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Error while trying to publish levy scheme declaration changed event. Error: {ex.Message}. Levy data: {JsonConvert.SerializeObject(levyDeclarationView)}");
                        throw;  //TODO: Not sure if we should rollback the levy processing tx if the publishing fails
                    }
                }
                var levyDeclarationEvent = new LevyDeclarationsProcessedEvent(accountId,null,null)
                {
                    LevyDeclaredInMonth = dasLevyDeclarations.Sum(levy => levy.LevyDeclaredInMonth),
                    PayrollMonth = payrollMonth.Value,
                    PayrollYear = payrollYear
                };
                await _messagePublisher.PublishAsync(levyDeclarationEvent);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error publishing levy declaration events. Error: {ex}");
                //TODO: Not sure if we should rollback the levy processing tx if the publishing fails
            }
        }
    }
}
