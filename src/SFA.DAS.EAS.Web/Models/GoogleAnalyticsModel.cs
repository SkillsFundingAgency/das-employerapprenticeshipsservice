using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;

namespace SFA.DAS.EAS.Web.Models
{
    public class GoogleAnalyticsModel
    {
        private const string ServiceName = "SFA.DAS.GoogleAnalyticsValues";

        private static GoogleAnalyticsModel _instance;

        private GoogleAnalyticsModel()
        {
            GetConfiguration();
        }

        public static GoogleAnalyticsModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GoogleAnalyticsModel();
                }
                return _instance;
            }
        }

        public string GoogleHeaderUrl { get; private set; }
        public string GoogleBodyUrl { get; private set; }

        public void GetConfiguration()
        {
            var configuration = ConfigurationHelper.GetConfiguration<GoogleAnalyticsSnippets>(ServiceName);
            PopulateGoogleEnvironmentDetails(configuration);
        }

        private void PopulateGoogleEnvironmentDetails(GoogleAnalyticsSnippets environmentConfig)
        {
            GoogleHeaderUrl = environmentConfig.GoogleAnalyticsValues.GoogleHeaderUrl;
            GoogleBodyUrl = environmentConfig.GoogleAnalyticsValues.GoogleBodyUrl;
        }
    }
}