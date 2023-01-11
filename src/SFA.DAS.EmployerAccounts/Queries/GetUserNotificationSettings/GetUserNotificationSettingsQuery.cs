namespace SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsQuery: IAsyncRequest<GetUserNotificationSettingsQueryResponse>
{
    public string UserRef { get; set; }
}