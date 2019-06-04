using System;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.HashingService;
using SFA.DAS.ObsoleteHashing;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            For<IHashingService>().Use(c => GetHashingService(c));
            For<IPublicHashingService>().Use(c => GetPublicHashingService(c));


            For<Infrastructure.MarkerInterfaces.IAccountLegalEntityPublicHashingService>()
                .Add<MarkerInterfaceWrapper>()
                .Ctor<IHashingService>()
                .Is(
                    c =>
                        new HashingService.HashingService(
                            c.GetInstance<EmployerApprenticeshipsServiceConfiguration>()
                                .PublicAllowedAccountLegalEntityHashstringCharacters,
                            c.GetInstance<EmployerApprenticeshipsServiceConfiguration>()
                                .PublicAllowedAccountLegalEntityHashstringSalt));
        }

        private IHashingService GetHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var hashingService = new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring);

            return hashingService;
        }

        private IPublicHashingService GetPublicHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var publicHashingService = new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring);

            return publicHashingService;
        }
    }

    class MarkerInterfaceWrapper
    : Infrastructure.MarkerInterfaces.IAccountLegalEntityPublicHashingService
    {
        private IHashingService
            _hashingServiceWithCorrectValuesForMarkerInterface;

        public MarkerInterfaceWrapper(IHashingService hashingServiceWithCorrectValuesForMarkerInterface)
        {
            _hashingServiceWithCorrectValuesForMarkerInterface = hashingServiceWithCorrectValuesForMarkerInterface;
        }

        public string HashValue(long id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.HashValue(id);
        }

        public string HashValue(Guid id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.HashValue(id);
        }

        public string HashValue(string id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.HashValue(id);
        }

        public long DecodeValue(string id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.DecodeValue(id);
        }

        public Guid DecodeValueToGuid(string id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.DecodeValueToGuid(id);
        }

        public string DecodeValueToString(string id)
        {
            return _hashingServiceWithCorrectValuesForMarkerInterface.DecodeValueToString(id);
        }
    }
}