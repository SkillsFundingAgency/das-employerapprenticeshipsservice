﻿using System;
using MediatR;
using SFA.DAS.EmployerFinance.Commands.PublishGenericEvent;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Events.ProcessDeclaration;
using SFA.DAS.EmployerFinance.Factories;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommandHandler : AsyncRequestHandler<RefreshEmployerLevyDataCommand>
    {
        private readonly IValidator<RefreshEmployerLevyDataCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IMediator _mediator;
        private readonly ILevyEventFactory _levyEventFactory;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IHashingService _hashingService;
        private readonly ILevyImportCleanerStrategy _levyImportCleanerStrategy;
        private readonly IEventPublisher _eventPublisher;

        public RefreshEmployerLevyDataCommandHandler(
            IValidator<RefreshEmployerLevyDataCommand> validator,
            IDasLevyRepository dasLevyRepository,
            IMediator mediator,
            ILevyEventFactory levyEventFactory,
            IGenericEventFactory genericEventFactory,
            IHashingService hashingService,
            ILevyImportCleanerStrategy levyImportCleanerStrategy,
            IEventPublisher eventPublisher)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _mediator = mediator;
            _levyEventFactory = levyEventFactory;
            _genericEventFactory = genericEventFactory;
            _hashingService = hashingService;
            _levyImportCleanerStrategy = levyImportCleanerStrategy;
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
                var declarations = await _levyImportCleanerStrategy.Cleanup(employerLevyData.EmpRef, employerLevyData.Declarations.Declarations);

                if (declarations.Length == 0) continue;

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
