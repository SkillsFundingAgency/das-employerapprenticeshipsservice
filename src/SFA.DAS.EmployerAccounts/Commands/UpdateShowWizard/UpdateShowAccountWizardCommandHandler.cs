using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;

public class UpdateShowAccountWizardCommandHandler : IRequestHandler<UpdateShowAccountWizardCommand>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IValidator<UpdateShowAccountWizardCommand> _validator;
    private readonly ILogger<UpdateShowAccountWizardCommandHandler> _logger;

    public UpdateShowAccountWizardCommandHandler(IMembershipRepository membershipRepository, IValidator<UpdateShowAccountWizardCommand> validator, ILogger<UpdateShowAccountWizardCommandHandler> logger)
    {
        _membershipRepository = membershipRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateShowAccountWizardCommand message, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.LogInformation("User {ExternalUserId} has set the show wizard toggle to {ShowWizard} for account {HashedAccountId}", message.ExternalUserId, message.ShowWizard, message.HashedAccountId);

        await _membershipRepository.SetShowAccountWizard(message.HashedAccountId, message.ExternalUserId, message.ShowWizard);

        return Unit.Value;
    }
}