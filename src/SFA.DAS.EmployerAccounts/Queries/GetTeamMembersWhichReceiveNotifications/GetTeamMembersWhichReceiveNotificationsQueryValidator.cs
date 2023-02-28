namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;

public class GetTeamMembersWhichReceiveNotificationsQueryValidator : IValidator<GetTeamMembersWhichReceiveNotificationsQuery>
{
    public ValidationResult Validate(GetTeamMembersWhichReceiveNotificationsQuery item)
    {
        var result = new ValidationResult();

        if (item.AccountId <= 0)
        {
            result.ValidationDictionary.Add(nameof(item.AccountId),
                "Account Id must have a value.");
        }

        return result;
    }

    public Task<ValidationResult> ValidateAsync(GetTeamMembersWhichReceiveNotificationsQuery item)
    {
        return Task.FromResult(Validate(item));
    }
}