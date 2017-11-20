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
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using IGenericEventFactory = SFA.DAS.EAS.Application.Factories.IGenericEventFactory;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Commands.AddPayeToAccount
{
    public class AddPayeToAccountCommandHandler : AsyncRequestHandler<AddPayeToAccountCommand>
    {

        private readonly IValidator<AddPayeToAccountCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IPayeSchemeEventFactory _payeSchemeEventFactory;
        private readonly IRefreshEmployerLevyService _refreshEmployerLevyService;
        private readonly IMembershipRepository _membershipRepository;

        public AddPayeToAccountCommandHandler(IValidator<AddPayeToAccountCommand> validator, IAccountRepository accountRepository, IMessagePublisher messagePublisher, IHashingService hashingService, 
            IMediator mediator, IGenericEventFactory genericEventFactory, IPayeSchemeEventFactory payeSchemeEventFactory, IRefreshEmployerLevyService refreshEmployerLevyService, IMembershipRepository membershipRepository)
        {
            _validator = validator;
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
            _hashingService = hashingService;
            _mediator = mediator;
            _genericEventFactory = genericEventFactory;
            _payeSchemeEventFactory = payeSchemeEventFactory;
            _refreshEmployerLevyService = refreshEmployerLevyService;
            _membershipRepository = membershipRepository;
        }

        protected override async Task HandleCore(AddPayeToAccountCommand message)
        {
            await ValidateMessage(message);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await _accountRepository.AddPayeToAccount(
                    new Paye
                    {
                        AccessToken = message.AccessToken,
                        RefreshToken = message.RefreshToken,
                        AccountId = accountId,
                        EmpRef = message.Empref,
                        RefName = message.EmprefName
                    }
                );

            await AddAuditEntry(message, accountId);

            await RefreshLevy(accountId, message.Empref);

            await AddPayeScheme(message.Empref, accountId, _membershipRepository.GetCaller(accountId,message.ExternalUserId).Result.FullName());

            await NotifyPayeSchemeAdded(message.HashedAccountId, message.Empref);
        }

        private async Task ValidateMessage(AddPayeToAccountCommand message)
        {
            var result = await _validator.ValidateAsync(message);

            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }
        }

        private async Task NotifyPayeSchemeAdded(string hashedAccountId, string payeRef)
        {
            var payeEvent = _payeSchemeEventFactory.CreatePayeSchemeAddedEvent(hashedAccountId, payeRef);

            var genericEvent = _genericEventFactory.Create(payeEvent);

            await _mediator.SendAsync(new PublishGenericEventCommand {Event = genericEvent});
        }

        private async Task RefreshLevy(long accountId, string payeRef)
        {

            await _refreshEmployerLevyService.QueueRefreshLevyMessage(accountId, payeRef);
            
        }

        private async Task AddPayeScheme(string payeRef, long accountId, string createdByName)
        {
            await _messagePublisher.PublishAsync(
                new PayeSchemeCreatedMessage(payeRef, accountId, createdByName)
            );
        }

        private async Task AddAuditEntry(AddPayeToAccountCommand message, long accountId)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Paye scheme {message.Empref} added to account {accountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        PropertyUpdate.FromString("Ref", message.Empref),
                        PropertyUpdate.FromString("AccessToken", message.AccessToken),
                        PropertyUpdate.FromString("RefreshToken", message.RefreshToken),
                        PropertyUpdate.FromString("Name", message.EmprefName)
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = accountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Paye", Id = message.Empref }
                }
            });
        }
    }
}