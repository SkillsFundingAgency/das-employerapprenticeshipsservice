using System;
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
using IGenericEventFactory = SFA.DAS.EAS.Application.Factories.IGenericEventFactory;

namespace SFA.DAS.EAS.Application.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommandHandler : AsyncRequestHandler<RenameEmployerAccountCommand>
    {
        private readonly IEmployerAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IValidator<RenameEmployerAccountCommand> _validator;
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IAccountEventFactory _accountEventFactory;


        public RenameEmployerAccountCommandHandler(
            IEmployerAccountRepository accountRepository, 
            IMembershipRepository membershipRepository, 
            IValidator<RenameEmployerAccountCommand> validator, 
            IHashingService hashingService, 
            IMediator mediator, 
            IGenericEventFactory genericEventFactory,
            IAccountEventFactory accountEventFactory)
        {
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

            await _accountRepository.RenameAccount(accountId, message.NewName);

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            await AddAuditEntry(owner.Email, accountId, message.NewName);

            await NotifyAccountRenamed(message.HashedAccountId);
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
