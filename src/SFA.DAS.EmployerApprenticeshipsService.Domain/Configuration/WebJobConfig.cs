using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class WebJobConfig : IWebJobConfiguration
    {
        public string DashboardConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
    }
}