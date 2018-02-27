using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Authentication
{
    public class IdentityServerConfigurationFactory : ConfigurationFactory
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public IdentityServerConfigurationFactory(EmployerApprenticeshipsServiceConfiguration configuration)
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