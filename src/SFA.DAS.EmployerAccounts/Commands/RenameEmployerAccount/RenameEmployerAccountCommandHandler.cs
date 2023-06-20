using System.Threading;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;

public class RenameEmployerAccountCommandHandler : IRequestHandler<RenameEmployerAccountCommand>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IEmployerAccountRepository _accountRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IValidator<RenameEmployerAccountCommand> _validator;
    private readonly IEncodingService _encodingService;
    private readonly IMediator _mediator;
    private readonly IGenericEventFactory _genericEventFactory;
    private readonly IAccountEventFactory _accountEventFactory;

    public RenameEmployerAccountCommandHandler(
        IEventPublisher eventPublisher,
        IEmployerAccountRepository accountRepository,
        IMembershipRepository membershipRepository,
        IValidator<RenameEmployerAccountCommand> validator,
        IEncodingService encodingService,
        IMediator mediator,
        IGenericEventFactory genericEventFactory,
        IAccountEventFactory accountEventFactory)
    {
        _eventPublisher = eventPublisher;
        _accountRepository = accountRepository;
        _membershipRepository = membershipRepository;
        _validator = validator;
        _encodingService = encodingService;
        _mediator = mediator;
        _genericEventFactory = genericEventFactory;
        _accountEventFactory = accountEventFactory;
    }

    public async Task<Unit> Handle(RenameEmployerAccountCommand message, CancellationToken cancellationToken)
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

        var accountId = _encodingService.Decode(message.HashedAccountId, EncodingType.AccountId);

        var account = await _accountRepository.GetAccountById(accountId);

        var accountPreviousName = account.Name;

        await _accountRepository.RenameAccount(accountId, message.NewName);

        var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

        await AddAuditEntry(owner.Email, accountId, message.NewName);

        await NotifyAccountRenamed(message.HashedAccountId);

        await PublishAccountRenamedMessage(accountId, accountPreviousName, message.NewName, owner.FullName(), owner.UserRef);

        return Unit.Value;
    }

    private Task PublishAccountRenamedMessage(
        long accountId, string previousName, string currentName, string creatorName, Guid creatorUserRef)
    {
        return _eventPublisher.Publish(new ChangedAccountNameEvent
        {
            PreviousName = previousName,
            CurrentName = currentName,
            AccountId = accountId,
            Created = DateTime.UtcNow,
            UserName = creatorName,
            UserRef = creatorUserRef
        });
    }

    private async Task NotifyAccountRenamed(string hashedAccountId)
    {
        var accountEvent = _accountEventFactory.CreateAccountRenamedEvent(hashedAccountId);

        var genericEvent = _genericEventFactory.Create(accountEvent);

        await _mediator.Send(new PublishGenericEventCommand { Event = genericEvent });
    }

    private async Task AddAuditEntry(string ownerEmail, long accountId, string name)
    {
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "UPDATED",
                Description = $"User {ownerEmail} has renamed account {accountId} to {name}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    new PropertyUpdate {PropertyName = "AccountId", NewValue = accountId.ToString()},
                    new PropertyUpdate {PropertyName = "Name", NewValue = name},
                },
                RelatedEntities = new List<AuditEntity> { new AuditEntity { Id = accountId.ToString(), Type = "Account" } },
                AffectedEntity = new AuditEntity { Type = "Account", Id = accountId.ToString() }
            }
        });
    }
}