using System.Threading;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Services;


namespace SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;

public class CreateUserAccountCommandHandler : IRequestHandler<CreateUserAccountCommand, CreateUserAccountCommandResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMediator _mediator;
    private readonly IValidator<CreateUserAccountCommand> _validator;
    private readonly IEncodingService _encodingService;
    private readonly IGenericEventFactory _genericEventFactory;
    private readonly IAccountEventFactory _accountEventFactory;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateUserAccountCommandHandler(
        IAccountRepository accountRepository,
        IMediator mediator,
        IValidator<CreateUserAccountCommand> validator,
        IEncodingService encodingService,
        IGenericEventFactory genericEventFactory,
        IAccountEventFactory accountEventFactory,
        IMembershipRepository membershipRepository,
        IEventPublisher eventPublisher)
    {
        _accountRepository = accountRepository;
        _mediator = mediator;
        _validator = validator;
        _encodingService = encodingService;
        _genericEventFactory = genericEventFactory;
        _accountEventFactory = accountEventFactory;
        _membershipRepository = membershipRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<CreateUserAccountCommandResponse> Handle(CreateUserAccountCommand message, CancellationToken cancellationToken)
    {
        ValidateMessage(message);

        var externalUserId = Guid.Parse(message.ExternalUserId);

        var userResponse = await _mediator.Send(new GetUserByRefQuery { UserRef = message.ExternalUserId }, cancellationToken);

        var createAccountResult = await _accountRepository.CreateUserAccount(userResponse.User.Id, message.OrganisationName);

        var hashedAccountId = _encodingService.Encode(createAccountResult.AccountId, EncodingType.AccountId);
        var publicHashedAccountId = _encodingService.Encode(createAccountResult.AccountId, EncodingType.PublicAccountId);

        await _accountRepository.UpdateAccountHashedIds(createAccountResult.AccountId, hashedAccountId, publicHashedAccountId);

        var caller = await _membershipRepository.GetCaller(createAccountResult.AccountId, message.ExternalUserId);

        var createdByName = caller.FullName();

        await PublishAccountCreatedMessage(createAccountResult.AccountId, hashedAccountId, publicHashedAccountId,
            message.OrganisationName, createdByName, externalUserId);

        await NotifyAccountCreated(hashedAccountId);

        await CreateAuditEntries(message, createAccountResult, hashedAccountId, userResponse.User);

        return new CreateUserAccountCommandResponse
        {
            HashedAccountId = hashedAccountId
        };
    }

    private Task NotifyAccountCreated(string hashedAccountId)
    {
        var accountEvent = _accountEventFactory.CreateAccountCreatedEvent(hashedAccountId);

        var genericEvent = _genericEventFactory.Create(accountEvent);

        return _mediator.Send(new PublishGenericEventCommand { Event = genericEvent });
    }

    private Task PublishAccountCreatedMessage(long accountId, string hashedId, string publicHashedId, string name,
        string createdByName, Guid userRef)
    {
        return _eventPublisher.Publish(new CreatedAccountEvent
        {
            AccountId = accountId,
            HashedId = hashedId,
            PublicHashedId = publicHashedId,
            Name = name,
            UserName = createdByName,
            UserRef = userRef,
            Created = DateTime.UtcNow
        });
    }

    private void ValidateMessage(CreateUserAccountCommand message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
            throw new InvalidRequestException(validationResult.ValidationDictionary);
    }

    private async Task CreateAuditEntries(CreateUserAccountCommand message, CreateUserAccountResult returnValue,
        string hashedAccountId, User user)
    {
        //Account
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "CREATED",
                Description = $"Account {message.OrganisationName} created with id {returnValue.AccountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromLong("AccountId", returnValue.AccountId),
                    PropertyUpdate.FromString("HashedId", hashedAccountId),
                    PropertyUpdate.FromString("Name", message.OrganisationName),
                    PropertyUpdate.FromDateTime("CreatedDate", DateTime.UtcNow),
                },
                AffectedEntity = new AuditEntity { Type = "Account", Id = returnValue.AccountId.ToString() },
                RelatedEntities = new List<AuditEntity>()
            }
        });

        //Membership Account
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "CREATED",
                Description = $"User {message.ExternalUserId} added to account {returnValue.AccountId} as owner",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromLong("AccountId", returnValue.AccountId),
                    PropertyUpdate.FromString("UserId", message.ExternalUserId),
                    PropertyUpdate.FromString("Role", Role.Owner.ToString()),
                    PropertyUpdate.FromDateTime("CreatedDate", DateTime.UtcNow)
                },
                RelatedEntities = new List<AuditEntity>
                {
                    new AuditEntity { Id = returnValue.AccountId.ToString(), Type = "Account" },
                    new AuditEntity { Id = user.Id.ToString(), Type = "User" }
                },
                AffectedEntity = new AuditEntity { Type = "Membership", Id = message.ExternalUserId }
            }
        });
    }
}