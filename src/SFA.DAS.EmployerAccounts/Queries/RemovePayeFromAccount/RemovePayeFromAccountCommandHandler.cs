using System.Threading;
using SFA.DAS.EmployerAccounts.Attributes;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;

public class RemovePayeFromAccountCommandHandler : IRequestHandler<RemovePayeFromAccountCommand>
{
    private readonly IMediator _mediator;
    private readonly IValidator<RemovePayeFromAccountCommand> _validator;
    private readonly IPayeRepository _payeRepository;
    private readonly IEncodingService _encodingService;
    private readonly IGenericEventFactory _genericEventFactory;
    private readonly IPayeSchemeEventFactory _payeSchemeEventFactory;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMembershipRepository _membershipRepository;

    [ServiceBusConnectionKey("employer_shared")]
    public RemovePayeFromAccountCommandHandler(
        IMediator mediator,
        IValidator<RemovePayeFromAccountCommand> validator,
        IPayeRepository payeRepository,
        IEncodingService encodingService,
        IGenericEventFactory genericEventFactory,
        IPayeSchemeEventFactory payeSchemeEventFactory,
        IEventPublisher eventPublisher,
        IMembershipRepository membershipRepository)
    {
        _mediator = mediator;
        _validator = validator;
        _payeRepository = payeRepository;
        _encodingService = encodingService;
        _genericEventFactory = genericEventFactory;
        _payeSchemeEventFactory = payeSchemeEventFactory;
        _eventPublisher = eventPublisher;
        _membershipRepository = membershipRepository;
    }

    public async Task<Unit> Handle(RemovePayeFromAccountCommand message, CancellationToken cancellationToken)
    {
        await ValidateMessage(message);

        var accountId = _encodingService.Decode(message.HashedAccountId, EncodingType.AccountId);

        await AddAuditEntry(message.UserId, message.PayeRef, accountId.ToString());

        await _payeRepository.RemovePayeFromAccount(accountId, message.PayeRef);

        var loggedInPerson = await _membershipRepository.GetCaller(accountId, message.UserId);

        await QueuePayeRemovedMessage(message.PayeRef, accountId, message.CompanyName, loggedInPerson.FullName(), loggedInPerson.UserRef);

        await NotifyPayeSchemeRemoved(message.HashedAccountId, message.PayeRef);

        return Unit.Value;
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

        await _mediator.Send(new PublishGenericEventCommand { Event = genericEvent });
    }


    private Task QueuePayeRemovedMessage(string payeRef, long accountId, string organisationName, string userName, Guid userRef)
    {
        return _eventPublisher.Publish(new DeletedPayeSchemeEvent
        {
            AccountId = accountId,
            PayeRef = payeRef,
            OrganisationName = organisationName,
            UserName = userName,
            UserRef = userRef,
            Created = DateTime.UtcNow
        });
    }

    private async Task AddAuditEntry(string userId, string payeRef, string accountId)
    {
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "DELETED",
                Description = $"User {userId} has removed PAYE schema {payeRef} from account {accountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    new PropertyUpdate {PropertyName = "AccountId", NewValue = accountId},
                    new PropertyUpdate {PropertyName = "UserId", NewValue = userId},
                    new PropertyUpdate {PropertyName = "PayeRef", NewValue = payeRef}
                },
                RelatedEntities = new List<AuditEntity> { new AuditEntity { Id = accountId, Type = "Account" } },
                AffectedEntity = new AuditEntity { Type = "Paye", Id = payeRef }
            }
        });
    }
}