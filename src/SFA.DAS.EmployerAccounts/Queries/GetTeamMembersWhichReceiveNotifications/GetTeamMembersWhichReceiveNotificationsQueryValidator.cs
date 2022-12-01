using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications
{
    public class GetTeamMembersWhichReceiveNotificationsQueryValidator : IValidator<GetTeamMembersWhichReceiveNotificationsQuery>
    {
        public ValidationResult Validate(GetTeamMembersWhichReceiveNotificationsQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.ValidationDictionary.Add(nameof(item.HashedAccountId),
                    "Hashed Account Id cannot be null or empty.");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetTeamMembersWhichReceiveNotificationsQuery item)
        {
            throw new System.NotImplementedException();
        }
    }
}
