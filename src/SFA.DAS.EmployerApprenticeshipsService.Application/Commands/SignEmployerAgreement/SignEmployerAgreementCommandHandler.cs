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
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using IGenericEventFactory = SFA.DAS.EAS.Application.Factories.IGenericEventFactory;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Commands.SignEmployerAgreement
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
        private readonly IMessagePublisher _messagePublisher;


        public SignEmployerAgreementCommandHandler(
            IMembershipRepository membershipRepository,
            IEmployerAgreementRepository employerAgreementRepository,
            IHashingService hashingService,
            IValidator<SignEmployerAgreementCommand> validator,
            IEmployerAgreementEventFactory agreementEventFactory,
            IGenericEventFactory genericEventFactory,
            IMediator mediator, 
            IMessagePublisher messagePublisher)
        {
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _validator = validator;
            _agreementEventFactory = agreementEventFactory;
            _genericEventFactory = genericEventFactory;
            _mediator = mediator;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(SignEmployerAgreementCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            if (owner == null || (Role) owner.RoleId != Role.Owner)
                throw new UnauthorizedAccessException();

            var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

            var signedAgreementDetails = new Domain.Models.EmployerAgreement.SignEmployerAgreement
            {
                SignedDate = message.SignedDate,
                AgreementId = agreementId,
                SignedById = owner.UserId,
                SignedByName = $"{owner.FirstName} {owner.LastName}"
            };

            await _employerAgreementRepository.SignAgreement(signedAgreementDetails);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await AddAuditEntry(message, accountId, agreementId);

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

            var hashedLegalEntityId = _hashingService.HashValue(agreement.LegalEntityId);

            var agreementEvent = _agreementEventFactory.CreateSignedEvent(message.HashedAccountId, hashedLegalEntityId,
                message.HashedAgreementId);

            var genericEvent = _genericEventFactory.Create(agreementEvent);

            await _mediator.SendAsync(new PublishGenericEventCommand {Event = genericEvent});

            await PublishAgreementSignedMessage(accountId, agreementId, message.OrganisationName, owner.FullName(), agreement.LegalEntityId);
        }

        private async Task PublishAgreementSignedMessage(long accountId, long agreementId, string organisationName, string signedByName, long legalEntityId)
        {
            await _messagePublisher.PublishAsync(new AgreementSignedMessage(accountId, agreementId, organisationName, signedByName, legalEntityId));
        }

        private async Task AddAuditEntry(SignEmployerAgreementCommand message, long accountId, long agreementId)
        {
            await _mediator.SendAsync(new CreateAuditCommand
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
                    RelatedEntities = new List<Entity> {new Entity {Id = accountId.ToString(), Type = "Account"}},
                    AffectedEntity = new Entity {Type = "Agreement", Id = agreementId.ToString()}
                }
            });
        }
    }
}