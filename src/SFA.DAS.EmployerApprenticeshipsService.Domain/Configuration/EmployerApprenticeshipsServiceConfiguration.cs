using System;
using System.Collections.Generic;
using Microsoft.Azure;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration : IConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string DashboardUrl { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public string DatabaseConnectionString { get; set; }

        public CommitmentsApiClientConfiguration CommitmentsApi { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
        public EmployerApprenticeshipApiConfiguration EmployerApprenticeshipApi { get; set; }

        public string Hashstring { get; set; }
        public ApprenticeshipInfoServiceConfiguration ApprenticeshipInfoService { get; set; }
		public PostcodeAnywhereConfiguration PostcodeAnywhere { get; set; }

        public CommitmentNotificationConfiguration CommitmentNotification { get; set; }
    }

    public class CommitmentNotificationConfiguration
    {
        public bool UseProviderEmail { get; set; }

        public bool SendEmail { get; set; }

        public List<string> ProviderTestEmails { get; set; }

        public string IdamsListUsersUrl { get; set; }

        public string DasUserRoleId { get; set; }

        public string SuperUserRoleId { get; set; }

        public string ClientToken { get; set; }
    }

    public class ApprenticeshipInfoServiceConfiguration : IApprenticeshipInfoServiceConfiguration
    {
        public string BaseUrl { get; set; }
    }

    
}