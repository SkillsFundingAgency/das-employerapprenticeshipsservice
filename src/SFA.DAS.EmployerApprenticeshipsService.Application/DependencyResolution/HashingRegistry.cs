using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Interfaces;
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
            For<IALEPublicHashingService>().Add(c => GetAccountLegalEntityPublicHashingservice(c));
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
            var publicHashingService = new IalePublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring);

            return publicHashingService;
        }

        private IALEPublicHashingService GetAccountLegalEntityPublicHashingservice(IContext context)
        {
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var agreementHashingService = new IalePublicHashingService(config.PublicAllowedAccountLegalEntityHashstringCharacters, config.PublicAllowedAccountLegalEntityHashstringSalt);

            return agreementHashingService;
        }
    }
}