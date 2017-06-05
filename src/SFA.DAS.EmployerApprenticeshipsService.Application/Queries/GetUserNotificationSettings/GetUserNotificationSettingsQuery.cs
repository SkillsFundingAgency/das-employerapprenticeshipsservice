using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsQuery: IAsyncRequest<GetUserNotificationSettingsQueryResponse>
    {
        public string UserRef { get; set; }
    }
}
