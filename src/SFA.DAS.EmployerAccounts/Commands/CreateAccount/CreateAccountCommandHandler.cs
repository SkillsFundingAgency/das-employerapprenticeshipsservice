﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.Authorization.Services;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;


namespace SFA.DAS.EmployerAccounts.Commands.CreateAccount
{
    //TODO this needs changing to be a facade and calling individual commands for each component
    public class CreateAccountCommandHandler : IAsyncRequestHandler<CreateAccountCommand, CreateAccountCommandResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateAccountCommand> _validator;
        private readonly IHashingService _hashingService;
        private readonly IPublicHashingService _publicHashingService;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityPublicHashingService;
        private readonly IGenericEventFactory _genericEventFactory;
        private readonly IAccountEventFactory _accountEventFactory;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAuthorizationService _authorizationService;

        public CreateAccountCommandHandler(
            IAccountRepository accountRepository,
            IMediator mediator,
            IValidator<CreateAccountCommand> validator,
            IHashingService hashingService,
            IPublicHashingService publicHashingService,
            IAccountLegalEntityPublicHashingService accountLegalEntityPublicHashingService,
            IGenericEventFactory genericEventFactory,
            IAccountEventFactory accountEventFactory,
            IMembershipRepository membershipRepository,
            IEmployerAgreementRepository employerAgreementRepository,
            IEventPublisher eventPublisher,
            IAuthorizationService authorizationService)
        {
            _accountRepository = accountRepository;
            _mediator = mediator;
            _validator = validator;
            _hashingService = hashingService;
            _publicHashingService = publicHashingService;
            _accountLegalEntityPublicHashingService = accountLegalEntityPublicHashingService;
            _genericEventFactory = genericEventFactory;
            _accountEventFactory = accountEventFactory;
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _eventPublisher = eventPublisher;
            _authorizationService = authorizationService;
        }

        public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommand message)
        {
            await ValidateMessage(message);

            var externalUserId = Guid.Parse(message.ExternalUserId);

            var userResponse = await _mediator.SendAsync(new GetUserByRefQuery { UserRef = message.ExternalUserId });

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
                Source = (short) message.OrganisationType,
                PublicSectorDataSource = message.PublicSectorDataSource,
                Sector = message.Sector,
                Aorn = message.Aorn,
                AgreementType = _authorizationService.IsAuthorized("EmployerFeature.ExpressionOfInterest") ? AgreementType.NonLevyExpressionOfInterest : AgreementType.Combined,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Unknown
            });   
            
            var hashedAccountId = _hashingService.HashValue(createAccountResult.AccountId);
            var publicHashedAccountId = _publicHashingService.HashValue(createAccountResult.AccountId);
            var hashedAgreementId = _hashingService.HashValue(createAccountResult.EmployerAgreementId);

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
                await _mediator.SendAsync(new AccountLevyStatusCommand
                {
                    AccountId = createAccountResult.AccountId,
                    ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy
                });
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
            var accountLegalEntityPublicHashedId = _accountLegalEntityPublicHashingService.HashValue(accountLegalEntityId);

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
                OrganisationType = (SFA.DAS.EmployerAccounts.Types.Models.OrganisationType) organisationType
            });
        }

        private Task NotifyAccountCreated(string hashedAccountId)
        {
            var accountEvent = _accountEventFactory.CreateAccountCreatedEvent(hashedAccountId);

            var genericEvent = _genericEventFactory.Create(accountEvent);

            return _mediator.SendAsync(new PublishGenericEventCommand { Event = genericEvent });
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
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
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
                    AffectedEntity = new Entity { Type = "Account", Id = returnValue.AccountId.ToString() },
                    RelatedEntities = new List<Entity>()
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

            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Legal Entity {message.OrganisationName} created of type {message.OrganisationType} with id {returnValue.LegalEntityId}",
                    ChangedProperties = changedProperties,
                    AffectedEntity = new Entity { Type = "LegalEntity", Id = returnValue.LegalEntityId.ToString() },
                    RelatedEntities = new List<Entity>()
                }
            });

            //EmployerAgreement 
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
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
                    RelatedEntities = new List<Entity> { new Entity { Id = returnValue.EmployerAgreementId.ToString(), Type = "LegalEntity" } },
                    AffectedEntity = new Entity { Type = "EmployerAgreement", Id = returnValue.EmployerAgreementId.ToString() }
                }
            });

            //AccountEmployerAgreement Account Employer Agreement
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Employer Agreement Created for {message.OrganisationName} legal entity id {returnValue.LegalEntityId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        PropertyUpdate.FromLong("AccountId", returnValue.AccountId),
                        PropertyUpdate.FromLong("EmployerAgreementId", returnValue.EmployerAgreementId),
                    },
                    RelatedEntities = new List<Entity>
                    {
                        new Entity { Id = returnValue.EmployerAgreementId.ToString(), Type = "LegalEntity" },
                        new Entity { Id = returnValue.AccountId.ToString(), Type = "Account" }
                    },
                    AffectedEntity = new Entity { Type = "AccountEmployerAgreement", Id = returnValue.EmployerAgreementId.ToString() }
                }
            });

            //Paye 
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Paye scheme {message.PayeReference} added to account {returnValue.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        PropertyUpdate.FromString("Ref", message.PayeReference),
                        PropertyUpdate.FromString("AccessToken", message.AccessToken),
                        PropertyUpdate.FromString("RefreshToken", message.RefreshToken),
                        PropertyUpdate.FromString("Name", message.EmployerRefName)
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = returnValue.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Paye", Id = message.PayeReference }
                }
            });

            //Membership Account
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
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
                    RelatedEntities = new List<Entity>
                    {
                        new Entity { Id = returnValue.AccountId.ToString(), Type = "Account" },
                        new Entity { Id = user.Id.ToString(), Type = "User" }
                    },
                    AffectedEntity = new Entity { Type = "Membership", Id = message.ExternalUserId }
                }
            });
        }
    }
}