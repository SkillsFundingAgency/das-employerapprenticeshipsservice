using SFA.DAS.Audit.Types;
using SFA.DAS.Authorization.Services;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;


namespace SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommandHandler : IAsyncRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>
    {
        private readonly IValidator<CreateLegalEntityCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly ILegalEntityEventFactory _legalEntityEventFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IHashingService _hashingService;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public CreateLegalEntityCommandHandler(
            IAccountRepository accountRepository,
            IMembershipRepository membershipRepository,
            IMediator mediator,
            IGenericEventFactory genericEventFactory,
            ILegalEntityEventFactory legalEntityEventFactory,
            IEventPublisher eventPublisher,
            IHashingService hashingService,
            IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService,
            IEmployerAgreementRepository employerAgreementRepository,
            IValidator<CreateLegalEntityCommand> validator,
            IAuthorizationService authorizationService)
        {
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _genericEventFactory = genericEventFactory;
            _legalEntityEventFactory = legalEntityEventFactory;
            _eventPublisher = eventPublisher;
            _hashingService = hashingService;
            _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
            _employerAgreementRepository = employerAgreementRepository;
            _validator = validator;
            _authorizationService = authorizationService;
        }

        public async Task<CreateLegalEntityCommandResponse> Handle(CreateLegalEntityCommand message)
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

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            var ownerExternalUserId = owner.UserRef;

            var createParams = new CreateLegalEntityWithAgreementParams
            {
                AccountId = owner.AccountId,
                Name = message.Name,
                Status = message.Status,
                Code = string.IsNullOrEmpty(message.Code) ? Guid.NewGuid().ToString() : message.Code,
                DateOfIncorporation = message.DateOfIncorporation,
                PublicSectorDataSource = message.PublicSectorDataSource,
                Source = message.Source,
                Address = message.Address,
                Sector = message.Sector,
                AgreementType = await UserIsWhitelistedForEOIOrThereIsAlreadyAnEOIAgreementForThisAccount(owner) ? AgreementType.NonLevyExpressionOfInterest : AgreementType.Combined
            };

            var agreementView = await _accountRepository.CreateLegalEntityWithAgreement(createParams);

            agreementView.HashedAgreementId = _hashingService.HashValue(agreementView.Id);
            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            agreementView.AccountLegalEntityPublicHashedId = _accountLegalEntityPublicHashingService.HashValue(agreementView.AccountLegalEntityId);

            await Task.WhenAll(
                CreateAuditEntries(owner, agreementView),
                NotifyLegalEntityCreated(message.HashedAccountId, agreementView.LegalEntityId),
                SetEmployerLegalEntityAgreementStatus(agreementView.AccountLegalEntityId, agreementView.Id, agreementView.VersionNumber),
				PublishLegalEntityAddedMessage(accountId, agreementView.Id, createParams.Name, owner.FullName(), agreementView.LegalEntityId,
                agreementView.AccountLegalEntityId, agreementView.AccountLegalEntityPublicHashedId, createParams.Code, message.Address, message.Source, ownerExternalUserId),
				PublishAgreementCreatedMessage(accountId, agreementView.Id, createParams.Name, owner.FullName(), agreementView.LegalEntityId, ownerExternalUserId)
            );

            return new CreateLegalEntityCommandResponse
            {
                AgreementView = agreementView
            };
        }

        private Task PublishLegalEntityAddedMessage(long accountId, long agreementId, string organisationName, string createdByName, long legalEntityId, long accountLegalEntityId, string accountLegalEntityPublicHashedId, string organisationReferenceNumber, string organisationAddress, OrganisationType organisationType, Guid userRef)
        {
            return _eventPublisher.Publish(new AddedLegalEntityEvent
            {
                AccountId = accountId,
                AgreementId = agreementId,
                LegalEntityId = legalEntityId,
                AccountLegalEntityId = accountLegalEntityId,
                AccountLegalEntityPublicHashedId = accountLegalEntityPublicHashedId,
                OrganisationName = organisationName,
                UserName = createdByName,
                UserRef = userRef,
                Created = DateTime.UtcNow,
                OrganisationReferenceNumber = organisationReferenceNumber,
                OrganisationAddress = organisationAddress,
                OrganisationType = (SFA.DAS.EmployerAccounts.Types.Models.OrganisationType) organisationType
            });
        }

        private Task PublishAgreementCreatedMessage(long accountId, long agreementId, string organisationName, string createdByName, long legalEntityId, Guid userRef)
        {
            return _eventPublisher.Publish(new CreatedAgreementEvent
            {
                AgreementId = agreementId,
                LegalEntityId = legalEntityId,
                OrganisationName = organisationName,
                AccountId = accountId,
                UserName = createdByName,
                UserRef = userRef,
                Created = DateTime.UtcNow
            });
        }

        private Task NotifyLegalEntityCreated(string hashedAccountId, long legalEntityId)
        {
            var legalEntityEvent = _legalEntityEventFactory.CreateLegalEntityCreatedEvent(hashedAccountId, legalEntityId);
            var genericEvent = _genericEventFactory.Create(legalEntityEvent);

            return _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
        }

        private Task SetEmployerLegalEntityAgreementStatus(long accountLegalEntityId, long agreementId, int agreementVersion)
        {
            return _employerAgreementRepository.SetAccountLegalEntityAgreementDetails(accountLegalEntityId, agreementId, agreementVersion, null, null);
        }

        private async Task CreateAuditEntries(MembershipView owner, EmployerAgreementView agreementView)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description = $"User {owner.Email} added legal entity {agreementView.LegalEntityId} to account {owner.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate { PropertyName = "AccountId", NewValue = agreementView.AccountId.ToString() },
                        new PropertyUpdate { PropertyName = "Id", NewValue = agreementView.LegalEntityId.ToString() },
                        new PropertyUpdate { PropertyName = "Name", NewValue = agreementView.LegalEntityName },
                        new PropertyUpdate { PropertyName = "Code", NewValue = agreementView.LegalEntityCode },
                        new PropertyUpdate { PropertyName = "Source", NewValue = agreementView.LegalEntitySource.ToString() },
                        new PropertyUpdate { PropertyName = "Status", NewValue = agreementView.LegalEntityStatus },
                        new PropertyUpdate { PropertyName = "Address", NewValue = agreementView.LegalEntityAddress },
                        new PropertyUpdate { PropertyName = "DateOfInception", NewValue = agreementView.LegalEntityInceptionDate.GetDateString("G") },
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = agreementView.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "LegalEntity", Id = agreementView.LegalEntityId.ToString() }
                }
            });

            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description = $"User {owner.Email} added signed agreement {agreementView.Id} to account {owner.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate { PropertyName = "AccountId", NewValue = agreementView.AccountId.ToString() },
                        new PropertyUpdate { PropertyName = "SignedDate", NewValue = agreementView.SignedDate.GetDateString("G") },
                        new PropertyUpdate { PropertyName = "SignedBy", NewValue = $"{owner.FirstName} {owner.LastName}" }
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = agreementView.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "EmployerAgreement", Id = agreementView.Id.ToString() }
                }
            });
        }

        private async Task<bool> UserIsWhitelistedForEOIOrThereIsAlreadyAnEOIAgreementForThisAccount(MembershipView accountOwner)
        {
            var existingAgreements = await _employerAgreementRepository.GetAccountAgreements(accountOwner.AccountId);

            return
                existingAgreements.Any(a => a.Template.AgreementType.Equals(AgreementType.NonLevyExpressionOfInterest));
        }
    }
}
