using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails;

public class UpdateOrganisationDetailsCommandHandler : IRequestHandler<UpdateOrganisationDetailsCommand>
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

    public async Task<Unit> Handle(UpdateOrganisationDetailsCommand command, CancellationToken cancellationToken)
    {
        var validationResults = _validator.Validate(command);

        if (!validationResults.IsValid())
            throw new InvalidRequestException(validationResults.ValidationDictionary);

        await _accountRepository.UpdateLegalEntityDetailsForAccount(
            command.AccountLegalEntityId,
            command.Name,
            command.Address);

        await PublishLegalEntityUpdatedMessage(command.HashedAccountId, command.AccountLegalEntityId, command.Name, command.Address, command.UserId);

        return default;
    }

    private async Task PublishLegalEntityUpdatedMessage(
        string hashedAccountId,
        long accountLegalEntityId,
        string name,
        string address,
        string userRef)
    {
        var accountId = _hashingService.DecodeValue(hashedAccountId);

        var caller = await _membershipRepository.GetCaller(accountId, userRef);
        var updatedByName = caller.FullName();

        await _eventPublisher.Publish(new UpdatedLegalEntityEvent
        {
            Name = name,
            Address = address,
            AccountLegalEntityId = accountLegalEntityId,
            UserName = updatedByName,
            UserRef = Guid.Parse(userRef),
            Created = DateTime.UtcNow
        });
    }
}