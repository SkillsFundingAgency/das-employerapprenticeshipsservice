using SFA.DAS.Authenication;
using SFA.DAS.Configuration;

namespace SFA.DAS.EmployerFinance.Configuration
{
    public class EmployerFinanceConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }

        public IdentityServerConfiguration Identity { get; set; }
        public string DashboardUrl { get; set; }
    }
}