using SFA.DAS.Audit.Types;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;
using IGenericEventFactory = SFA.DAS.EmployerAccounts.Factories.IGenericEventFactory;

namespace SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;

public class SignEmployerAgreementCommandHandler : IAsyncRequestHandler<SignEmployerAgreementCommand, SignEmployerAgreementCommandResponse>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IHashingService _hashingService;
    private readonly IValidator<SignEmployerAgreementCommand> _validator;
    private readonly IEmployerAgreementEventFactory _agreementEventFactory;
    private readonly IGenericEventFactory _genericEventFactory;
    private readonly IMediator _mediator;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICommitmentV2Service _commitmentService;

    public SignEmployerAgreementCommandHandler(
        IMembershipRepository membershipRepository,
        IEmployerAgreementRepository employerAgreementRepository,
        IHashingService hashingService,
        IValidator<SignEmployerAgreementCommand> validator,
        IEmployerAgreementEventFactory agreementEventFactory,
        IGenericEventFactory genericEventFactory,
        IMediator mediator,
        IEventPublisher eventPublisher,
        ICommitmentV2Service commitmentService)
    {
        _membershipRepository = membershipRepository;
        _employerAgreementRepository = employerAgreementRepository;
        _hashingService = hashingService;
        _validator = validator;
        _agreementEventFactory = agreementEventFactory;
        _genericEventFactory = genericEventFactory;
        _mediator = mediator;
        _eventPublisher = eventPublisher;
        _commitmentService = commitmentService;
    }

    public async Task<SignEmployerAgreementCommandResponse> Handle(SignEmployerAgreementCommand message)
    {
        await ValidateRequest(message);
        var owner = await VerifyUserIsAccountOwner(message);
        var userResponse = await _mediator.SendAsync(new GetUserByRefQuery { UserRef = message.ExternalUserId });

        var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

        await SignAgreement(message, agreementId, owner);

        var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

        var hashedLegalEntityId = _hashingService.HashValue((long)agreement.LegalEntityId);

        await Task.WhenAll(
            AddAuditEntry(message, agreement.AccountId, agreementId),
            _employerAgreementRepository.SetAccountLegalEntityAgreementDetails(agreement.AccountLegalEntityId, null, null, agreement.Id, agreement.VersionNumber),
            PublishEvents(message, hashedLegalEntityId, agreement, owner, userResponse.User.CorrelationId)
        );

        return new SignEmployerAgreementCommandResponse
        {
            AgreementType = agreement.AgreementType,
            LegalEntityName = agreement.LegalEntityName
        };
    }

    private async Task PublishEvents(SignEmployerAgreementCommand message, string hashedLegalEntityId, EmployerAgreementView agreement, MembershipView owner, string correlationId)
    {
        await Task.WhenAll(
            PublishLegalGenericEvent(message, hashedLegalEntityId),
            PublihAgreementSignedMessage(agreement, owner, correlationId)
        );
    }

    private async Task PublihAgreementSignedMessage(EmployerAgreementView agreement, MembershipView owner, string correlationId)
    {
        var commitments = await _commitmentService.GetEmployerCommitments(agreement.AccountId);
        var accountHasCommitments = commitments?.Any() ?? false;

        await PublishAgreementSignedMessage(agreement.AccountId, agreement.AccountLegalEntityId, agreement.LegalEntityId, agreement.LegalEntityName,
            agreement.Id, accountHasCommitments, owner.FullName(), owner.UserRef, agreement.AgreementType,
            agreement.VersionNumber, correlationId);
    }

    private async Task PublishLegalGenericEvent(SignEmployerAgreementCommand message, string hashedLegalEntityId)
    {
        var agreementEvent = _agreementEventFactory.CreateSignedEvent(message.HashedAccountId, hashedLegalEntityId, message.HashedAgreementId);
        var genericEvent = _genericEventFactory.Create(agreementEvent);

        await _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
    }

    private async Task<MembershipView> VerifyUserIsAccountOwner(SignEmployerAgreementCommand message)
    {
        var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

        if (owner == null || owner.Role != Role.Owner)
            throw new UnauthorizedAccessException();
        return owner;
    }

    private async Task ValidateRequest(SignEmployerAgreementCommand message)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
            throw new InvalidRequestException(validationResult.ValidationDictionary);
    }

    private async Task SignAgreement(SignEmployerAgreementCommand message, long agreementId, MembershipView owner)
    {
        var signedAgreementDetails = new Models.EmployerAgreement.SignEmployerAgreement
        {
            SignedDate = message.SignedDate,
            AgreementId = agreementId,
            SignedById = owner.UserId,
            SignedByName = $"{owner.FirstName} {owner.LastName}"
        };

        await _employerAgreementRepository.SignAgreement(signedAgreementDetails);
    }

    private Task PublishAgreementSignedMessage(
        long accountId, long accountLegalEntityId, long legalEntityId, string legalEntityName, long agreementId,
        bool cohortCreated, string currentUserName, Guid currentUserRef,
        AgreementType agreementType, int versionNumber, string correlationId)
    {
        return _eventPublisher.Publish(new SignedAgreementEvent
        {
            AccountId = accountId,
            AgreementId = agreementId,
            AccountLegalEntityId = accountLegalEntityId,
            LegalEntityId = legalEntityId,
            OrganisationName = legalEntityName,
            CohortCreated = cohortCreated,
            Created = DateTime.UtcNow,
            UserName = currentUserName,
            UserRef = currentUserRef,
            AgreementType = agreementType,
            SignedAgreementVersion = versionNumber,
            CorrelationId = correlationId
        });
    }

    private async Task AddAuditEntry(SignEmployerAgreementCommand message, long accountId, long agreementId)
    {
        await _mediator.SendAsync(new CreateAuditCommand
        {
            EasAuditMessage = new EasAuditMessage
            {
                Category = "UPDATED",
                Description = $"Agreement {agreementId} added to account {accountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromString("UserId", message.ExternalUserId),
                    PropertyUpdate.FromString("SignedDate", message.SignedDate.ToString("U"))
                },
                RelatedEntities = new List<Entity> { new Entity { Id = accountId.ToString(), Type = "Account" } },
                AffectedEntity = new Entity { Type = "Agreement", Id = agreementId.ToString() }
            }
        });
    }
}