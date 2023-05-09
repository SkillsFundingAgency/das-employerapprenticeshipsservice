namespace SFA.DAS.EAS.Domain.Configuration;

public class GoogleAnalyticsSnippets
{
    public GoogleAnalyticsDetails GoogleAnalyticsValues { get; set; }

    public class GoogleAnalyticsDetails
    {
        public string GoogleHeaderUrl { get; set; }
        public string GoogleBodyUrl { get; set; }
    }
}
