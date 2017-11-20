﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Commands.RemoveLegalEntity
{
  
    public class RemoveLegalEntityCommandHandler : AsyncRequestHandler<RemoveLegalEntityCommand>
    {
        private readonly IValidator<RemoveLegalEntityCommand> _validator;
        private readonly ILog _logger;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IEmployerAgreementEventFactory _employerAgreementEventFactory;
        private readonly IMessagePublisher _messagePublisher;
        
        public RemoveLegalEntityCommandHandler(
            IValidator<RemoveLegalEntityCommand> validator,
            ILog logger,
            IEmployerAgreementRepository employerAgreementRepository,
            IMediator mediator,
            IHashingService hashingService,
            IGenericEventFactory genericEventFactory,
            IEmployerAgreementEventFactory employerAgreementEventFactory,
            IMessagePublisher messagePublisher)
        {
            _validator = validator;
            _logger = logger;
            _employerAgreementRepository = employerAgreementRepository;
            _mediator = mediator;
            _hashingService = hashingService;
            _genericEventFactory = genericEventFactory;
            _employerAgreementEventFactory = employerAgreementEventFactory;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(RemoveLegalEntityCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                _logger.Info($"User {message.UserId} tried to remove {message.HashedLegalAgreementId} from Account {message.HashedAccountId}");
                throw new UnauthorizedAccessException();
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var legalAgreementId = _hashingService.DecodeValue(message.HashedLegalAgreementId);

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(legalAgreementId);

            await _employerAgreementRepository.RemoveLegalEntityFromAccount(legalAgreementId);

            await AddAuditEntry(accountId, message.HashedLegalAgreementId);

            await CreateEvent(message.HashedLegalAgreementId);

            if (agreement != null)
            {
                await PublishLegalEntityRemovedMessage(accountId, legalAgreementId,
                    agreement.Status, agreement.SignedByName, agreement.LegalEntityId);
            }
        }

        private async Task PublishLegalEntityRemovedMessage(long accountId, 
            long agreementId, EmployerAgreementStatus status, string removedByName, long legalEntityId)
        {
            await _messagePublisher.PublishAsync(new LegalEntityRemovedMessage(accountId, agreementId, status == EmployerAgreementStatus.Signed, removedByName, legalEntityId));
        }

        private async Task AddAuditEntry(long accountId, string employerAgreementId)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description = $"EmployerAgreement {employerAgreementId} removed from account {accountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        PropertyUpdate.FromString("Status", EmployerAgreementStatus.Removed.ToString())
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = accountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "EmployerAgreement", Id = employerAgreementId }
                }
            });
        }

        private async Task CreateEvent(string hashedAgreementId)
        {
            var agreementEvent = _employerAgreementEventFactory.RemoveAgreementEvent(hashedAgreementId);

            var genericEvent = _genericEventFactory.Create(agreementEvent);

            await _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
        }
    }
}
