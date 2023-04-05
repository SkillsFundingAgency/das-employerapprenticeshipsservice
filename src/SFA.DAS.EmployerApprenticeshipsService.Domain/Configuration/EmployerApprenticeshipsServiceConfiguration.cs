using System.Collections.Generic;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string EmployerAccountsBaseUrl { get; set; }
        public string EmployerCommitmentsBaseUrl { get; set; }
        public string EmployerFinanceBaseUrl { get; set; }
        public string EmployerPortalBaseUrl { get; set; }
        public string EmployerProjectionsBaseUrl { get; set; }
        public string EmployerRecruitBaseUrl { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public EmployerAccountsApiConfiguration EmployerAccountsApi { get; set; }
        public EmployerFinanceApiConfiguration EmployerFinanceApi { get; set; }
    }
}