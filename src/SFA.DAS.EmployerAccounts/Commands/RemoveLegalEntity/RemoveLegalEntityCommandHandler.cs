using System;
using System.Collections.Generic;
using System.Globalization;
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
using SFA.DAS.Hashing;
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
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;
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
            IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService,
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
            _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
            _genericEventFactory = genericEventFactory;
            _employerAgreementEventFactory = employerAgreementEventFactory;
            _agreementService = agreementService;
            _membershipRepository = membershipRepository;
            _eventPublisher = eventPublisher;
        }

        protected override async Task HandleCore(RemoveLegalEntityCommand message)
        {
            await ValidateMessage(message);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var accountLegalEntityId = _accountLegalEntityPublicHashingService.DecodeValue(message.AccountLegalEntityPublicHashedId);

            var removedAgreements = await _employerAgreementRepository.RemoveLegalEntityFromAccount(accountLegalEntityId);

            await _agreementService.RemoveFromCacheAsync(accountId);

            await ProcessRemovedAgreements(accountId, accountLegalEntityId, message.UserId, removedAgreements);
        }

        private async Task<string> GetCallerName(string userId, long accountId)
        {
            var caller = await _membershipRepository.GetCaller(accountId, userId);
            var createdByName = caller.FullName();
            return createdByName;
        }

        private async Task ProcessRemovedAgreements(long accountId, long accountLegalEntityId, string userId, EmployerAgreementRemoved[] removedAgreements)
        {
            var createdByName = await GetCallerName(userId, accountId);

            foreach (var removedAgreement in removedAgreements)
            {
                await ProcessRemovedAgreement(accountId, userId, removedAgreement, createdByName);
            }
        }

        private async Task ProcessRemovedAgreement(long accountId, string userId,
            EmployerAgreementRemoved removedAgreement, string createdByName)
        {
            await AddAuditEntry(accountId, removedAgreement.EmployerAgreementId);

            var hashedAgreementId = _hashingService.HashValue(removedAgreement.EmployerAgreementId);

            await CreateEvent(hashedAgreementId);

            await PublishLegalEntityRemovedMessage(
                accountId,
                removedAgreement.EmployerAgreementId,
                removedAgreement.Signed,
                createdByName,
                removedAgreement.LegalEntityId,
                removedAgreement.LegalEntityName,
                removedAgreement.AccountLegalEntityId,
                userId);
        }

        private async Task ValidateMessage(RemoveLegalEntityCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                _logger.Info(
                    $"User {message.UserId} tried to remove {message.AccountLegalEntityPublicHashedId} from Account {message.HashedAccountId}");
                throw new UnauthorizedAccessException();
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

        private async Task AddAuditEntry(long accountId, long employerAgreementId)
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
                    RelatedEntities = new List<Entity> { new Entity { Id = accountId.ToString(CultureInfo.InvariantCulture), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "EmployerAgreement", Id = employerAgreementId.ToString(CultureInfo.InvariantCulture) }
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
