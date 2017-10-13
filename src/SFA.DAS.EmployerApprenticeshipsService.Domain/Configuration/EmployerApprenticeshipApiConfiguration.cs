using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class EmployerApprenticeshipApiConfiguration
    {
        public string BaseUrl { get; set; }
        public NotificationsApiClientConfiguration NotificationApi { get; set; }
    }
}
