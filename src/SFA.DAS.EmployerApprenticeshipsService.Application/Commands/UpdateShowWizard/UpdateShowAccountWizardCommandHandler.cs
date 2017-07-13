using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Commands.UpdateShowWizard
{
    public class UpdateShowAccountWizardCommandHandler : AsyncRequestHandler<UpdateShowAccountWizardCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IValidator<UpdateShowAccountWizardCommand> _validator;
        private readonly ILog _logger;

        public UpdateShowAccountWizardCommandHandler(IMembershipRepository membershipRepository, IValidator<UpdateShowAccountWizardCommand> validator, ILog logger)
        {
            _membershipRepository = membershipRepository;
            _validator = validator;
            _logger = logger;
        }
        protected override async Task HandleCore(UpdateShowAccountWizardCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            _logger.Info($"User {message.ExternalUserId} has set the show wizard toggle to {message.ShowWizard} for account {message.HashedAccountId}");

            await _membershipRepository.SetShowAccountWizard(message.HashedAccountId, message.ExternalUserId, message.ShowWizard);
        }
    }
}
