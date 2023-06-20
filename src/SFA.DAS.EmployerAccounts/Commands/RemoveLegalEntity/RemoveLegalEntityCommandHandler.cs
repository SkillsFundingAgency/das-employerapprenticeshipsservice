using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;

public class RemoveLegalEntityCommandHandler : IRequestHandler<RemoveLegalEntityCommand>
{
    private readonly IValidator<RemoveLegalEntityCommand> _validator;
    private readonly ILogger<RemoveLegalEntityCommandHandler> _logger;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IMediator _mediator;
    private readonly IEncodingService _encodingService;
    private readonly IGenericEventFactory _genericEventFactory;
    private readonly IEmployerAgreementEventFactory _employerAgreementEventFactory;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICommitmentsV2ApiClient _commitmentsV2ApiClient;

    public RemoveLegalEntityCommandHandler(
        IValidator<RemoveLegalEntityCommand> validator,
        ILogger<RemoveLegalEntityCommandHandler> logger,
        IEmployerAgreementRepository employerAgreementRepository,
        IMediator mediator,
        IEncodingService encodingService,
        IGenericEventFactory genericEventFactory,
        IEmployerAgreementEventFactory employerAgreementEventFactory,
        IMembershipRepository membershipRepository,
        IEventPublisher eventPublisher,
        ICommitmentsV2ApiClient commitmentsV2ApiClient)
    {
        _validator = validator;
        _logger = logger;
        _employerAgreementRepository = employerAgreementRepository;
        _mediator = mediator;
        _encodingService = encodingService;
        _genericEventFactory = genericEventFactory;
        _employerAgreementEventFactory = employerAgreementEventFactory;
        _membershipRepository = membershipRepository;
        _eventPublisher = eventPublisher;
        _commitmentsV2ApiClient = commitmentsV2ApiClient;
    }

    public async Task<Unit> Handle(RemoveLegalEntityCommand message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        if (validationResult.IsUnauthorized)
        {
            _logger.LogInformation("User {UserId} tried to remove {AccountLegalEntityId} from Account {AccountId}", message.UserId, message.AccountLegalEntityId, message.AccountId);
            throw new UnauthorizedAccessException();
        }

        var agreements = await _employerAgreementRepository.GetAccountLegalEntityAgreements(message.AccountLegalEntityId);
        var legalAgreement = agreements.OrderByDescending(a => a.TemplateId).First();

        var hashedAccountId = _encodingService.Encode(message.AccountId, EncodingType.AccountId);
        var hashedLegalAgreementId = _encodingService.Encode(legalAgreement.Id, EncodingType.AccountId);

        var agreement = await _employerAgreementRepository.GetEmployerAgreement(legalAgreement.Id);

        if (agreements.Any(x => x.SignedDate.HasValue))
        {
            await ValidateLegalEntityHasNoCommitments(agreement, message.AccountId, validationResult);
        }

        await _employerAgreementRepository.RemoveLegalEntityFromAccount(legalAgreement.Id);

        await Task.WhenAll(
            AddAuditEntry(hashedAccountId, hashedLegalAgreementId),
            CreateEvent(hashedLegalAgreementId)
        );

        // it appears that an agreement is created whenever we create a legal entity, so there should always be an agreement associated with a legal entity
        if (agreement == null)
        {
            return Unit.Value;
        }

        var agreementSigned = agreement.Status == EmployerAgreementStatus.Signed;
        var caller = await _membershipRepository.GetCaller(message.AccountId, message.UserId);
        var createdByName = caller.FullName();

        await PublishLegalEntityRemovedMessage(
            message.AccountId,
            legalAgreement.Id,
            agreementSigned,
            createdByName,
            agreement.LegalEntityId,
            agreement.LegalEntityName,
            agreement.AccountLegalEntityId,
            message.UserId);

        return Unit.Value;
    }

    private async Task ValidateLegalEntityHasNoCommitments(EmployerAgreementView agreement, long accountId, ValidationResult validationResult)
    {
        var commitments = await _commitmentsV2ApiClient.GetEmployerAccountSummary(accountId);

        var commitment = commitments.ApprenticeshipStatusSummaryResponse.FirstOrDefault(c =>
            !string.IsNullOrEmpty(c.LegalEntityIdentifier)
            && c.LegalEntityIdentifier.Equals(agreement.LegalEntityCode)
            && c.LegalEntityOrganisationType == agreement.LegalEntitySource);

        if (commitment != null && (commitment.ActiveCount + commitment.PausedCount + commitment.PendingApprovalCount + commitment.WithdrawnCount) != 0)
        {
            validationResult.AddError(nameof(agreement.Id), "Agreement has already been signed and has active commitments");
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }
    }

    private Task PublishLegalEntityRemovedMessage(
        long accountId, long agreementId, bool agreementSigned, string createdBy,
        long legalEntityId, string organisationName, long accountLegalEntityId, string userRef)
    {
        return _eventPublisher.Publish(new RemovedLegalEntityEvent
        {
            AccountId = accountId,
            AgreementId = agreementId,
            LegalEntityId = legalEntityId,
            AgreementSigned = agreementSigned,
            OrganisationName = organisationName,
            AccountLegalEntityId = accountLegalEntityId,
            Created = DateTime.UtcNow,
            UserName = createdBy,
            UserRef = Guid.Parse(userRef)
        });
    }

    private async Task AddAuditEntry(string hashedAccountId, string employerAgreementId)
    {
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "UPDATED",
                Description = $"EmployerAgreement {employerAgreementId} removed from account {hashedAccountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromString("Status", EmployerAgreementStatus.Removed.ToString())
                },
                RelatedEntities = new List<AuditEntity> { new() { Id = hashedAccountId, Type = "Account" } },
                AffectedEntity = new AuditEntity { Type = "EmployerAgreement", Id = employerAgreementId }
            }
        });
    }

    private async Task CreateEvent(string hashedAgreementId)
    {
        var agreementEvent = _employerAgreementEventFactory.RemoveAgreementEvent(hashedAgreementId);

        var genericEvent = _genericEventFactory.Create(agreementEvent);

        await _mediator.Send(new PublishGenericEventCommand { Event = genericEvent });
    }
}