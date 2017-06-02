using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsQuery: IAsyncRequest<GetUserNotificationSettingsQueryResponse>
    {
        public long AccountId { get; set; }
        public string UserRef { get; set; }
    }
}
