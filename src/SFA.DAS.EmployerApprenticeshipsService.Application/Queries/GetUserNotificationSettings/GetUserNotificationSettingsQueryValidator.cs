using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsQueryValidator: IValidator<GetUserNotificationSettingsQuery>
    {
        public ValidationResult Validate(GetUserNotificationSettingsQuery item)
        {
            var result = new ValidationResult();

            if(item.ExternalUserId.Equals(Guid.Empty))
            {
                result.AddError(nameof(item.ExternalUserId));
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetUserNotificationSettingsQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
