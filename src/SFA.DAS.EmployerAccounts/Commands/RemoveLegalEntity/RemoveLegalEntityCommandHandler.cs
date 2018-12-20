using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity
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
        private readonly IAgreementService _agreementService;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEventPublisher _eventPublisher;

        public RemoveLegalEntityCommandHandler(
            IValidator<RemoveLegalEntityCommand> validator,
            ILog logger,
            IEmployerAgreementRepository employerAgreementRepository,
            IMediator mediator,
            IHashingService hashingService,
            IGenericEventFactory genericEventFactory,
            IEmployerAgreementEventFactory employerAgreementEventFactory,
            IAgreementService agreementService,
            IMembershipRepository membershipRepository,
            IEventPublisher eventPublisher)
        {
            _validator = validator;
            _logger = logger;
            _employerAgreementRepository = employerAgreementRepository;
            _mediator = mediator;
            _hashingService = hashingService;
            _genericEventFactory = genericEventFactory;
            _employerAgreementEventFactory = employerAgreementEventFactory;
            _agreementService = agreementService;
            _membershipRepository = membershipRepository;
            _eventPublisher = eventPublisher;
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

            await _agreementService.RemoveFromCacheAsync(accountId);

            await AddAuditEntry(accountId, message.HashedLegalAgreementId);

            await CreateEvent(message.HashedLegalAgreementId);

            // it appears that an agreement is created whenever we create a legal entity, so there should always be an agreement associated with a legal entity
            if (agreement != null)
            {
                var agreementSigned = agreement.Status == EmployerAgreementStatus.Signed;
                var caller = await _membershipRepository.GetCaller(accountId, message.UserId);
                var createdByName = caller.FullName();

                await PublishLegalEntityRemovedMessage(
                    accountId, 
                    legalAgreementId,
                    agreementSigned, 
                    createdByName, 
                    agreement.LegalEntityId, 
                    agreement.LegalEntityName,
                    agreement.AccountLegalEntityId,
                    message.UserId);
            }
        }

        private Task PublishLegalEntityRemovedMessage(
            long accountId, long agreementId, bool agreementSigned, string createdBy,
            long legalEntityId, string organisationName, long accountLegalEntityId, string userRef)
        {
            return _eventPublisher.Publish(new RemovedLegalEntityEvent
            {
                AccountId = accountId,
                AgreementId = agreementId,
                LegalEntityId = legalEntityId,
                AgreementSigned = agreementSigned,
                OrganisationName = organisationName,
                AccountLegalEntityId = accountLegalEntityId,
                Created = DateTime.UtcNow,
                UserName = createdBy,
                UserRef = Guid.Parse(userRef)
            });
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
