using System.Threading;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;

public class AddPayeToAccountCommandHandler : IRequestHandler<AddPayeToAccountCommand>
{
    private readonly IValidator<AddPayeToAccountCommand> _validator;
    private readonly IPayeRepository _payeRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEncodingService _encodingService;
    private readonly IMediator _mediator;
    private readonly IGenericEventFactory _genericEventFactory;
    private readonly IPayeSchemeEventFactory _payeSchemeEventFactory;

    public AddPayeToAccountCommandHandler(
        IValidator<AddPayeToAccountCommand> validator,
        IPayeRepository payeRepository,
        IEventPublisher eventPublisher,
        IEncodingService encodingService,
        IMediator mediator,
        IGenericEventFactory genericEventFactory,
        IPayeSchemeEventFactory payeSchemeEventFactory)
    {
        _validator = validator;
        _payeRepository = payeRepository;
        _eventPublisher = eventPublisher;
        _encodingService = encodingService;
        _mediator = mediator;
        _genericEventFactory = genericEventFactory;
        _payeSchemeEventFactory = payeSchemeEventFactory;
    }

    public async Task<Unit> Handle(AddPayeToAccountCommand message, CancellationToken cancellationToken)
    {
        await ValidateMessage(message);

        var accountId = _encodingService.Decode(message.HashedAccountId, EncodingType.AccountId);

        await _payeRepository.AddPayeToAccount(
            new Paye
            {
                AccessToken = message.AccessToken,
                RefreshToken = message.RefreshToken,
                AccountId = accountId,
                EmpRef = message.Empref,
                RefName = message.EmprefName,
                Aorn = message.Aorn
            }
        );

        var userResponse = await _mediator.Send(new GetUserByRefQuery { UserRef = message.ExternalUserId }, cancellationToken);

        await AddAuditEntry(message, accountId);

        await AddPayeScheme(message.Empref, accountId, userResponse.User.FullName, userResponse.User.UserRef, message.Aorn, message.EmprefName, userResponse.User.CorrelationId);

        await NotifyPayeSchemeAdded(message.HashedAccountId, message.Empref);

        return Unit.Value;
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

        await _mediator.Send(new PublishGenericEventCommand { Event = genericEvent });
    }

    private async Task AddPayeScheme(string payeRef, long accountId, string userName, string userRef, string aorn, string schemeName, string correlationId)
    {
        await _eventPublisher.Publish(new AddedPayeSchemeEvent
        {
            PayeRef = payeRef,
            AccountId = accountId,
            UserName = userName,
            UserRef = Guid.Parse(userRef),
            Created = DateTime.UtcNow,
            Aorn = aorn,
            SchemeName = schemeName,
            CorrelationId = correlationId
        });

        if (!string.IsNullOrWhiteSpace(aorn))
        {
            await _mediator.Send(new AccountLevyStatusCommand
            {
                AccountId = accountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy
            });
        }
    }

    private async Task AddAuditEntry(AddPayeToAccountCommand message, long accountId)
    {
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "CREATED",
                Description = $"Paye scheme {message.Empref} added to account {accountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromString("Ref", message.Empref),
                    PropertyUpdate.FromString("AccessToken", message.AccessToken),
                    PropertyUpdate.FromString("RefreshToken", message.RefreshToken),
                    PropertyUpdate.FromString("Name", message.EmprefName),
                    PropertyUpdate.FromString("Aorn", message.Aorn)
                },
                RelatedEntities = new List<AuditEntity> { new AuditEntity { Id = accountId.ToString(), Type = "Account" } },
                AffectedEntity = new AuditEntity { Type = "Paye", Id = message.Empref }
            }
        });
    }
}