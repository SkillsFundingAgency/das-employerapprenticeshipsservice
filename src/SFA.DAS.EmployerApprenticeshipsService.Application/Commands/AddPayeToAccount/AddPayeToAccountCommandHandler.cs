using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Commands.AddPayeToAccount
{
    public class AddPayeToAccountCommandHandler : AsyncRequestHandler<AddPayeToAccountCommand>
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IValidator<AddPayeToAccountCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;
        private readonly IEventPublisher _eventPublisher;

        public AddPayeToAccountCommandHandler(IValidator<AddPayeToAccountCommand> validator, IAccountRepository accountRepository, IMessagePublisher messagePublisher, IHashingService hashingService, IMediator mediator, IEventPublisher eventPublisher)
        {
            _validator = validator;
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
            _hashingService = hashingService;
            _mediator = mediator;
            _eventPublisher = eventPublisher;
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

            await RefreshLevy(accountId);

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
            await _eventPublisher.PublishPayeSchemeAddedEvent(hashedAccountId, payeRef);
        }

        private async Task RefreshLevy(long accountId)
        {
            await _messagePublisher.PublishAsync(
                new EmployerRefreshLevyQueueMessage
                {
                    AccountId = accountId
                });
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