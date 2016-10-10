using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeWithExistingLegalEntity
{
    public class AddPayeToAccountForExistingLegalEntityCommandHandler : AsyncRequestHandler<AddPayeToAccountForExistingLegalEntityCommand>
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly AddPayeToAccountForExistingLegalEntityCommandValidator _validator;

        public AddPayeToAccountForExistingLegalEntityCommandHandler(IAccountRepository accountRepository, IMembershipRepository membershipRepository, IEmployerAgreementRepository employerAgreementRepository, IMessagePublisher messagePublisher)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (employerAgreementRepository == null)
                throw new ArgumentNullException(nameof(employerAgreementRepository));
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _employerAgreementRepository = employerAgreementRepository;
            _messagePublisher = messagePublisher;
            _validator = new AddPayeToAccountForExistingLegalEntityCommandValidator();
        }

        protected override async Task HandleCore(AddPayeToAccountForExistingLegalEntityCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var caller = await _membershipRepository.GetCaller(message.HashedId, message.ExternalUserId);

            if (caller == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this Account" } });
            if ((Role)caller.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var legalEntities = await _employerAgreementRepository.GetLegalEntitiesLinkedToAccount(caller.AccountId);

            var isLinked = legalEntities.Exists(x => x.Id == message.LegalEntityId);

            if (!isLinked)
                throw new InvalidRequestException(new Dictionary<string, string> { { "LegalEntity", "LegalEntity is not linked to this Account" } });

            await _accountRepository.AddPayeToAccountForExistingLegalEntity(caller.AccountId, message.LegalEntityId, message.EmpRef, message.AccessToken, message.RefreshToken);

            await _messagePublisher.PublishAsync(
                new EmployerRefreshLevyQueueMessage
                {
                    AccountId = caller.AccountId
                });
        }
    }
}