using SFA.DAS.Authenication;
using SFA.DAS.Configuration;

namespace SFA.DAS.EmployerAccounts.Configuration
{
    public class EmployerAccountsConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }

        public IdentityServerConfiguration Identity { get; set; }
        public string DashboardUrl { get; set; }
    }
}