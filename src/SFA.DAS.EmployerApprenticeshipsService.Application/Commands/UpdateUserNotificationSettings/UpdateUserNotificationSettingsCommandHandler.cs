using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.Settings;

namespace SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsCommandHandler :
        AsyncRequestHandler<UpdateUserNotificationSettingsCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IValidator<UpdateUserNotificationSettingsCommand> _validator;
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        public UpdateUserNotificationSettingsCommandHandler(IAccountRepository accountRepository,
            IValidator<UpdateUserNotificationSettingsCommand> validator, IMediator mediator, IHashingService hashingService)
        {
            _accountRepository = accountRepository;
            _validator = validator;
            _mediator = mediator;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(UpdateUserNotificationSettingsCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            DecodeAccountIds(message.Settings);

            await _accountRepository.UpdateUserAccountSettings(message.UserRef, message.Settings);

            foreach (var setting in message.Settings)
            {
                await AddAuditEntry(setting);
            }
        }

        private void DecodeAccountIds(List<UserNotificationSetting> source)
        {
            foreach (var setting in source)
            {
                setting.AccountId = _hashingService.DecodeValue(setting.HashedAccountId);
            }
        }

        private async Task AddAuditEntry(UserNotificationSetting setting)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description =
                        $"User {setting.UserId} has updated email notification setting for account {setting.HashedAccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate
                        {
                            PropertyName = "ReceiveNotifications",
                            NewValue = setting.ReceiveNotifications.ToString()
                        }
                    },
                    RelatedEntities = new List<Entity> {new Entity {Id = setting.UserId.ToString(), Type = "User"}},
                    AffectedEntity = new Entity {Type = "UserAccountSetting", Id = setting.Id.ToString()}
                }
            });
        }
    }
}
