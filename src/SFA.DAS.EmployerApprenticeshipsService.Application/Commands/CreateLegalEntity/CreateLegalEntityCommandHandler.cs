﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommandHandler : IAsyncRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly ILegalEntityEventFactory _legalEntityEventFactory;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IHashingService _hashingService;


        public CreateLegalEntityCommandHandler(
            IAccountRepository accountRepository, 
            IMembershipRepository membershipRepository, 
            IMediator mediator, 
            IGenericEventFactory genericEventFactory,
            ILegalEntityEventFactory legalEntityEventFactory,
            IMessagePublisher messagePublisher,
            IHashingService hashingService)
        {
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _genericEventFactory = genericEventFactory;
            _legalEntityEventFactory = legalEntityEventFactory;
            _messagePublisher = messagePublisher;
            _hashingService = hashingService;
        }

        public async Task<CreateLegalEntityCommandResponse> Handle(CreateLegalEntityCommand message)
        {
            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            if (string.IsNullOrEmpty(message.LegalEntity.Code))
            {
                message.LegalEntity.Code = Guid.NewGuid().ToString();
            }

            var agreementView = await _accountRepository.CreateLegalEntity(
                owner.AccountId,
                message.LegalEntity);

            

            await CreateAuditEntries(owner, agreementView);

            await NotifyLegalEntityCreated(message.HashedAccountId, agreementView.LegalEntityId);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await PublishAgreementCreatedMessage(accountId,
                agreementView.Id, message.LegalEntity.Name, owner.FullName(), agreementView.LegalEntityId);

            return new CreateLegalEntityCommandResponse
            {
                AgreementView = agreementView
            };
        }

        private async Task PublishAgreementCreatedMessage(long accountId, long agreementId, string organisationName, string createdByName, long legalEntityId)
        {
            await _messagePublisher.PublishAsync(new AgreementCreatedMessage(accountId, agreementId, organisationName, createdByName, legalEntityId));
        }

        private async Task NotifyLegalEntityCreated(string hashedAccountId, long legalEntityId)
        {
            var legalEntityEvent = _legalEntityEventFactory.CreateLegalEntityCreatedEvent(
                                            hashedAccountId, legalEntityId);

            var genericEvent = _genericEventFactory.Create(legalEntityEvent);

            await _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
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
