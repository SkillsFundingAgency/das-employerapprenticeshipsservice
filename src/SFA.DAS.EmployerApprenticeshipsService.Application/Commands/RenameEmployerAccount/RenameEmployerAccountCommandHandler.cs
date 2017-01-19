using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Application.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommandHandler : AsyncRequestHandler<RenameEmployerAccountCommand>
    {
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IValidator<RenameEmployerAccountCommand> _validator;
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;

        public RenameEmployerAccountCommandHandler(IEmployerAccountRepository accountRepository, IMembershipRepository membershipRepository, IValidator<RenameEmployerAccountCommand> validator, IHashingService hashingService, IMediator mediator)
        {
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _validator = validator;
            _hashingService = hashingService;
            _mediator = mediator;
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

            await _accountRepository.RenameAccount(accountId, message.NewName);

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            await AddAuditEntry(owner.Email, accountId, message.NewName);

            await _mediator.PublishAsync(new CreateAccountEventCommand
            {
                HashedAccountId = message.HashedAccountId,
                Event = "AccountRenamed"
            });
        }

        private async Task AddAuditEntry(string ownerEmail, long accountId, string name)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
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
