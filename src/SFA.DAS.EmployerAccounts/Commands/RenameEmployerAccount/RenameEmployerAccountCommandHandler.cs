﻿using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Factories;
using Entity = SFA.DAS.Audit.Types.Entity;


namespace SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommandHandler : AsyncRequestHandler<RenameEmployerAccountCommand>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IValidator<RenameEmployerAccountCommand> _validator;
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IAccountEventFactory _accountEventFactory;

        public RenameEmployerAccountCommandHandler(
            IEventPublisher eventPublisher,
            IEmployerAccountRepository accountRepository,
            IMembershipRepository membershipRepository,
            IValidator<RenameEmployerAccountCommand> validator,
            IHashingService hashingService,
            IMediator mediator,
            IGenericEventFactory genericEventFactory,
            IAccountEventFactory accountEventFactory)
        {
            _eventPublisher = eventPublisher;
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _validator = validator;
            _hashingService = hashingService;
            _mediator = mediator;
            _genericEventFactory = genericEventFactory;
            _accountEventFactory = accountEventFactory;
        }

        protected override async Task HandleCore(RenameEmployerAccountCommand message)
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

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            var account = await _accountRepository.GetAccountById(accountId);

            var accountPreviousName = account.Name;

            await _accountRepository.RenameAccount(accountId, message.NewName);

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            await AddAuditEntry(owner.Email, accountId, message.NewName);

            await NotifyAccountRenamed(message.HashedAccountId);

            await PublishAccountRenamedMessage(
                accountId, accountPreviousName, message.NewName, owner.FullName(), owner.UserRef);
        }

        private Task PublishAccountRenamedMessage(
            long accountId, string previousName, string currentName, string creatorName, string creatorUserRef)
        {
            return _eventPublisher.Publish(new ChangedAccountNameEvent
            {
                PreviousName = previousName,
                CurrentName = currentName,
                AccountId = accountId,
                Created = DateTime.UtcNow,
                UserName = creatorName,
                UserRef = Guid.Parse(creatorUserRef)
            });
        }

        private async Task NotifyAccountRenamed(string hashedAccountId)
        {
            var accountEvent = _accountEventFactory.CreateAccountRenamedEvent(hashedAccountId);

            var genericEvent = _genericEventFactory.Create(accountEvent);

            await _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
        }

        private async Task AddAuditEntry(string ownerEmail, long accountId, string name)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description = $"User {ownerEmail} has renamed account {accountId} to {name}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "AccountId", NewValue = accountId.ToString()},
                        new PropertyUpdate {PropertyName = "Name", NewValue = name},
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = accountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Account", Id = accountId.ToString() }
                }
            });
        }
    }
}
