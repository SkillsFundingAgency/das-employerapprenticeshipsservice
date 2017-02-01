using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommandHandler : IAsyncRequestHandler<CreateLegalEntityCommand, CreateLegalEntityCommandResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly IEventPublisher _eventPublisher;

        public CreateLegalEntityCommandHandler(IAccountRepository accountRepository, IMembershipRepository membershipRepository, IMediator mediator, IEventPublisher eventPublisher)
        {
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _eventPublisher = eventPublisher;
        }

        public async Task<CreateLegalEntityCommandResponse> Handle(CreateLegalEntityCommand message)
        {
            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            var agreementView = await _accountRepository.CreateLegalEntity(
                owner.AccountId, 
                message.LegalEntity, 
                message.SignAgreement, 
                message.SignedDate,
                owner.UserId);
            
            await CreateAuditEntries(owner, agreementView);

            await NotifyLegalEntityCreated(message.HashedAccountId, agreementView.LegalEntityId);
            
            return new CreateLegalEntityCommandResponse
            {
                AgreementView = agreementView
            };
        }

        private async Task NotifyLegalEntityCreated(string hashedAccountId, long legalEntityId)
        {
            await _eventPublisher.PublishLegalEntityCreatedEvent(hashedAccountId, legalEntityId);
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
                        new PropertyUpdate {PropertyName = "AccountId", NewValue = agreementView.AccountId.ToString()},
                        new PropertyUpdate {PropertyName = "Id", NewValue = agreementView.LegalEntityId.ToString()},
                        new PropertyUpdate {PropertyName = "Name", NewValue = agreementView.LegalEntityName},
                        new PropertyUpdate {PropertyName = "Code", NewValue = agreementView.LegalEntityCode},
                        new PropertyUpdate {PropertyName = "Status", NewValue = agreementView.LegalEntityStatus},
                        new PropertyUpdate {PropertyName = "Address", NewValue = agreementView.LegalEntityAddress},
                        new PropertyUpdate {PropertyName = "DateOfInception", NewValue = agreementView.LegalEntityInceptionDate.GetDateString("G")},
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
                        new PropertyUpdate {PropertyName = "AccountId", NewValue = agreementView.AccountId.ToString()},
                        new PropertyUpdate {PropertyName = "SignedDate", NewValue = agreementView.SignedDate.GetDateString("G")},
                        new PropertyUpdate {PropertyName = "SignedBy", NewValue = $"{owner.FirstName} {owner.LastName}"}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = agreementView.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "EmployerAgreement", Id = agreementView.Id.ToString() }
                }
            });
        }
    }
}
