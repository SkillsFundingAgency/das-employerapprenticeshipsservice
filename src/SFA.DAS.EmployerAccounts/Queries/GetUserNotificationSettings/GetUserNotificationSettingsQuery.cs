namespace SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsQuery: IRequest<GetUserNotificationSettingsQueryResponse>
{
    public string UserRef { get; set; }
}