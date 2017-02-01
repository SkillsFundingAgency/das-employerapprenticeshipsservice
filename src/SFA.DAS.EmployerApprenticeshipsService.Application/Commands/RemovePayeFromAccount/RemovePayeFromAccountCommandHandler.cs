using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommandHandler : AsyncRequestHandler<RemovePayeFromAccountCommand>
    {
        private readonly IMediator _mediator;
        private readonly IValidator<RemovePayeFromAccountCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IHashingService _hashingService;
        private readonly IEventPublisher _eventPublisher;

        public RemovePayeFromAccountCommandHandler(IMediator mediator, IValidator<RemovePayeFromAccountCommand> validator, IAccountRepository accountRepository, IHashingService hashingService, IEventPublisher eventPublisher)
        {
            _mediator = mediator;
            _validator = validator;
            _accountRepository = accountRepository;
            _hashingService = hashingService;
            _eventPublisher = eventPublisher;
        }

        protected override async Task HandleCore(RemovePayeFromAccountCommand message)
        {
            await ValidateMessage(message);

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await AddAuditEntry(message.UserId, message.PayeRef, accountId.ToString());

            await _accountRepository.RemovePayeFromAccount(accountId, message.PayeRef);

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
            await _eventPublisher.PublishPayeSchemeAddedEvent(hashedAccountId, payeRef);
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
