﻿using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = SFA.DAS.Audit.Types.Entity;
using IGenericEventFactory = SFA.DAS.EAS.Application.Factories.IGenericEventFactory;

namespace SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommandHandler : AsyncRequestHandler<RemovePayeFromAccountCommand>
    {
        private readonly IMediator _mediator;
        private readonly IValidator<RemovePayeFromAccountCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IHashingService _hashingService;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IPayeSchemeEventFactory _payeSchemeEventFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMembershipRepository _membershipRepository;

        [ServiceBusConnectionKey("employer_shared")]
        public RemovePayeFromAccountCommandHandler(
            IMediator mediator,
            IValidator<RemovePayeFromAccountCommand> validator,
            IAccountRepository accountRepository,
            IHashingService hashingService,
            IGenericEventFactory genericEventFactory,
            IPayeSchemeEventFactory payeSchemeEventFactory,
            IEventPublisher eventPublisher,
            IMembershipRepository membershipRepository)
        {
            _mediator = mediator;
            _validator = validator;
            _accountRepository = accountRepository;
            _hashingService = hashingService;
            _genericEventFactory = genericEventFactory;
            _payeSchemeEventFactory = payeSchemeEventFactory;
            _eventPublisher = eventPublisher;
            _membershipRepository = membershipRepository;
        }

        protected override async Task HandleCore(RemovePayeFromAccountCommand message)
        {
            await ValidateMessage(message);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await AddAuditEntry(message.UserId, message.PayeRef, accountId.ToString());

            await _accountRepository.RemovePayeFromAccount(accountId, message.PayeRef);

            var loggedInPerson = await _membershipRepository.GetCaller(accountId, message.UserId);

            await QueuePayeRemovedMessage(message.PayeRef, accountId, message.CompanyName, loggedInPerson.FullName(), loggedInPerson.UserRef);

            await NotifyPayeSchemeRemoved(message.HashedAccountId, message.PayeRef);
        }

        private async Task ValidateMessage(RemovePayeFromAccountCommand message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }
        }

        private async Task NotifyPayeSchemeRemoved(string hashedAccountId, string payeRef)
        {
            var payeEvent = _payeSchemeEventFactory.CreatePayeSchemeRemovedEvent(hashedAccountId, payeRef);

            var genericEvent = _genericEventFactory.Create(payeEvent);

            await _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
        }


        private Task QueuePayeRemovedMessage(string payeRef, long accountId, string organisationName, string userName, string userRef)
        {
            return _eventPublisher.Publish<DeletedPayeSchemeEvent>(e =>
            {
                e.AccountId = accountId;
                e.PayeRef = payeRef;
                e.OrganisationName = organisationName;
                e.UserName = userName;
                e.UserRef = Guid.Parse(userRef);
            });
        }

        private async Task AddAuditEntry(string userId, string payeRef, string accountId)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "DELETED",
                    Description = $"User {userId} has removed PAYE schema {payeRef} from account {accountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "AccountId", NewValue = accountId},
                        new PropertyUpdate {PropertyName = "UserId", NewValue = userId},
                        new PropertyUpdate {PropertyName = "PayeRef", NewValue = payeRef}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = accountId, Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Paye", Id = payeRef }
                }
            });
        }
    }
}
