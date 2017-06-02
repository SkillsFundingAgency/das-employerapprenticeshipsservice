using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsQueryHandler: IAsyncRequestHandler<GetUserNotificationSettingsQuery,GetUserNotificationSettingsQueryResponse>
    {
        private IValidator<GetUserNotificationSettingsQuery> _validator;
        private IAccountRepository _accountRepository;

        public GetUserNotificationSettingsQueryHandler(IAccountRepository accountRepository, IValidator<GetUserNotificationSettingsQuery> validator)
        {
            _accountRepository = accountRepository;
            _validator = validator;
        }

        public async Task<GetUserNotificationSettingsQueryResponse> Handle(GetUserNotificationSettingsQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var data = await _accountRepository.GetUserLegalEntitySettings(message.UserRef, message.AccountId);

            return new GetUserNotificationSettingsQueryResponse
            {
                NotificationSettings = data
            };

        }
    }
}
