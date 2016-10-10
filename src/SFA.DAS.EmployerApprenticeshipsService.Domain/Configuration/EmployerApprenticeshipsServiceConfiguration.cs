using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Tasks.Api.Client.Configuration;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration : IConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public SmtpConfiguration SmtpServer { get; set; }
        public string DashboardUrl { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public string DatabaseConnectionString { get; set; }
        public CommitmentsApiClientConfiguration CommitmentsApi { get; set; }
        public TasksApiClientConfiguration TasksApi { get; set; }
        public string Hashstring { get; set; }
        public PrivacyConfiguration Privacy { get; set; }

    }

    public class PrivacyConfiguration
    {
        public string GoogleAnalyticsUrl { get; set; }

        public string ApplicationInsightsUrl { get; set; }

        public string AboutCookiesUrl { get; set; }

        public string SurveyProviderUrl { get; set; }
    }

    public class CommitmentsApiClientConfiguration : ICommitmentsApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }

    public class TasksApiClientConfiguration : ITasksApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}