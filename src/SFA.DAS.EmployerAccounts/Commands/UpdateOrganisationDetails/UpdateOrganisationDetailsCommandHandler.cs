using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails;

public class UpdateOrganisationDetailsCommandHandler : IRequestHandler<UpdateOrganisationDetailsCommand>
{
    private readonly IValidator<UpdateOrganisationDetailsCommand> _validator;
    private readonly IAccountRepository _accountRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IEventPublisher _eventPublisher;

    public UpdateOrganisationDetailsCommandHandler(
        IValidator<UpdateOrganisationDetailsCommand> validator,
        IAccountRepository accountRepository,
        IMembershipRepository membershipRepository,
        IEventPublisher eventPublisher)
    {
        _validator = validator;
        _accountRepository = accountRepository;
        _membershipRepository = membershipRepository;
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

        await PublishLegalEntityUpdatedMessage(command.AccountId, command.AccountLegalEntityId, command.Name, command.Address, command.UserId);

        return Unit.Value;
    }

    private async Task PublishLegalEntityUpdatedMessage(
        long accountId,
        long accountLegalEntityId,
        string name,
        string address,
        string userRef)
    { 
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