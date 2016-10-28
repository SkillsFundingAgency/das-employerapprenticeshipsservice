using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class NotificationsApiClientConfiguration : INotificationsApiClientConfiguration, IConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}