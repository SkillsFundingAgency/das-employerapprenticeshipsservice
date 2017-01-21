using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.CreateAccountEvent;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;
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
        
        public CreateAccountCommandHandler(IAccountRepository accountRepository, IUserRepository userRepository, IMessagePublisher messagePublisher, IMediator mediator, IValidator<CreateAccountCommand> validator, IHashingService hashingService)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
            _mediator = mediator;
            _validator = validator;
            _hashingService = hashingService;
        }

        public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var user = await _userRepository.GetByUserRef(message.ExternalUserId);

            if (user == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "User", "User does not exist" } });

            var emprefs = message.PayeReference.Split(',');

            var returnValue = await _accountRepository.CreateAccount(user.Id, message.OrganisationReferenceNumber, message.OrganisationName, message.OrganisationAddress, message.OrganisationDateOfInception, emprefs[0], message.AccessToken, message.RefreshToken,message.OrganisationStatus,message.EmployerRefName, (short)message.OrganisationType, message.PublicSectorDataSource);

            var hashedAccountId = _hashingService.HashValue(returnValue.Item1);
            await _accountRepository.SetHashedId(hashedAccountId, returnValue.Item1);

            if (emprefs.Length > 1)
            {
                for (var i = 1; i < emprefs.Length; i++)
                {
                    await _accountRepository.AddPayeToAccount(new Paye {AccountId= returnValue.Item1, EmpRef= emprefs[i], AccessToken= message.AccessToken, RefreshToken = message.RefreshToken});
                }
            }
            

            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage
            {
                AccountId = returnValue.Item1
            });

            await _mediator.PublishAsync(new CreateAccountEventCommand { HashedAccountId = hashedAccountId, Event = "AccountCreated" });

            //Account
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Account {message.OrganisationName} created with id {returnValue.Item1}",
                    ChangedProperties = new List<PropertyUpdate>
                    {

                        PropertyUpdate.FromLong("AccountId",returnValue.Item1),
                        PropertyUpdate.FromString("HashedId",hashedAccountId),
                        PropertyUpdate.FromString("Name",message.OrganisationName),
                        PropertyUpdate.FromDateTime("CreatedDate",DateTime.UtcNow),
                    },
                    AffectedEntity = new Entity { Type = "Account", Id = returnValue.Item1.ToString() },
                    RelatedEntities = new List<Entity>()
                }
            });
            //LegalEntity
            var changedProperties = new List<PropertyUpdate>
            {

                PropertyUpdate.FromLong("Id",returnValue.Item2),
                PropertyUpdate.FromString("Name",message.OrganisationName),
                PropertyUpdate.FromString("Code",message.OrganisationReferenceNumber),
                PropertyUpdate.FromString("RegisteredAddress",message.OrganisationAddress),
                      
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
                    Description = $"Legal Entity {message.OrganisationName} created of type {message.OrganisationType} with id {returnValue.Item2}",
                    ChangedProperties = changedProperties,
                    AffectedEntity = new Entity { Type = "LegalEntity", Id = returnValue.Item2.ToString() },
                    RelatedEntities = new List<Entity>()
                }
            });

            //EmployerAgreement 
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Employer Agreement Created for {message.OrganisationName} legal entity id {returnValue.Item2}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        PropertyUpdate.FromLong("Id",returnValue.Item3),
                        PropertyUpdate.FromLong("LegalEntityId",returnValue.Item2),
                        PropertyUpdate.FromString("TemplateId",hashedAccountId),
                        PropertyUpdate.FromInt("StatusId",2),
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id=returnValue.Item3.ToString(),Type="LegalEntity"} },
                    AffectedEntity = new Entity { Type = "EmployerAgreement", Id = returnValue.Item3.ToString() }
                }
            });

            //AccountEmployerAgreement Account Employer Agreement
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Employer Agreement Created for {message.OrganisationName} legal entity id {returnValue.Item2}",
                    ChangedProperties = new List<PropertyUpdate>
                    {

                        PropertyUpdate.FromLong("AccountId",returnValue.Item1),
                        PropertyUpdate.FromLong("EmployerAgreementId",returnValue.Item3),
                        
                    },
                    RelatedEntities = new List<Entity>
                    {
                        new Entity { Id = returnValue.Item3.ToString(), Type = "LegalEntity" },
                        new Entity { Id = returnValue.Item1.ToString(), Type = "Account" }
                    },
                    AffectedEntity = new Entity { Type = "AccountEmployerAgreement", Id = returnValue.Item3.ToString() }
                }
            });

            //Paye 
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Paye scheme {message.PayeReference} added to account {returnValue.Item1}",
                    ChangedProperties = new List<PropertyUpdate>
                    {

                        PropertyUpdate.FromString("Ref",message.PayeReference),
                        PropertyUpdate.FromString("AccessToken",message.AccessToken),
                        PropertyUpdate.FromString("RefreshToken",message.RefreshToken),
                        PropertyUpdate.FromString("Name",message.EmployerRefName)
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = returnValue.Item1.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Paye", Id = message.PayeReference }
                }
            });
            
            //Membership Account
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"User {message.ExternalUserId} added to account {returnValue.Item1} as owner",
                    ChangedProperties = new List<PropertyUpdate>
                    {

                        PropertyUpdate.FromLong("AccountId",returnValue.Item1),
                        PropertyUpdate.FromString("UserId",message.ExternalUserId),
                        PropertyUpdate.FromString("RoleId",Role.Owner.ToString()),
                        PropertyUpdate.FromDateTime("CreatedDate",DateTime.UtcNow)
                    },
                    RelatedEntities = new List<Entity>
                    {
                        new Entity { Id = returnValue.Item1.ToString(), Type = "Account" },
                        new Entity { Id = user.Id.ToString(), Type = "User" }
                    },
                    AffectedEntity = new Entity { Type = "Membership", Id = message.ExternalUserId }
                }
            });

            return new CreateAccountCommandResponse
            {
                HashedAccountId = hashedAccountId
            };
        }
    }
}