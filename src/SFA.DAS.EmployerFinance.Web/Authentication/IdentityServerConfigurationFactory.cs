using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerFinance.Web.Authentication
{
    public class IdentityServerConfigurationFactory : ConfigurationFactory
    {
        private readonly EmployerFinanceConfiguration _configuration;

        public IdentityServerConfigurationFactory(EmployerFinanceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override ConfigurationContext Get()
        {
            return new ConfigurationContext
            {
                AccountActivationUrl = _configuration.Identity.BaseAddress.Replace("/identity", "") + _configuration.Identity.AccountActivationUrl
            };
        }
    }
}