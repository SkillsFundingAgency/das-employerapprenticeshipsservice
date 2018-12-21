using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;

namespace SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount
{
    public class AddPayeToAccountCommandHandler : AsyncRequestHandler<AddPayeToAccountCommand>
    {

        private readonly IValidator<AddPayeToAccountCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IPayeSchemeEventFactory _payeSchemeEventFactory;
        private readonly IRefreshEmployerLevyService _refreshEmployerLevyService;

        public AddPayeToAccountCommandHandler(
            IValidator<AddPayeToAccountCommand> validator,
            IAccountRepository accountRepository,
            IEventPublisher eventPublisher,
            IHashingService hashingService,
            IMediator mediator,
            IGenericEventFactory genericEventFactory,
            IPayeSchemeEventFactory payeSchemeEventFactory,
            IRefreshEmployerLevyService refreshEmployerLevyService)
        {
            _validator = validator;
            _accountRepository = accountRepository;
            _eventPublisher = eventPublisher;
            _hashingService = hashingService;
            _mediator = mediator;
            _genericEventFactory = genericEventFactory;
            _payeSchemeEventFactory = payeSchemeEventFactory;
            _refreshEmployerLevyService = refreshEmployerLevyService;
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

            var userResponse = await _mediator.SendAsync(new GetUserByRefQuery { UserRef = message.ExternalUserId });

            await AddAuditEntry(message, accountId);

            await RefreshLevy(accountId, message.Empref);

            await AddPayeScheme(message.Empref, accountId, userResponse.User.FullName, userResponse.User.UserRef);

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

            await _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
        }

        private async Task RefreshLevy(long accountId, string payeRef)
        {
            await _refreshEmployerLevyService.QueueRefreshLevyMessage(accountId, payeRef);
        }

        private Task AddPayeScheme(string payeRef, long accountId, string userName, string userRef)
        {
            return _eventPublisher.Publish(new AddedPayeSchemeEvent
            {
                PayeRef = payeRef,
                AccountId = accountId,
                UserName = userName,
                UserRef = Guid.Parse(userRef),
                Created = DateTime.UtcNow
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