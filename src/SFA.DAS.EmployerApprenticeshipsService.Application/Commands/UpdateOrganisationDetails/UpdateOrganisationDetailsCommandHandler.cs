using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Commands.UpdateOrganisationDetails
{
    public class UpdateOrganisationDetailsCommandHandler : AsyncRequestHandler<UpdateOrganisationDetailsCommand>
    {
        private readonly IValidator<UpdateOrganisationDetailsCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IHashingService _hashingService;
        private readonly IEventPublisher _eventPublisher;

        public UpdateOrganisationDetailsCommandHandler(
            IValidator<UpdateOrganisationDetailsCommand> validator,
            IAccountRepository accountRepository,
            IMembershipRepository membershipRepository,
            IHashingService hashingService,
            IEventPublisher eventPublisher)
        {
            _validator = validator;
            _accountRepository = accountRepository;
            _membershipRepository = membershipRepository;
            _hashingService = hashingService;
            _eventPublisher = eventPublisher;
        }

        protected override async Task HandleCore(UpdateOrganisationDetailsCommand command)
        {
            var validationResults = _validator.Validate(command);

            if (!validationResults.IsValid())
                throw new InvalidRequestException(validationResults.ValidationDictionary);

            await _accountRepository.UpdateLegalEntityDetailsForAccount(
                command.AccountLegalEntityId, 
                command.Name, 
                command.Address);

            await PublishLegalEntityUpdatedMessage(command.HashedAccountId, command.AccountLegalEntityId, command.Name, command.UserId);
        }

        private async Task PublishLegalEntityUpdatedMessage(
            string hashedAccountId,
            long accountLegalEntityId,
            string organisationName,
            //long accountId, long agreementId, long legalEntityId,
            string userRef)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var caller = await _membershipRepository.GetCaller(accountId, userRef);
            var updatedByName = caller.FullName();

            await _eventPublisher.Publish(new UpdatedLegalEntityEvent
            {
                //AccountId = accountId,
                //AgreementId = agreementId,
                //LegalEntityId = legalEntityId,
                OrganisationName = organisationName,
                AccountLegalEntityId = accountLegalEntityId,
                //Updated = DateTime.UtcNow,
                UserName = updatedByName,
                UserRef = Guid.Parse(userRef)
            });
        }

    }
}
