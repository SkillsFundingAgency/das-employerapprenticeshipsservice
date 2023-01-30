using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Validation;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Services;
using Entity = SFA.DAS.Audit.Types.Entity;
using ValidationResult = SFA.DAS.EmployerAccounts.Validation.ValidationResult;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;

public class RemoveLegalEntityCommandHandler : IRequestHandler<RemoveLegalEntityCommand>
{
    private readonly IValidator<RemoveLegalEntityCommand> _validator;
    private readonly ILogger<RemoveLegalEntityCommandHandler> _logger;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IMediator _mediator;
    private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityHashingService;
    private readonly IHashingService _hashingService;
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
        IAccountLegalEntityPublicHashingService accountLegalEntityHashingService,
        IHashingService hashingService,
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
        _accountLegalEntityHashingService = accountLegalEntityHashingService;
        _hashingService = hashingService;
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
            _logger.LogInformation($"User {message.UserId} tried to remove {message.HashedAccountLegalEntityId} from Account {message.HashedAccountId}");
            throw new UnauthorizedAccessException();
        }

        var accountId = _hashingService.DecodeValue(message.HashedAccountId);
        var accountLegalEntityId = _accountLegalEntityHashingService.DecodeValue(message.HashedAccountLegalEntityId);
        var agreements = (await _employerAgreementRepository.GetAccountLegalEntityAgreements(accountLegalEntityId)).ToList();
        var legalAgreement = agreements.OrderByDescending(a => a.TemplateId).First();
        var hashedLegalAgreementId = _hashingService.HashValue(legalAgreement.Id);

        var agreement = await _employerAgreementRepository.GetEmployerAgreement(legalAgreement.Id);

        if (agreements.Any(x => x.SignedDate.HasValue))
        {
            await ValidateLegalEntityHasNoCommitments(agreement, accountId, validationResult);
        }

        await _employerAgreementRepository.RemoveLegalEntityFromAccount(legalAgreement.Id);

        await Task.WhenAll(
            AddAuditEntry(accountId, hashedLegalAgreementId),
            CreateEvent(hashedLegalAgreementId)
        );

        // it appears that an agreement is created whenever we create a legal entity, so there should always be an agreement associated with a legal entity
        if (agreement != null)
        {
            var agreementSigned = agreement.Status == EmployerAgreementStatus.Signed;
            var caller = await _membershipRepository.GetCaller(accountId, message.UserId);
            var createdByName = caller.FullName();

            await PublishLegalEntityRemovedMessage(
                accountId,
                legalAgreement.Id,
                agreementSigned,
                createdByName,
                agreement.LegalEntityId,
                agreement.LegalEntityName,
                agreement.AccountLegalEntityId,
                message.UserId);
        }

        return default;
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
            validationResult.AddError(nameof(agreement.HashedAgreementId), "Agreement has already been signed and has active commitments");
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

    private async Task AddAuditEntry(long accountId, string employerAgreementId)
    {
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new EasAuditMessage
            {
                Category = "UPDATED",
                Description = $"EmployerAgreement {employerAgreementId} removed from account {accountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromString("Status", EmployerAgreementStatus.Removed.ToString())
                },
                RelatedEntities = new List<Entity> { new Entity { Id = accountId.ToString(), Type = "Account" } },
                AffectedEntity = new Entity { Type = "EmployerAgreement", Id = employerAgreementId }
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