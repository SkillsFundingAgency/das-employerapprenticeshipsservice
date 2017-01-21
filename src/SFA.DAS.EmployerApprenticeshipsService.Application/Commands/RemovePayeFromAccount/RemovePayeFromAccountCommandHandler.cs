using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
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

        public RemovePayeFromAccountCommandHandler(IMediator mediator, IValidator<RemovePayeFromAccountCommand> validator, IAccountRepository accountRepository, IHashingService hashingService)
        {
            _mediator = mediator;
            _validator = validator;
            _accountRepository = accountRepository;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(RemovePayeFromAccountCommand message)
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

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await AddAuditEntry(message.UserId, message.PayeRef, accountId.ToString());

            await _accountRepository.RemovePayeFromAccount(accountId, message.PayeRef);
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
