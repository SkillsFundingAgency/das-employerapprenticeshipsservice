using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Commands.CreateAccount
{
    //TODO this needs changing to be a facade and calling individual commands for each component
    public class CreateAccountCommandHandler : IAsyncRequestHandler<CreateAccountCommand, CreateAccountCommandResponse>
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateAccountCommand> _validator;
        private readonly IHashingService _hashingService;
        private readonly IEventPublisher _eventPublisher;

        public CreateAccountCommandHandler(IAccountRepository accountRepository, IUserRepository userRepository, IMessagePublisher messagePublisher, IMediator mediator, IValidator<CreateAccountCommand> validator, IHashingService hashingService, IEventPublisher eventPublisher)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
            _mediator = mediator;
            _validator = validator;
            _hashingService = hashingService;
            _eventPublisher = eventPublisher;
        }

        public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommand message)
        {
            await ValidateMessage(message);

            var user = await GetUser(message);

            var emprefs = message.PayeReference.Split(',');

            if (string.IsNullOrEmpty(message.OrganisationReferenceNumber))
            {
                message.OrganisationReferenceNumber = Guid.NewGuid().ToString();
            }

            var returnValue = await _accountRepository.CreateAccount(user.Id, message.OrganisationReferenceNumber, message.OrganisationName, message.OrganisationAddress, message.OrganisationDateOfInception, emprefs[0], message.AccessToken, message.RefreshToken, message.OrganisationStatus, message.EmployerRefName, (short)message.OrganisationType, message.PublicSectorDataSource, message.Sector);

            var hashedAccountId = _hashingService.HashValue(returnValue.AccountId);
            await _accountRepository.SetHashedId(hashedAccountId, returnValue.AccountId);

            await AddPayeSchemes(message, emprefs, returnValue);

            await RefreshLevy(returnValue);

            await NotifyAccountCreated(hashedAccountId);

            await CreateAuditEntries(message, returnValue, hashedAccountId, user);

            return new CreateAccountCommandResponse
            {
                HashedAccountId = hashedAccountId
            };
        }

        private async Task NotifyAccountCreated(string hashedAccountId)
        {
            await _eventPublisher.PublishAccountCreatedEvent(hashedAccountId);
        }

        private async Task AddPayeSchemes(CreateAccountCommand message, string[] emprefs, CreateAccountResult returnValue)
        {
            if (emprefs.Length > 1)
            {
                for (var i = 1; i < emprefs.Length; i++)
                {
                    await _accountRepository.AddPayeToAccount(new Paye { AccountId = returnValue.AccountId, EmpRef = emprefs[i], AccessToken = message.AccessToken, RefreshToken = message.RefreshToken });
                }
            }
        }

        private async Task RefreshLevy(CreateAccountResult returnValue)
        {
            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage
            {
                AccountId = returnValue.AccountId
            });
        }

        private async Task<User> GetUser(CreateAccountCommand message)
        {
            var user = await _userRepository.GetByUserRef(message.ExternalUserId);

            if (user == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "User", "User does not exist" } });
            return user;
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
                        PropertyUpdate.FromString("RoleId", Role.Owner.ToString()),
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