using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;
using IGenericEventFactory = SFA.DAS.EmployerAccounts.Factories.IGenericEventFactory;

namespace SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommandHandler : AsyncRequestHandler<SignEmployerAgreementCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<SignEmployerAgreementCommand> _validator;
        private readonly IEmployerAgreementEventFactory _agreementEventFactory;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IMediator _mediator;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICommitmentService _commitmentService;
        private readonly IAgreementService _agreementService;


        public SignEmployerAgreementCommandHandler(
            IMembershipRepository membershipRepository,
            IEmployerAgreementRepository employerAgreementRepository,
            IHashingService hashingService,
            IValidator<SignEmployerAgreementCommand> validator,
            IEmployerAgreementEventFactory agreementEventFactory,
            IGenericEventFactory genericEventFactory,
            IMediator mediator,
            IEventPublisher eventPublisher,
            ICommitmentService commitmentService,
            IAgreementService agreementService)
        {
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _validator = validator;
            _agreementEventFactory = agreementEventFactory;
            _genericEventFactory = genericEventFactory;
            _mediator = mediator;
            _eventPublisher = eventPublisher;
            _commitmentService = commitmentService;
            _agreementService = agreementService;
        }

        protected override async Task HandleCore(SignEmployerAgreementCommand message)
        {
            await ValidateRequest(message);
            var owner = await VerifyUserIsAccountOwner(message);

            var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

            await SignAgreement(message, agreementId, owner);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await AddAuditEntry(message, accountId, agreementId);

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

            await _employerAgreementRepository.EvaluateEmployerLegalEntityAgreementStatus(accountId, agreement.LegalEntityId);

            var hashedLegalEntityId = _hashingService.HashValue((long) agreement.LegalEntityId);

            await PublishEvents(message, hashedLegalEntityId, accountId, agreement, agreementId, owner);

            await _agreementService.RemoveFromCacheAsync(accountId);
        }

        private async Task PublishEvents(SignEmployerAgreementCommand message, string hashedLegalEntityId, long accountId,
            EmployerAgreementView agreement, long agreementId, MembershipView owner)
        {
            await PublishLegalGenericEvent(message, hashedLegalEntityId);

            var commitments = await _commitmentService.GetEmployerCommitments(accountId);

            var accountHasCommitments = commitments?.Any() ?? false;

            await PublishAgreementSignedMessage(accountId, agreement.LegalEntityId, agreement.LegalEntityName, agreementId,
                accountHasCommitments, owner.FullName(), owner.UserRef, agreement.AgreementType, agreement.VersionNumber);
        }

        private async Task PublishLegalGenericEvent(SignEmployerAgreementCommand message, string hashedLegalEntityId)
        {
            var agreementEvent =
                _agreementEventFactory.CreateSignedEvent(message.HashedAccountId, hashedLegalEntityId,
                    message.HashedAgreementId);

            var genericEvent = _genericEventFactory.Create(agreementEvent);

            await _mediator.SendAsync(new PublishGenericEventCommand {Event = genericEvent});
        }

        private async Task<MembershipView> VerifyUserIsAccountOwner(SignEmployerAgreementCommand message)
        {
            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            if (owner == null || owner.Role != Role.Owner)
                throw new UnauthorizedAccessException();
            return owner;
        }

        private async Task ValidateRequest(SignEmployerAgreementCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        private async Task SignAgreement(SignEmployerAgreementCommand message, long agreementId, MembershipView owner)
        {
            var signedAgreementDetails = new Models.EmployerAgreement.SignEmployerAgreement
            {
                SignedDate = message.SignedDate,
                AgreementId = agreementId,
                SignedById = owner.UserId,
                SignedByName = $"{owner.FirstName} {owner.LastName}"
            };

            await _employerAgreementRepository.SignAgreement(signedAgreementDetails);
        }

        private Task PublishAgreementSignedMessage(
            long accountId, long legalEntityId, string legalEntityName, long agreementId,
            bool cohortCreated, string currentUserName, string currentUserRef, AgreementType agreementType, int versionNumber)
        {
            return _eventPublisher.Publish(new SignedAgreementEvent
            {
                AccountId = accountId,
                AgreementId = agreementId,
                LegalEntityId = legalEntityId,
                OrganisationName = legalEntityName,
                CohortCreated = cohortCreated,
                Created = DateTime.UtcNow,
                UserName = currentUserName,
                UserRef = Guid.Parse(currentUserRef),
                AgreementType = agreementType,
                SignedAgreementVersion = versionNumber
            });
        }

        private async Task AddAuditEntry(SignEmployerAgreementCommand message, long accountId, long agreementId)
        {
            await _mediator.SendAsync<Unit>(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description = $"Agreement {agreementId} added to account {accountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        PropertyUpdate.FromString("UserId", message.ExternalUserId),
                        PropertyUpdate.FromString("SignedDate", message.SignedDate.ToString("U"))
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = accountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Agreement", Id = agreementId.ToString() }
                }
            });
        }
    }
}