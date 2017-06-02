using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsCommandHandler: AsyncRequestHandler<UpdateUserNotificationSettingsCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IValidator<UpdateUserNotificationSettingsCommand> _validator;

        public UpdateUserNotificationSettingsCommandHandler(IAccountRepository accountRepository, IValidator<UpdateUserNotificationSettingsCommand> validator)
        {
            _accountRepository = accountRepository;
            _validator = validator;
        }

        protected override async Task HandleCore(UpdateUserNotificationSettingsCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            await _accountRepository.UpdateUserLegalEntitySettings(message.UserRef, message.AccountId, message.Settings);
        }
    }
}
