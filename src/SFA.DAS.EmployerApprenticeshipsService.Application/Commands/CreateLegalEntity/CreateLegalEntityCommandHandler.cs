﻿using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;
using SFA.DAS.Hashing;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommandHandler : IAsyncRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>
    {
        private readonly IValidator<CreateLegalEntityCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly ILegalEntityEventFactory _legalEntityEventFactory;

        private readonly IEventPublisher _eventPublisher;
        private readonly IHashingService _hashingService;
        private readonly IHashingService _publicHashingService;
        private readonly IAgreementService _agreementService;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public CreateLegalEntityCommandHandler(
            IAccountRepository accountRepository,
            IMembershipRepository membershipRepository,
            IMediator mediator,
            IGenericEventFactory genericEventFactory,
            ILegalEntityEventFactory legalEntityEventFactory,
            IEventPublisher eventPublisher,
            IHashingService hashingService,
            IPublicHashingService publicHashingService,
            IAgreementService agreementService,
            IEmployerAgreementRepository employerAgreementRepository, 
            IValidator<CreateLegalEntityCommand> validator)
        {
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _genericEventFactory = genericEventFactory;
            _legalEntityEventFactory = legalEntityEventFactory;
            _eventPublisher = eventPublisher;
            _hashingService = hashingService;
            _publicHashingService = publicHashingService;
            _agreementService = agreementService;
            _employerAgreementRepository = employerAgreementRepository;
            _validator = validator;
        }

        public async Task<CreateLegalEntityCommandResponse> Handle(CreateLegalEntityCommand message)
        {

            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }
            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            var ownerExternalUserId = Guid.Parse(owner.UserRef);

            var createParams = new CreateLegalEntityWithAgreementParams
            {
                AccountId = owner.AccountId,
                Name = message.Name,
                Status = message.Status,
                Code = string.IsNullOrEmpty(message.Code) ? Guid.NewGuid().ToString() : message.Code,
                DateOfIncorporation = message.DateOfIncorporation,
                PublicSectorDataSource = message.PublicSectorDataSource,
                Source = message.Source,
                Address = message.Address,
                Sector = message.Sector
            };

            var agreementView = await _accountRepository.CreateLegalEntityWithAgreement(createParams);

            agreementView.HashedAgreementId = _hashingService.HashValue(agreementView.Id);

            await CreateAuditEntries(owner, agreementView);

            await NotifyLegalEntityCreated(message.HashedAccountId, agreementView.LegalEntityId);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await EvaluateEmployerLegalEntityAgreementStatus(owner.AccountId, agreementView.LegalEntityId);

            agreementView.AccountLegalEntityPublicHashedId = _publicHashingService.HashValue(agreementView.AccountLegalEntityId);

            await PublishLegalEntityAddedMessage(accountId, agreementView.Id, createParams.Name, owner.FullName(), agreementView.LegalEntityId,
                agreementView.AccountLegalEntityId, agreementView.AccountLegalEntityPublicHashedId, ownerExternalUserId);

            await PublishAgreementCreatedMessage(accountId, agreementView.Id, createParams.Name, owner.FullName(), agreementView.LegalEntityId, ownerExternalUserId);

            await _agreementService.RemoveFromCacheAsync(accountId);

            return new CreateLegalEntityCommandResponse
            {
                AgreementView = agreementView
            };
        }

        private Task PublishLegalEntityAddedMessage(long accountId, long agreementId, string organisationName, string createdByName, long legalEntityId, long accountLegalEntityId, string accountLegalEntityPublicHashedId, Guid userRef)
        {
            return _eventPublisher.Publish(new AddedLegalEntityEvent
            {
                AccountId = accountId,
                AgreementId = agreementId,
                LegalEntityId = legalEntityId,
                AccountLegalEntityId = accountLegalEntityId,
                AccountLegalEntityPublicHashedId = accountLegalEntityPublicHashedId,
                OrganisationName = organisationName,
                UserName = createdByName,
                UserRef = userRef,
                Created = DateTime.UtcNow
            });
        }

        private Task PublishAgreementCreatedMessage(long accountId, long agreementId, string organisationName, string createdByName, long legalEntityId, Guid userRef)
        {
            return _eventPublisher.Publish(new CreatedAgreementEvent
            {
                AgreementId = agreementId,
                LegalEntityId = legalEntityId,
                OrganisationName = organisationName,
                AccountId = accountId,
                UserName = createdByName,
                UserRef = userRef,
                Created = DateTime.UtcNow
            });
        }

        private Task NotifyLegalEntityCreated(string hashedAccountId, long legalEntityId)
        {
            var legalEntityEvent = _legalEntityEventFactory.CreateLegalEntityCreatedEvent(
                                            hashedAccountId, legalEntityId);

            var genericEvent = _genericEventFactory.Create(legalEntityEvent);

            return _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
        }

        private Task EvaluateEmployerLegalEntityAgreementStatus(long accountId, long legalEntityId)
        {
            return _employerAgreementRepository.EvaluateEmployerLegalEntityAgreementStatus(accountId, legalEntityId);
        }

        private async Task CreateAuditEntries(MembershipView owner, EmployerAgreementView agreementView)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description = $"User {owner.Email} added legal entity {agreementView.LegalEntityId} to account {owner.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "AccountId", NewValue = agreementView.AccountId.ToString()},
                        new PropertyUpdate {PropertyName = "Id", NewValue = agreementView.LegalEntityId.ToString()},
                        new PropertyUpdate {PropertyName = "Name", NewValue = agreementView.LegalEntityName},
                        new PropertyUpdate {PropertyName = "Code", NewValue = agreementView.LegalEntityCode},
                        new PropertyUpdate {PropertyName = "Source", NewValue = agreementView.LegalEntitySource.ToString()},
                        new PropertyUpdate {PropertyName = "Status", NewValue = agreementView.LegalEntityStatus},
                        new PropertyUpdate {PropertyName = "Address", NewValue = agreementView.LegalEntityAddress},
                        new PropertyUpdate {PropertyName = "DateOfInception", NewValue = agreementView.LegalEntityInceptionDate.GetDateString("G")},
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = agreementView.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "LegalEntity", Id = agreementView.LegalEntityId.ToString() }
                }
            });

            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description = $"User {owner.Email} added signed agreement {agreementView.Id} to account {owner.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "AccountId", NewValue = agreementView.AccountId.ToString()},
                        new PropertyUpdate {PropertyName = "SignedDate", NewValue = agreementView.SignedDate.GetDateString("G")},
                        new PropertyUpdate {PropertyName = "SignedBy", NewValue = $"{owner.FirstName} {owner.LastName}"}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = agreementView.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "EmployerAgreement", Id = agreementView.Id.ToString() }
                }
            });
        }
    }
}
