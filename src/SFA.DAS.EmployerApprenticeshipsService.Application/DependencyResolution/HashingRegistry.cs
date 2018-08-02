using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            For<IHashingService>().Use(c => GetHashingService(c));
            For<IPublicHashingService>().Use(c => GetPublicHashingservice(c));
            For<IPublicHashingService>().Add(c => GetAccountLegalEntityPublicHashingservice(c)).Named("accountLegalEntityHashingService");
        }

        private IHashingService GetHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var hashingService = new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring);

            return hashingService;
        }

        private IPublicHashingService GetPublicHashingservice(IContext context)
        {
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var publicHashingService = new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring);

            return publicHashingService;
        }

        private IPublicHashingService GetAccountLegalEntityPublicHashingservice(IContext context)
        {
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var agreementHashingService = new PublicHashingService(config.PublicAllowedAccountLegalEntityHashstringCharacters, config.PublicAllowedAccountLegalEntityHashstringSalt);

            return agreementHashingService;
        }
    }
}