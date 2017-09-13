using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.EnvironmentInfo;
using SFA.DAS.EAS.Web.EnvironmentInfo;

namespace SFA.DAS.EAS.Web.Models
{
    public class GoogleAnalyticsModel
    {
        private const string ServiceName = "SFA.DAS.GoogleAnalyticsValues";

        private static GoogleAnalyticsModel _instance;

        private readonly IConfugurationInfo<GoogleAnalyticsSnippets> _configInfo;

        private GoogleAnalyticsModel()
        {
            _configInfo = new ConfigurationInfo<GoogleAnalyticsSnippets>();
            var snippetInfo =_configInfo.GetConfiguration(ServiceName, null);
            PopulateGoogleEnvironmentDetails(snippetInfo);
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

        private void PopulateGoogleEnvironmentDetails(GoogleAnalyticsSnippets environmentConfig)
        {
            GoogleHeaderUrl = environmentConfig.GoogleAnalyticsValues.GoogleHeaderUrl;
            GoogleBodyUrl = environmentConfig.GoogleAnalyticsValues.GoogleBodyUrl;
        }
    }
}