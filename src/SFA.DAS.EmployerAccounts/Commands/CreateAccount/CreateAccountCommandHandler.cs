using System.Threading;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Commands.CreateAccount;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, CreateAccountCommandResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMediator _mediator;
    private readonly IValidator<CreateAccountCommand> _validator;
    private readonly IEncodingService _encodingService;
    private readonly IGenericEventFactory _genericEventFactory;
    private readonly IAccountEventFactory _accountEventFactory;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IEventPublisher _eventPublisher;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository,
        IMediator mediator,
        IValidator<CreateAccountCommand> validator,
        IEncodingService encodingService,
        IGenericEventFactory genericEventFactory,
        IAccountEventFactory accountEventFactory,
        IMembershipRepository membershipRepository,
        IEmployerAgreementRepository employerAgreementRepository,
        IEventPublisher eventPublisher)
    {
        _accountRepository = accountRepository;
        _mediator = mediator;
        _validator = validator;
        _encodingService = encodingService;
        _genericEventFactory = genericEventFactory;
        _accountEventFactory = accountEventFactory;
        _membershipRepository = membershipRepository;
        _employerAgreementRepository = employerAgreementRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommand message, CancellationToken cancellationToken)
    {
        await ValidateMessage(message);

        var externalUserId = Guid.Parse(message.ExternalUserId);

        var userResponse = await _mediator.Send(new GetUserByRefQuery { UserRef = message.ExternalUserId }, cancellationToken);

        if (string.IsNullOrEmpty(message.OrganisationReferenceNumber))
        {
            message.OrganisationReferenceNumber = Guid.NewGuid().ToString();
        }

        var createAccountResult = await _accountRepository.CreateAccount(new CreateAccountParams
        {
            UserId = userResponse.User.Id,
            EmployerNumber = message.OrganisationReferenceNumber,
            EmployerName = message.OrganisationName,
            EmployerRegisteredAddress = message.OrganisationAddress,
            EmployerDateOfIncorporation = message.OrganisationDateOfInception,
            EmployerRef = message.PayeReference,
            AccessToken = message.AccessToken,
            RefreshToken = message.RefreshToken,
            CompanyStatus = message.OrganisationStatus,
            EmployerRefName = message.EmployerRefName,
            Source = (short)message.OrganisationType,
            PublicSectorDataSource = message.PublicSectorDataSource,
            Sector = message.Sector,
            Aorn = message.Aorn,
            AgreementType = AgreementType.Combined,
            ApprenticeshipEmployerType = ApprenticeshipEmployerType.Unknown
        });

        var hashedAccountId = _encodingService.Encode(createAccountResult.AccountId, EncodingType.AccountId);
        var publicHashedAccountId = _encodingService.Encode(createAccountResult.AccountId, EncodingType.PublicAccountId);
        var hashedAgreementId = _encodingService.Encode(createAccountResult.EmployerAgreementId, EncodingType.AccountId);

        await Task.WhenAll(
            _accountRepository.UpdateAccountHashedIds(createAccountResult.AccountId, hashedAccountId, publicHashedAccountId),
            SetAccountLegalEntityAgreementStatus(createAccountResult.AccountLegalEntityId, createAccountResult.EmployerAgreementId, createAccountResult.AgreementVersion)
        );

        var caller = await _membershipRepository.GetCaller(createAccountResult.AccountId, message.ExternalUserId);
        var createdByName = caller.FullName();

        await Task.WhenAll(
            PublishAddPayeSchemeMessage(message.PayeReference, createAccountResult.AccountId, createdByName, userResponse.User.Ref, message.Aorn, message.EmployerRefName, userResponse.User.CorrelationId),
            PublishAccountCreatedMessage(createAccountResult.AccountId, hashedAccountId, publicHashedAccountId, message.OrganisationName, createdByName, externalUserId),
            NotifyAccountCreated(hashedAccountId),
            CreateAuditEntries(message, createAccountResult, hashedAccountId, userResponse.User),
            PublishLegalEntityAddedMessage(createAccountResult.AccountId, createAccountResult.LegalEntityId,
                createAccountResult.EmployerAgreementId, createAccountResult.AccountLegalEntityId, message.OrganisationName,
                message.OrganisationReferenceNumber, message.OrganisationAddress, message.OrganisationType, createdByName, externalUserId),
            PublishAgreementCreatedMessage(createAccountResult.AccountId, createAccountResult.LegalEntityId, createAccountResult.EmployerAgreementId, message.OrganisationName, createdByName, externalUserId)
        );

        if (!string.IsNullOrWhiteSpace(message.Aorn))
        {
            await _mediator.Send(new AccountLevyStatusCommand
            {
                AccountId = createAccountResult.AccountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy
            }, cancellationToken);
        }

        return new CreateAccountCommandResponse
        {
            HashedAccountId = hashedAccountId,
            HashedAgreementId = hashedAgreementId
        };
    }

    private Task SetAccountLegalEntityAgreementStatus(long accountLegalEntityId, long employerAgreementId, int agreementVersion)
    {
        return _employerAgreementRepository.SetAccountLegalEntityAgreementDetails(accountLegalEntityId, employerAgreementId, agreementVersion, null, null);
    }

    private Task PublishAgreementCreatedMessage(long accountId, long legalEntityId, long employerAgreementId, string organisationName, string userName, Guid userRef)
    {
        return _eventPublisher.Publish(new CreatedAgreementEvent
        {
            AgreementId = employerAgreementId,
            LegalEntityId = legalEntityId,
            OrganisationName = organisationName,
            AccountId = accountId,
            UserName = userName,
            UserRef = userRef,
            Created = DateTime.UtcNow
        });
    }

    private Task PublishLegalEntityAddedMessage(long accountId, long legalEntityId, long employerAgreementId, long accountLegalEntityId, string organisationName, string organisationReferenceNumber, string organisationAddress, OrganisationType organisationType, string userName, Guid userRef)
    {
        var accountLegalEntityPublicHashedId = _encodingService.Encode(accountLegalEntityId, EncodingType.PublicAccountLegalEntityId);

        return _eventPublisher.Publish(new AddedLegalEntityEvent
        {
            AgreementId = employerAgreementId,
            LegalEntityId = legalEntityId,
            OrganisationName = organisationName,
            AccountId = accountId,
            AccountLegalEntityId = accountLegalEntityId,
            AccountLegalEntityPublicHashedId = accountLegalEntityPublicHashedId,
            UserName = userName,
            UserRef = userRef,
            Created = DateTime.UtcNow,
            OrganisationReferenceNumber = organisationReferenceNumber,
            OrganisationAddress = organisationAddress,
            OrganisationType = (Types.Models.OrganisationType)organisationType
        });
    }

    private Task NotifyAccountCreated(string hashedAccountId)
    {
        var accountEvent = _accountEventFactory.CreateAccountCreatedEvent(hashedAccountId);

        var genericEvent = _genericEventFactory.Create(accountEvent);

        return _mediator.Send(new PublishGenericEventCommand { Event = genericEvent });
    }

    private Task PublishAddPayeSchemeMessage(string empref, long accountId, string createdByName, Guid userRef, string aorn, string schemeName, string correlationId)
    {
        return _eventPublisher.Publish(new AddedPayeSchemeEvent
        {
            PayeRef = empref,
            AccountId = accountId,
            UserName = createdByName,
            UserRef = userRef,
            Created = DateTime.UtcNow,
            Aorn = aorn,
            SchemeName = schemeName,
            CorrelationId = correlationId
        });
    }

    private Task PublishAccountCreatedMessage(long accountId, string hashedId, string publicHashedId, string name, string createdByName, Guid userRef)
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

    private async Task ValidateMessage(CreateAccountCommand message)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
            throw new InvalidRequestException(validationResult.ValidationDictionary);
    }

    private async Task CreateAuditEntries(CreateAccountCommand message, CreateAccountResult returnValue, string hashedAccountId, User user)
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
        //LegalEntity
        var changedProperties = new List<PropertyUpdate>
        {
            PropertyUpdate.FromLong("Id", returnValue.LegalEntityId),
            PropertyUpdate.FromString("Name", message.OrganisationName),
            PropertyUpdate.FromString("Code", message.OrganisationReferenceNumber),
            PropertyUpdate.FromString("RegisteredAddress", message.OrganisationAddress),
            PropertyUpdate.FromString("OrganisationType", message.OrganisationType.ToString()),
            PropertyUpdate.FromString("PublicSectorDataSource", message.PublicSectorDataSource.ToString()),
            PropertyUpdate.FromString("Sector", message.Sector)
        };
        if (message.OrganisationDateOfInception != null)
        {
            changedProperties.Add(PropertyUpdate.FromDateTime("DateOfIncorporation", message.OrganisationDateOfInception.Value));
        }

        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "CREATED",
                Description = $"Legal Entity {message.OrganisationName} created of type {message.OrganisationType} with id {returnValue.LegalEntityId}",
                ChangedProperties = changedProperties,
                AffectedEntity = new AuditEntity { Type = "LegalEntity", Id = returnValue.LegalEntityId.ToString() },
                RelatedEntities = new List<AuditEntity>()
            }
        });

        //EmployerAgreement 
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "CREATED",
                Description = $"Employer Agreement Created for {message.OrganisationName} legal entity id {returnValue.LegalEntityId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromLong("Id", returnValue.EmployerAgreementId),
                    PropertyUpdate.FromLong("LegalEntityId", returnValue.LegalEntityId),
                    PropertyUpdate.FromString("TemplateId", hashedAccountId),
                    PropertyUpdate.FromInt("StatusId", 2),
                },
                RelatedEntities = new List<AuditEntity> { new AuditEntity { Id = returnValue.EmployerAgreementId.ToString(), Type = "LegalEntity" } },
                AffectedEntity = new AuditEntity { Type = "EmployerAgreement", Id = returnValue.EmployerAgreementId.ToString() }
            }
        });

        //AccountEmployerAgreement Account Employer Agreement
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "CREATED",
                Description = $"Employer Agreement Created for {message.OrganisationName} legal entity id {returnValue.LegalEntityId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromLong("AccountId", returnValue.AccountId),
                    PropertyUpdate.FromLong("EmployerAgreementId", returnValue.EmployerAgreementId),
                },
                RelatedEntities = new List<AuditEntity>
                {
                    new AuditEntity { Id = returnValue.EmployerAgreementId.ToString(), Type = "LegalEntity" },
                    new AuditEntity { Id = returnValue.AccountId.ToString(), Type = "Account" }
                },
                AffectedEntity = new AuditEntity { Type = "AccountEmployerAgreement", Id = returnValue.EmployerAgreementId.ToString() }
            }
        });

        //Paye 
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "CREATED",
                Description = $"Paye scheme {message.PayeReference} added to account {returnValue.AccountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromString("Ref", message.PayeReference),
                    PropertyUpdate.FromString("AccessToken", message.AccessToken),
                    PropertyUpdate.FromString("RefreshToken", message.RefreshToken),
                    PropertyUpdate.FromString("Name", message.EmployerRefName),
                    PropertyUpdate.FromString("Aorn", message.Aorn)
                },
                RelatedEntities = new List<AuditEntity> { new AuditEntity { Id = returnValue.AccountId.ToString(), Type = "Account" } },
                AffectedEntity = new AuditEntity { Type = "Paye", Id = message.PayeReference }
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